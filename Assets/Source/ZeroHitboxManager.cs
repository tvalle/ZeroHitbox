using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HitboxCollisionInfo
{
    public GameObject GameObject;

    public string CurrentAnimation;

    public override string ToString()
    {
        string info;

        info = "HitboxCollisionInfo" + Environment.NewLine;
        info += "GameObject Name: " + GameObject.name + Environment.NewLine;
        info += "CurrentAnimation: " + CurrentAnimation;

        return info;
    }
}

public class ZeroHitboxManager : MonoBehaviour
{
    private static ZeroHitboxManager _instance = null;
    public static ZeroHitboxManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("AlphaHitboxManager");
                gameObject.transform.position = Vector3.zero;
                gameObject.AddComponent<ZeroHitboxManager>();
            }

            return _instance;
        }
    }

    [HideInInspector]
    private List<ZeroHitbox> hitboxList;

    public const string COLLISION_EVENT_ENTER = "OnHitboxCollisionEnter";
    public const string COLLISION_EVENT_STAY = "OnHitboxCollisionStay";
    public const string COLLISION_EVENT_EXIT = "OnHitboxCollisionExit";

    private CollisionSolver solver;

    void Awake()
    {
        if (hitboxList == null)
        {
            hitboxList = new List<ZeroHitbox>();
        }

        solver = new CollisionSolver(new BruteForceStrategy());

        _instance = this;
    }

    public void AddAlphaHitbox(ZeroHitbox alphaHitbox)
    {
        //TODO check this projectile thing
        //ProjectileController pController = alphaHitbox.GetComponent<ProjectileController>();
        //if (pController != null)
        //{
        //    alphaHitbox.IsProjectile = true;
        //    alphaHitbox.ProjectileController = pController;
        //}

        hitboxList.Add(alphaHitbox);
    }

    void Update()
    {
        solver.SolveCollisions(hitboxList);
    }

    public void SendCollisionMessage(ZeroHitbox receiver, ZeroHitbox collider, string message)
    {
        HitboxCollisionInfo collisionInfo = new HitboxCollisionInfo();
        collisionInfo.GameObject = collider.gameObject;
        collisionInfo.CurrentAnimation = receiver.AnimationClips[receiver.AnimationClipsIndex].Name;

        receiver.gameObject.SendMessage(message, collisionInfo, SendMessageOptions.DontRequireReceiver);
    }
}

public class CollisionSolver
{
    CollisionStrategy strategy;

    public CollisionSolver(CollisionStrategy strategy)
    {
        this.strategy = strategy;
    }

    public void SolveCollisions(List<ZeroHitbox> hitboxList)
    {
        strategy.CheckForCollisions(hitboxList);
    }
}

public abstract class CollisionStrategy
{
    public abstract void CheckForCollisions(List<ZeroHitbox> hitboxList);
}

public class BruteForceStrategy : CollisionStrategy
{
    List<ZeroHitbox> collisionList;

    public BruteForceStrategy()
    {
        collisionList = new List<ZeroHitbox>();
    }

    public override void CheckForCollisions(List<ZeroHitbox> hitboxList)
    {
        ResetCollisionList();

        foreach (ZeroHitbox zeroHitbox in hitboxList)
        {
            if (!zeroHitbox.IsActive || !zeroHitbox.enabled)
                continue;

            Hitbox[] currentHitboxes = zeroHitbox.CurrentHitboxes;

            for (int i = 0; i < currentHitboxes.Length; i++)
            {
                if (currentHitboxes[i].Type == HitboxType.Projectile
                 || currentHitboxes[i].Type == HitboxType.Attacking)
                {
                    foreach (ZeroHitbox alphaTemp in hitboxList)
                    {
                        //TODO check projectile handling
                        //if (alpha.IsProjectile && alpha.ProjectileController.ownerGameObject == alphaTemp.gameObject)
                        //    continue;

                        if (!alphaTemp.enabled)
                            continue;

                        if (alphaTemp.gameObject != zeroHitbox.gameObject)
                        {
                            if (CollidesWithlist(currentHitboxes[i], zeroHitbox.gameObject,
                                alphaTemp.AnimationClips[alphaTemp.AnimationClipsIndex].keyframes[alphaTemp.KeyframesIndex].hitboxes, alphaTemp.gameObject))
                            {
                                if (!collisionList.Contains(alphaTemp))
                                {
                                    collisionList.Add(alphaTemp);
                                    alphaTemp.MarkedForCollision = true;
                                    ZeroHitboxManager.Instance.SendCollisionMessage(alphaTemp, zeroHitbox, ZeroHitboxManager.COLLISION_EVENT_ENTER);
                                }
                                else
                                {
                                    alphaTemp.MarkedForCollision = true;
                                    ZeroHitboxManager.Instance.SendCollisionMessage(alphaTemp, zeroHitbox, ZeroHitboxManager.COLLISION_EVENT_STAY);
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = collisionList.Count - 1; i >= 0; i--)
        {
            if (collisionList[i].MarkedForCollision == false)
            {
                collisionList.RemoveAt(i);
            }
        }
    }

    bool CollidesWithlist(Hitbox hitbox, GameObject gameObject1, Hitbox[] hitboxes, GameObject gameObject2)
    {
        bool collisionHappened = false;

        for (int i = 0; i < hitboxes.Length; i++)
        {
            collisionHappened = CollidesWithHitbox(hitbox, gameObject1, hitboxes[i], gameObject2);

            if (collisionHappened)
                return true;
        }

        return collisionHappened;
    }

    private void ResetCollisionList()
    {
        foreach (ZeroHitbox zeroHitbox in collisionList)
        {
            zeroHitbox.MarkedForCollision = false;
        }
    }

    bool CollidesWithHitbox(Hitbox hitbox1, GameObject gameObject1, Hitbox hitbox2, GameObject gameObject2)
    {
        if (hitbox2.Type != HitboxType.Hittable)
            return false;

        if (hitbox1.Shape == HitboxShape.Rectangle && hitbox2.Shape == HitboxShape.Rectangle)
        {
            Rect rect1 = new Rect(gameObject1.transform.position.x + hitbox1.Rect.x,
                                  gameObject1.transform.position.y + hitbox1.Rect.y,
                                  hitbox1.Rect.width, hitbox1.Rect.height);

            Rect rect2 = new Rect(gameObject2.transform.position.x + hitbox2.Rect.x,
                                  gameObject2.transform.position.y + hitbox2.Rect.y,
                                  hitbox2.Rect.width, hitbox2.Rect.height);

            return RectangleCollision(rect1, rect2);
        }
        else
            return false;

        //else if (hitbox1.Shape == HitboxShape.Circle && hitbox2.Shape == HitboxShape.Circle)
        //{
        //    return CircleCollision(hitbox1, hitbox2);
        //}
        //else
        //{
        //    if (hitbox1.Shape == HitboxShape.Rectangle)
        //        return MixCollision(hitbox1, hitbox2);
        //    else
        //        return MixCollision(hitbox2, hitbox1);
        //}
    }

    private bool MixCollision(Hitbox rectangle, Hitbox circle)
    {
        //TODO implement this
        return false;
    }

    private bool RectangleCollision(Rect rect1, Rect rect2)
    {
        if (rect1.xMax < rect2.x)
            return false;
        if (rect1.x > rect2.xMax)
            return false;
        if (rect1.yMax < rect2.y)
            return false;
        if (rect1.y > rect2.yMax)
            return false;

        return true;
    }
}

//public abstract class CollisionAlgorithm
//{
//    public abstract bool Collision(Hitbox hitbox1, Hitbox hitbox2);
//}

//public class CircleCollision : CollisionAlgorithm
//{
//    public override bool Collision(Hitbox hitbox1, Hitbox hitbox2)
//    {
//        Vector2 pos1 = new Vector2(hitbox1.CircleX, hitbox1.CircleY);
//        Vector2 pos2 = new Vector2(hitbox2.CircleX, hitbox2.CircleY);

//        float collisionDistance = hitbox1.Radius + hitbox2.Radius;

//        return Mathf.Abs(Vector2.Distance(pos1, pos2)) <= collisionDistance;
//    }
//}

//public class RectangleCollision : CollisionAlgorithm
//{
//    public override bool Collision(Hitbox hitbox1, Hitbox hitbox2)
//    {
//        //if (rect1.xMax < rect2.x)
//        //    return false;
//        //if (rect1.x > rect2.xMax)
//        //    return false;
//        //if (rect1.yMax < rect2.y)
//        //    return false;
//        //if (rect1.y > rect2.yMax)
//        //    return false;

//        return true;
//    }
//}
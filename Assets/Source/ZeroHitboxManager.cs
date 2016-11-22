using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
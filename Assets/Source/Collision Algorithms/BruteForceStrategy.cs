using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BruteForceStrategy : CollisionStrategy
{
    List<ZeroHitbox> collisionList;

    public BruteForceStrategy()
    {
        collisionList = new List<ZeroHitbox>();
    }

    public override void CheckForCollisions(IEnumerable<ZeroHitbox> hitboxList)
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
                        if (!alphaTemp.enabled)
                            continue;

                        if (alphaTemp.gameObject != zeroHitbox.gameObject)
                        {
                            if (CollisionDetection.CollidesWithlist(currentHitboxes[i], zeroHitbox.gameObject,
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

    private void ResetCollisionList()
    {
        foreach (ZeroHitbox zeroHitbox in collisionList)
        {
            zeroHitbox.MarkedForCollision = false;
        }
    }
}
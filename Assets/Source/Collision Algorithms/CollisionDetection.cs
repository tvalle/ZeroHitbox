using UnityEngine;
using System.Collections;

public static class CollisionDetection
{
    public static bool CollidesWithlist(Hitbox hitbox, GameObject gameObject1, Hitbox[] hitboxes, GameObject gameObject2)
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

    private static bool CollidesWithHitbox(Hitbox hitbox1, GameObject gameObject1, Hitbox hitbox2, GameObject gameObject2)
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

    private static bool MixCollision(Hitbox rectangle, Hitbox circle)
    {
        //TODO implement this
        return false;
    }

    private static bool RectangleCollision(Rect rect1, Rect rect2)
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

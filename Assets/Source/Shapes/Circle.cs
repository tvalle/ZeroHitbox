using UnityEngine;
using System.Collections;
using System;

public class Circle : IShape
{
    public void DrawGizmo(Hitbox hitbox, Transform transform)
    {
        Vector3 hitboxPos = new Vector3(hitbox.CircleX * transform.localScale.x,
                                                    hitbox.CircleY * transform.localScale.y,
                                                    0f);

        Gizmos.DrawSphere(hitboxPos + transform.position, hitbox.Radius);
    }

    public Hitbox DrawSceneHandle(Hitbox hitbox, TargetComponents targetComponents)
    {
        return hitbox;
    }
}

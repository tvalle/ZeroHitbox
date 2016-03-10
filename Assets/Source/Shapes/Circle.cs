using UnityEngine;
using System.Collections;
using System;

public class Circle : IShape
{
    public void DrawGizmo(Hitbox hitbox, Transform transform)
    {
        Vector3 hitboxPos = new Vector3(hitbox.Position.x * transform.localScale.x,
                                                    hitbox.Position.y * transform.localScale.y,
                                                    0f);

        Gizmos.DrawSphere(hitboxPos + transform.position, hitbox.Boundaries.width);
    }

    public Hitbox DrawSceneHandle(Hitbox hitbox, TargetComponents targetComponents)
    {
        return hitbox;
    }
}

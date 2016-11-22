using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Hitbox
{
    public HitboxType Type;
    public HitboxShape Shape;

    public Vector3 Position { get; set; }
    public Rect Boundaries { get; set; }

    public static Dictionary<HitboxShape, IShape> ShapeDictionary = new Dictionary<HitboxShape, IShape>
    {
        { HitboxShape.Rectangle, new Rectangle() },
        { HitboxShape.Circle, new Circle() }
    };

    public Hitbox(Rect boundaries, HitboxType hitboxType)
    {
        Type = hitboxType;
        Position = Vector3.zero;
        Boundaries = boundaries;

        //Always assume default as rectangle
        Shape = HitboxShape.Rectangle;
    }

    internal void DrawGizmo(Transform transform)
    {
        ShapeDictionary[Shape].DrawGizmo(this, transform);
    }

    public Vector3 GetHandlePosition()
    {
        if (Shape == HitboxShape.Rectangle)
        {
            return new Vector3(Boundaries.x, Boundaries.y, 0f);
        }
        //else
        //{
        //    return new Vector3(CircleX, CircleY, 0f);
        //}

        return Vector3.zero;
    }
    public Vector3 GetHandleXScale()
    {
        return new Vector3(Boundaries.x + Boundaries.width,
                           Boundaries.y + Boundaries.height / 2);
    }
    public Vector3 GetHandleYScale()
    {
        return new Vector3(Boundaries.x + Boundaries.width / 2,
                           Boundaries.y + Boundaries.height);
    }
}
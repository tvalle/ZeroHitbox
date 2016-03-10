using UnityEngine;
using System.Collections;

public interface IShape
{
    void DrawGizmo(Hitbox hitbox, Transform transform);

    Hitbox DrawSceneHandle(Hitbox hitbox, TargetComponents targetComponents);

    //Vector3 GetHandlePosition();
    //Vector3 GetHandleXScale();
    //Vector3 GetHandleYScale();
}
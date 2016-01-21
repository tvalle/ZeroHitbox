using UnityEngine;
using System.Collections;

public interface IShape
{
    void DrawGizmo(Hitbox hitbox, Transform transform);

    Hitbox DrawSceneHandle(Hitbox hitbox, TargetComponents targetComponents);
}
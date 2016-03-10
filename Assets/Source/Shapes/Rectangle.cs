using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class Rectangle : IShape
{
    public void DrawGizmo(Hitbox hitbox, Transform transform)
    {
        Vector3 hitboxPos = new Vector3((hitbox.Boundaries.x + hitbox.Boundaries.width / 2) * transform.localScale.x,
                                                        (hitbox.Boundaries.y + hitbox.Boundaries.height / 2) * transform.localScale.y,
                                                        0f);
        Vector3 hitboxSize = new Vector3(hitbox.Boundaries.width, hitbox.Boundaries.height, 0.1f);

        Debug.Log(hitboxPos);
        Debug.Log(hitboxSize);
        Debug.Log("---");

        Gizmos.DrawCube(hitboxPos + transform.position, hitboxSize);
    }

    public Hitbox DrawSceneHandle(Hitbox hitbox, TargetComponents targetComponents)
    {
        if (Tools.current == Tool.Move)
        {
            return DrawMove(hitbox, targetComponents);
        }
        else if (Tools.current == Tool.Scale)
        {
            return DrawScale(hitbox, targetComponents);
        }

        return hitbox;
    }

    private Hitbox DrawMove(Hitbox currentHitbox, TargetComponents targetComponents)
    {
        Vector3 handlePosition = currentHitbox.GetHandlePosition();
        Vector3 moveHandlePos = Handles.FreeMoveHandle(handlePosition + targetComponents.GameObject.transform.position, Quaternion.identity, 0.05f, Vector3.one, Handles.RectangleCap);
        moveHandlePos -= targetComponents.GameObject.transform.position;

        currentHitbox.Boundaries = new Rect(moveHandlePos.x, moveHandlePos.y,
                                                currentHitbox.Boundaries.width,
                                                currentHitbox.Boundaries.height);

        return currentHitbox;
    }
    private Hitbox DrawScale(Hitbox currentHitbox, TargetComponents targetComponents)
    {
        Vector3 ScaleHandleXPos;
        Vector3 ScaleHandleYPos;

        ScaleHandleXPos = Handles.FreeMoveHandle(currentHitbox.GetHandleXScale() + targetComponents.GameObject.transform.position,
                                                 Quaternion.identity,
                                                 0.05f,
                                                 Vector3.one,
                                                 Handles.DotCap);
        ScaleHandleXPos -= targetComponents.GameObject.transform.position;

        ScaleHandleYPos = Handles.FreeMoveHandle(currentHitbox.GetHandleYScale() + targetComponents.GameObject.transform.position,
                                                 Quaternion.identity,
                                                 0.05f,
                                                 Vector3.one,
                                                 Handles.DotCap);
        ScaleHandleYPos -= targetComponents.GameObject.transform.position;

        currentHitbox.Boundaries = new Rect(currentHitbox.Boundaries.x, currentHitbox.Boundaries.y,
                                      ScaleHandleXPos.x - currentHitbox.Boundaries.x,
                                      ScaleHandleYPos.y - currentHitbox.Boundaries.y);
        return currentHitbox;
    }
}

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class Rectangle : IShape
{
    public void DrawGizmo(Hitbox hitbox, Transform transform)
    {
        Vector3 hitboxPos = new Vector3((hitbox.Rect.x + hitbox.Rect.width / 2) * transform.localScale.x,
                                                        (hitbox.Rect.y + hitbox.Rect.height / 2) * transform.localScale.y,
                                                        0f);
        Vector3 hitboxSize = new Vector3(hitbox.Rect.width, hitbox.Rect.height, 0.1f);

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

        currentHitbox.Rect = new Rect(moveHandlePos.x, moveHandlePos.y,
                                                currentHitbox.Rect.width,
                                                currentHitbox.Rect.height);

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

        currentHitbox.Rect = new Rect(currentHitbox.Rect.x, currentHitbox.Rect.y,
                                      ScaleHandleXPos.x - currentHitbox.Rect.x,
                                      ScaleHandleYPos.y - currentHitbox.Rect.y);
        return currentHitbox;
    }
}

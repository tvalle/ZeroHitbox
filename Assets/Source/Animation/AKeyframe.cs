using System;
using UnityEngine;

[Serializable]
public class AKeyframe
{
    public Hitbox[] hitboxes;
    public Sprite Sprite;

    public AKeyframe(Sprite sprite)
    {
        Sprite = sprite;
        hitboxes = new Hitbox[0];
    }
}
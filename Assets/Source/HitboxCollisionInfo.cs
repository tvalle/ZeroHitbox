using System;
using UnityEngine;

public class HitboxCollisionInfo
{
    public GameObject GameObject;

    public string CurrentAnimation;

    public override string ToString()
    {
        string info;

        info = "HitboxCollisionInfo" + Environment.NewLine;
        info += "GameObject Name: " + GameObject.name + Environment.NewLine;
        info += "CurrentAnimation: " + CurrentAnimation;

        return info;
    }
}
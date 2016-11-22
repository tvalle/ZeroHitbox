using System;

[Serializable]
public class AAnimationClip
{
    public AKeyframe[] keyframes;
    public string Name;

    public AAnimationClip(string name)
    {
        Name = name;
    }
}
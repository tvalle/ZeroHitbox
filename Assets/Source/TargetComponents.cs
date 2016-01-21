using UnityEngine;
using System.Collections;

public struct TargetComponents
{
    public TargetComponents(UnityEngine.Object target)
    {
        ZeroHitbox = (target as ZeroHitbox);
        GameObject = ZeroHitbox.gameObject;

        Animator = ZeroHitbox.GetComponent<Animator>();

        SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        spriteWhenGotFocus = SpriteRenderer.sprite;
    }

    public GameObject GameObject { get; set; }
    public ZeroHitbox ZeroHitbox { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public Sprite spriteWhenGotFocus { get; set; } //TODO always has to get this in OnEnable, even if targetcomponents are the same
    public Animator Animator { get; set; }
}

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System;

[Serializable]
public enum HitboxType
{
    Hittable,
    Attacking,
    Projectile
}

[Serializable]
public enum HitboxShape
{
    Rectangle,
    Circle
}

[Serializable]
public struct Hitbox
{
    public HitboxType Type;
    public HitboxShape Shape;

    public Rect Rect;

    public float Radius;
    public float CircleX, CircleY;

    public Vector3 GetHandlePosition()
    {
        if (Shape == HitboxShape.Rectangle)
        {
            return new Vector3(Rect.x, Rect.y, 0f);
        }
        else
        {
            return new Vector3(CircleX, CircleY, 0f);
        }
    }
    public Vector3 GetHandleXScale()
    {
        return new Vector3(Rect.x + Rect.width, Rect.y + Rect.height / 2);
    }
    public Vector3 GetHandleYScale()
    {
        return new Vector3(Rect.x + Rect.width / 2, Rect.y + Rect.height);
    }

    public Hitbox(Rect rect, HitboxType hitboxType)
    {
        Rect = rect;
        Type = hitboxType;

        //Always assume default as rectangle
        Shape = HitboxShape.Rectangle;

        Radius = 0f;
        CircleX = CircleY = 0f;
    }
}

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

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class ZeroHitbox : MonoBehaviour
{
    public bool HasLists
    {
        get
        {
            return AnimationClips != null 
                && AnimationClips.Length > 0
                && AnimationClips[AnimationClipsIndex].keyframes != null
                && AnimationClips[AnimationClipsIndex].keyframes.Length > 0;
        }
    }

    [HideInInspector]
    public bool IsActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    [HideInInspector]
    public Hitbox[] CurrentHitboxes
    {
        get
        {
            return AnimationClips[AnimationClipsIndex].keyframes[KeyframesIndex].hitboxes;
        }
    }

    [SerializeField]
    public AAnimationClip[] AnimationClips;
    [SerializeField]
    public string[] AnimationClipsStringList;

    public int AnimationClipsIndex;
    public int KeyframesIndex;

    [SerializeField]
    public AKeyframe[] KeyframesList;

    private Animator animator;

    //Used for AlphaHitBoxManager to check Enter, Stay and Exit messages
    [HideInInspector]
    public bool MarkedForCollision;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            //TODO add plugins
            //spriteAnimator = GetComponent<SpriteAnimator>();
        }
        else
        {

            //spriteRenderer = GetComponent<SpriteRenderer>();

            ZeroHitboxManager.Instance.AddAlphaHitbox(this);
        }
    }

    void Update()
    {
        if (AnimationClips != null)
        {
            if (animator != null)
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

                var clipState = animator.GetCurrentAnimatorStateInfo(0);
                float elapsedTime = clipState.normalizedTime - (float)Math.Truncate(clipState.normalizedTime);

                //Finding the current index of the animation clip
                for (int i = 0; i < AnimationClipsStringList.Length && clipInfo.Length > 0; i++)
                {
                    if (AnimationClipsStringList[i].Equals(clipInfo[0].clip.name))
                    {
                        AnimationClipsIndex = i;
                        break;
                    }
                }

                KeyframesIndex = Convert.ToInt32(Math.Floor(AnimationClips[AnimationClipsIndex].keyframes.Length * elapsedTime));
            }
        }
    }

    void OnDrawGizmos()
    {
        if (AnimationClips != null)
        {
            if (Application.isPlaying)
            {
                if (animator != null)
                {
                    var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

                    var clipState = animator.GetCurrentAnimatorStateInfo(0);
                    float elapsedTime = clipState.normalizedTime - (float)Math.Truncate(clipState.normalizedTime);

                    //Finding the current index of the animation clip
                    //TODO Maybe faster with dictionaries?
                    for (int i = 0; i < AnimationClipsStringList.Length; i++)
                    {
                        if (AnimationClipsStringList[i].Equals(clipInfo[0].clip.name))
                        {
                            AnimationClipsIndex = i;
                            break;
                        }
                    }

                    KeyframesIndex = Convert.ToInt32(Math.Floor(AnimationClips[AnimationClipsIndex].keyframes.Length * elapsedTime));
                }
            }

            if (!HasLists)
                return;

            Hitbox[] hitboxList = AnimationClips[AnimationClipsIndex].keyframes[KeyframesIndex].hitboxes;

            for (int i = 0; i < hitboxList.Length; i++)
            {
                if (hitboxList[i].Type == HitboxType.Hittable)
                {
                    Gizmos.color = new Color(0f, 0f, 1f, 0.4f);
                }
                else if (hitboxList[i].Type == HitboxType.Attacking)
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                }
                else if (hitboxList[i].Type == HitboxType.Projectile)
                {
                    Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
                }

                if (hitboxList[i].Shape == HitboxShape.Rectangle)
                {
                    Vector3 hitboxPos = new Vector3((hitboxList[i].Rect.x + hitboxList[i].Rect.width / 2) * this.transform.localScale.x,
                                                (hitboxList[i].Rect.y + hitboxList[i].Rect.height / 2) * this.transform.localScale.y,
                                                0f);
                    Vector3 hitboxSize = new Vector3(hitboxList[i].Rect.width, hitboxList[i].Rect.height, 0.1f);
                    Gizmos.DrawCube(hitboxPos + this.transform.position, hitboxSize);
                }
                else
                {
                    Vector3 hitboxPos = new Vector3(hitboxList[i].CircleX * this.transform.localScale.x,
                                                    hitboxList[i].CircleY * this.transform.localScale.y,
                                                    0f);

                    Gizmos.DrawSphere(hitboxPos + this.transform.position, hitboxList[i].Radius);
                }
            }
        }
    }
}

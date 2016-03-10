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

    public static Dictionary<HitboxType, Color> ColorDictionary = new Dictionary<HitboxType, Color>()
    {
        { HitboxType.Hittable, new Color(0f, 0f, 1f, 0.4f) },
        { HitboxType.Attacking, new Color(1f, 0f, 0f, 0.4f) },
        { HitboxType.Projectile, new Color(1f, 1f, 0f, 0.4f) },
    };

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
            //TODO check if this is redundant
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
                Gizmos.color = ColorDictionary[hitboxList[i].Type];

                hitboxList[i].DrawGizmo(this.transform);
            }
        }
    }
}

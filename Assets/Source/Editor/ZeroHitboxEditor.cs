﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

[CustomEditor(typeof(ZeroHitbox))]
public class ZeroHitboxEditor : Editor
{
    private bool showGUI = true;
    private bool previousShowGUI;

    //TODO check how to save this when unfocusing
    private bool showHitboxesGUI = true;

    int animationClipsIndex;
    int previousClipsIndex;
    int keyframesIndex;

    private TargetComponents targetComponents;
    private ZeroHitBoxClipsFeeder zeroHitboxClipsFeeder;

    #region Drawing Methods
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        GUI.skin.label.wordWrap = true;

        AAnimationClip currentClip = null;
        AKeyframe currentKeyframe = null;

        if (targetComponents.ZeroHitbox.AnimationClipsStringList != null && targetComponents.ZeroHitbox.AnimationClips.Length > 0)
        {
            DrawAnimationClips();
        }
        else
        {
            GUILayout.Label("You don't have any animation clips in your Animator component. Add some so we can show them.");

            GUILayout.EndVertical();
            return;
        }

        if (targetComponents.ZeroHitbox.HasLists)
        {
            currentClip = targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex];
            currentKeyframe = currentClip.keyframes[keyframesIndex];

            DrawFrames(currentClip, currentKeyframe);
        }

        showHitboxesGUI = EditorGUILayout.Foldout(showHitboxesGUI, "Hitboxes");

        if (showHitboxesGUI)
        {
            if (previousShowGUI != showGUI)
            {
                previousShowGUI = showGUI;
                SceneView.RepaintAll();
            }

            DrawHitboxes(currentKeyframe);
        }

        GUILayout.EndVertical();
    }

    void OnSceneGUI()
    {
        serializedObject.Update();

        if (showGUI && targetComponents.ZeroHitbox.AnimationClips != null)
        {
            AKeyframe currentKeyframe;

            currentKeyframe = targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes[keyframesIndex];

            if (currentKeyframe.hitboxes.Length > 0)
            {
                for (int i = 0; i < currentKeyframe.hitboxes.Length; i++)
                {
                    Vector3 handlePos;

                    //TODO implement properties in the hitbox to hide this ifs checking the shape to get the position
                    if (currentKeyframe.hitboxes[i].Shape == HitboxShape.Rectangle)
                    {
                        handlePos = new Vector3(currentKeyframe.hitboxes[i].Rect.x, currentKeyframe.hitboxes[i].Rect.y, 0f);
                    }
                    else
                    {
                        handlePos = new Vector3(currentKeyframe.hitboxes[i].CircleX, currentKeyframe.hitboxes[i].CircleY, 0f);
                    }

                    Vector3 handleXScale = new Vector3(currentKeyframe.hitboxes[i].Rect.x + currentKeyframe.hitboxes[i].Rect.width,
                                                       currentKeyframe.hitboxes[i].Rect.y + currentKeyframe.hitboxes[i].Rect.height / 2);
                    Vector3 handleYScale = new Vector3(currentKeyframe.hitboxes[i].Rect.x + currentKeyframe.hitboxes[i].Rect.width / 2,
                                                       currentKeyframe.hitboxes[i].Rect.y + currentKeyframe.hitboxes[i].Rect.height);

                    if (Tools.current == Tool.Move)
                    {
                        Vector3 moveHandlePos;
                        moveHandlePos = Handles.FreeMoveHandle(handlePos + targetComponents.GameObject.transform.position, Quaternion.identity, 0.05f, Vector3.one, Handles.RectangleCap);
                        moveHandlePos -= targetComponents.GameObject.transform.position;

                        if (currentKeyframe.hitboxes[i].Shape == HitboxShape.Rectangle)
                        {
                            currentKeyframe.hitboxes[i].Rect = new Rect(moveHandlePos.x, moveHandlePos.y,
                                                                    currentKeyframe.hitboxes[i].Rect.width,
                                                                    currentKeyframe.hitboxes[i].Rect.height);
                        }
                        else
                        {
                            currentKeyframe.hitboxes[i].CircleX = moveHandlePos.x;
                            currentKeyframe.hitboxes[i].CircleY = moveHandlePos.y;
                        }
                    }
                    //TODO change this && we have to add a handle to resize circles
                    if (Tools.current == Tool.Scale && currentKeyframe.hitboxes[i].Shape == HitboxShape.Rectangle)
                    {
                        Vector3 ScaleHandleXPos;
                        Vector3 ScaleHandleYPos;

                        ScaleHandleXPos = Handles.FreeMoveHandle(handleXScale + targetComponents.GameObject.transform.position, Quaternion.identity, 0.05f, Vector3.one, Handles.DotCap);
                        ScaleHandleXPos -= targetComponents.GameObject.transform.position;

                        ScaleHandleYPos = Handles.FreeMoveHandle(handleYScale + targetComponents.GameObject.transform.position, Quaternion.identity, 0.05f, Vector3.one, Handles.DotCap);
                        ScaleHandleYPos -= targetComponents.GameObject.transform.position;

                        currentKeyframe.hitboxes[i].Rect = new Rect(currentKeyframe.hitboxes[i].Rect.x, currentKeyframe.hitboxes[i].Rect.y,
                                                      ScaleHandleXPos.x - currentKeyframe.hitboxes[i].Rect.x,
                                                      ScaleHandleYPos.y - currentKeyframe.hitboxes[i].Rect.y);
                    }
                }
            }
        }

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAnimationClips()
    {
        animationClipsIndex = EditorGUILayout.Popup("Animation Clip", animationClipsIndex, targetComponents.ZeroHitbox.AnimationClipsStringList);

        //If different then a change was made(another animation clip was selected)
        if (previousClipsIndex != animationClipsIndex)
        {
            previousClipsIndex = animationClipsIndex;
            keyframesIndex = 0;
        }

        if (GUILayout.Button("Add Keyframe", GUILayout.Width(130)))
        {
            Array.Resize(ref targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes,
                         targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes.Length + 1);
            targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes[targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes.Length - 1] = new AKeyframe(null);
        }
    }
    private void DrawFrames(AAnimationClip currentClip, AKeyframe currentKeyframe)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sprite", GUILayout.Width(70));
        currentKeyframe.Sprite = EditorGUILayout.ObjectField(currentKeyframe.Sprite, typeof(Sprite)) as Sprite;
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Frame " + (keyframesIndex + 1) + "/" +
                                         currentClip.keyframes.Length);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(30)))
        {
            keyframesIndex--;
            if (keyframesIndex < 0)
                keyframesIndex = currentClip.keyframes.Length - 1;
        }
        if (GUILayout.Button(">", GUILayout.Width(30)))
        {
            keyframesIndex++;
            if (keyframesIndex >= currentClip.keyframes.Length)
            {
                keyframesIndex = 0;
            }
        }
        GUILayout.EndHorizontal();
    }
    private void DrawHitboxes(AKeyframe currentKeyframe)
    {
        if (currentKeyframe == null)
            return;

        if (currentKeyframe.hitboxes != null && currentKeyframe.hitboxes.Length > 0)
        {
            for (int i = 0; i < currentKeyframe.hitboxes.Length; i++)
            {
                GUIStyle blackBackground = new GUIStyle();
                blackBackground.normal.background = MakeTex(1, 1, Color.gray);
                GUILayout.BeginVertical(blackBackground);
                GUILayout.Label("Hitbox " + i);

                GUILayout.BeginHorizontal();
                currentKeyframe.hitboxes[i].Type = (HitboxType)EditorGUILayout.EnumPopup(currentKeyframe.hitboxes[i].Type, GUILayout.Width(100));
                currentKeyframe.hitboxes[i].Shape = (HitboxShape)EditorGUILayout.EnumPopup(currentKeyframe.hitboxes[i].Shape, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                if (currentKeyframe.hitboxes[i].Shape == HitboxShape.Rectangle)
                {
                    currentKeyframe.hitboxes[i].Rect = EditorGUILayout.RectField(currentKeyframe.hitboxes[i].Rect);
                }
                else
                {
                    currentKeyframe.hitboxes[i].Radius = EditorGUILayout.FloatField("Radius ", currentKeyframe.hitboxes[i].Radius);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    for (int j = i; j < currentKeyframe.hitboxes.Length - 1; j++)
                    {
                        currentKeyframe.hitboxes[j] = currentKeyframe.hitboxes[j + 1];
                    }

                    Array.Resize(ref currentKeyframe.hitboxes, currentKeyframe.hitboxes.Length - 1);
                    SceneView.RepaintAll();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Height(10));
                GUILayout.Label("", GUILayout.Height(10));
                GUILayout.EndVertical();
            }
        }

        if (currentKeyframe.hitboxes.Length == 0 && GUILayout.Button("Copy previous frame Hitboxes", GUILayout.Width(250)))
        {
            if (keyframesIndex != 0)
            {
                Hitbox[] hitboxes = targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes[keyframesIndex - 1].hitboxes.Clone() as Hitbox[];
                Array.Resize(ref currentKeyframe.hitboxes, hitboxes.Length);

                for (int i = 0; i < hitboxes.Length; i++)
                {
                    Hitbox hitbox = new Hitbox(hitboxes[i].Rect, hitboxes[i].Type);
                    hitbox.Shape = hitboxes[i].Shape;
                    hitbox.Radius = hitboxes[i].Radius;

                    currentKeyframe.hitboxes[i] = hitbox;
                }

                SceneView.RepaintAll();
            }
            else
            {
                Debug.LogError("You are on the first frame, can't copy previous frame Hitboxes!");
            }
        }

        if (GUILayout.Button("Add Hitbox", GUILayout.Width(100)))
        {
            Array.Resize(ref currentKeyframe.hitboxes, currentKeyframe.hitboxes.Length + 1);
            currentKeyframe.hitboxes[currentKeyframe.hitboxes.Length - 1] = new Hitbox(new Rect(0f, 0f, 1f, 1.5f), HitboxType.Hittable);

            SceneView.RepaintAll();
        }
    }
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
    #endregion

    void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;

        targetComponents = new TargetComponents(target);
        zeroHitboxClipsFeeder = new ZeroHitBoxClipsFeeder(targetComponents.ZeroHitbox);

        zeroHitboxClipsFeeder.FeedAnimationClips(targetComponents.Animator);

        //TODO maybe get previously set value?
        keyframesIndex = 0;
        animationClipsIndex = 0;

        showGUI = targetComponents.ZeroHitbox.showHitbox;
    }

    private void OnLostFocus()
    {
        EditorApplication.update -= OnEditorUpdate;

        if (targetComponents.SpriteRenderer != null)
        {
            //TODO check why not working
            targetComponents.SpriteRenderer.sprite = targetComponents.spriteWhenGotFocus;
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OnEditorUpdate()
    {
        if (targetComponents.ZeroHitbox == null)
        {
            OnLostFocus();
            return;
        }

        if (!targetComponents.ZeroHitbox.enabled)
            return;

        //TODO: check if these 2 lines don't slow down performance
        //(they are needed to update the values back to the monobehaviour, maybe use serializedproperties)
        targetComponents.ZeroHitbox.AnimationClipsIndex = animationClipsIndex;
        targetComponents.ZeroHitbox.KeyframesIndex = keyframesIndex;

        if (Selection.activeGameObject != targetComponents.GameObject)
        {
            OnLostFocus();
        }

        //if (targetSpriteRenderer != null && targetAlphaHitbox.AnimationClips != null)
        if (targetComponents.ZeroHitbox.HasLists && targetComponents.SpriteRenderer != null)
        {
            if (targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes[keyframesIndex].Sprite != null)
                targetComponents.SpriteRenderer.sprite = targetComponents.ZeroHitbox.AnimationClips[animationClipsIndex].keyframes[keyframesIndex].Sprite;
        }

        targetComponents.ZeroHitbox.showHitbox = showGUI;
    }
}

public class ZeroHitBoxClipsFeeder
{
    private ZeroHitbox zeroHitbox;

    public ZeroHitBoxClipsFeeder(ZeroHitbox zeroHitbox)
    {
        this.zeroHitbox = zeroHitbox;
    }

    private void FeedClipsFromAnimator(Animator animator)
    {
        if (animator != null)
        {
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                AnimationClip clip = animator.runtimeAnimatorController.animationClips[i];

                //targetAlphaHitbox.AnimationClips.Add(new AAnimationClip(clip.name));
                zeroHitbox.AnimationClips[i] = new AAnimationClip(clip.name);

                var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                for (int j = 0; j < bindings.Length; j++)
                {
                    ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, bindings[j]);
                    //targetAlphaHitbox.AnimationClips[i].keyframes = new List<AKeyframe>();
                    zeroHitbox.AnimationClips[i].keyframes = new AKeyframe[keyframes.Length];

                    for (int k = 0; k < keyframes.Length; k++)
                    {
                        AKeyframe keyframe = new AKeyframe(keyframes[k].value as Sprite);
                        //targetAlphaHitbox.AnimationClips[i].keyframes.Add(keyframe);
                        zeroHitbox.AnimationClips[i].keyframes[k] = keyframe;
                    }
                }
            }
        }
    }

    private void FeedClipsStringFromAnimator(Animator animator)
    {
        if (animator != null)
        {
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                zeroHitbox.AnimationClipsStringList[i] = animator.runtimeAnimatorController.animationClips[i].name;
            }
        }
    }

    public void FeedAnimationClips(Animator animator)
    {
        if ((zeroHitbox.AnimationClips == null || zeroHitbox.AnimationClips.Length == 0)
            && animator != null
            && animator.runtimeAnimatorController != null)
        {
            zeroHitbox.AnimationClips = new AAnimationClip[animator.runtimeAnimatorController.animationClips.Length];
            FeedClipsFromAnimator(animator);

            zeroHitbox.AnimationClipsStringList = new string[animator.runtimeAnimatorController.animationClips.Length];
            FeedClipsStringFromAnimator(animator);
        }

        
    }
}

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
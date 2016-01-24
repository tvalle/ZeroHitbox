using UnityEngine;
using System.Collections;

public class HeroCollision : MonoBehaviour
{
    void OnHitboxCollisionEnter(HitboxCollisionInfo info)
    {
        Debug.Log("collided with");
        Debug.Log(info);
    }
}

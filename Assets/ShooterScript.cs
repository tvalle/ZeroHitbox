using UnityEngine;
using System.Collections;

public class ShooterScript : MonoBehaviour
{
    public GameObject Bullet;

    private bool isShooting = false;

    void Update()
    {
        if (!isShooting)
        {
            isShooting = true;

            Bullet.transform.position = this.transform.position;
        }
        else
        {
            Bullet.transform.Translate(Vector3.left * Time.deltaTime * 2);

            if (Bullet.transform.position.x < this.transform.position.x - 5f)
            {
                isShooting = false;
            }
        }
    }
}

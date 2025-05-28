using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject BulletHole;

    private void OnCollisionEnter(Collision collision)
    {
        Bullet b = Cache.GetBullet(collision.gameObject);
        if (b != null)
        {
            ContactPoint contact = collision.contacts[0];
            //Debug.Log("B");
            GameObject bulletHoleInstance = Instantiate(BulletHole, contact.point, Quaternion.LookRotation(contact.normal));
            ParticleSystem ps = bulletHoleInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            SimplePool.Despawn(b);
        }
    }
}

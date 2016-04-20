using UnityEngine;
using System.Collections;

public class BulletStandardDestroy : MonoBehaviour {

    public float damage;
    public float bulletSpeed;
    public float rateOfFire;

    public bool destroy = true;

    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;
        var health = hit.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        if(destroy)
            Destroy(gameObject);
    }
}

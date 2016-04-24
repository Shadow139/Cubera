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
            if (health.isLocalPlayer)
            {
                var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                if (damage > 0.0f)
                    animation.startDamageAnimation(damage);
            }
            
            health.TakeDamage(damage);
        }
        if(destroy)
            Destroy(gameObject);
    }
}

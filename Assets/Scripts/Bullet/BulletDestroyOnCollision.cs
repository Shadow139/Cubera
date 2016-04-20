using UnityEngine;
using System.Collections;

public class BulletDestroyOnCollision : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;
        var health = hit.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(5);
        }

        Destroy(gameObject);
    }
}

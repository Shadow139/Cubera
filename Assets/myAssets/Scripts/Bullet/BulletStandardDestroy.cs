using UnityEngine;
using System.Collections;

public class BulletStandardDestroy : MonoBehaviour {

    public float maxDamage;
    public float damage;
    public float bulletSpeed;
    public float rateOfFire;
    public float bulletOffsetMultiplier;
    public bool destroy = true;

    public GameObject playerHitEffectPrefab;
    public GameObject enemyHitEffectPrefab;

    public CubeMovement owner;

    void Start()
    {
        Destroy(gameObject, 10.0f);

    }

    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;
        var health = hit.GetComponent<PlayerHealth>();


        if (health != null)
        {

            ParticleSystemRenderer p = enemyHitEffectPrefab.GetComponent<ParticleSystemRenderer>();
            p.sharedMaterial.color = hit.GetComponent<MeshRenderer>().sharedMaterial.color;

            Instantiate(enemyHitEffectPrefab, transform.position, Quaternion.identity);
            health.TakeDamage(damage,owner);
        }
        else
        {
            Instantiate(playerHitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (destroy)
            Destroy(gameObject);
    }
}

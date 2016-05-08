using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class BladeDancer : NetworkBehaviour
{

    public CubeMovement owner;
    public Transform target;
    public float orbitDistance = 10.0f;
    public float orbitDegreesPerSec = 180.0f;

    public GameObject enemyHitEffectPrefab;


    void Start () {
        Destroy(gameObject, 35.0f);

        if (!isServer) return;

        target = owner.gameObject.transform;
    }

    void Update () {
	
	}

    void LateUpdate()
    {
        if (!isServer) return;

        Orbit();
    }

    void Orbit()
    {
        if (target != null)
        {
            // Keep us at orbitDistance from target
            transform.position = target.position + (transform.position - target.position).normalized * orbitDistance;
            transform.RotateAround(target.position, Vector3.up, orbitDegreesPerSec * Time.deltaTime);
        }
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
            health.TakeDamage(25.0f, owner);
        }
        else
        {
            //Instantiate(playerHitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (false)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        var health = hit.GetComponent<PlayerHealth>();
        
        if (health != null)
        {
            ParticleSystemRenderer p = enemyHitEffectPrefab.GetComponent<ParticleSystemRenderer>();
            p.sharedMaterial.color = hit.GetComponent<MeshRenderer>().sharedMaterial.color;

            Instantiate(enemyHitEffectPrefab, transform.position, Quaternion.identity);

            health.TakeDamage(25.0f, owner);
        }
    }
}

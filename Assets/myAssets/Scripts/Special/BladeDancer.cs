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
        Destroy(gameObject, 25.0f);

        if (isClient)
        {
            GameObject[] gObjs = GameObject.FindGameObjectsWithTag("Player");

            float min = 99999.0f;

            foreach (GameObject g in gObjs)
            {
                float dist = Vector3.Distance(transform.position, g.transform.position);

                if (dist < min)
                {
                    min = dist;
                    owner = g.GetComponent<CubeMovement>();
                }
            }
        }
        ///if (!isServer) return;

        target = owner.gameObject.transform;
    }

    void LateUpdate()
    {
        //if (!isServer) return;
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

    void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        var health = hit.GetComponent<PlayerHealth>();
        
        if (health != null && hit.GetComponent<CubeMovement>() != owner)
        {
            ParticleSystemRenderer p = enemyHitEffectPrefab.GetComponent<ParticleSystemRenderer>();
            p.sharedMaterial.color = hit.GetComponent<MeshRenderer>().sharedMaterial.color;

            Instantiate(enemyHitEffectPrefab, transform.position, Quaternion.identity);

            health.TakeDamage(25.0f, owner);
        }
    }
}

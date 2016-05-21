using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Explosion : NetworkBehaviour
{

    public CubeMovement owner;
    public Transform target;

    public GameObject explosionEffectPrefab;

    private Collider[] hitColliders;
    public float blastRadius;
    public float explosionPower;

    void Start()
    {
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

        target = owner.gameObject.transform;
        StartCoroutine(explosionCountdown());
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        var hit = other.gameObject;

        if (other.gameObject.CompareTag("Bullet"))
        {
            triggerExplosion(true);
        }
    }

    private IEnumerator explosionCountdown()
    {
        yield return new WaitForSeconds(4.0f);
        triggerExplosion(false);
    }

    [ClientRpc]
    void RpcExplosionVisuals()
    {
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    }

    public void triggerExplosion(bool getsDamage)
    {
        if (isServer)
        {
            RpcExplosionVisuals();
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        ExplosionWork(transform.position,getsDamage);
        Destroy(gameObject);
    }

    void ExplosionWork(Vector3 explosionPoint,bool getsDamage)
    {
        hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius);

        foreach (Collider hitCol in hitColliders)
        {
            if (hitCol.GetComponent<Rigidbody>() != null)
            {
                if(!(hitCol.gameObject.GetComponent<CubeMovement>() == owner))
                {
                    hitCol.GetComponent<Rigidbody>().isKinematic = false;
                    hitCol.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPoint, blastRadius, 1, ForceMode.Impulse);

                    if (hitCol.CompareTag("Player"))
                    {
                        float distance = Vector3.Distance(transform.position, hitCol.transform.position);

                        var health = hitCol.gameObject.GetComponent<PlayerHealth>();

                        distance = Random.Range(75,105) - (2 * distance);
                        if (distance > 100)
                            distance = 100;

                        if (health != null)
                            health.TakeDamage((int)distance, owner);
                    }
                }
                if(getsDamage && (hitCol.gameObject.GetComponent<CubeMovement>() == owner))
                {
                    hitCol.GetComponent<Rigidbody>().isKinematic = false;
                    hitCol.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPoint, blastRadius, 1, ForceMode.Impulse);

                    if (hitCol.CompareTag("Player"))
                    {
                        float distance = Vector3.Distance(transform.position, hitCol.transform.position);

                        var health = hitCol.gameObject.GetComponent<PlayerHealth>();

                        distance = Random.Range(75, 105) - (2 * distance);
                        if (distance > 100)
                            distance = 100;

                        if (health != null)
                            health.TakeDamage((int)distance, owner);
                    }
                }
             
            }
        }
    }

}


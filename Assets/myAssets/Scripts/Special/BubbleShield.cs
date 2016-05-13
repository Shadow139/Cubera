using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BubbleShield : NetworkBehaviour
{
    public CubeMovement owner;
    public Transform target;

    void Start () {
        Destroy(gameObject, 15.0f);

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
        //if (!isServer) return;

        target = owner.gameObject.transform;
    }

    void LateUpdate()
    {
        //if (!isServer) return;

        transform.position = target.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        
        var hit = other.gameObject;

        if (other.gameObject.CompareTag("Bullet") && !(other.gameObject.GetComponent<BulletStandardDestroy>().owner == owner))
        {
            hit.GetComponent<BulletStandardDestroy>().owner = owner;

            RpcReverseBullet(hit);

        }
    }

    [ClientRpc]
    public void RpcReverseBullet(GameObject bullet)
    {
        var rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = -(rb.velocity * 1.5f);
        }
    }
}

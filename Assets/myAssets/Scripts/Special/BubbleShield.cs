using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BubbleShield : NetworkBehaviour
{

    public CubeMovement owner;
    public Transform target;

    void Start () {
        Destroy(gameObject, 10.0f);

        if (!isServer) return;

        target = owner.gameObject.transform;
    }

    void LateUpdate()
    {
        if (!isServer) return;

        transform.position = target.position;
    }

    void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;

        if (other.gameObject.CompareTag("Bullet"))
        {
            var rb = hit.GetComponent<Rigidbody>();

            hit.GetComponent<BulletStandardDestroy>().owner = owner;

            if (rb != null)
            {
                rb.velocity = -rb.velocity * 2.0f;
            }

        }
    }
}

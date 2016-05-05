using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BubbleShield : NetworkBehaviour
{

    public CubeMovement owner;
    public Transform target;

    void Start () {
        Destroy(gameObject, 10.0f);
        target = owner.gameObject.transform;
    }

    void LateUpdate()
    {
        transform.position = target.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            var hit = other.gameObject;
            var rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = -rb.velocity * 1.5f;
            }

        }
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InvisiblePack : NetworkBehaviour
{

    [SerializeField] private float amount;
    [SerializeField] private GameObject powerUpSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject special = (GameObject)Instantiate(powerUpSound, transform.position, Quaternion.identity);

            var hit = other.gameObject;
            var health = hit.GetComponent<PlayerHealth>();

            if (health != null)
            {
                if (health.isLocalPlayer)
                {
                    var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                    animation.startArmorAnimation(amount);
                }

                health.makeInvisible(amount);
            }

            if (isServer)
            {
                var respawn = FindObjectOfType<PowerUpSpawner>();
                respawn.respawn(transform, 20.0f, 30.0f);
            }

            Destroy(gameObject);
        }
    }
}

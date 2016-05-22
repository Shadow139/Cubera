using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RushPack : NetworkBehaviour
{
    [SerializeField]
    private float amount;
    [SerializeField]
    private GameObject powerUpSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject special = (GameObject)Instantiate(powerUpSound, transform.position, Quaternion.identity);

            var hit = other.gameObject;
            var cubeMovement = hit.GetComponent<CubeMovement>();

            if (cubeMovement != null)
            {
                if (cubeMovement.isLocalPlayer)
                {
                    var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                    animation.startRushAnimation(amount);
                    cubeMovement.startRushPowerUp(amount);
                }
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

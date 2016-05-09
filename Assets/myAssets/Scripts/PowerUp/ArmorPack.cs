using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class ArmorPack : NetworkBehaviour
{

    public float amount;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var hit = other.gameObject;
            var armor = hit.GetComponent<PlayerHealth>();

            if (armor != null)
            {
                if (armor.isLocalPlayer)
                {
                    var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                    animation.startArmorAnimation(amount);
                }

                armor.addArmor(amount);
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

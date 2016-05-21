using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class ArmorPack : NetworkBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private GameObject powerUpSound;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject special = (GameObject)Instantiate(powerUpSound ,transform.position,Quaternion.identity);

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

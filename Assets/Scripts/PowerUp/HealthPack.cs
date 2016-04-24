using UnityEngine;
using System.Collections;

public class HealthPack : MonoBehaviour {

    public float amount;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var hit = other.gameObject;
            var health = hit.GetComponent<PlayerHealth>();

            if (health != null)
            {
                if (health.isLocalPlayer)
                {
                    var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                    animation.startHealingAnimation(amount);
                }

                health.heal(amount);
            }
            
            Destroy(gameObject);
        }
    }
}

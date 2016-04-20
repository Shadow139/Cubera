using UnityEngine;
using System.Collections;

public class ArmorPack : MonoBehaviour {

    public float amount;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var hit = other.gameObject;
            var armor = hit.GetComponent<PlayerHealth>();
            if (armor != null)
                armor.addArmor(amount);

            Destroy(gameObject);
        }
    }
}

﻿using UnityEngine;
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
            {
                if (armor.isLocalPlayer)
                {
                    var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
                    animation.startArmorAnimation(amount);
                }

                armor.addArmor(amount);
            }

            Destroy(gameObject);
        }
    }
}

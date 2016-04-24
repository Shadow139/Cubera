using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100;
    public float maxArmor = 100;

    [SyncVar]
    public float currentHealth = 100;
    [SyncVar]
    public float currentArmor = 0;
    public bool hasArmor = false;

    void Update()
    {
        HealthStatus();
    }

    void HealthStatus()
    {
        if(currentArmor > 0)
        {
            hasArmor = true;
        }
        else
        {
            hasArmor = false;
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (!isServer)
        {
            return;
        }

        if (hasArmor){            currentArmor -= amount;        }
        else         {            currentHealth -= amount;       }

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            RpcRespawn();
        }
    }

    public void heal(float amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth += amount;
        if (currentHealth > 100)
        {
            currentHealth = maxHealth;
        }
    }

    public void addArmor(float amount)
    {
        if (!isServer)
        {
            return;
        }

        currentArmor += amount;
        if (currentArmor > 100)
        {
            currentArmor = maxArmor;
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            transform.position = Vector3.zero;
        }
    }

}

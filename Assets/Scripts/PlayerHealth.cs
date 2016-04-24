using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100;
    public float maxArmor = 100;
    public bool destroyOnDeath;

    private NetworkStartPosition[] spawnPoints;

    [SyncVar]
    public float currentHealth = 100;
    [SyncVar]
    public float currentArmor = 0;
    public bool hasArmor = false;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

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
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;
                RpcRespawn();
            }
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
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
    }

}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100;
    public float maxArmor = 100;
    public bool destroyOnDeath;

    private RectTransform[] healthBarRect;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    public Vector3 healthBarOffset;


    private NetworkStartPosition[] spawnPoints;

    [SyncVar(hook = "OnChangedHealth")]
    public float currentHealth = 100;
    [SyncVar(hook = "OnChangedArmor")]
    public float currentArmor = 0;
    public bool hasArmor = false;

    void Start()
    {
        healthBar = (GameObject)Instantiate(healthBarPrefab, this.gameObject.transform.position + healthBarOffset, Quaternion.identity);
        healthBarRect = healthBar.GetComponentsInChildren<RectTransform>();
    }

    public override void OnStartLocalPlayer()
    {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }


    void Update()
    {
        HealthStatus();

        if(healthBar != null)
            healthBar.transform.position = gameObject.transform.position + healthBarOffset;
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
    
    public void TakeDamage(float amount, CubeMovement owner)
    {
        if (!isServer)
        {
            return;
        }

        if (hasArmor){            currentArmor -= amount;        }
        else         {
            currentHealth -= amount;
            owner.score += (int)amount;
        }

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
                owner.kills += 1;
                owner.score += 50;
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

    void OnChangedHealth(float newValue)
    {
        currentHealth = newValue;
        healthBarRect[2].sizeDelta = new Vector2(newValue * 2, healthBarRect[2].sizeDelta.y);
    }

    void OnChangedArmor(float newValue)
    {
        currentArmor = newValue;
        healthBarRect[3].sizeDelta = new Vector2(newValue * 2, healthBarRect[3].sizeDelta.y);
    }

    void OnDestroy()
    {
        Destroy(healthBar);
    }
}

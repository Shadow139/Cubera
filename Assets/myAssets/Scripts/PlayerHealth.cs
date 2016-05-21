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

    public GameObject deathHitEffectPrefab;

    private NetworkStartPosition[] spawnPoints;

    [SyncVar(hook = "OnChangedHealth")]
    public float currentHealth = 100;
    [SyncVar(hook = "OnChangedArmor")]
    public float currentArmor = 0;
    public bool hasArmor = false;

    private Color color;

    void Start()
    {
        healthBar = (GameObject)Instantiate(healthBarPrefab, this.gameObject.transform.position + healthBarOffset, Quaternion.identity);
        healthBarRect = healthBar.GetComponentsInChildren<RectTransform>();
        color = gameObject.GetComponent<CubeMovement>().color;
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

        if (transform.position.y < -75 && isServer)
            RpcRespawn(color);
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

            if(amount > 0.9f)
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

                RpcShowKill(owner.playerName, this.gameObject.GetComponent<CubeMovement>().playerName, owner.color, color, 0);
                RpcRespawn(color);

                this.gameObject.GetComponent<CubeMovement>().deaths += 1;
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
    void RpcRespawn(Vector4 col)
    {
        ParticleSystemRenderer p = deathHitEffectPrefab.GetComponent<ParticleSystemRenderer>();
        p.sharedMaterial.color = col;
        
        GameObject particle = (GameObject)Instantiate(deathHitEffectPrefab, transform.position, Quaternion.identity);
        healthBar.SetActive(false);
        
        Vector3 spawnPoint = Vector3.zero;

        if (isLocalPlayer)
        {
            GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<MouseCamera>().setCameraPivot(particle.transform);

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }
        }
        gameObject.GetComponent<CubeMovement>().rb.velocity = new Vector3(0, 0, 0);
        transform.position = new Vector3(0.0f,5.0f,400.0f);

        StartCoroutine(respawn(spawnPoint));
    }

    [ClientRpc]
    void RpcShowKill(string p1,string p2, Vector4 p1Col, Vector4 p2Col, int method)
    {
        Debug.Log(p1 + " killed " + p2);

        GameObject.FindGameObjectWithTag("KillList").GetComponent<ListOfKilledPlayers>().addDeadPlayerToList(p1,p2,p1Col,p2Col);

    }

    private IEnumerator respawn(Vector3 spawn)
    {
        Renderer rend = this.gameObject.GetComponent<Renderer>();
        rend.enabled = false;
        yield return new WaitForSeconds(5.0f);
        transform.position = spawn;
        healthBar.SetActive(true);

        if (isLocalPlayer)
            GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<MouseCamera>().setCameraPivot(transform);

        rend.enabled = true;
    }

    void OnChangedHealth(float newValue)
    {
        currentHealth = newValue;
        healthBarRect[2].sizeDelta = new Vector2(newValue * 2, healthBarRect[2].sizeDelta.y);
    }

    void OnChangedArmor(float newValue)
    {
        currentArmor = newValue;
    }

    void OnDestroy()
    {
        Destroy(healthBar);
    }
}

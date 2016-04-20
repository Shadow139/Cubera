using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UI : NetworkBehaviour
{

    public PlayerHealth playerHealthScript;
    public Text ip;
    public Image healthBar;
    public Image armorBar;
    
    void Start()
    {
        ip.text = Network.player.ipAddress;
    }

	void Update () {
        if(!playerHealthScript && CubeMovement.player != null)
            playerHealthScript = CubeMovement.player.GetComponent<PlayerHealth>();

        if(playerHealthScript != null)
        {
            healthBar.fillAmount = playerHealthScript.currentHealth / playerHealthScript.maxHealth;
            armorBar.fillAmount = playerHealthScript.currentArmor / playerHealthScript.maxArmor;
        }
    }
    
}

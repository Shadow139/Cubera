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

    public Image[] bullets;
    public Image[] specials;

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

    public void changeBulletIcon(int icon)
    {
        foreach(Image bullet in bullets)
        {
            bullet.enabled = false;
        }

        bullets[icon].enabled = true;
    }

    public void changeSpecialIcon(int icon)
    {
        foreach (Image special in specials)
        {
            special.enabled = false;
        }

        specials[icon].enabled = true;
    }
}

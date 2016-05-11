using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RushPanel : MonoBehaviour {

    public Image rush;
    public Image noRush;
    public bool hasCooldown = false;
    public float cooldown = 0.0f;


    void Update() {
        if (hasCooldown)
        {
            cooldown += Time.deltaTime;

            rush.fillAmount = cooldown / 5.0f;

            if(cooldown > 4.99)
            {
                hasCooldown = false;
                rush.fillAmount = 1.0f;
            }
        }
	
	}
	
    public void startCooldown()
    {
        cooldown = 0.0f;
        hasCooldown = true;
    }
}

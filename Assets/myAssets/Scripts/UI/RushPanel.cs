using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RushPanel : MonoBehaviour {

    public Image rush;
    public Image noRush;
    public Image rushBg;

    public bool hasCooldown = false;
    public float cooldown = 0.0f;

    private CubeMovement cubeScript;

    void Update() {
        if (hasCooldown)
        {
            float maxCooldown = 5.0f;
            if (cubeScript == null)
            {
                cubeScript = CubeMovement.player.GetComponent<CubeMovement>();
            }

            maxCooldown = cubeScript.rushCooldown;

            cooldown += Time.deltaTime;

            rush.fillAmount = cooldown / maxCooldown;

            if(cooldown > maxCooldown - 0.01f)
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

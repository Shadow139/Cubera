using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RollLoading : MonoBehaviour {

    [SerializeField] private Image loadingCircle;
    [SerializeField] private Image loadingBG;
    [SerializeField] private Image[] specials;
    public CanvasGroup cg;

    private CubeMovement playerScript;
    float time;


	void FixedUpdate () {
        if (playerScript == null)
            playerScript = CubeMovement.player.GetComponent<CubeMovement>();

        if(playerScript != null)
        {
            time = playerScript.getTimeWithoutMovement();

            if(time > 0.5)
            {
                //loadingCircle.enabled = true;
                //loadingBG.enabled = true;
                cg.alpha = 1.0f;         
                changeSpecialIcon(playerScript.roll());
            }
            else if(time < 0.5)
            {
                cg.alpha = 0f;
                //loadingCircle.enabled = false;
                //loadingBG.enabled = false;
            }

            loadingCircle.fillAmount = time / 5.0f;

        }

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

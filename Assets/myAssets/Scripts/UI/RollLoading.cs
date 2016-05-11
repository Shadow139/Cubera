using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RollLoading : MonoBehaviour {

    [SerializeField] private Image loadingCircle;
    private CubeMovement playerScript;


	void FixedUpdate () {
        if (playerScript == null)
            playerScript = CubeMovement.player.GetComponent<CubeMovement>();

        if(playerScript != null)
            loadingCircle.fillAmount = playerScript.getTimeWithoutMovement() / 5.0f;

    }
}

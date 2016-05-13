using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BombLoading : MonoBehaviour {


    [SerializeField]
    private Image loadingCircle;
    private CubeMovement playerScript;


    void FixedUpdate()
    {
        if (playerScript == null)
            playerScript = CubeMovement.player.GetComponent<CubeMovement>();

        if (playerScript != null)
            loadingCircle.fillAmount = playerScript.getBombLoadingTime() / 4.0f;

    }
}

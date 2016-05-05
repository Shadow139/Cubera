using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    GameObject playerCam;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(playerCam == null)
            playerCam = GameObject.FindWithTag("PlayerCamera");

        if (playerCam != null)
            transform.LookAt(playerCam.transform);
	}
}

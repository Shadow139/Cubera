using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour {

    public GameObject playerScoreBoard;

    void Start () {
                
    }
	
	void Update () {

        if (Input.GetKey(KeyCode.Tab))
        {
            playerScoreBoard.active = true;
        }
        else
        {
            playerScoreBoard.active = false;
        }
    }
}

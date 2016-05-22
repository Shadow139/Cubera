using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour {

    public GameObject playerScoreBoard;
    public bool isActiva = true;

    void Start () {
                
    }
	
	void Update () {
        if (isActiva)
        {
            if (Input.GetKey(KeyCode.Tab) || Input.GetButton("Back"))
            {
                setActive(true);
            }
            else
            {
                setActive(false);
            }
        }
    }

    public void setActive(bool value)
    {
        playerScoreBoard.active = value;
    }

    public void setTabListener(bool value)
    {
        isActiva = value;
    }
}

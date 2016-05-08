using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RushPanel : MonoBehaviour {

    public Image rush;
    public Image noRush;


    void Start () {
	
	}
	
    public void setRush()
    {
        noRush.enabled = false;
        rush.enabled = true;
    }

    public void setNoRush()
    {
        noRush.enabled = true;
        rush.enabled = false;
    }
}

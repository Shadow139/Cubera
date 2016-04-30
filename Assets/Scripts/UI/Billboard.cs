using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject t = GameObject.FindWithTag("PlayerCamera");
        if (t != null)
            transform.LookAt(t.transform);
	}
}

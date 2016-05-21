using UnityEngine;
using System.Collections;

public class ObjectDeastroyer : MonoBehaviour {

    public float destroyTime = 0.0f;

	void Start () {
        Destroy(gameObject, destroyTime);
	}	

}

using UnityEngine;
using System.Collections;

public class IconFollow : MonoBehaviour {

    public GameObject followObject;
    public float miniMapHeight = 30.0f;

    void LateUpdate()
    {
        if (followObject != null)
            transform.position = new Vector3(followObject.transform.position.x, 90, followObject.transform.position.z);
    }
}

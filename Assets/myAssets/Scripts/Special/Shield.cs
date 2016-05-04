using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public GameObject owner;
    public Transform target;

    void Start () {
        Destroy(gameObject, 10.0f);
        target = owner.transform;
    }

    void LateUpdate()
    {
        transform.position = target.position;
    }
}

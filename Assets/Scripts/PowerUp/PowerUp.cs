using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
                 
            Destroy(gameObject);
        }
    }


}

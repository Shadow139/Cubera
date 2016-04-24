using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PowerUpSpawner : NetworkBehaviour
{
    public Transform[] powerUpSpawns;
    public GameObject[] powerUps;

    // Use this for initialization
    public override void OnStartServer()
    { 
        foreach(Transform spawnPoint in powerUpSpawns)
        {
            var powerUp = (GameObject)Instantiate(powerUps[Random.Range(0, powerUps.Length)], spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(powerUp);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

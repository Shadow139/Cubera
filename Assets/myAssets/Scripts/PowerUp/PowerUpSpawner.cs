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

    public void respawn(Transform spawnPoint, float minWaitTime, float maxWaitTime)
    {
        Transform temp = new GameObject().transform;
        temp.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        temp.rotation = spawnPoint.rotation;

        StartCoroutine(SpawnObject(temp, Random.Range(minWaitTime, maxWaitTime)));
    }

    public IEnumerator SpawnObject(Transform spawnPoint, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        var powerUp = (GameObject)Instantiate(powerUps[Random.Range(0, powerUps.Length)], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(powerUp);
    }
}

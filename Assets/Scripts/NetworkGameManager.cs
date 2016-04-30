using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class NetworkGameManager : NetworkBehaviour
{

    public static List<CubeMovement> sPlayers = new List<CubeMovement>();
    
    public Text[] playerNames;
    public Text[] playerScores;
    public Text[] playerLatencys;

    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        int i = 0;

        foreach (CubeMovement p in sPlayers)
        {
            playerNames[i].text = p.playerName;
            playerScores[i].text = p.score + "";
            playerLatencys[i].text = p.latency + "";

            i++;
        }

    }

    void FixedUpdate()
    {
        sPlayers.Sort((IComparer<CubeMovement>)new sort());
    }

    private class sort : IComparer<CubeMovement>
    {
        int IComparer<CubeMovement>.Compare(CubeMovement _objA, CubeMovement _objB)
        {
            int t1 = _objA.score;
            int t2 = _objB.score;
            return t1.CompareTo(t2);
        }
    }
    }

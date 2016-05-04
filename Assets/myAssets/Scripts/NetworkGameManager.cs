﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class NetworkGameManager : NetworkBehaviour
{

    public static List<CubeMovement> sPlayers = new List<CubeMovement>();
    
    public Text[] playerNames;
    public Text[] playerScores;
    public Text[] playerLatencys;

    private float lastListUpdate = 0.0f;


    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void FixedUpdate()
    {
        if (Time.time > 0.6f + lastListUpdate)
        {
            sPlayers.Sort((IComparer<CubeMovement>)new sort2());
            lastListUpdate = Time.time;

            updateView();
        }
    }

    void updateView()
    {
        int i = 0;

        foreach (CubeMovement p in sPlayers)
        {
            if (p.isLocalPlayer)
            {
                playerNames[i].color = Color.yellow;
                playerScores[i].color = Color.yellow;
                playerLatencys[i].color = Color.yellow;
            }
            else
            {
                playerNames[i].color = Color.white;
                playerScores[i].color = Color.white;
                playerLatencys[i].color = Color.white;
            }

            playerNames[i].text = p.playerName;
            playerScores[i].text = p.score + "";
            playerLatencys[i].text = p.kills + "";

            i++;
        }
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

    private class sort2 : IComparer<CubeMovement>
    {
        int IComparer<CubeMovement>.Compare(CubeMovement _objA, CubeMovement _objB)
        {
            int t1 = _objA.score;
            int t2 = _objB.score;
            int tmp;
            if (t1 < t2)
            {
                tmp = 1;
            }
            else if (t1 > t2)
            {
                tmp = -1;
            }
            else
            {
                tmp = 0;
            }

            return tmp;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IPAddressUIDisplay : MonoBehaviour {

    public Text ip;

    void FixedUpdate()
    {

        ip.text = Network.player.ipAddress;
    }
    }



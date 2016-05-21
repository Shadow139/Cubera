using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class MapChooser : NetworkBehaviour
{
    [SerializeField]private Slider mapSlider;
    [SerializeField]private Image mapImage;
    [SerializeField]private Text mapText;
        
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public void setMap()
    {
        if (mapSlider)
        {
            sendFromLocalPlayer((int)mapSlider.value);
        }

    }

    void sendFromLocalPlayer(int id)
    {
        UnityStandardAssets.Network.LobbyPlayer[] go = GameObject.FindObjectsOfType<UnityStandardAssets.Network.LobbyPlayer>();

        foreach (UnityStandardAssets.Network.LobbyPlayer g in go)
        {
            if (g.isLocalPlayer)
            {
                g.sendMapChange(id);
            }
        }
    }

    public void changeMapVisuals(int newMapId)
    {
        mapText.text = newMapId.ToString();

        if (mapSlider)
        {
            mapSlider.value = newMapId;
            if (newMapId == 0)
            {
                mapImage.color = Color.red;
            }
            else if (newMapId == 1)
            {
                mapImage.color = Color.black;
            }
            else
            {
                mapImage.color = Color.green;

            }
        }
    }

    [ClientRpc]
    void RpcChangeMap(int id)
    {
        mapText.text = id.ToString();

        if (mapSlider)
        {
            mapSlider.value = id;
            if (id == 0)
            {
                mapImage.color = Color.red;
            }
            else if (id == 1)
            {
                mapImage.color = Color.black;
            }
            else
            {
                mapImage.color = Color.green;

            }
        }
    }
}

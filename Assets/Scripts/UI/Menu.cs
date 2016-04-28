using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Menu : MonoBehaviour {
    [SerializeField]
    private GameObject MenuItem;
    [SerializeField]
    private NetworkManager _manager;
    public InputField IP;
    private string _address;
    

	// Use this for initialization
	void Start () {
        _address = _manager.networkAddress;
        IP.text = _address;
	}
	
	// Update is called once per frame
	void Update () {
        if (!NetworkClient.active && !NetworkServer.active && _manager.matchMaker == null)
        {
            MenuItem.SetActive(true);
        }
        else {
            MenuItem.SetActive(false);
        }
        }

    public void StartHost() {
        _manager.StartHost();
    }
    public void JoinLocal() {
        _manager.SetMatchHost("localhost", 1337, false);
    }
    public void join_Game() {
        _manager.StartClient();
        _manager.networkAddress = _address;
        _manager.SetMatchHost(_address, 1337, false);
    }
    public void StartServer() {
        _manager.StartServer();
    }

    public void ChangeAddress(string value) {
        _address = value;
    }

    void OnGUI()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            if (GUI.Button(new Rect(0, 0, 200, 20), "Stop (X)"))
            {
                
                _manager.StopHost();
            }
           
        }
    }
    }

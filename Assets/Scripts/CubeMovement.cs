using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CubeMovement : NetworkBehaviour
{
    public static GameObject player;

    #region Cameras
    public Camera cameraPrefab;
    public Camera miniMapPrefab;
    public GameObject playerCameraObject;
    private GameObject miniMapCameraObject;
    private GameObject mainCamera;
    #endregion

    #region lookDirections
    private int mode = 0;
    private Vector3 rotateHorizontal = Vector3.back;
    private Vector3 rotateVertical = Vector3.right;
    private Vector3 forwardForce = Vector3.forward;
    private Vector3 rightForce = Vector3.right;
    #endregion

    #region public
    public float torque;
    public float jumpSpeed;
    public GameObject[] bulletPrefabs;
    public GameObject specialPrefab;

    public Text latencyText;
    #endregion

    #region private
    private int bullet = 0;
    private Vector3 bulletOffset = Vector3.forward;
    private Vector3 bulletDirection = Vector3.forward;
    private float lastShot = 0.0f;
    private Rigidbody rigidbody;
    private BulletStandardDestroy bulletScript;
    private bool isGrounded = false;
    private GameObject ui;
    private UI uiScript;
    #endregion

    #region Network Synced
    [SyncVar(hook = "OnScoreChanged")]
    public int score;
    [SyncVar(hook = "OnKillsChanged")]
    public int kills;
    [SyncVar]
    public Color color;
    [SyncVar]
    public string playerName;

    #endregion

    private NetworkClient nClient;
    private int latency;

    void Awake()
    {
    }

    void Start()
    {
        NetworkGameManager.sPlayers.Add(this);
        GetComponent<MeshRenderer>().material.color = color;        
    }

    public override void OnStartLocalPlayer()
    {
        player = gameObject;

        Camera playerCamera = (Camera)Camera.Instantiate(cameraPrefab, new Vector3(0, 3.5f, -6.5f), Quaternion.AngleAxis(15, Vector3.right));
        Camera miniMapCamera = (Camera)Camera.Instantiate(miniMapPrefab, new Vector3(0, 120, 0), Quaternion.AngleAxis(90, Vector3.right));
        playerCameraObject = playerCamera.gameObject;
        miniMapCameraObject = miniMapCamera.gameObject;
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);
        rigidbody = GetComponent<Rigidbody>();
        //nClient = GameObject.Find("LobbyManager").GetComponent<NetworkManager>().client;
        //latencyText = GameObject.Find("Latency").GetComponent<Text>();
    }

    void Update()
    {

        if (!isLocalPlayer)     return;

        //showLatency();

        if (hasShootingInput())
        {
            bulletScript = bulletPrefabs[bullet].GetComponent<BulletStandardDestroy>();
            if (Time.time > bulletScript.rateOfFire + lastShot)
            {
                CmdFire(bullet,bulletOffset, bulletDirection, bulletScript.bulletSpeed);
                lastShot = Time.time;
            }
        }

        if (hasSecondaryShootingInput())
        {
            CmdFireSecondary();
        }

        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
            Jump();

        if (Input.GetKeyDown(KeyCode.Backspace))
            toggleLethalBullets();

        if(hasSwitchingBulletInput())
            switchCurrentBullet();

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)     return;

        float moveHorizontal = Input.GetAxis("Horizontal") * torque ;
        float moveVertical = Input.GetAxis("Vertical") * torque ;

        rigidbody.AddTorque(rotateHorizontal * moveHorizontal);
        rigidbody.AddTorque(rotateVertical * moveVertical);

        rigidbody.AddForce(forwardForce * moveVertical * 0.002f);
        rigidbody.AddForce(rightForce * moveHorizontal * 0.002f);        
    }

    void OnCollisionStay() //check if you are colliding with the ground
    {
        isGrounded = true;
    }

    [Command]
    void CmdFire(int current, Vector3 offset, Vector3 direction, float speed)
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefabs[current],
            transform.position + (offset * bulletPrefabs[current].GetComponent<BulletStandardDestroy>().bulletOffsetMultiplier),
            Quaternion.identity);

        BulletStandardDestroy bulletScript = bullet.GetComponent<BulletStandardDestroy>();

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = direction * speed;
        bulletScript.owner = this;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdFireSecondary()
    {
        var special = (GameObject)Instantiate(
            specialPrefab,
            transform.position,
            Quaternion.identity);

        BladeDancer bladeScript = special.GetComponent<BladeDancer>();

        bladeScript.owner = this.gameObject;

        NetworkServer.Spawn(special);

    }

    void Jump()
    {
        isGrounded = false;
        rigidbody.AddForce(Vector3.up * jumpSpeed);
    }

    void Tackle()
    {
       
    }

    public void switchMode(int change)
    {
        mode += change;

        if (mode < 0)
            mode = 3;

        if (mode > 3)
            mode = 0;

        if(mode == 0)
        {
            rotateHorizontal = Vector3.back;
            rotateVertical = Vector3.right;

            forwardForce = Vector3.forward;
            rightForce = Vector3.right;

            bulletOffset = Vector3.forward;
            bulletDirection = Vector3.forward;
        }
        else if(mode == 1)
        {
            rotateHorizontal = Vector3.left;
            rotateVertical = Vector3.back;

            forwardForce = Vector3.right;
            rightForce = Vector3.back;

            bulletOffset = Vector3.right;
            bulletDirection = Vector3.right;
        }
        else if (mode == 2)
        {
            rotateHorizontal = Vector3.forward;
            rotateVertical = Vector3.left;

            forwardForce = Vector3.back;
            rightForce = Vector3.left;

            bulletOffset = Vector3.back;
            bulletDirection = Vector3.back;            
        }
        else if (mode == 3)
        {
            rotateHorizontal = Vector3.right;
            rotateVertical = Vector3.forward;

            forwardForce = Vector3.left;
            rightForce = Vector3.forward;

            bulletOffset = Vector3.left;
            bulletDirection = Vector3.left;
        }

    }

    void switchCurrentBullet()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bullet = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bullet = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bullet = 2;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            bullet++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            bullet--;
        }

        bullet = Mathf.Clamp(bullet,0,bulletPrefabs.Length - 1);
        if(ui == null || uiScript == null)
        {
            ui = GameObject.FindGameObjectWithTag("UI");
            uiScript = ui.GetComponent<UI>();
        }
        
        uiScript.changeBulletIcon(bullet);
    }

    bool hasSwitchingBulletInput()
    {
        //checks for Scroll Wheel Input and the Alphanumericals 1,2,3 above the Alpha Keys
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetKeyDown(KeyCode.Alpha2)) || Input.GetKeyDown(KeyCode.Alpha3)
            || (Input.GetAxis("Mouse ScrollWheel") > 0) || (Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool hasShootingInput()
    {
        //chocks for Left MouseButton, the Contol Buttons and the Contoller 2nd Trigger Button
        if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool hasSecondaryShootingInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void toggleLethalBullets()
    {
        foreach(GameObject prefab in bulletPrefabs)
        {
            prefab.GetComponent<BulletStandardDestroy>().destroy = !prefab.GetComponent<BulletStandardDestroy>().destroy;

            if (prefab.GetComponent<BulletStandardDestroy>().damage > 0)
            {
                prefab.GetComponent<BulletStandardDestroy>().damage = 0;
            }
            else
            {
                prefab.GetComponent<BulletStandardDestroy>().damage = prefab.GetComponent<BulletStandardDestroy>().maxDamage;
            }
        }    
    }

    void showLatency()
    {
        latency = nClient.GetRTT();
        if(latency < 100)
        {
            latencyText.color = Color.green;
        }
        else if(latency >= 100 && latency < 200)
        {
            latencyText.color = Color.yellow;
        }
        else
        {
            latencyText.color = Color.red;
        }

        latencyText.text = latency.ToString();
    }
    
    void OnDestroy()
    {
        if(mainCamera != null)
            mainCamera.SetActive(true);

        Destroy(playerCameraObject);
        Destroy(miniMapCameraObject);
        NetworkGameManager.sPlayers.Remove(this);
    }
    
    void OnScoreChanged(int newValue)
    {
        score = newValue;        
    }

    void OnKillsChanged(int newValue)
    {
        kills = newValue;
    }

}

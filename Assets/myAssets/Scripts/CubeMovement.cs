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
    public GameObject[] specialPrefab;

    public int rollValue = 0;
    #endregion

    #region private
    private int bullet = 0;
    private Vector3 bulletOffset = Vector3.forward;
    private Vector3 bulletDirection = Vector3.forward;
    private float lastShot = 0.0f;
    public Rigidbody rb;
    private BulletStandardDestroy bulletScript;
    private bool isGrounded = false;
    private bool isAbleToTackle = true;
    private bool isAbleToRoll = false;
    private bool hasSpecialSkill = false;
    private GameObject ui;
    private UI uiScript;
    private NetworkClient nClient;
    private int latency;
    private bool hasTurret = false;
    private float lastTurretShot = 0.0f;
    #endregion

    #region Network Synced
    [SyncVar(hook = "OnScoreChanged")]
    public int score;
    [SyncVar(hook = "OnKillsChanged")]
    public int kills;
    [SyncVar(hook = "OnDeathsChanged")]
    public int deaths;
    [SyncVar]
    public Color color = Color.white;
    [SyncVar]
    public string playerName;

    #endregion
    
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
        playerCamera.gameObject.GetComponent<MouseCamera>().cameraPivot = transform;
        Camera miniMapCamera = (Camera)Camera.Instantiate(miniMapPrefab, new Vector3(0, 120, 0), Quaternion.AngleAxis(90, Vector3.right));
        playerCameraObject = playerCamera.gameObject;
        miniMapCameraObject = miniMapCamera.gameObject;
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);
        rb = GetComponent<Rigidbody>();
        nClient = GameObject.Find("LobbyManager").GetComponent<NetworkManager>().client;

        /*foreach(GameObject b in bulletPrefabs)
        {
            b.GetComponent<MeshRenderer>().sharedMaterial.color = color; 
        }*/
    }

    void Update()
    {

        if (!isLocalPlayer)     return;
        if (NetworkGameManager.gameover) return;
        
        if (hasShootingInput())
        {
            bulletScript = bulletPrefabs[bullet].GetComponent<BulletStandardDestroy>();
            if (Time.time > bulletScript.rateOfFire + lastShot)
            {
                CmdFire(bullet,bulletOffset, bulletDirection, bulletScript.bulletSpeed);
                lastShot = Time.time;
            }
        }

        if (hasSecondaryShootingInput() && hasSpecialSkill)
        {
            checkSpecialSkill();
        }

        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
            Jump();

        if (Input.GetKeyDown(KeyCode.Backspace))
            toggleLethalBullets();


        if (isAbleToTackle && Input.GetKeyDown(KeyCode.LeftShift))
            Tackle();

        if (hasSwitchingBulletInput())
            switchCurrentBullet();

        if (hasStopped() && isAbleToRoll)
        {
            rollNewSpecial();
        }

        if (hasTurret && hasShootingInput())
            fireTurret();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)     return;
        if (NetworkGameManager.gameover) return;

        float moveHorizontal = Input.GetAxis("Horizontal") * torque ;
        float moveVertical = Input.GetAxis("Vertical") * torque ;

        getLookDirection();

        rb.AddTorque(rotateHorizontal * moveHorizontal);
        rb.AddTorque(rotateVertical * moveVertical);

        rb.AddForce(forwardForce * moveVertical * 0.002f);
        rb.AddForce(rightForce * moveHorizontal * 0.002f);        
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

        bullet.GetComponent<Rigidbody>().velocity = direction * speed;
        bulletScript.owner = this;

        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdBladedancer(int current)
    {
        var special = (GameObject)Instantiate(
            specialPrefab[current],
            transform.position,
            Quaternion.identity);

        BladeDancer script = special.GetComponent<BladeDancer>();
        script.owner = this;

        NetworkServer.Spawn(special);
    }

    [Command]
    void CmdBubbleshield(int current)
    {
        var special = (GameObject)Instantiate(
            specialPrefab[current],
            transform.position,
            Quaternion.identity);

        BubbleShield script = special.GetComponent<BubbleShield>();
        script.owner = this;

        NetworkServer.Spawn(special);
    }

    [Command]
    void CmdTurretship(int current)
    {
        var special = (GameObject)Instantiate(
            specialPrefab[current],
            transform.position,
            Quaternion.identity);

        TurretShip script = special.GetComponent<TurretShip>();
        script.owner = this;

        NetworkServer.Spawn(special);
    }

    [Command]
    void CmdTurretFire(Vector3 offset, Vector3 direction, float speed)
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefabs[0],
            transform.position + (offset) + direction * 2,
            Quaternion.identity);

        BulletStandardDestroy bulletScript = bullet.GetComponent<BulletStandardDestroy>();

        bullet.GetComponent<Rigidbody>().velocity = direction * speed;
        bulletScript.owner = this;

        NetworkServer.Spawn(bullet);
    }

    void fireTurret()
    {
        if (Time.time > 0.2 + lastTurretShot)
        {
            CmdTurretFire(getRight(), getForward(), 50);
            CmdTurretFire(-getRight(), getForward(), 50);

            lastTurretShot = Time.time;
        }
    }

    [Command]
    void CmdExplosion(int current)
    {
        GameObject special = (GameObject)Instantiate(
            specialPrefab[current],
            transform.position,
            Quaternion.identity);

        Explosion script = special.GetComponent<Explosion>();
        script.owner = this;

        NetworkServer.Spawn(special);
    }

    void checkSpecialSkill()
    {
        switch (rollValue)
        {
            case 0:
                Debug.Log("Error: 0 was used as special Skill!");
                break;
            case 1:

                break;
            case 2:
                CmdExplosion(rollValue);
                break;
            case 3:
                CmdTurretship(rollValue);
                hasTurret = true;
                StartCoroutine(disableTurret(10.9f));
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(10.9f);
                break;
            case 4:
                CmdBladedancer(rollValue);
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(15.9f);
                break;
            case 5:
                CmdBubbleshield(rollValue);
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(10.9f);
                break;
            case 6:
                break;
            default:
                break;
        }

        hasSpecialSkill = false;
        rollValue = 0;
        uiScript.changeSpecialIcon(rollValue);
    }

    void Jump()
    {
        isGrounded = false;
        rb.AddForce(Vector3.up * jumpSpeed);
    }

    void Tackle()
    {
        isAbleToTackle = false;
        rb.AddTorque(rotateHorizontal * 20, ForceMode.Impulse);
        rb.AddForce(forwardForce * 15, ForceMode.Impulse);
        StartCoroutine(enableTackle(10.0f));
    }

    private IEnumerator enableTackle(float seconds)
    {
        GameObject.FindGameObjectWithTag("RushPanel").GetComponent<RushPanel>().setNoRush();
        yield return new WaitForSeconds(seconds);
        isAbleToTackle = true;
        GameObject.FindGameObjectWithTag("RushPanel").GetComponent<RushPanel>().setRush();

    }

    private IEnumerator disableTurret(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasTurret = false;
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

    void rollNewSpecial()
    {
        rollValue = roll();

        if (ui == null || uiScript == null)
        {
            ui = GameObject.FindGameObjectWithTag("UI");
            uiScript = ui.GetComponent<UI>();
        }
        hasSpecialSkill = true;
        isAbleToRoll = false;
        GameObject.FindGameObjectWithTag("RollCooldown").GetComponent<CountdownScript>().startCountdownSeconds(0.1f);
        uiScript.changeSpecialIcon(rollValue);
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

    int roll()
    {
        if (transform.up.y > 0.89f)
        {
            return 1;
        }
        else if ((-transform.up).y > 0.89f)
        {
            return 6;
        }
        else if (transform.forward.y > 0.89f)
        {
            return 3;
        }
        else if ((-transform.forward).y > 0.89f)
        {
            return 5;
        }
        else if (transform.right.y > 0.89f)
        {
            return 4;
        }
        else if ((-transform.right).y > 0.89f)
        {
            return 2;
        }

        return 0;
    }

    bool hasStopped()
    {
        if((rb.velocity.x == 0.0f) && (rb.velocity.y == 0.0f) && (rb.velocity.x == 0.0f))
        {
            return true;
        }

        return false;
    }

    void getLookDirection()
    {
        forwardForce.x = playerCameraObject.transform.forward.x;
        forwardForce.y = 0.0f;
        forwardForce.z = playerCameraObject.transform.forward.z;

        rightForce.x = playerCameraObject.transform.right.x;
        rightForce.y = 0.0f;
        rightForce.z = playerCameraObject.transform.right.z;

        rotateHorizontal.x = -playerCameraObject.transform.forward.x;
        rotateHorizontal.y = 0.0f;
        rotateHorizontal.z = -playerCameraObject.transform.forward.z;

        rotateVertical.x = playerCameraObject.transform.right.x;
        rotateVertical.y = 0.0f;
        rotateVertical.z = playerCameraObject.transform.right.z;

        bulletOffset.x = playerCameraObject.transform.forward.x;
        bulletOffset.y = 0.0f;
        bulletOffset.z = playerCameraObject.transform.forward.z;

        bulletDirection.x = playerCameraObject.transform.forward.x;
        bulletDirection.y = 0.0f;
        bulletDirection.z = playerCameraObject.transform.forward.z;
    }

    public void setIsAbleToRoll(bool value)
    {
        isAbleToRoll = value;
    }

    public int getLatency()
    {
        if(nClient == null)
        {
            return -1;
        }
        else
        {
            return nClient.GetRTT();
        }
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
        if (isLocalPlayer)
        {
            int val = newValue - score;
            var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
            if (newValue > 0.0f)
                animation.startDamageAnimation(val);
        }

        score = newValue;        
    }

    void OnKillsChanged(int newValue)
    {
        if (isLocalPlayer)
        {
            var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
            animation.startKillAnimation();
        }
        kills = newValue;
    }

    void OnDeathsChanged(int newValue)
    {
        deaths = newValue;
    }
    
    public Vector3 getForward()
    {
        return forwardForce;
    }

    public Vector3 getRight()
    {
        return rightForce;
    }

}

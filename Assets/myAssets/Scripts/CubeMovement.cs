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
    [SerializeField]private GameObject cubeIconPrefab;
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
    private bool hasSpecialSkill = false;
    private GameObject ui;
    private UI uiScript;
    private NetworkClient nClient;
    private int latency;
    private bool hasTurret = false;
    private float lastTurretShot = 0.0f;
    private float timeWithoutMovement = 0.0f;

    private bool loadingBomb = false;
    private float bombTimer = 0.0f;
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
    
    void Start()
    {
        NetworkGameManager.sPlayers.Add(this);
        GetComponent<MeshRenderer>().material.color = color;
        GameObject cubeIcon = (GameObject)Instantiate(cubeIconPrefab, transform.position, Quaternion.identity);
        cubeIcon.GetComponent<MeshRenderer>().material.color = color;
        cubeIcon.GetComponent<IconFollow>().followObject = this.gameObject;
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
        
    }
    #region Update Functions
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
            checkSpecialSkill();

        if (!hasSecondaryShootingInput())
            loadingBomb = false;

        if (!loadingBomb)
            bombTimer -= Time.deltaTime;

        if (isGrounded && (Input.GetButton("Jump") || Input.GetAxis("LeftTrigger") > 0))
            Jump();

        if (Input.GetKeyDown(KeyCode.Backspace))
            toggleLethalBullets();


        if (isAbleToTackle && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButton("Rush")))
            Tackle();

            switchCurrentBullet();

        if (hasStopped())
        {
            timeWithoutMovement += Time.deltaTime;
        }
        else
        {
            timeWithoutMovement = 0.0f;
        }
        
        if (hasStopped() && timeWithoutMovement > 4.99f)
        {
            rollNewSpecial();
        }

        if (hasTurret && hasShootingInput())
            fireTurret();

        if (timeWithoutMovement > 5.0f)
            timeWithoutMovement = 5.0f;

        if (bombTimer < 0.0f)
            bombTimer = 0.0f;

        if (bombTimer > 3.99f)
            bombTimer = 4.0f;
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
    #endregion

    void OnCollisionStay() //check if you are colliding with the ground
    {
        isGrounded = true;
    }

    #region [Commands]
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
            bulletPrefabs[2],
            transform.position + (offset) + direction * 2,
            Quaternion.identity);

        BulletStandardDestroy bulletScript = bullet.GetComponent<BulletStandardDestroy>();

        bullet.GetComponent<Rigidbody>().velocity = direction * speed;
        bulletScript.owner = this;

        NetworkServer.Spawn(bullet);
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
    #endregion
    
    void fireTurret()
    {
        if (Time.time > 0.3 + lastTurretShot)
        {
            CmdTurretFire(getRight(), getForward(), 50);
            CmdTurretFire(-getRight(), getForward(), 50);

            lastTurretShot = Time.time;
        }
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
                if(bombTimer > 3.98f)
                {
                    CmdExplosion(rollValue);
                    adjustSpecialSkill();
                    bombTimer = 0.0f;
                }

                bombTimer += Time.deltaTime;
                loadingBomb = true;
                break;
            case 3:
                CmdTurretship(rollValue);
                hasTurret = true;
                StartCoroutine(disableTurret(15.9f));
                adjustSpecialSkill();
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(15.9f);
                break;
            case 4:
                CmdBladedancer(rollValue);
                adjustSpecialSkill();
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(25.9f);
                break;
            case 5:
                CmdBubbleshield(rollValue);
                adjustSpecialSkill();
                GameObject.FindGameObjectWithTag("SpecialActivationtime").GetComponent<CountdownScript>().startCountdownSeconds(15.9f);
                break;
            case 6:
                break;
            default:
                break;
        }

    }

    void adjustSpecialSkill()
    {
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
        StartCoroutine(enableTackle(5.0f));
    }

    private IEnumerator enableTackle(float seconds)
    {
        GameObject.FindGameObjectWithTag("RushPanel").GetComponent<RushPanel>().startCooldown();
        yield return new WaitForSeconds(seconds);
        isAbleToTackle = true;
    }

    private IEnumerator disableTurret(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasTurret = false;
    }

    void switchCurrentBullet()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetAxis("DPadY") < 0))
        {
            bullet = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || (Input.GetAxis("DPadX") < 0))
        {
            bullet = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || (Input.GetAxis("DPadY") > 0))
        {
            bullet = 2;
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            bullet++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 ) // back
        {
            bullet--;
        }

        bullet = Mathf.Clamp(bullet,0,bulletPrefabs.Length - 1);
        if(ui == null || uiScript == null)
        {
            ui = GameObject.FindGameObjectWithTag("UI");
            if(ui != null)
                uiScript = ui.GetComponent<UI>();
        }
        if(uiScript != null)
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
        timeWithoutMovement = 0.0f;
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
        if(Input.GetButton("Fire1") || Input.GetAxis("RightTrigger") > 0)
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
        if (Input.GetButton("Fire2"))
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

    public float getTimeWithoutMovement()
    {
        return timeWithoutMovement;
    }

    public float getBombLoadingTime()
    {
        return bombTimer;
    }

    void OnDestroy()
    {
        if (mainCamera != null)
            mainCamera.SetActive(true);

        Destroy(playerCameraObject);
        Destroy(miniMapCameraObject);
        NetworkGameManager.sPlayers.Remove(this);
    }

}

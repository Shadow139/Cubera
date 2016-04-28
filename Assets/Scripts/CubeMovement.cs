using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CubeMovement : NetworkBehaviour
{
    public float torque;
    public float jumpSpeed;
    public GameObject[] bulletPrefabs;
    private int bullet = 0;
    private float oldDamage = 0.0f;
    public Vector3 bulletOffset;
    public Vector3 bulletDirection = Vector3.forward;
    public Camera cameraPrefab;
    public Camera miniMapPrefab;
    private float lastShot = 0.0f;
        
    private Rigidbody rigidbody;
    private BulletStandardDestroy bulletScript;
    private GameObject cam;
    private GameObject miniMap;

    private bool isGrounded = false;
    private int mode = 0;
    private Vector3 rotateHorizontal = Vector3.back;
    private Vector3 rotateVertical = Vector3.right;
    private Vector3 forwardForce = Vector3.forward;
    private Vector3 rightForce = Vector3.right;

    public GameObject ui;
    private UI uiScript;

    private GameObject mainCamera;

    public static GameObject player;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Camera playerCamera = (Camera)Camera.Instantiate(cameraPrefab, new Vector3(0, 3.5f, -6.5f), Quaternion.AngleAxis(15,Vector3.right));
        Camera miniMapCamera = (Camera)Camera.Instantiate(miniMapPrefab, new Vector3(0, 120, 0), Quaternion.AngleAxis(90, Vector3.right));
        cam = GameObject.Find("prfPlayerCamera(Clone)");
        miniMap = GameObject.Find("prfMiniMap(Clone)");
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);
        rigidbody = GetComponent<Rigidbody>();
        ui = GameObject.FindGameObjectWithTag("UI");
        //uiScript = ui.GetComponent<UI>();
        

    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
            bulletScript = bulletPrefabs[bullet].GetComponent<BulletStandardDestroy>();
            if (Time.time > bulletScript.rateOfFire + lastShot)
            {
                CmdFire(bullet,bulletOffset, bulletDirection, bulletScript.bulletSpeed);
                //RpcFire(bullet, bulletOffset, bulletDirection, bulletScript.bulletSpeed);

                lastShot = Time.time;
            }
        }

        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            toggleLethalBullets();
        }
        if(isSwitchingBullet())
            switchCurrentBullet();
    }

    void FixedUpdate()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal") * torque ;
        float moveVertical = Input.GetAxis("Vertical") * torque ;

        rigidbody.AddTorque(rotateHorizontal * moveHorizontal);
        rigidbody.AddTorque(rotateVertical * moveVertical);

        rigidbody.AddForce(forwardForce * moveVertical * 0.002f);
        rigidbody.AddForce(rightForce * moveHorizontal * 0.002f);
        
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        player = gameObject;

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

        Destroy(bullet, 5.0f);
    }

    [ClientRpc]
    void RpcFire(int current, Vector3 offset, Vector3 direction, float speed)
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

        Destroy(bullet, 5.0f);
    }

    void Jump()
    {
        isGrounded = false;
        rigidbody.AddForce(Vector3.up * jumpSpeed);
    }

    void Dash(float moveHorizontal, float moveVertical)
    {
        if(moveHorizontal < 0)
        {
            rigidbody.AddForce(Vector3.left * 100);
        }
        else if (moveHorizontal > 0)
        {
            rigidbody.AddForce(Vector3.right * 100);
        }

        if (moveVertical < 0)
        {
            rigidbody.AddForce(Vector3.back * 100);
        }
        else if (moveVertical > 0)
        {
            rigidbody.AddForce(Vector3.forward * 100);
        }
        rigidbody.AddForce(Vector3.up * 10);

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

    bool isSwitchingBullet()
    {
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

    void OnDestroy()
    {
        if(mainCamera != null)
            mainCamera.SetActive(true);

        Destroy(cam);
        Destroy(miniMap);
    }

}

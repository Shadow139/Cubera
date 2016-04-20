using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CubeMovement : NetworkBehaviour
{
    public float torque;
    public float jumpSpeed;
    public GameObject bulletPrefab;
    public Vector3 bulletOffset;
    public Vector3 bulletDirection = Vector3.forward;
    private float lastShot = 0.0f;
    
    private Rigidbody rigidbody;
    private BulletStandardDestroy bulletScript;
    private Camera cam;
    private bool isGrounded = false;
    private int mode = 0;
    private Vector3 rotateHorizontal = Vector3.back;
    private Vector3 rotateVertical = Vector3.right;

    public static GameObject player;

    void Start()
    {
        cam = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            bulletScript = bulletPrefab.GetComponent<BulletStandardDestroy>();
            if (Time.time > bulletScript.rateOfFire + lastShot)
            {
                CmdFire(bulletOffset, bulletDirection, bulletScript.bulletSpeed);
                lastShot = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                toggleLethalBullets();
            }
        }
    }

    void FixedUpdate()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal") * torque * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * torque * Time.deltaTime;

        rigidbody.AddTorque(rotateHorizontal * moveHorizontal);
        rigidbody.AddTorque(rotateVertical * moveVertical);


        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
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
    void CmdFire(Vector3 offset, Vector3 direction, float speed)
    {

        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            transform.position + offset,
            Quaternion.identity);

        // Add velocity to the bullet
        //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        bullet.GetComponent<Rigidbody>().velocity = direction * speed;

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
            bulletOffset = Vector3.forward;
            bulletDirection = Vector3.forward;
        }
        else if(mode == 1)
        {
            rotateHorizontal = Vector3.left;
            rotateVertical = Vector3.back;
            bulletOffset = Vector3.right;
            bulletDirection = Vector3.right;
        }
        else if (mode == 2)
        {
            rotateHorizontal = Vector3.forward;
            rotateVertical = Vector3.left;
            bulletOffset = Vector3.back;
            bulletDirection = Vector3.back;            
        }
        else if (mode == 3)
        {
            rotateHorizontal = Vector3.right;
            rotateVertical = Vector3.forward;
            bulletOffset = Vector3.left;
            bulletDirection = Vector3.left;
        }

    }

    void toggleLethalBullets()
    {
        bulletPrefab.GetComponent<BulletStandardDestroy>().destroy = !(bulletPrefab.GetComponent<BulletStandardDestroy>().destroy);

        if(bulletPrefab.GetComponent<BulletStandardDestroy>().damage > 0)
        {
            bulletPrefab.GetComponent<BulletStandardDestroy>().damage = 0;
        }
        else
        {
            bulletPrefab.GetComponent<BulletStandardDestroy>().damage = 5.0f;
        }

    }

}

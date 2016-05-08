using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TurretShip : NetworkBehaviour
{

    public CubeMovement owner;
    public GameObject bulletPrefab;
    public Transform target;

    public Vector3 bulletDirection;

    private float lastShot = 0.0f;
    
    void Start () {
        Destroy(gameObject, 10.0f);
        setOwnerOnClient();        
        
        target = owner.gameObject.transform;
        bulletPrefab = owner.bulletPrefabs[0];
	}
	
	void Update () {

        transform.position = target.position;
        transform.LookAt(target.position + owner.getForward());

        if (!isServer) return;

        if (Time.time > 0.2 + lastShot)
        {
            bulletDirection = owner.getForward();
            CmdFire(owner.getRight(), bulletDirection, 50);
            CmdFire(-owner.getRight(), bulletDirection, 50);

            lastShot = Time.time;
        }
    }

    [Command]
    void CmdFire(Vector3 offset, Vector3 direction, float speed)
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            transform.position + (offset) + transform.forward * 2,
            Quaternion.identity);

        BulletStandardDestroy bulletScript = bullet.GetComponent<BulletStandardDestroy>();

        bullet.GetComponent<Rigidbody>().velocity = direction * speed;
        bulletScript.owner = owner;

        NetworkServer.Spawn(bullet);
    }

    void setOwnerOnClient()
    {
        if (isClient)
        {
            GameObject[] gObjs = GameObject.FindGameObjectsWithTag("Player");

            float min = 99999.0f;

            foreach (GameObject g in gObjs)
            {
                float dist = Vector3.Distance(transform.position, g.transform.position);

                if (dist < min)
                {
                    min = dist;
                    owner = g.GetComponent<CubeMovement>();
                }
            }
        }
    }
}

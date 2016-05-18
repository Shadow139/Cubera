using UnityEngine;
using System.Collections;

public class MiniMapFollow : MonoBehaviour {

    public float miniMapHeight = 100.0f;
    public float compassRotationSpeed = 1f;
    public float compassOrientationOffset = 0f;

    public RectTransform north;

    private GameObject player;
    private Transform playerTransform;

    void Start()
    {
        north = GameObject.FindGameObjectWithTag("North").GetComponent<RectTransform>();
    }

    void LateUpdate () {
        if (!player)
        {
            player = CubeMovement.player;
            playerTransform = player.transform;
        }
        if (!north){
            north = GameObject.FindGameObjectWithTag("North").GetComponent<RectTransform>();
        }

        adjustMinimapToPlayer();

    }

    void adjustMinimapToPlayer()
    {
        if (player != null)
        {
            float temp = -SignedAngleBetween(Vector3.forward, player.GetComponent<CubeMovement>().getForward(), Vector3.up);

            transform.position = new Vector3(player.transform.position.x, miniMapHeight, player.transform.position.z);
            //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, temp, transform.rotation.eulerAngles.z));

            if(north != null)
                north.transform.rotation = Quaternion.Euler(0,0,-temp);

            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(90, 0, temp + compassOrientationOffset),Time.deltaTime * compassRotationSpeed);
        }
    }

    float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }
}

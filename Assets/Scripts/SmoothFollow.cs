using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    private GameObject player;
    private Vector3 cameraOffset;

    void Start()
    {
        //player = GameObject.Find("prfPlayer(Clone)");
        player = CubeMovement.player;
        cameraOffset = transform.position;
        Debug.Log("cameraOffset " + cameraOffset);
    }

    void LateUpdate()
    {
        if (!player)
            player = CubeMovement.player;
           // player = GameObject.Find("prfPlayer(Clone)");

        if (player != null)
            transform.position = player.transform.position + cameraOffset;

        if (Input.GetKeyDown(KeyCode.E))
        {
            rotateCameraRight();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotateCameraLeft();
        }        
    }

    void rotateCameraRight()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, 90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(1);
        Debug.Log("cameraOffset " + cameraOffset);

    }

    void rotateCameraLeft()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, -90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(-1);
        Debug.Log("cameraOffset " + cameraOffset);

    }

}

﻿using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    private GameObject player;
    private Vector3 cameraOffset;

    void Start()
    {
        player = CubeMovement.player;
        cameraOffset = transform.position;
    }

    void LateUpdate()
    {
        if (!player)
            player = CubeMovement.player;

        if (player != null)
            transform.position = player.transform.position + cameraOffset;

        if (Input.GetKeyDown(KeyCode.E) | Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            rotateCameraRight();
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            rotateCameraLeft();
        }        
    }

    void rotateCameraRight()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, 90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(1);
    }

    void rotateCameraLeft()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, -90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(-1);
    }

    void rotateCameraRightSmooth()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, 90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(1);
    }

    void rotateCameraLeftSmooth()
    {
        transform.RotateAround(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.up, -90);
        cameraOffset = transform.position - player.transform.position;
        player.GetComponent<CubeMovement>().switchMode(-1);
    }

}
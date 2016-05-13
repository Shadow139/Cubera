using UnityEngine;
using System.Collections;

public class MiniMapFollow : MonoBehaviour {

    private GameObject player;
    public float miniMapHeight = 100.0f;

    void LateUpdate () {
        if (!player)
            player = CubeMovement.player;


        if (player!=null)
            transform.position = new Vector3(player.transform.position.x, miniMapHeight, player.transform.position.z);
    }
}

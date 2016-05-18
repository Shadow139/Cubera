using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gizmos : MonoBehaviour
{
    public bool debug = true;

    public Text posX;
    public Text negX;
    public Text posY;
    public Text negY;
    public Text posZ;
    public Text negZ;

    public Text roll;
    public Text velocity;
    public Text latency;

    public Vector3 temp = Vector3.zero;
    
    void Start()
    {

    }

    void Update()
    {
        if (debug)
        {
            if (CubeMovement.player != null)
            {
                posX.text = CubeMovement.player.transform.right.ToString();
                posY.text = CubeMovement.player.transform.up.ToString();
                posZ.text = CubeMovement.player.transform.forward.ToString();

                negX.text = CubeMovement.player.transform.right * (-1) + "";
                negY.text = CubeMovement.player.transform.up * (-1) + "";
                negZ.text = CubeMovement.player.transform.forward * (-1) + "";

                velocity.text = "Velocity: " + CubeMovement.player.GetComponent<CubeMovement>().rb.velocity.ToString();
                roll.text = "Rolled: " + CubeMovement.player.GetComponent<CubeMovement>().rollValue;
                latency.text = showLatency();
            }

            if (CubeMovement.player != null)
            {
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + CubeMovement.player.transform.right, Color.red);
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + (-CubeMovement.player.transform.right), Color.red);
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + CubeMovement.player.transform.up, Color.green);
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + (-CubeMovement.player.transform.up), Color.green);
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + CubeMovement.player.transform.forward, Color.blue);
                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + (-CubeMovement.player.transform.forward), Color.blue);

                Debug.DrawLine(CubeMovement.player.transform.position, CubeMovement.player.transform.position + (CubeMovement.player.GetComponent<CubeMovement>().getForward()), Color.magenta);

            }
        }
        else
        {
            posX.text = "";
            posY.text = "";
            posZ.text = "";

            negX.text = "";
            negY.text = "";
            negZ.text = "";

            velocity.text = "";
            roll.text = "";
            latency.text = "";
        }
    }

    string showLatency()
    {
        int tmp = CubeMovement.player.GetComponent<CubeMovement>().getLatency();

        if (tmp < 100)
        {
            latency.color = Color.green;
        }
        else if (tmp >= 100 && tmp < 200)
        {
            latency.color = Color.yellow;
        }
        else
        {
            latency.color = Color.red;
        }

        return tmp + "";
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.Network;

public class CountdownScript : NetworkBehaviour
{

    public bool displaySecondsOnly;
    public float timeLeft = 300.0f;
    public bool stop = true;

    public bool isRollCooldown;
    public bool isGameCountdown;

    private int minutes;
    private int seconds;
    private int fraction;

    private Text textField;


    void Start () {
        textField = GetComponent<Text>();
        textField.text = "";

        if (displaySecondsOnly)
        {
            startCountdownSeconds(timeLeft);
        }
        else
        {
            startCountdown(timeLeft);
        }
    }

    void Update()
    {
        if (stop) return;
        timeLeft -= Time.deltaTime;

        minutes = (int)(timeLeft / 60);
        seconds = (int)timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0 || seconds < 0)
        {
            stop = true;
            minutes = 0;
            seconds = 0;

            textField.text = "";

            if (isRollCooldown)
            {
                textField.text = "";
                CubeMovement.player.GetComponent<CubeMovement>().setIsAbleToRoll(true);
            }
            if (isGameCountdown)
            {
                GameObject.FindGameObjectWithTag("TopPanel").GetComponent<LobbyTopPanel>().ToggleVisibility(true);
                GameObject.FindGameObjectWithTag("UI").GetComponent<PlayerScore>().setTabListener(false);
                GameObject.FindGameObjectWithTag("UI").GetComponent<PlayerScore>().setActive(true);
                GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<MouseCamera>().hideCursor = false;

                NetworkGameManager.gameover = true;
            }

        }
        //        fraction = (timeLeft * 100) % 100;
    }

    public void startCountdown(float from)
    {
        stop = false;
        timeLeft = from;
        Update();
        StartCoroutine(updateCoroutine());
    }

    public void startCountdownSeconds(float from)
    {
        stop = false;
        timeLeft = from;
        Update();
        StartCoroutine(updateCoroutineSeconds());
    }


    private IEnumerator updateCoroutine()
    {
        while (!stop)
        {
            textField.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator updateCoroutineSeconds()
    {
        while (!stop)
        {
            textField.text = seconds + "";
            yield return new WaitForSeconds(0.2f);
        }
    }

}

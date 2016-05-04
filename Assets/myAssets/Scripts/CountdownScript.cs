using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScript : MonoBehaviour {

    public float timeLeft = 300.0f;
    public bool stop = true;

    private int minutes;
    private int seconds;
    private int fraction;

    private Text textField;


    void Start () {
        textField = GetComponent<Text>();
        startCountdown(timeLeft);
    }

    void Update()
    {
        if (stop) return;
        timeLeft -= Time.deltaTime;

        minutes = (int)(timeLeft / 60);
        seconds = (int)timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0)
        {
            stop = true;
            minutes = 0;
            seconds = 0;
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


    private IEnumerator updateCoroutine()
    {
        while (!stop)
        {
            textField.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(0.2f);
        }
    }
}

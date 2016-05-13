using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeScript : MonoBehaviour {
    
private float startTime;
private string textTime;
public bool stop = true;
    
private float guiTime;

private int minutes;
private int seconds;
private int fraction;

private Text textField;
    

void Start()
    {
        startTime = Time.time;
        textField = GetComponent<Text>();
        startTimer();
    }

    void Update()
    {
        if (stop) return;

        guiTime = Time.time - startTime;

        minutes = (int)guiTime / 60; 
        seconds = (int)guiTime % 60;
        fraction = (int)(guiTime * 100) % 100;
    }

    public void startTimer()
    {
        stop = false;
        Update();
        StartCoroutine(updateCoroutine());
    }

    public void startTimerFractions()
    {
        stop = false;
        Update();
        StartCoroutine(updateCoroutineFractions());
    }

    private IEnumerator updateCoroutine()
    {
        while (!stop)
        {
            textField.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator updateCoroutineFractions()
    {
        while (!stop)
        {
            textField.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
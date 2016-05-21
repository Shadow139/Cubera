using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListOfKilledPlayers : MonoBehaviour {

    [SerializeField] private GameObject[] slots;
    [SerializeField] private CanvasGroup fadingList;
    private float elapsedTime = 0.0f;
    private bool isDisplayingFade = false;
    private int index = 0;

    public void addDeadPlayerToList(string p1, string p2, Vector4 p1Col, Vector4 p2Col)
    {
        if(index < 10)
        {
            checkIfDisabled();

            Text[] txt = slots[index % 10].GetComponentsInChildren<Text>();
            txt[0].text = p1;
            txt[0].color = p1Col;
            txt[1].text = p2;
            txt[1].color = p2Col;
        }
        else
        {
            shiftList(p1, p2, p1Col, p2Col);
        }

        elapsedTime = 0.0f;
        if (!isDisplayingFade)
            StartCoroutine(startFadingScreen());

        index++;

    }

    void checkIfDisabled()
    {
        if (slots[index % 10].active == false)
            slots[index % 10].active = true;
    }

    void shiftList(string p1, string p2, Vector4 p1Col, Vector4 p2Col)
    {

        for (int i = 0; i < 9; i++)
        {

            Text[] txt_prev = slots[i].GetComponentsInChildren<Text>();
            Text[] txt = slots[i + 1].GetComponentsInChildren<Text>();

            txt_prev[0].text = txt[1].text;
            txt_prev[0].color = txt[1].color;

            txt_prev[1].text = txt[1].text;
            txt_prev[1].color = txt[1].color;
        }

        Text[] txt_last = slots[9].GetComponentsInChildren<Text>();
        txt_last[0].text = p1;
        txt_last[0].color = p1Col;
        txt_last[1].text = p2;
        txt_last[1].color = p2Col;

    }

    private IEnumerator startFadingScreen()
    {
        isDisplayingFade = true;
        elapsedTime = 0.0f;
        float wait = 5.0f;
        fadingList.alpha = 1.0f;

        yield return null;

        while (elapsedTime < wait)
        {
            if(elapsedTime > 3.0f)
                fadingList.alpha = 1.0f - (elapsedTime - 3.0f / wait - 3.0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        fadingList.alpha = 0.0f;
        isDisplayingFade = false;
    }

}

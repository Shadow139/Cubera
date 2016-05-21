using UnityEngine;
using System.Collections;

public class SplashScreenFader : MonoBehaviour {

    [SerializeField] private CanvasGroup m_FadingScreen;
    
    void Start () {
        StartCoroutine(startFading());
	}	

    private IEnumerator startFading()
    {
        float elapsedTime = 0.0f;
        float wait = 0.6f;
        m_FadingScreen.alpha = 1.0f;

        yield return new WaitForSeconds(3.0f);

        while (elapsedTime < wait)
        {
            m_FadingScreen.alpha = 1.0f - (elapsedTime / wait);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        m_FadingScreen.blocksRaycasts = false;
        m_FadingScreen.alpha = 0.0f;
    }
}

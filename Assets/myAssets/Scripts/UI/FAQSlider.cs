using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FAQSlider : MonoBehaviour {

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private GameObject xbox;
    [SerializeField]
    private GameObject pc;
    [SerializeField]
    private GameObject pcControls;
    [SerializeField]
    private GameObject xboxControls;
    [SerializeField]
    private GameObject specialSkills;
    [SerializeField]
    private GameObject powerUps;
    [SerializeField]
    private GameObject powerUpSizes;

	// Update is called once per frame
	void Update () {
	
	}

    public void SetSliderValue()
    {
        int i = (int)slider.value;

        switch (i)
        {
            case 0:
                specialSkills.SetActive(false);
                powerUps.SetActive(false);
                powerUpSizes.SetActive(false);
                break;
            case 1:
                specialSkills.SetActive(true);
                powerUps.SetActive(false);
                powerUpSizes.SetActive(false);
                break;
            case 2:
                specialSkills.SetActive(false);
                powerUps.SetActive(true);
                powerUpSizes.SetActive(false);
                break;
            case 3:
                specialSkills.SetActive(false);
                powerUps.SetActive(false);
                powerUpSizes.SetActive(true);
                break;
            default:
                break;
        }

     }
}

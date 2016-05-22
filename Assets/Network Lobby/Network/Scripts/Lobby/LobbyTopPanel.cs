using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;
        public bool isInLoadingScreen = false;

        protected bool isDisplayed = true;
        protected Image panelImage;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }


        void Update()
        {
            if (!isInGame || isInLoadingScreen)
                return;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        #region Default Values
        [Header("Default Menu Values")]
        [SerializeField] private float defaultVolume;

        [Header("Levels To Load")]
        public int newGameIndex;
        private string levelToLoad;

        private int menuNumber;
        #endregion

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject GeneralSettingsCanvas;
        [SerializeField] private GameObject controlMenu;
        [SerializeField] private GameObject soundMenu;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        #endregion

        #region Slider Linking
        [Header("Menu Sliders")]
        [SerializeField] private Text volumeText;
        [SerializeField] private Slider volumeSlider;
        [Space(10)]
        #endregion

        public AudioSource BGM;

        public GameObject mainMenuFirstBtn, newGameDialogFirstBtn, loadGameDialogFirstBtn, noSaveGameDialogFirstBtn, settingsMenuFirstBtn;
        public GameObject controlMenuFirstBtn, soundMenuFirstBtn;

        private void Start()
        {
            LoadGameSetting();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenuFirstBtn);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menuNumber == 2 || menuNumber == 7 || menuNumber == 8)
                {
                    GoBackToMainMenu();
                    ClickSound();
                }
                else if (menuNumber == 3 || menuNumber == 4 || menuNumber == 5)
                {
                    GoBackToOptionsMenu();
                    ClickSound();
                }
            }
        }

        private void LoadGameSetting()
        {
            float volume = PlayerPrefs.GetFloat("masterVolume", defaultVolume) * 100;

            AudioListener.volume = volume / 100;

            volumeText.text = volume.ToString("0");
            volumeSlider.value = volume * 100;
        }

        private void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        public void HoverBtn()
        {
            GetComponent<AudioSource>().Play();

            EventSystem.current.SetSelectedGameObject(null);
        }

        #region Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {
            if (buttonType == "Control")
            {
                settingsMenuFirstBtn = GameObject.Find("Control Btn");
                GeneralSettingsCanvas.SetActive(false);
                controlMenu.SetActive(true);
                menuNumber = 3;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(controlMenuFirstBtn);
            }

            if (buttonType == "Sound")
            {
                settingsMenuFirstBtn = GameObject.Find("Sound Btn");
                GeneralSettingsCanvas.SetActive(false);
                soundMenu.SetActive(true);
                menuNumber = 4;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(soundMenuFirstBtn);
            }

            if (buttonType == "Exit")
            {
                Debug.Log("YES QUIT!");
                Application.Quit();
            }

            if (buttonType == "Options")
            {
                mainMenuFirstBtn = GameObject.Find("Options UI Btn");
                menuDefaultCanvas.SetActive(false);
                GeneralSettingsCanvas.SetActive(true);
                menuNumber = 2;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(settingsMenuFirstBtn);
            }

            if (buttonType == "LoadGame")
            {
                mainMenuFirstBtn = GameObject.Find("Load Game UI Btn");
                menuDefaultCanvas.SetActive(false);
                loadGameDialog.SetActive(true);
                menuNumber = 8;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(loadGameDialogFirstBtn);
            }

            if (buttonType == "NewGame")
            {
                mainMenuFirstBtn = GameObject.Find("New Game UI Btn");
                menuDefaultCanvas.SetActive(false);
                newGameDialog.SetActive(true);
                menuNumber = 7;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(newGameDialogFirstBtn);
            }
        }
        #endregion

        public void VolumeSlider(float volume)
        {
            AudioListener.volume = volume / 100f;
            volumeText.text = volume.ToString("0");
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            Debug.Log(PlayerPrefs.GetFloat("masterVolume"));
        }

        #region ResetButton
        public void ResetButton(string GraphicsMenu)
        {
            if (GraphicsMenu == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeText.text = (defaultVolume * 100).ToString("0");
                VolumeApply();
            }
        }
        #endregion

        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                SceneController.GoToScene(newGameIndex);
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }

        public void ClickLoadGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                string filePath = Application.dataPath + "/StreamingAssets" + "/Save/PlaySave/save.json";
                if (File.Exists(filePath))
                {
                    //LOAD LAST SAVED SCENE
                    try
                    {
                        DataBase.Singleton.Load();
                    }
                    catch (System.Exception)
                    {
                        menuDefaultCanvas.SetActive(false);
                        loadGameDialog.SetActive(false);
                        noSaveDialog.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(noSaveGameDialogFirstBtn);
                    }
                }
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }
        #endregion

        #region Back to Menus
        public void GoBackToOptionsMenu()
        {
            GeneralSettingsCanvas.SetActive(true);
            controlMenu.SetActive(false);
            soundMenu.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsMenuFirstBtn);

            VolumeApply();

            PlayerPrefs.Save();

            menuNumber = 2;
        }

        public void GoBackToMainMenu()
        {
            if (GameObject.Find("Control Btn"))
                settingsMenuFirstBtn = GameObject.Find("Control Btn");
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            noSaveDialog.SetActive(false);
            GeneralSettingsCanvas.SetActive(false);
            controlMenu.SetActive(false);
            soundMenu.SetActive(false);
            menuNumber = 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenuFirstBtn);
        }

        public void ClickQuitOptions()
        {
            GoBackToMainMenu();
        }

        public void ClickNoSaveDialog()
        {
            GoBackToMainMenu();
        }
        #endregion
    }
}

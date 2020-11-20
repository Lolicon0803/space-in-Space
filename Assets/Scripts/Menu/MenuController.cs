using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        #region Default Values
        [Header("Default Menu Values")]
        [SerializeField] private float defaultBrightness;
        [SerializeField] private float defaultVolume;
        [SerializeField] private int defaultSen;
        [SerializeField] private bool defaultInvertY;

        [Header("Levels To Load")]
        public string _newGameButtonLevel;
        private string levelToLoad;

        private int menuNumber;
        #endregion

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject GeneralSettingsCanvas;
        [SerializeField] private GameObject graphicsMenu;
        [SerializeField] private GameObject soundMenu;
        [SerializeField] private GameObject gameplayMenu;
        [SerializeField] private GameObject confirmationMenu;
        [SerializeField] private GameObject loadGameMenu;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        #endregion

        #region Slider Linking
        [Header("Menu Sliders")]
        [SerializeField] private Text controllerSenText;
        [SerializeField] private Slider controllerSenSlider;
        public float controlSenFloat = 2f;
        [Space(10)]
        [SerializeField] private Brightness brightnessEffect;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Text brightnessText;
        [Space(10)]
        [SerializeField] private Text volumeText;
        [SerializeField] private Text BGMvolumeText;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider BGMvolumeSlider;
        [Space(10)]
        #endregion

        public AudioSource BGM;

        public GameObject mainMenuFirstBtn, newGameDialogFirstBtn, loadGameMenuFirstBtn, loadGameDialogFirstBtn, noSaveGameDialogFirstBtn, settingsMenuFirstBtn;
        public GameObject graphicsMenuFirstBtn, soundMenuFirstBtn, gameplayMenuFirstBtn;

        //MAIN SECTION
        public IEnumerator ConfirmationBox()
        {
            confirmationMenu.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationMenu.SetActive(false);
        }

        private void Start()
        {
            LoadGameSetting();
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

                else if (menuNumber == 6) //CONTROLS MENU
                {
                    GoBackToGameplayMenu();
                    ClickSound();
                }
            }
        }

        private void LoadGameSetting()
        {
            float volume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
            float bgmVolume = PlayerPrefs.GetFloat("BGM", defaultVolume);

            AudioListener.volume = volume;
            BGM.volume = bgmVolume;

            volumeSlider.value = volume;
            volumeText.text = volume.ToString("0.0");
            BGMvolumeSlider.value = bgmVolume;
            BGMvolumeText.text = bgmVolume.ToString("0.0");
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
            if (buttonType == "Graphics")
            {
                settingsMenuFirstBtn = GameObject.Find("Graphics Btn");
                GeneralSettingsCanvas.SetActive(false);
                graphicsMenu.SetActive(true);
                menuNumber = 3;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(graphicsMenuFirstBtn);
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

            if (buttonType == "Gameplay")
            {
                settingsMenuFirstBtn = GameObject.Find("Gameplay Btn");
                GeneralSettingsCanvas.SetActive(false);
                gameplayMenu.SetActive(true);
                menuNumber = 5;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(gameplayMenuFirstBtn);
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
                loadGameMenu.SetActive(true);
                menuNumber = 8;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(loadGameMenuFirstBtn);
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
            AudioListener.volume = volume;
            volumeText.text = volume.ToString("0.0");
        }

        public void BGMVolumeSlider(float volume)
        {
            BGM.volume = volume;
            BGMvolumeText.text = volume.ToString("0.0");
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            Debug.Log(PlayerPrefs.GetFloat("masterVolume"));
            PlayerPrefs.SetFloat("BGM", BGM.volume);
            StartCoroutine(ConfirmationBox());
        }

        public void BrightnessSlider(float brightness)
        {
            brightnessEffect.brightness = brightness;
            brightnessText.text = brightness.ToString("0.0");
        }

        public void BrightnessApply()
        {
            PlayerPrefs.SetFloat("masterBrightness", brightnessEffect.brightness);
            Debug.Log(PlayerPrefs.GetFloat("masterBrightness"));
            StartCoroutine(ConfirmationBox());
        }

        public void ControllerSen()
        {
            controllerSenText.text = controllerSenSlider.value.ToString("0");
            controlSenFloat = controllerSenSlider.value;
        }

        public void GameplayApply()
        {
            PlayerPrefs.SetFloat("masterSen", controlSenFloat);
            Debug.Log("Sensitivity" + " " + PlayerPrefs.GetFloat("masterSen"));

            StartCoroutine(ConfirmationBox());
        }

        #region ResetButton
        public void ResetButton(string GraphicsMenu)
        {
            if (GraphicsMenu == "Brightness")
            {
                brightnessEffect.brightness = defaultBrightness;
                brightnessSlider.value = defaultBrightness;
                brightnessText.text = defaultBrightness.ToString("0.0");
                BrightnessApply();
            }

            if (GraphicsMenu == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeText.text = defaultVolume.ToString("0.0");
                VolumeApply();
            }

            if (GraphicsMenu == "Graphics")
            {
                controllerSenText.text = defaultSen.ToString("0");
                controllerSenSlider.value = defaultSen;
                controlSenFloat = defaultSen;

                GameplayApply();
            }
        }
        #endregion

        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                ScenesManager.nextScene();
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
                if (PlayerPrefs.HasKey("SavedLevel"))
                {
                    Debug.Log("I WANT TO LOAD THE SAVED GAME");
                    //LOAD LAST SAVED SCENE
                    levelToLoad = PlayerPrefs.GetString("SavedLevel");
                    SceneManager.LoadScene(levelToLoad);
                }

                else
                {
                    Debug.Log("Load Game Dialog");
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(false);
                    noSaveDialog.SetActive(true);

                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(noSaveGameDialogFirstBtn);
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
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsMenuFirstBtn);

            GameplayApply();
            BrightnessApply();
            VolumeApply();

            PlayerPrefs.Save();

            menuNumber = 2;
        }

        public void GoBackToMainMenu()
        {
            if (GameObject.Find("Graphics Btn"))
                settingsMenuFirstBtn = GameObject.Find("Graphics Btn");
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            loadGameMenu.SetActive(false);
            noSaveDialog.SetActive(false);
            GeneralSettingsCanvas.SetActive(false);
            graphicsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);
            menuNumber = 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenuFirstBtn);
        }

        public void GoBackToGameplayMenu()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameplayMenuFirstBtn);

            gameplayMenu.SetActive(true);
            menuNumber = 5;
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

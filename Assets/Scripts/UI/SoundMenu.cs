using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{

    private PlayerInputActions inputWrapper;
    private PlayerInputActions.UIActions controller;
    private InputButtonWrapper escapeButton;
    private bool initializedControls;


    [SerializeField] private Slider volume;
    [SerializeField] private Toggle whackySounds;
    [SerializeField] private Toggle easyMode;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button exitToMainMenu;
    [SerializeField] private Button confirmExit;
    [SerializeField] private Button menuOpenBtn;
    [SerializeField] private GameObject wholeMenu;

    private bool menuOpened = false;


    #region Input setup logic
    private void InitControls() {

        inputWrapper = new PlayerInputActions();
        controller = inputWrapper.UI;

        escapeButton = new InputButtonWrapper(controller.Escape);
        escapeButton.onButtonDown += (InputAction.CallbackContext ctx) => { if (menuOpened) { CloseMenu(); } else { OpenMenu(); } };

        initializedControls = true;
        controller.Enable();
    }


    private void OnEnable() {
        if (initializedControls) { controller.Enable(); }
    }

    private void OnDisable() {
        if (initializedControls) { controller.Disable(); }
    }

    #endregion



    public void Awake() {
        easyMode.onValueChanged.AddListener((bool selected) => GameManager.easyMode = selected);
        whackySounds.onValueChanged.AddListener(SetSounds);
        volume.onValueChanged.AddListener(SetVolume);
        continueBtn.onClick.AddListener(CloseMenu);
        exitToMainMenu.onClick.AddListener(() => { confirmExit.gameObject.SetActive(true); exitToMainMenu.enabled = false; EventSystem.current.SetSelectedGameObject(confirmExit.gameObject); });
        confirmExit.onClick.AddListener(() => { Time.timeScale = 1; ScreenTransition.Instance.LoadScene("MainMenu"); });

        menuOpenBtn.onClick.AddListener(() => {
            if (menuOpened)
                CloseMenu();
            else {
                OpenMenu();
            }
        });

        
        InitControls();
    }

    public void ReadValues() {
        volume.value = GetVolume();
        whackySounds.isOn = WackySoundsEnabled();
        easyMode.isOn = GameManager.easyMode;
    }



    public void CloseMenu() {
        if (menuOpened) {
            menuOpened = false;
            wholeMenu.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            exitToMainMenu.enabled = true;
            confirmExit.gameObject.SetActive(false);

            Time.timeScale = 1;
        }
    }

    public void OpenMenu() {
        if (!menuOpened) {
            menuOpened = true;
            ReadValues();
            wholeMenu.SetActive(true);

            EventSystem.current.SetSelectedGameObject(whackySounds.gameObject);

            Time.timeScale = 0;
        }
    }



    //MICHA PLEASE IMPLEMENT THE FOLLOWING FUNCTIONS
    public bool WackySoundsEnabled() {
        return SoundManager.Instance.GetIsHomeBrewActive();
    }
    public void SetSounds(bool enabled) {
        Debug.Log($"Whacky Sounds: {enabled}");
        SoundManager.Instance.ChangeHomeBrewMode(enabled);
    }
    public float GetVolume() {
        return SoundManager.Instance.GetMasterVolume();
    }
    public void SetVolume(float volume) {
        SoundManager.Instance.ChangeMasterVolume(volume);
        MusicManager.Instance.ChangeRelativeMasterVolume(volume);
    }
}

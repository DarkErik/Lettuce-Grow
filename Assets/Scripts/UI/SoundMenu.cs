using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{

    [SerializeField] private Slider volume;
    [SerializeField] private Toggle whackySounds;
    [SerializeField] private Toggle easyMode;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button menuOpenBtn;
    [SerializeField] private GameObject wholeMenu;

    private bool menuOpened = false;

    public void Awake() {
        easyMode.onValueChanged.AddListener((bool selected) => GameManager.easyMode = selected);
        whackySounds.onValueChanged.AddListener(SetSounds);
        volume.onValueChanged.AddListener(SetVolume);
        continueBtn.onClick.AddListener(CloseMenu);
        menuOpenBtn.onClick.AddListener(() => {
            if (menuOpened)
                CloseMenu();
            else {
                OpenMenu();
            }
        });
    }

    public void ReadValues() {
        volume.value = GetVolume();
        whackySounds.isOn = WackySoundsEnabled();
        easyMode.isOn = GameManager.easyMode;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menuOpened)
                CloseMenu();
            else
                OpenMenu();

        }
    }

    public void CloseMenu() {
        if (menuOpened) {
            menuOpened = false;
            wholeMenu.SetActive(false);
        }
    }

    public void OpenMenu() {
        if (!menuOpened) {
            menuOpened = true;
            ReadValues();
            wholeMenu.SetActive(true);
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

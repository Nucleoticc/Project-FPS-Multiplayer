using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("Sensitivity")]
    [SerializeField] Slider xSensitivitySlider;
    [SerializeField] InputField xSensitivityInput;
    [SerializeField] Slider ySensitivitySlider;
    [SerializeField] InputField ySensitivityInput;

    [Header("Crosshair")]
    [SerializeField] Image sampleImage;
    [SerializeField] Button[] crosshairButtons;
    [SerializeField] Slider[] crosshairColorSliders;
    [SerializeField] Slider crosshairSizeSlider;
    [SerializeField] Toggle dynamicCrosshairToggle;

    Sprite currentStaticCrosshair;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("xSensitivity", xSensitivitySlider.value);
        PlayerPrefs.SetFloat("ySensitivity", ySensitivitySlider.value);

        string imageName = sampleImage.sprite.name;
        PlayerPrefs.SetString("crosshairImage", imageName);

        PlayerPrefs.SetInt("crosshairSize", (int)crosshairSizeSlider.value);
        PlayerPrefs.SetInt("redValue", (int)crosshairColorSliders[0].value);
        PlayerPrefs.SetInt("greenValue", (int)crosshairColorSliders[1].value);
        PlayerPrefs.SetInt("blueValue", (int)crosshairColorSliders[2].value);
        PlayerPrefs.SetInt("alphaValue", (int)crosshairColorSliders[3].value);

        PlayerPrefs.SetInt("dynamicCrosshair", dynamicCrosshairToggle.isOn ? 1 : 0);
    }

    void LoadSettings()
    {
        xSensitivitySlider.value = PlayerPrefs.GetFloat("xSensitivity", 0.5f);
        ySensitivitySlider.value = PlayerPrefs.GetFloat("ySensitivity", 0.5f);

        string imageName = PlayerPrefs.GetString("crosshairImage", "crosshair005");
        sampleImage.sprite = Resources.Load<Sprite>(Path.Combine("Crosshairs", imageName));
        currentStaticCrosshair = sampleImage.sprite;

        crosshairSizeSlider.value = PlayerPrefs.GetInt("crosshairSize", 128);
        crosshairColorSliders[0].value = PlayerPrefs.GetInt("redValue", 255);
        crosshairColorSliders[1].value = PlayerPrefs.GetInt("greenValue", 255);
        crosshairColorSliders[2].value = PlayerPrefs.GetInt("blueValue", 255);
        crosshairColorSliders[3].value = PlayerPrefs.GetInt("alphaValue", 255);

        dynamicCrosshairToggle.isOn = PlayerPrefs.GetInt("dynamicCrosshair", 0) == 1;
    }

    public void SyncInputAndSliderX()
    {
        xSensitivityInput.text = xSensitivitySlider.value.ToString("F2");
    }

    public void SyncInputAndSliderY()
    {
        ySensitivityInput.text = ySensitivitySlider.value.ToString("F2");
    }

    public void SyncSliderAndImageSize()
    {
        sampleImage.rectTransform.sizeDelta = new Vector2(crosshairSizeSlider.value, crosshairSizeSlider.value);
    }

    public void SyncSliderAndImageColor()
    {
        float redValue = crosshairColorSliders[0].value / 255f;
        float greenValue = crosshairColorSliders[1].value / 255f;
        float blueValue = crosshairColorSliders[2].value / 255f;
        float alphaValue = crosshairColorSliders[3].value / 255f;

        sampleImage.color = new Color(redValue, greenValue, blueValue, alphaValue);
    }

    public void SetCrosshairImage(Image image)
    {
        dynamicCrosshairToggle.isOn = false;
        sampleImage.sprite = image.sprite;
        currentStaticCrosshair = image.sprite;
    }

    public void SetDynamicCrosshair()
    {
        if (dynamicCrosshairToggle.isOn)
        {
            sampleImage.sprite = Resources.Load<Sprite>(Path.Combine("Crosshairs", "crosshair012"));
        }
        else
        {
            sampleImage.sprite = currentStaticCrosshair;
        }
    }
}

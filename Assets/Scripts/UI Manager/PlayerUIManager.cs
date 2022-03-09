using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class PlayerUIManager : MonoBehaviour
{
    InputHandler inputHandler;

    public GunWeapon CurrentWeapon { get; set; } = null;
    public GunItem CurrentGunItem { get; set; } = null;

    [Header("Player Canvas")]
    [SerializeField] Canvas playerCanvas;

    [Header("Username")]
    [SerializeField] Text usernameText;

    [Header("Ammo and Weapon")]
    [SerializeField] Text ammoCounterText;

    [Header("HealthKit")]
    [SerializeField] Image healthKitImage;

    [Header("Crosshair")]
    [SerializeField] Image staticCrosshairImage;
    [SerializeField] RectTransform dynamicCrosshair;
    float restingSize;
    float maxSize;
    float currentSize;

    [Header("Grenade")]
    [SerializeField] Image fragGrenadeImage;
    [SerializeField] Image flashGrenadeImage;
    [SerializeField] Image smokeGrenadeImage;

    [Header("Death UI")]
    [SerializeField] GameObject deathUI;
    [SerializeField] Text deathMessageText;
    [SerializeField] Text countdownText;

    [Header("Escape")]
    [SerializeField] public GameObject escape;

    Color transparentColor;
    int countDown = 10;

    bool isUsingDynamicCrosshair = false;
    public bool isReloadingWeapon { private get; set; } = false;

    PhotonView pV;

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();

        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (pV.IsMine)
        {
            usernameText.gameObject.SetActive(false);
        }
        else
        {
            playerCanvas.gameObject.SetActive(false);
        }
        usernameText.text = pV.Owner.NickName;

        transparentColor = new Color(1, 1, 1, 0);

        SetCrosshair();
    }

    void Update()
    {
        UpdateBulletCounter();
        UpdateCrosshair();
        HandleScoreboardInput();
        HandleEscapeInput();
        if (isUsingDynamicCrosshair)
        {
            HandleDynamicCrosshair();
        }
    }

    void UpdateBulletCounter()
    {
        if (CurrentWeapon != null)
        {
            ammoCounterText.text = $"{CurrentWeapon.CurrentAmmo}/{CurrentWeapon.ReserveAmmo}";
        }
        else
        {
            ammoCounterText.text = "";
        }
    }

    void HandleScoreboardInput()
    {
        if (inputHandler.showScoreboardInput)
        {
            ScoreboardUIManager.instance.ShowScoreboard();
        }
        else
        {
            ScoreboardUIManager.instance.HideScoreboard();
        }
    }

    void HandleEscapeInput()
    {
        if (inputHandler.escapeInput)
        {
            inputHandler.escapeInput = false;
            escape.SetActive(!escape.activeSelf);
        }
    }

    #region Crosshair
    void UpdateCrosshair()
    {
        if (CurrentGunItem == null)
        {
            maxSize = 256f;
            return;
        }

        if (CurrentGunItem.itemName == "Beretta")
        {
            staticCrosshairImage.enabled = false;
            dynamicCrosshair.gameObject.SetActive(false);
        }
        else
        {
            if (isUsingDynamicCrosshair)
            {
                dynamicCrosshair.gameObject.SetActive(true);
            }
            else
            {
                staticCrosshairImage.enabled = true;
            }
        }

        if (CurrentGunItem.itemName == "Remington")
        {
            maxSize = staticCrosshairImage.rectTransform.sizeDelta.x + (CurrentWeapon.currentWeapon.spread.x * 1000f);
        }
        else
        {
            maxSize = staticCrosshairImage.rectTransform.sizeDelta.x + (CurrentWeapon.currentWeapon.spread.x * 7000f);
        }

        if (inputHandler.crouchInput)
        {
            maxSize = maxSize / 2f;
        }
    }

    void HandleDynamicCrosshair()
    {
        if (inputHandler.movementInput.magnitude > 0.1f || (inputHandler.primaryFireInput && !isReloadingWeapon))
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * 2f);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * 2f);
        }

        dynamicCrosshair.sizeDelta = new Vector2(currentSize, currentSize);
    }

    void SetCrosshair()
    {
        if (PlayerPrefs.GetInt("dynamicCrosshair", 0) == 1)
        {
            isUsingDynamicCrosshair = true;
            staticCrosshairImage.enabled = false;
            dynamicCrosshair.gameObject.SetActive(true);
        }
        else
        {
            staticCrosshairImage.enabled = true;
            dynamicCrosshair.gameObject.SetActive(false);
            string imageName = PlayerPrefs.GetString("crosshairImage", "crosshair005");
            staticCrosshairImage.sprite = Resources.Load<Sprite>(Path.Combine("Crosshairs", imageName));
        }

        staticCrosshairImage.rectTransform.sizeDelta = new Vector2(PlayerPrefs.GetInt("crosshairSize", 128), PlayerPrefs.GetInt("crosshairSize", 128));
        restingSize = staticCrosshairImage.rectTransform.sizeDelta.x;
        maxSize = staticCrosshairImage.rectTransform.sizeDelta.x * 2f;

        float redValue = PlayerPrefs.GetInt("redValue", 255) / 255f;
        float greenValue = PlayerPrefs.GetInt("greenValue", 255) / 255f;
        float blueValue = PlayerPrefs.GetInt("blueValue", 255) / 255f;
        float alphaValue = PlayerPrefs.GetInt("alphaValue", 255) / 255f;

        staticCrosshairImage.color = new Color(redValue, greenValue, blueValue, alphaValue);
    }
    #endregion

    #region Cooldowns
    public void healthKitCooldown(float duration)
    {
        healthKitImage.fillAmount = 0;
        StartCoroutine(healthKitCooldownCoroutine(duration));
    }

    IEnumerator healthKitCooldownCoroutine(float seconds)
    {
        float animationTime = 0f;
        while (animationTime < seconds)
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            healthKitImage.fillAmount = Mathf.Lerp(0, 1, lerpValue);
            yield return null;
        }

        healthKitImage.fillAmount = 1;
    }

    public void FragGrenadeCooldown(float duration)
    {
        fragGrenadeImage.color = transparentColor;
        StartCoroutine(GrenadeCooldownCoroutine(fragGrenadeImage, duration));
    }

    public void FlashGrenadeCooldown(float duration)
    {
        flashGrenadeImage.color = transparentColor;
        StartCoroutine(GrenadeCooldownCoroutine(flashGrenadeImage, duration));
    }

    public void SmokeGrenadeCooldown(float duration)
    {
        smokeGrenadeImage.color = transparentColor;
        StartCoroutine(GrenadeCooldownCoroutine(smokeGrenadeImage, duration));
    }

    IEnumerator GrenadeCooldownCoroutine(Image grenadeImage, float seconds)
    {
        float animationTime = 0f;
        while (animationTime < seconds)
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            grenadeImage.color = Color.Lerp(transparentColor, Color.white, lerpValue);
            yield return null;
        }

        grenadeImage.color = Color.white;
    }
    #endregion

    #region Death
    public void ShowDeathUI(string attacker)
    {
        countDown = 10;
        deathUI.SetActive(true);
        deathMessageText.text = $"You were killed by {attacker}";
        if (countDown == 0)
        {
            CancelInvoke("CountDown");
            deathUI.SetActive(false);
        }
        if (!IsInvoking("CountDown"))
        {
            InvokeRepeating("CountDown", 0f, 1f);
        }
    }

    void CountDown()
    {
        countdownText.text = $"{countDown}";
        countDown--;
    }
    #endregion
}

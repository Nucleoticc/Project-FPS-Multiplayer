using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class WeaponInventory : MonoBehaviourPunCallbacks
{
    InputHandler inputHandler;
    PlayerAnimationHandler playerAnimationHandler;
    PlayerAudioHandler playerAudioHandler;
    PlayerUIManager playerUIManager;
    PlayerManager playerManager;

    [Header("HealthKit Inventory")]
    [SerializeField] HealthKitItem healthKitItem;
    GameObject healthKitModel;
    GameObject healthKitFPSModel;
    public Animator healthKitAnimator { get; private set; }

    [Header("Weapon Inventory")]
    [SerializeField] List<Item> weapons = new List<Item>();
    List<GameObject> weaponModels = new List<GameObject>();
    List<GameObject> fpsWeaponModels = new List<GameObject>();
    public Animator CurrentWeaponAnimator { get; private set; }
    [HideInInspector] public Weapon currentWeapon;
    [HideInInspector] public Item currentWeaponItem;

    [Header("Weapon UI")]
    [SerializeField] Image weaponImage;

    // Weapon Index
    int currentWeaponIndex;
    int nextWeaponIndex;

    // Grenade Index
    int currentGrenadeIndex = 0;
    int[] grenadeIndexes = new int[3] { 6, 7, 8 };

    [Header("Weapon Holding Slots")]
    [SerializeField] Transform weaponHolderSlot;
    [SerializeField] public Transform fpsWeaponHolderSlot;

    PhotonView pV;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        playerAudioHandler = GetComponent<PlayerAudioHandler>();
        playerUIManager = GetComponent<PlayerUIManager>();
        playerManager = GetComponent<PlayerManager>();

        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        LoadAllWeapons();
        LoadHealthKit();
        ManagePlayerMesh();
    }

    void Update()
    {
        if (!pV.IsMine)
        {
            return;
        }
        LoadWeapon();
    }

    //Load All Weapons
    void LoadAllWeapons()
    {
        foreach (Item weapon in weapons)
        {
            GameObject model = Instantiate(weapon.modelPrefab, weaponHolderSlot);
            GameObject fpsModel = Instantiate(weapon.FpsWeaponPrefab, fpsWeaponHolderSlot);
            Transform currentWeaponMuzzleFlashPosition = fpsModel.transform.GetChild(0).Find("MuzzleFlashPosition");
            if (weapon is GunItem)
            {
                playerAudioHandler.AddSoundToDictionary(weapon.audioClip.name, weapon.audioClip);
                GunWeapon thisWeapon = model.GetComponent<GunWeapon>();
                GunItem gunItem = weapon as GunItem;
                thisWeapon.UpdateGunSettings(weapon as GunItem, currentWeaponMuzzleFlashPosition, gunItem.gunType);
            }
            else if (weapon is KnifeItem)
            {
                playerAudioHandler.AddSoundToDictionary(weapon.audioClip.name, weapon.audioClip);
                MeleeWeapon thisWeapon = model.GetComponent<MeleeWeapon>();
                MeleeDamage meleeDamage = fpsModel.GetComponent<MeleeDamage>();
                thisWeapon.UpdateMeleeSettings(weapon as KnifeItem, meleeDamage);
            }
            else if (weapon is GrenadeItem)
            {
                GrenadeWeapon thisWeapon = model.GetComponent<GrenadeWeapon>();
                Transform grenadePosition = fpsModel.transform.GetChild(0);
                thisWeapon.UpdateGrenadeSettings(weapon as GrenadeItem, grenadePosition);
            }
            model.transform.localScale = new Vector3(1f, 1f, 1f);
            model.SetActive(false);
            weaponModels.Add(model);
            fpsModel.SetActive(false);
            fpsWeaponModels.Add(fpsModel);
        }
        nextWeaponIndex = 5;
        ActivateWeapon();
    }

    void LoadHealthKit()
    {
        //3rd person model
        healthKitModel = Instantiate(healthKitItem.modelPrefab, weaponHolderSlot);
        healthKitModel.transform.localScale = new Vector3(1f, 1f, 1f);
        healthKitModel.transform.GetChild(0).gameObject.SetActive(false);

        //FPS model
        healthKitFPSModel = Instantiate(healthKitItem.FpsWeaponPrefab, fpsWeaponHolderSlot);
        healthKitFPSModel.SetActive(false);

        //Animator
        healthKitAnimator = healthKitFPSModel.GetComponent<Animator>();

        //Set HealthKit Settings
        HealthKit healthKit = healthKitModel.GetComponent<HealthKit>();
        healthKit.UpdateHealthKitSettings(healthKitItem);
    }

    void ManagePlayerMesh()
    {
        playerManager.fpsWeaponHolder = fpsWeaponHolderSlot;
        playerManager.thirdPersonWeapons = weaponModels.ToArray();
        playerManager.firstPersonModels = fpsWeaponModels.ToArray();
        playerManager.healthKitModel = healthKitModel;
        playerManager.healthKitFPSModel = healthKitFPSModel;
        playerManager.ManageMesh();
    }

    void LoadWeapon()
    {
        if (CurrentWeaponAnimator.GetBool("IsInteracting"))
        {
            return;
        }

        if (healthKitFPSModel.activeSelf)
        {
            return;
        }

        //Scroll Weapon
        if (inputHandler.mouseScrollInput < 0)
        {
            nextWeaponIndex++;
            if (nextWeaponIndex > 5)
            {
                nextWeaponIndex = 0;
            }
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.mouseScrollInput > 0)
        {
            nextWeaponIndex--;
            if (nextWeaponIndex < 0)
            {
                nextWeaponIndex = 5;
            }
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }

        //Use Keys
        if (inputHandler.switchRifleInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 0;
            inputHandler.switchRifleInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchPistolInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 1;
            inputHandler.switchPistolInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchShotgunInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 2;
            inputHandler.switchShotgunInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchSMGInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 3;
            inputHandler.switchSMGInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchSniperInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 4;
            inputHandler.switchSniperInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchMeleeInput)
        {
            currentGrenadeIndex = 0;
            nextWeaponIndex = 5;
            inputHandler.switchMeleeInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
        else if (inputHandler.switchGrenadeInput)
        {
            nextWeaponIndex = grenadeIndexes[currentGrenadeIndex++];
            if (currentGrenadeIndex >= grenadeIndexes.Length)
            {
                currentGrenadeIndex = 0;
            }
            inputHandler.switchGrenadeInput = false;
            if (currentWeaponIndex != nextWeaponIndex)
            {
                SwitchWeaponWithAnimation();
            }
        }
    }

    public void ActivateWeapon()
    {
        if (nextWeaponIndex >= weaponModels.Count)
        {
            return;
        }

        if (pV.IsMine && currentWeapon is SniperWeapon)
        {
            SniperWeapon sniperWeapon = currentWeapon as SniperWeapon;
            sniperWeapon.CloseScope();
        }

        for (int i = 0; i < weaponModels.Count; i++)
        {
            if (i == nextWeaponIndex)
            {
                if (pV.IsMine)
                {
                    fpsWeaponModels[i].SetActive(true);
                }
                weaponModels[i].SetActive(true);
                currentWeapon = weaponModels[i].GetComponent<Weapon>();
                currentWeaponItem = weapons[i];
                if (currentWeapon is GunWeapon)
                {
                    weaponImage.sprite = currentWeaponItem.itemIcon;
                    playerUIManager.CurrentWeapon = currentWeapon as GunWeapon;
                    playerUIManager.CurrentGunItem = currentWeaponItem as GunItem;
                    if (currentWeapon is PistolWeapon)
                    {
                        playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingPistolHash, true);
                        playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingGunHash, false);
                    }
                    else
                    {
                        playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingPistolHash, false);
                        playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingGunHash, true);
                    }
                }
                else if (currentWeapon is GrenadeWeapon)
                {
                    playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingPistolHash, false);
                    playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingGunHash, false);
                    playerUIManager.CurrentWeapon = null;
                    playerUIManager.CurrentGunItem = null;
                }
                else if (currentWeapon is MeleeWeapon)
                {
                    playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingPistolHash, false);
                    playerAnimationHandler.SetBool(playerAnimationHandler.isHoldingGunHash, false);
                    weaponImage.sprite = currentWeaponItem.itemIcon;
                    playerUIManager.CurrentWeapon = null;
                    playerUIManager.CurrentGunItem = null;
                }
                CurrentWeaponAnimator = fpsWeaponModels[i].GetComponent<Animator>();
                currentWeaponIndex = nextWeaponIndex;
            }
            else
            {
                if (pV.IsMine)
                {
                    fpsWeaponModels[i].SetActive(false);
                }
                weaponModels[i].SetActive(false);
            }
        }

        if (pV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", nextWeaponIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void DeactivateWeaponAndActivateHealthKit()
    {
        // Deactivate current weapon
        SwitchWeaponWithAnimation();
        weaponModels[currentWeaponIndex].SetActive(false);
        fpsWeaponModels[currentWeaponIndex].SetActive(false);

        // Activate health kit
        healthKitModel.transform.GetChild(0).gameObject.SetActive(true);
        healthKitFPSModel.SetActive(true);
    }

    public void ReactivateWeapon()
    {
        // Deactivate health kit
        healthKitModel.transform.GetChild(0).gameObject.SetActive(false);
        healthKitFPSModel.SetActive(false);

        // Activate current weapon
        weaponModels[currentWeaponIndex].SetActive(true);
        fpsWeaponModels[currentWeaponIndex].SetActive(true);
    }

    public void CallWeaponAnimation(string animationName, bool isInteracting)
    {
        playerAnimationHandler.PlayTargetAnimation(animationName, isInteracting, CurrentWeaponAnimator);
    }

    void SwitchWeaponWithAnimation()
    {
        if (weapons[currentWeaponIndex] is GunItem)
        {
            CallWeaponAnimation("Hide", true);
        }
        else if (weapons[currentWeaponIndex] is KnifeItem)
        {
            CallWeaponAnimation("Hide", true);
        }
        else if (weapons[currentWeaponIndex] is GrenadeItem)
        {
            if (CurrentWeaponAnimator.GetBool("IsCooking"))
            {
                DestroyGrenade();
                ActivateWeapon();
                inputHandler.primaryFireInput = false;
            }
            else
            {
                CallWeaponAnimation("Hide", true);
            }
        }

    }

    void DestroyGrenade()
    {
        GrenadeWeapon grenadeWeapon = currentWeapon as GrenadeWeapon;
        if (grenadeWeapon != null)
        {
            Destroy(grenadeWeapon.instantiatedGrenade);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!pV.IsMine && targetPlayer == pV.Owner)
        {
            nextWeaponIndex = (int)changedProps["itemIndex"];
            ActivateWeapon();
        }
    }
}

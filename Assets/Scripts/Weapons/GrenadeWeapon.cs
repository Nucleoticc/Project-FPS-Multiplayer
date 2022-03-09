using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class GrenadeWeapon : Weapon
{
    GrenadeItem grenadeItem;
    Grenade grenade;

    Transform grenadeSpawnPoint;

    bool isOnCooldown = false;

    [HideInInspector] public GameObject instantiatedGrenade;

    WaitForSeconds explosionDelay;
    Coroutine cooldownCoroutine;

    Camera cam;

    //Animator Hash
    int IsCookingHash;
    int IsOnThrowLoopHash;

    bool isCooking = false;

    PhotonView pV;

    protected override void Awake()
    {
        base.Awake();
        pV = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        if (!pV.IsMine)
        {
            return;
        }
        cam = Camera.main;

        explosionDelay = new WaitForSeconds(grenadeItem.delay);

        IsCookingHash = Animator.StringToHash("IsCooking");
        IsOnThrowLoopHash = Animator.StringToHash("IsOnThrowLoop");
    }

    void Update()
    {
        if (!pV.IsMine)
        {
            return;
        }
        if (playerAnimationHandler.GetBool(playerAnimationHandler.isDeadHash))
        {
            return;
        }
        HandleAttack();
    }

    public void UpdateGrenadeSettings(GrenadeItem item, Transform grenadeSpawnPoint)
    {
        grenadeItem = item;
        this.grenadeSpawnPoint = grenadeSpawnPoint;
    }

    protected override void HandleAttack()
    {
        if (inputHandler.primaryFireInput && !isOnCooldown)
        {
            if (weaponInventory.CurrentWeaponAnimator.GetBool("IsInteracting")
            || weaponInventory.CurrentWeaponAnimator.GetBool("IsOnThrowLoop"))
            {
                return;
            }

            if (isCooking)
            {
                return;
            }

            isCooking = true;
            weaponInventory.CurrentWeaponAnimator.SetBool(IsCookingHash, isCooking);
            weaponInventory.CurrentWeaponAnimator.SetBool(IsOnThrowLoopHash, true);
            
            playerAnimationHandler.SetBool(playerAnimationHandler.usedGrenadeHash, true);

            playerAnimationHandler.PlayTargetAnimation("Throw_Start", false, weaponInventory.CurrentWeaponAnimator);

            if (cooldownCoroutine == null)
            {
                StartCoroutine(HandleCooldown());
            }
        }
        else if (!inputHandler.primaryFireInput)
        {
            isCooking = false;
            weaponInventory.CurrentWeaponAnimator.SetBool(IsCookingHash, isCooking);
        }
    }

    protected override void HandleInspect() { }

    public void InstantiateGrenade()
    {
        if (!pV.IsMine)
        {
            return;
        }
        //Instantiating grenade
        // instantiatedGrenade = Instantiate(grenadeItem.physicalObjectPrefab, grenadeSpawnPoint);
        instantiatedGrenade = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", grenadeItem.physicalObjectPrefab.name), grenadeSpawnPoint.position, grenadeSpawnPoint.rotation, 0);
        instantiatedGrenade.transform.parent = grenadeSpawnPoint.transform;

        //Setting Grenade Settings
        grenade = instantiatedGrenade.GetComponent<Grenade>();
        grenade.delay = grenadeItem.delay;
        grenade.explosionVFX = grenadeItem.explosionVFX.name;
        grenade.explosionRadius = grenadeItem.explosionRadius;
        grenade.baseDamage = grenadeItem.baseDamage;
        grenade.ownerNickname = pV.Owner.NickName;
    }

    public void ActivateRigidBodyAndThrowGrenade()
    {
        if (!pV.IsMine)
        {
            return;
        }

        if (instantiatedGrenade == null)
        {
            return;
        }

        grenade.isStillInHand = false;

        StartCooldown();

        instantiatedGrenade.transform.parent = null;
        instantiatedGrenade.transform.LookAt(cam.ViewportToWorldPoint(new Vector3(0.49f, 0.55f, 25f)));
        grenade = instantiatedGrenade.GetComponent<Grenade>();

        grenade.Throw(grenadeItem.throwForce);
    }

    public void StartCooldown()
    {
        cooldownCoroutine = null;
        if (!isOnCooldown)
        {
            if (grenadeItem.itemName == "Frag Grenade")
            {
                playerUIManager.FragGrenadeCooldown(grenadeItem.cooldown);
            }
            else if (grenadeItem.itemName == "FlashBang Grenade")
            {
                playerUIManager.FlashGrenadeCooldown(grenadeItem.cooldown);
            }
            else if (grenadeItem.itemName == "Smoke Grenade")
            {
                playerUIManager.SmokeGrenadeCooldown(grenadeItem.cooldown);
            }

            isOnCooldown = true;
            Invoke("EndCooldown", grenadeItem.cooldown);
        }
    }

    void EndCooldown()
    {
        isOnCooldown = false;
    }

    IEnumerator HandleCooldown()
    {
        yield return explosionDelay;
        StartCooldown();
    }
}
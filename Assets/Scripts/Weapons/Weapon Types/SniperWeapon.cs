using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SniperWeapon : GunWeapon
{
    Image scopeOverlay;
    GameObject scopeCamera;
    Camera mainCamera;

    Coroutine scopedCoroutine;
    WaitForSeconds scopeTime;
    bool isScoped = false;

    [Header("Scopes")]
    [SerializeField] float scopedFOV = 25f;
    [SerializeField] float normalFOV = 77f;

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
        fpsWeaponHolder = transform.root.GetChild(2).GetChild(2);
        scopeCamera = Camera.allCameras[1].gameObject;
        scopeOverlay = transform.root.GetChild(5).GetChild(3).GetComponent<Image>();
        scopeTime = new WaitForSeconds(0.30f);
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
        HandleReloading();
        HandleInspect();
    }

    protected override void HandleAttack()
    {
        if (!inputHandler.primaryFireInput)
        {
            //reset camera rotation to 0 over time
            ResetRecoil();
        }

        if (inputHandler.secondaryFireInput)
        {
            isScoped = true;
            weaponInventory.CurrentWeaponAnimator.SetBool("IsAiming", true);
            if (scopedCoroutine == null)
            {
                scopedCoroutine = StartCoroutine(HandleScope());
            }
        }
        else
        {
            weaponInventory.CurrentWeaponAnimator.SetBool("IsAiming", false);
            CloseScope();
            if (scopedCoroutine != null)
            {
                StopCoroutine(scopedCoroutine);
                scopedCoroutine = null;
            }
        }

        if (weaponInventory.CurrentWeaponAnimator.GetBool("IsInteracting") ||
            weaponInventory.CurrentWeaponAnimator.GetBool("IsReloading"))
        {
            return;
        }

        if (inputHandler.primaryFireInput && CurrentAmmo > 0)
        {
            inputHandler.primaryFireInput = false;
            Shoot();
        }
    }

    protected override void Shoot()
    {
        if (Time.time > nextFire)
        {
            playerAnimationHandler.SetBool(playerAnimationHandler.usedSniperHash, true);
            playerAudioHandler.PlayAudio(currentWeapon.audioClip.name);
            InstantiateMuzzleFlash();
            CurrentAmmo--;
            nextFire = Time.time + currentWeapon.fireRate;
            Vector3 startPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            Vector3 direction = cam.transform.forward;
            if (!isScoped)
            {
                direction = HandleBulletSpread();
            }
            HandleRecoil();
            playerAnimationHandler.PlayTargetAnimation("Shot", false, weaponInventory.CurrentWeaponAnimator);
            ShootWithRaycastAndPenetrate(startPos, direction, true);
            recoilResetTime = currentWeapon.recoilResetTime;
            currentRecoilIndex += 1;
            if (currentRecoilIndex >= currentWeapon.recoil.Count)
            {
                currentRecoilIndex = 0;
            }
        }
    }

    public void Reload()
    {
        int ammoToReload = currentWeapon.magSize - CurrentAmmo;
        if (ReserveAmmo >= ammoToReload)
        {
            ReserveAmmo -= ammoToReload;
            CurrentAmmo = currentWeapon.magSize;
        }
        else if (ReserveAmmo < ammoToReload && ReserveAmmo > 0)
        {
            CurrentAmmo += ReserveAmmo;
            ReserveAmmo = 0;
        }
        playerUIManager.isReloadingWeapon = false;
    }

    IEnumerator HandleScope()
    {
        yield return scopeTime;
        scopeOverlay.enabled = true;
        scopeCamera.SetActive(false);
        cam.fieldOfView = scopedFOV;
    }

    public void CloseScope()
    {
        scopeOverlay.enabled = false;
        scopeCamera.SetActive(true);
        cam.fieldOfView = normalFOV;
    }
}

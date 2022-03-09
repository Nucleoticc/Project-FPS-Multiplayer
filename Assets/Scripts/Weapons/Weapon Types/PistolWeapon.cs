using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PistolWeapon : GunWeapon
{
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
            recoilResetTime -= Time.deltaTime;
            if (recoilResetTime <= 0)
            {
                currentRecoilIndex = 0;
            }

            //reset camera rotation to 0 over time
            ResetRecoil();
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
            playerAnimationHandler.SetBool(playerAnimationHandler.usedPistolHash, true);
            playerAudioHandler.PlayAudio(currentWeapon.audioClip.name);
            InstantiateMuzzleFlash();
            CurrentAmmo--;
            nextFire = Time.time + currentWeapon.fireRate;
            Vector3 startPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            Vector3 direction = HandleBulletSpread();
            HandleRecoil();
            playerAnimationHandler.PlayTargetAnimation("Shot", false, weaponInventory.CurrentWeaponAnimator);
            ShootWithRayCast(startPos, direction);
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
}

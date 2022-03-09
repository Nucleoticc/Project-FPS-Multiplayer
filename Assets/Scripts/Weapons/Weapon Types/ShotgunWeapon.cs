using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShotgunWeapon : GunWeapon
{
    int bulletCountHash;

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
        bulletCountHash = Animator.StringToHash("BulletCount");
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
            //reset camera rotation to 0 over time
            ResetRecoil();
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
            playerAnimationHandler.SetBool(playerAnimationHandler.usedShotgunHash, true);
            playerAudioHandler.PlayAudio(currentWeapon.audioClip.name);
            InstantiateMuzzleFlash();
            CurrentAmmo--;
            weaponInventory.CurrentWeaponAnimator.SetInteger(bulletCountHash, CurrentAmmo);
            nextFire = Time.time + currentWeapon.fireRate;
            Vector3 startPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            List<Vector3> direction = HandleShotgunBulletSpread();
            HandleRecoil();
            playerAnimationHandler.PlayTargetAnimation("Shot", false, weaponInventory.CurrentWeaponAnimator);
            for (int i = 0; i < 8; i++)
            {
                ShootWithRayCast(startPos, direction[i]);
            }

            recoilResetTime = currentWeapon.recoilResetTime;
            currentRecoilIndex += 1;
            if (currentRecoilIndex >= currentWeapon.recoil.Count)
            {
                currentRecoilIndex = 0;
            }
        }
    }

    List<Vector3> HandleShotgunBulletSpread()
    {
        List<Vector3> directions = new List<Vector3>(8);
        for (int i = 0; i < 8; i++)
        {
            directions.Add(HandleBulletSpread() * 0.5f);
        }
        return directions;
    }

    public void Reload()
    {
        CurrentAmmo += 1;
        ReserveAmmo -= 1;
        weaponInventory.CurrentWeaponAnimator.SetInteger(bulletCountHash, CurrentAmmo);
        playerUIManager.isReloadingWeapon = false;
    }
}

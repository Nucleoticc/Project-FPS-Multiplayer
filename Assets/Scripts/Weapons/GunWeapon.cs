using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunWeapon : Weapon
{
    public GunItem currentWeapon;

    [Header("Local Helper Variables")]
    protected int currentRecoilIndex = 0;
    protected float nextFire;
    protected float recoilResetTime;
    protected bool SemiAutoShootCheck;
    protected Transform muzzleFlashPosition;
    protected GameObject muzzleFlashPrefab;

    [Header("Camera")]
    protected Camera cam;
    protected Transform fpsWeaponHolder;

    public Item.GunType gunType { get; protected set; }

    public int CurrentAmmo { get; protected set; }
    public int ReserveAmmo { get; protected set; }

    protected PhotonView pV;

    public virtual void UpdateGunSettings(GunItem gunItem, Transform newMuzzleFlashPosition, Item.GunType newWeaponType)
    {
        if (gunItem == null) { return; }

        currentWeapon = gunItem;

        gunType = newWeaponType;

        CurrentAmmo = gunItem.magSize;
        ReserveAmmo = gunItem.reserveAmmo;

        nextFire = 0;
        recoilResetTime = gunItem.recoilResetTime;
        muzzleFlashPosition = newMuzzleFlashPosition;
        muzzleFlashPrefab = gunItem.muzzleFlashPrefab;
    }

    protected virtual void Shoot()
    { }

    protected virtual void ShootWithRaycastAndPenetrate(Vector3 startPos, Vector3 direction, bool multiPenetrate = false)
    {
        bool hasHitWall = false;
        int damage = currentWeapon.baseDamage;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, currentWeapon.range);
        List<RaycastHit> sortedHitList = new List<RaycastHit>(hits);
        sortedHitList.Sort((x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit hit in sortedHitList)
        {
            if (damage <= damage * 0.2f)
            {
                break;
            }

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                CharacterController characterController = hit.transform.GetComponent<CharacterController>();
                float headShotPoint = characterController.height - 0.20f * characterController.height;
                float hitPoint = hit.transform.InverseTransformPoint(hit.point).y;
                int damageToDo = damage;
                if (hitPoint >= headShotPoint)
                {
                    damageToDo *= 2;
                }
                hit.transform.gameObject.GetComponent<IDamageable>()?.TakeDamage(damageToDo, pV.Owner.NickName, currentWeapon.itemIcon.name);
                damage = Mathf.RoundToInt(damage * 0.75f);

                if (!multiPenetrate)
                {
                    break;
                }
            }
            else
            {
                Environment environment = hit.collider.GetComponent<Environment>();
                if (environment != null)
                {
                    environment.SpawnHitMarker(environment.name, hit.point, Quaternion.LookRotation(hit.normal));

                    damage = Mathf.RoundToInt(damage * 0.50f);

                    if (!environment.isPenetrable || hasHitWall)
                    {
                        break;
                    }
                    hasHitWall = true;
                }
            }
        }
    }

    protected virtual void ShootWithRayCast(Vector3 startPos, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos, direction, out hit, currentWeapon.range))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                int damage = currentWeapon.baseDamage;
                CharacterController characterController = hit.transform.GetComponent<CharacterController>();
                float headShotPoint = characterController.height - 0.20f * characterController.height;
                float hitPoint = hit.transform.InverseTransformPoint(hit.point).y;
                if (hitPoint >= headShotPoint)
                {
                    damage *= 2;
                }
                hit.transform.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, pV.Owner.NickName, currentWeapon.itemIcon.name);
            }
            else
            {
                Environment environment = hit.collider.GetComponent<Environment>();
                if (environment != null)
                {
                    environment.SpawnHitMarker(environment.name, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }

    protected virtual Vector3 HandleBulletSpread()
    {
        float xSpread = 0f;
        float ySpread = 0f;

        Vector3 destinationVector = cam.transform.forward;

        if (currentWeapon.spread.magnitude > 0)
        {
            xSpread = Random.Range(-currentWeapon.spread.x, currentWeapon.spread.x);
            ySpread = Random.Range(-currentWeapon.spread.y, currentWeapon.spread.y);
        }

        if (!playerMovementHandler.isGrounded)                                               // 1.5x Spread if in air
        {
            destinationVector += new Vector3(xSpread * 1.5f, ySpread * 1.5f, xSpread * 1.5f);
        }
        else if (inputHandler.crouchInput)                                                   // No spread while crouching
        {
            if (inputHandler.movementInput.magnitude > 0)
                destinationVector += new Vector3(xSpread * 0.4f, ySpread * 0.4f, xSpread * 0.4f);
            else
                destinationVector += new Vector3(xSpread * 0.1f, ySpread * 0.1f, xSpread * 0.1f);
        }
        else if (inputHandler.movementInput.magnitude > 0)                                  // Add Spread while moving
        {
            destinationVector += new Vector3(xSpread, ySpread, xSpread);
        }
        else if (inputHandler.movementInput.magnitude == 0)                                 // Reduced spread while standing
        {
            destinationVector += new Vector3(xSpread * 0.3f, ySpread * 0.3f, xSpread * 0.3f);
        }

        return destinationVector;
    }

    protected virtual void HandleRecoil()
    {
        float xRecoil = currentWeapon.recoil[currentRecoilIndex].x * 500f;
        float yRecoil = currentWeapon.recoil[currentRecoilIndex].y * 500f;

        Quaternion targetRotation = Quaternion.Euler(-yRecoil, xRecoil, 0f);
        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRotation, 0.04f);
        fpsWeaponHolder.localRotation = Quaternion.Slerp(fpsWeaponHolder.localRotation, targetRotation, 0.04f);
    }

    protected virtual void ResetRecoil()
    {
        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.identity, 8f * Time.deltaTime);
        fpsWeaponHolder.localRotation = Quaternion.Slerp(fpsWeaponHolder.localRotation, Quaternion.identity, 8f * Time.deltaTime);
    }

    protected virtual void InstantiateMuzzleFlash()
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPosition);
        muzzleFlash.transform.parent = muzzleFlashPosition.transform;
        Destroy(muzzleFlash, 0.2f);
    }

    protected virtual void HandleReloading()
    {
        if (weaponInventory.CurrentWeaponAnimator.GetBool("IsInteracting"))
        {
            return;
        }

        if (weaponInventory.CurrentWeaponAnimator.GetBool("IsReloading"))
        {
            return;
        }

        bool reloadButtonPressed = inputHandler.reloadInput;
        bool outOfAmmo = inputHandler.primaryFireInput && CurrentAmmo <= 0;
        bool fullAmmo = CurrentAmmo == currentWeapon.magSize;

        if (reloadButtonPressed || outOfAmmo)
        {
            if (fullAmmo)
            {
                return;
            }
            if (ReserveAmmo <= 0)
            {
                //Play empty gun sound
                return;
            }
            playerUIManager.isReloadingWeapon = true;
            weaponInventory.CallWeaponAnimation("Reload", false);
            weaponInventory.CurrentWeaponAnimator.SetBool("IsReloading", true);
            playerAnimationHandler.SetBool(playerAnimationHandler.isReloadingHash, true);
        }
    }

    int CalculateDamage(int damage, float distance)
    {
        if (distance < currentWeapon.damageFalloffStartFactor * currentWeapon.range)
        {
            return damage;
        }
        float distanceFactor = 1f - (distance / currentWeapon.range);
        return Mathf.RoundToInt(damage * distanceFactor);
    }
}

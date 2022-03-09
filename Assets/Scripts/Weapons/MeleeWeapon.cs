using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeleeWeapon : Weapon
{
    KnifeItem knifeItem;
    MeleeDamage meleeDamage;

    int damageOnAttack;

    PhotonView pV;

    protected override void Awake()
    {
        base.Awake();
        pV = GetComponentInParent<PhotonView>();
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
        HandleInspect();
    }

    public void UpdateMeleeSettings(KnifeItem weapon, MeleeDamage meleeDamage)
    {
        knifeItem = weapon;
        this.meleeDamage = meleeDamage;
    }

    protected override void HandleAttack()
    {
        if (weaponInventory.CurrentWeaponAnimator.GetBool("IsInteracting"))
        {
            return;
        }

        if (inputHandler.primaryFireInput)
        {
            playerAudioHandler.PlayAudio(knifeItem.audioClip.name);
            playerAnimationHandler.SetBool(playerAnimationHandler.usedMeleeHash, true);
            damageOnAttack = knifeItem.primaryBaseDamage;
            playerAnimationHandler.PlayTargetAnimation("Attack_01", true, weaponInventory.CurrentWeaponAnimator);
        }
        else if (inputHandler.secondaryFireInput)
        {
            playerAudioHandler.PlayAudio(knifeItem.audioClip.name);
            playerAnimationHandler.SetBool(playerAnimationHandler.usedMeleeHash, true);
            damageOnAttack = knifeItem.secondaryBaseDamage;
            playerAnimationHandler.PlayTargetAnimation("Attack_02", true, weaponInventory.CurrentWeaponAnimator);
        }
    }

    public void OpenMeleeDamageCollider()
    {
        meleeDamage.ActivateCollider(damageOnAttack);
    }

    public void CloseMeleeDamageCollider()
    {
        meleeDamage.DeactivateCollider();
    }
}

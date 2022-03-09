using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected InputHandler inputHandler;
    protected PlayerAnimationHandler playerAnimationHandler;
    protected PlayerMovementHandler playerMovementHandler;
    protected WeaponInventory weaponInventory;
    protected PlayerUIManager playerUIManager;
    protected PlayerAudioHandler playerAudioHandler;

    protected virtual void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        playerAnimationHandler = GetComponentInParent<PlayerAnimationHandler>();
        playerMovementHandler = GetComponentInParent<PlayerMovementHandler>();
        weaponInventory = GetComponentInParent<WeaponInventory>();
        playerUIManager = GetComponentInParent<PlayerUIManager>();
        playerAudioHandler = GetComponentInParent<PlayerAudioHandler>();
    }

    protected virtual void HandleAttack()
    { }

    protected virtual void HandleInspect()
    {
        if (inputHandler.inspectInput)
        {
            playerAnimationHandler.PlayTargetAnimation("Inspect", false, weaponInventory.CurrentWeaponAnimator);
            inputHandler.inspectInput = false;
        }
    }
}

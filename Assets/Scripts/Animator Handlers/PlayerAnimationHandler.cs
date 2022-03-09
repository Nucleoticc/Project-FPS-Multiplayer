using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationHandler : MonoBehaviour
{
    Animator animator;
    PlayerMovementHandler playerMovementHandler;

    public int horizontalHash;
    public int verticalHash;
    public int isInteractingHash;
    public int isGroundedHash;
    public int isCrouchingHash;
    public int isRunningHash;
    public int movementHash;
    public int isDeadHash;

    //Weapons
    public int isHoldingPistolHash;
    public int isHoldingGunHash;
    public int isReloadingHash;
    public int usedMeleeHash;
    public int usedGrenadeHash;
    public int usedPistolHash;
    public int usedRifleHash;
    public int usedSniperHash;
    public int usedShotgunHash;
    public int usedSMGHash;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        SetAnimatorHashValues();
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isWalking, bool isCrouched, Animator externalAnimator)
    {
        float v = 0;
        float h = 0;

        if (!isWalking || isCrouched)
        {
            if (verticalMovement > 0)
            {
                v = 0.5f;
            }
            else if (verticalMovement < 0)
            {
                v = -0.5f;
            }
            else
            {
                v = 0;
            }

            if (horizontalMovement > 0)
            {
                h = 0.5f;
            }
            else if (horizontalMovement < 0)
            {
                h = -0.5f;
            }
            else
            {
                h = 0;
            }
            externalAnimator.SetBool(isRunningHash, false);
        }
        else
        {
            if (verticalMovement > 0)
            {
                v = 1f;
            }
            else if (verticalMovement < 0)
            {
                v = -0.5f;
            }
            else
            {
                v = 0;
            }

            if (horizontalMovement > 0)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0)
            {
                h = -1f;
            }
            else
            {
                h = 0;
            }
            if (v == -0.5f)
            {
                externalAnimator.SetBool(isRunningHash, false);
            }
            else
            {
                externalAnimator.SetBool(isRunningHash, true);
            }

        }

        if (v != 0 || h != 0)
        {
            externalAnimator.SetBool(movementHash, true);
        }
        else
        {
            externalAnimator.SetBool(movementHash, false);
        }

        animator.SetFloat(verticalHash, v, 0.1f, Time.deltaTime);
        animator.SetFloat(horizontalHash, h, 0.1f, Time.deltaTime);
    }

    public void UpdateMovementValues(float movementSpeed, bool isRunning)
    {
        bool isMoving = movementSpeed > 0.1f;
    }

    void SetAnimatorHashValues()
    {
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        isInteractingHash = Animator.StringToHash("IsInteracting");
        isGroundedHash = Animator.StringToHash("IsGrounded");
        isCrouchingHash = Animator.StringToHash("IsCrouching");
        isRunningHash = Animator.StringToHash("IsRunning");
        movementHash = Animator.StringToHash("IsMoving");
        isDeadHash = Animator.StringToHash("IsDead");

        //Weapons
        isHoldingPistolHash = Animator.StringToHash("IsHoldingPistol");
        isHoldingGunHash = Animator.StringToHash("IsHoldingGun");
        isReloadingHash = Animator.StringToHash("IsReloading");
        usedMeleeHash = Animator.StringToHash("UsedMelee");
        usedGrenadeHash = Animator.StringToHash("UsedGrenade");
        usedPistolHash = Animator.StringToHash("UsedPistol");
        usedRifleHash = Animator.StringToHash("UsedRifle");
        usedSniperHash = Animator.StringToHash("UsedSniper");
        usedShotgunHash = Animator.StringToHash("UsedShotgun");
        usedSMGHash = Animator.StringToHash("UsedSMG");
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false, Animator externalAnimator = null)
    {
        if (animator.GetBool(isDeadHash))
        {
            return;
        }

        if (externalAnimator != null)
        {
            externalAnimator.SetBool(isInteractingHash, isInteracting);
            externalAnimator.CrossFade(targetAnim, 0.1f);
        }
        else
        {
            animator.SetBool(isInteractingHash, isInteracting);
            animator.CrossFade(targetAnim, 0.2f);
        }
    }

    public void SetBool(int targetBoolHash, bool value)
    {
        animator.SetBool(targetBoolHash, value);
    }

    public bool GetBool(int targetBoolHash)
    {
        return animator.GetBool(targetBoolHash);
    }

    void OnAnimatorMove()
    {
        Vector3 newPosition = animator.deltaPosition;
        newPosition.y = 0;
        Vector3 velocity = newPosition / Time.deltaTime;
        playerMovementHandler.characterController.velocity.Set(velocity.x, velocity.y, velocity.z);
    }
}

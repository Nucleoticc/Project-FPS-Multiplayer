using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovementHandler : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;

    PlayerAnimationHandler playerAnimationHandler;
    PlayerUIManager playerUIManager;
    WeaponInventory weaponInventory;
    InputHandler inputHandler;
    PlayerManager playerManager;
    Transform cam;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float airSpeed = 0.5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float normalHeight = 2f;
    [SerializeField] float crouchHeight = 0.5f;
    float speed;
    float refSpeed = 0f;
    bool isCrouched;

    [Header("Gravity")]
    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = Vector3.zero;
    [SerializeField] LayerMask groundMask;
    [HideInInspector] public bool isGrounded;

    [Header("Camera")]
    [SerializeField] Transform playerCameraHandler;
    [SerializeField] float xClamp = 85f;
    [SerializeField] float sensitivityX = 8f;
    [SerializeField] float sensitivityY = 0.5f;
    [SerializeField] float cameraNormalHeight = 2f;
    [SerializeField] float cameraCrouchHeight = 0.5f;
    float xRotation = 0f;
    Vector3 refRotate = Vector3.zero;

    [Header("Extras")]
    [SerializeField] Transform armatureHead;
    bool isDead = false;

    PhotonView pV;

    void Awake()
    {
        // PhotonNetwork.OfflineMode = true;
        characterController = GetComponent<CharacterController>();

        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        playerUIManager = GetComponent<PlayerUIManager>();
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();
        weaponInventory = GetComponent<WeaponInventory>();

        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!pV.IsMine)
        {
            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                Destroy(cam.gameObject);
            }
        }

        SetSensitivity();
    }

    void Update()
    {
        if (!pV.IsMine) { return; }
        if (isDead) { return; }

        HandleSprintAndCrouch();
        HandleMovement();
        HandleVoidZone();
    }

    void LateUpdate()
    {
        if (!pV.IsMine) { return; }
        if (isDead) { return; }

        HandleRotation();
    }

    void SetSensitivity()
    {
        sensitivityX = PlayerPrefs.GetFloat("xSensitivity", 0.5f);
        sensitivityY = PlayerPrefs.GetFloat("ySensitivity", 0.5f);
    }

    void HandleSprintAndCrouch()
    {
        if (inputHandler.crouchInput)
        {
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0f, 0.78f, 0f);
            isCrouched = true;
            speed = Mathf.SmoothDamp(speed, crouchSpeed, ref refSpeed, 0.1f);
            playerAnimationHandler.SetBool(playerAnimationHandler.isCrouchingHash, true);
            playerCameraHandler.localPosition = Vector3.SmoothDamp(playerCameraHandler.localPosition, new Vector3(0f, cameraCrouchHeight, 0f), ref refRotate, 0.1f);

        }
        else
        {
            playerCameraHandler.localPosition = Vector3.SmoothDamp(playerCameraHandler.localPosition, new Vector3(0f, cameraNormalHeight, 0f), ref refRotate, 0.1f);
            if (inputHandler.sprintInput && inputHandler.movementInput.y > 0f)
            {
                speed = Mathf.SmoothDamp(speed, sprintSpeed, ref refSpeed, 0.1f);
            }
            else
            {
                speed = Mathf.SmoothDamp(speed, walkSpeed, ref refSpeed, 0.1f);
            }
            isCrouched = false;
            characterController.height = normalHeight;
            characterController.center = new Vector3(0f, 0.97f, 0f);
            playerAnimationHandler.SetBool(playerAnimationHandler.isCrouchingHash, false);
        }
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
        playerAnimationHandler.SetBool(playerAnimationHandler.isGroundedHash, isGrounded);
        if (isGrounded)
        {
            verticalVelocity.y = 0f;
        }
        float newSpeed = speed;
        if (weaponInventory.currentWeapon != null)
        {
            newSpeed = speed * weaponInventory.currentWeaponItem.movementSpeedFactor;
        }
        Vector3 movementVelocity = (inputHandler.movementInput.x * transform.right + inputHandler.movementInput.y * transform.forward).normalized * newSpeed;
        movementVelocity.y = 0f;

        if (!isGrounded)
        {
            characterController.Move(movementVelocity * airSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(movementVelocity * Time.deltaTime);
        }

        playerAnimationHandler.UpdateAnimatorValues(inputHandler.movementInput.y, inputHandler.movementInput.x, inputHandler.sprintInput, inputHandler.crouchInput, weaponInventory.CurrentWeaponAnimator);

        if (inputHandler.jumpInput && isGrounded)
        {
            if (isCrouched)
            {
                verticalVelocity.y = Mathf.Sqrt(-2 * (jumpHeight + 0.5f) * gravity);
            }
            else
            {
                verticalVelocity.y = Mathf.Sqrt(-2 * jumpHeight * gravity);
            }
            playerAnimationHandler.PlayTargetAnimation("JumpStart", false);
            inputHandler.jumpInput = false;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (playerUIManager.escape.activeSelf)
        {
            return;
        }

        float currentSensitivityX = sensitivityX;
        float currentSensitivityY = sensitivityY;

        if (inputHandler.secondaryFireInput && weaponInventory.currentWeaponItem.itemName == "Beretta")
        {
            currentSensitivityX = sensitivityX * 0.1f;
            currentSensitivityY = sensitivityY * 0.1f;
        }

        float mouseX = inputHandler.cameraInput.x * currentSensitivityX;
        float mouseY = inputHandler.cameraInput.y * currentSensitivityY;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        playerCameraHandler.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleVoidZone()
    {
        if (transform.position.y < -15f)
        {
            playerManager.Die("Void Zone", null);
        }
    }

    public void Died()
    {
        isDead = true;
        characterController.height = 0f;
        characterController.radius = 0f;
        characterController.stepOffset = 0f;
    }
}

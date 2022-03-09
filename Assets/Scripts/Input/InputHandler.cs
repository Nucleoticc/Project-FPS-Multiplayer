using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    PlayerInput playerInput;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 cameraInput;
    [HideInInspector] public bool sprintInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool crouchInput;

    [HideInInspector] public bool primaryFireInput;
    [HideInInspector] public bool secondaryFireInput;
    [HideInInspector] public bool reloadInput;

    [HideInInspector] public bool switchRifleInput;
    [HideInInspector] public bool switchPistolInput;
    [HideInInspector] public bool switchShotgunInput;
    [HideInInspector] public bool switchSMGInput;
    [HideInInspector] public bool switchSniperInput;
    [HideInInspector] public bool switchMeleeInput;
    [HideInInspector] public bool switchGrenadeInput;
    [HideInInspector] public float mouseScrollInput;

    [HideInInspector] public bool inspectInput;

    [HideInInspector] public bool healthKitInputStarted;
    [HideInInspector] public bool healthKitInputEnded;

    [HideInInspector] public bool showScoreboardInput;
    [HideInInspector] public bool escapeInput;

    //
    [HideInInspector] public bool ResetGameInput;
    //

    void OnEnable()
    {
        if (playerInput == null)
        {

            playerInput = new PlayerInput();

            //Movement
            playerInput.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            playerInput.Player.Walk.performed += ctx => sprintInput = true;
            playerInput.Player.Walk.canceled += ctx => sprintInput = false;
            playerInput.Player.Crouch.performed += ctx => crouchInput = true;
            playerInput.Player.Crouch.canceled += ctx => crouchInput = false;
            playerInput.Player.Jump.performed += ctx => jumpInput = true;
            playerInput.Player.Jump.canceled += ctx => jumpInput = false;

            //Weapon
            playerInput.Player.PrimaryFire.performed += ctx => primaryFireInput = true;
            playerInput.Player.PrimaryFire.canceled += ctx => primaryFireInput = false;
            playerInput.Player.SecondaryFire.performed += ctx => secondaryFireInput = true;
            playerInput.Player.SecondaryFire.canceled += ctx => secondaryFireInput = false;
            playerInput.Player.Reload.performed += ctx => reloadInput = true;
            playerInput.Player.Reload.canceled += ctx => reloadInput = false;

            //Weapon Switch
            playerInput.Player.SwitchRifle.performed += ctx => switchRifleInput = true;
            playerInput.Player.SwitchPistol.performed += ctx => switchPistolInput = true;
            playerInput.Player.SwitchShotgun.performed += ctx => switchShotgunInput = true;
            playerInput.Player.SwitchSMG.performed += ctx => switchSMGInput = true;
            playerInput.Player.SwitchSniper.performed += ctx => switchSniperInput = true;
            playerInput.Player.SwitchMelee.performed += ctx => switchMeleeInput = true;
            playerInput.Player.SwitchGrenade.performed += ctx => switchGrenadeInput = true;
            playerInput.Player.ChangeWeapon.performed += ctx => mouseScrollInput = ctx.ReadValue<float>();

            //General
            playerInput.Player.Inspect.performed += ctx => inspectInput = true;
            playerInput.Player.UseHealthKit.performed += ctx =>
            {
                healthKitInputStarted = true;
                healthKitInputEnded = false;
            };
            playerInput.Player.UseHealthKit.canceled += ctx =>
            {
                healthKitInputStarted = false;
                healthKitInputEnded = true;
            };

            //UI
            playerInput.Player.Scoreboard.performed += ctx => showScoreboardInput = true;
            playerInput.Player.Scoreboard.canceled += ctx => showScoreboardInput = false;

            playerInput.Utility.Escape.performed += ctx => escapeInput = true;

            //Utility
            playerInput.Utility.ResetGame.performed += ctx => ResetGameInput = true;
        }
        playerInput.Enable();
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}

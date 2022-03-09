using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IDamageable
{
    InputHandler inputHandler;
    PlayerSpawnerManager spawnerManager;
    PlayerUIManager uIManager;
    PlayerMovementHandler movementHandler;
    PlayerAnimationHandler animationHandler;

    [Header("Health")]
    [SerializeField] int health;
    [SerializeField] int maxHealth = 100;
    [SerializeField] Slider healthSlider;

    [Header("Mesh Management")]
    [SerializeField] SkinnedMeshRenderer thirdPersonModel;
    public GameObject[] thirdPersonWeapons { get; set; }
    public GameObject[] firstPersonModels { get; set; }
    public GameObject healthKitModel { get; set; }
    public GameObject healthKitFPSModel { get; set; }
    public Transform fpsWeaponHolder { get; set; }

    bool isDead = false;

    PhotonView pV;

    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        uIManager = GetComponent<PlayerUIManager>();
        movementHandler = GetComponent<PlayerMovementHandler>();
        animationHandler = GetComponent<PlayerAnimationHandler>();

        pV = GetComponent<PhotonView>();
        spawnerManager = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<PlayerSpawnerManager>();
        HandleCursor();
    }

    void Start()
    {
        isDead = false;
        health = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        HandleCursor();
    }

    void HandleCursor()
    {
        if (uIManager.escape.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TakeDamage(int damage, string attacker, string weaponSprite, bool canSelfDamage = false)
    {
        if (!canSelfDamage && (pV.Owner.NickName == attacker))
        {
            return;
        }
        pV.RPC("RPC_TakeDamage", RpcTarget.All, damage, attacker, weaponSprite);
    }

    public void Heal(int heal)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        healthSlider.value = health;
    }

    public bool isFullyHealed()
    {
        return health == maxHealth;
    }

    public void ManageMesh()
    {
        if (pV.IsMine)
        {
            thirdPersonModel.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            foreach (GameObject weapon in thirdPersonWeapons)
            {
                weapon.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            healthKitModel.transform.GetChild(0).GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        else
        {
            foreach (GameObject weapon in firstPersonModels)
            {
                foreach (SkinnedMeshRenderer smr in weapon.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    smr.enabled = false;
                }
            }
            foreach (SkinnedMeshRenderer smr in healthKitFPSModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                smr.enabled = false;
            }
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage, string attacker, string weaponSprite)
    {
        if (!pV.IsMine)
        {
            return;
        }

        health -= damage;
        if (health <= 0 && !isDead)
        {
            animationHandler.SetBool(animationHandler.isDeadHash, true);
            isDead = true;
            health = 0;
            Die(attacker, weaponSprite);
        }
        UpdateHealthUI();
    }

    public void Die(string attacker, string weaponSprite)
    {
        fpsWeaponHolder.gameObject.SetActive(false);
        movementHandler.Died();
        uIManager.ShowDeathUI(attacker);
        KillFeedUIManager.instance.AddKillfeedItem(attacker, pV.Owner.NickName, weaponSprite);
        ScoreboardUIManager.instance.UpdateScoreBoardItem(attacker, pV.Owner.NickName);
        spawnerManager.Die();
    }
}

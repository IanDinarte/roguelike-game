using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

//equip unarmed como? algum jeito do Weapon_Unarmed ser pego, sera que faço o scriptable addressable?
public class EquipmentController : MonoBehaviour
{
    private AttackController attackController;
    private DefendController defendController;
    private PlayerController playerController;

    //animators
    private Animator mainAnimator;
    public RuntimeAnimatorController unarmedAnimator;

    private TextMeshProUGUI arrowCounterText;
    private TextMeshProUGUI goldCounterText;

    private PlayerInput playerInput;

    //inventory stuff
    //public GameObject inventory;

    //equipped weapons
    public Weapon[] equippedWeapons = new Weapon[2]; //0 right hand, 1 left hand
    public Weapon unarmedWeapon;
    public Weapon emptyHand;
    //public GameObject unarmedPrefab;

    public Transform rightHand;
    public Transform leftHand;
    public Transform dropLocation;

    //variables 
    public int arrows;
    public int gold;

    public Weapon[] EquippedWeapons => equippedWeapons;

    public int Arrows => arrows;

    public int Gold => gold;

    public void SetArrows(int arrows)
    {
        this.arrows = arrows;
        UpdateArrowCounter();
    }

    public void SetGold(int gold)
    {
        this.gold = gold;
        UpdateGoldCounterText();
    }

    private void Start()
    {
        GameObject arrowTextObject = GameObject.Find("ArrowsCounterText");
        GameObject goldTextObject = GameObject.Find("GoldCounterText");
        if (arrowTextObject != null && goldTextObject != null)
        {
            arrowCounterText = arrowTextObject.GetComponent<TextMeshProUGUI>();
            goldCounterText = goldTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Counter Objects not found in the scene.");
        }

        attackController = this.GetComponent<AttackController>();
        defendController = this.GetComponent<DefendController>();

        mainAnimator = this.GetComponent<Animator>();

        playerController = this.GetComponent<PlayerController>();

        playerInput = playerController.PlayerInput;
        playerInput.CharacterControls.Drop.started += Drop;
        //playerInput = new PlayerInput();

        equippedWeapons[0] = ScriptableObject.CreateInstance<Weapon>();
        equippedWeapons[1] = ScriptableObject.CreateInstance<Weapon>();

        EquipWeapon(unarmedWeapon, rightHand);
        EquipWeapon(emptyHand, leftHand);

        // EquipUnarmed(leftHand);
        // EquipUnarmed(rightHand);
        UpdateArrowCounter();
    }

    private void Update() { }

    void OnEnable()
    {
        //     //playerInput.CharacterControls.Inventory.started += OpenInventory;
        //     //playerInput.CharacterControls.SwitchWeapon.started += SwitchWeapon;
        //     playerInput.Enable();
    }

    void OnDisable()
    {
        //     //playerInput.CharacterControls.Inventory.canceled -= OpenInventory;
        //     playerInput.Disable();
    }

    private void UpdateArrowCounter()
    {
        if (arrowCounterText != null)
        {
            arrowCounterText.text = "x " + arrows.ToString();
        }
    }

    private void UpdateGoldCounterText()
    {
        if (goldCounterText != null)
        {
            goldCounterText.text = gold.ToString();
        }
    }

    // void OpenInventory(InputAction.CallbackContext context)
    // {
    //     inventory.SetActive(!inventory.activeSelf);
    // }

    public void ChangeWeapon(Weapon weapon)
    {
        switch (weapon.WeaponType)
        {
            case string a when a.Contains("OneHanded"):
                DropWeapon(rightHand);
                EquipWeapon(weapon, rightHand);

                if (equippedWeapons[1].WeaponType.Contains("TwoHanded"))
                {
                    DropWeapon(leftHand);
                    EquipWeapon(emptyHand, leftHand);
                }

                break;
            case string a when a.Contains("TwoHanded"):
                DropWeapon(rightHand);
                DropWeapon(leftHand);

                //this part is for bow
                if (weapon.ItemId == "03")
                {
                    EquipWeapon(weapon, leftHand);
                    EquipWeapon(emptyHand, rightHand);
                }
                else
                {
                    EquipWeapon(weapon, rightHand);
                    EquipWeapon(emptyHand, leftHand);
                }
                break;
            case string a when a.Contains("OffHand"):
                if (equippedWeapons[0].WeaponType.Contains("TwoHanded"))
                {
                    DropWeapon(rightHand);
                    EquipWeapon(unarmedWeapon, rightHand);
                }

                if (equippedWeapons[1].WeaponType.Contains("TwoHanded"))
                {
                    EquipWeapon(unarmedWeapon, rightHand);
                }

                DropWeapon(leftHand);
                EquipWeapon(weapon, leftHand);
                break;
        }
    }

    private void EquipWeapon(Weapon weapon, Transform hand)
    {
        //int slot = hand == rightHand ? 0 : 1;
        equippedWeapons[hand == rightHand ? 0 : 1] = weapon;

        if (weapon.ItemId != "0") //empty hands dont have prefab
        {
            string address = "Assets/Prefabs/Weapons/Player Weapons/" + weapon.GetFileName() + ".prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (weapon.WeaponAnimator != null)
                        {
                            mainAnimator.runtimeAnimatorController = weapon.WeaponAnimator;
                        }

                        if (weapon.AttackSound != null)
                        {
                            attackController.AttackSound.clip = weapon.AttackSound;
                        }

                        mainAnimator.SetFloat("AtkSpeed", weapon.AttackSpeed == 0 ? mainAnimator.GetFloat("AtkSpeed") : weapon.AttackSpeed);

                        GameObject weaponInstance = Instantiate(handle.Result, hand.position, hand.rotation);
                        weaponInstance.transform.SetParent(hand);


                        //Debug.Log($"Successfully instantiated {weapon.GetFileName()}.");
                    }
                    else
                    {
                        //Debug.LogError($"Failed to load prefab with address: {address}");
                    }
                };
        }
    }

    public void ClearHands()
    {
        //currentOffHand = OffHand.Empty;
        ClearItems(rightHand);
        ClearItems(leftHand);
    }

    public void ClearItems(Transform parent)
    {
        // Loop through all children of the parent GameObject
        foreach (Transform child in parent.transform)
        {
            // Destroy the child GameObject
            GameObject.Destroy(child.gameObject);
        }
    }

    private void Drop(InputAction.CallbackContext context)
    {
        if (!attackController.isAttacking && !defendController.isDefending
        && (!equippedWeapons[0].WeaponType.Contains("Empty") || !equippedWeapons[1].WeaponType.Contains("Empty")))
        {
            if (!equippedWeapons[1].WeaponType.Contains("Empty"))
            {
                DropWeapon(leftHand);
                //ClearItems(leftHand);
                if (equippedWeapons[1].ItemId == "03")//bow
                {
                    EquipWeapon(unarmedWeapon, rightHand);
                }
                EquipWeapon(emptyHand, leftHand);
                //EquipUnarmed(leftHand);
            }
            else
            {
                DropWeapon(rightHand);
                //ClearItems(rightHand);
                EquipWeapon(unarmedWeapon, rightHand);
                //EquipUnarmed(rightHand);
            }
        }
    }

    public void DropWeapon(Transform hand)
    {
        int slot = hand == rightHand ? 0 : 1;
        Weapon weapon = equippedWeapons[slot];

        if (weapon != null && !weapon.WeaponType.Contains("Empty"))
        {
            string address = "Assets/Prefabs/Pickables/Items/Weapon/" + weapon.GetFileName() + ".prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Instantiate(handle.Result, dropLocation.position, handle.Result.transform.rotation);
                    //Debug.Log($"Successfully instantiated {weapon.GetFileName()}.");
                }
                else
                {
                    //Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
        }
        ClearItems(hand);
    }

    public bool CheckEquippedWeaponName(string itemName)
    {
        if (itemName.StartsWith(equippedWeapons[0].ItemName) || itemName.StartsWith(equippedWeapons[1].ItemName))
        {
            return true;
        }
        return false;
    }

    public bool CheckEquippedWeaponType(string weaponType)
    {
        if (weaponType.Equals(equippedWeapons[0].WeaponType) || weaponType.Equals(equippedWeapons[1].WeaponType))
        {
            return true;
        }
        return false;
    }

    public bool CheckEquippedWeaponId(string ItemId)
    {
        if (ItemId.Equals(equippedWeapons[0].ItemId) || ItemId.Equals(equippedWeapons[1].ItemId))
        {
            return true;
        }
        return false;
    }
}
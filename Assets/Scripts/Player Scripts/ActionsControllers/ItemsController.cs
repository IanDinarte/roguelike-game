using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemsController : MonoBehaviour
{
    private AnimationMovementController playerMovement;
    private Animator animator;
    private AttackController attackController;
    private DodgeController dodgeController;
    private HitController hitController;
    private PlayerInput playerInput;
    public Transform dropLocation;

    public Item[] itemArray = new Item[3];
    private int itemCount = 0;

    public Item[] GetItemArray() => itemArray;

    public int GetItemCount() => itemCount;

    //item specific variables
    private int amagicMagicResist = 50; //
    //private float bloodRegen = 0.5f; // hp/second
    private float bloodHeal = 10f; // 10hp per kill
    private int lightPower = 30;
    private int terraArmor = 50; // 30% damage mitigation
    private int warCritFactor = 15; // 15%
    private float windSpeed = 1.2f; // 20%
    private float darkDistanceModifier = -2f;

    private void Awake()
    {
        playerMovement = this.GetComponent<AnimationMovementController>();
        animator = this.GetComponent<Animator>();
        attackController = this.GetComponent<AttackController>();
        dodgeController = this.GetComponent<DodgeController>();
        hitController = this.GetComponent<HitController>();
        playerInput = new PlayerInput();

        itemCount = 0;
    }

    void OnEnable()
    {
        playerInput.CharacterControls.DropItem.started += DropItem;
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }

    private void ApplyRuneEffect(Item item)
    {
        switch (item.ItemId)
        {
            case "01":
                hitController.SetMagicResistValue(hitController.MagicResistValue + amagicMagicResist);
                break;
            case "02":
                //hitController.StartPassiveHealthRegen(bloodRegen);
                attackController.HealOnSlain += bloodHeal;
                break;
            case "03":
                hitController.SetLightAngle(hitController.LightAngle + new Vector2(lightPower, lightPower));
                break;
            case "04":
                hitController.isDeathProtected = true;
                break;
            case "05":
                hitController.SetArmorValue(hitController.ArmorValue + terraArmor);
                break;
            case "06":
                attackController.SetPassiveCritFactor(attackController.PassiveCritFactor + warCritFactor);
                break;
            case "07":
                playerMovement.SetMoveSpeed(playerMovement.MoveSpeed * windSpeed);
                playerMovement.footStepsSound.pitch *= windSpeed;
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") * windSpeed);
                break;
            case "08":
                dodgeController.SetDodgeDistance(dodgeController.DodgeDistance + darkDistanceModifier);
                dodgeController.isTeleport = true;
                break;
        }
    }

    private void RemoveRuneEffect(Item item)
    {
        switch (item.ItemId)
        {
            case "01":
                hitController.SetMagicResistValue(hitController.MagicResistValue - amagicMagicResist);
                break;
            case "02":
                attackController.HealOnSlain -= bloodHeal;
                break;
            case "03":
                hitController.SetLightAngle(hitController.LightAngle - new Vector2(lightPower, lightPower)); 
                break;
            case "04":
                hitController.isDeathProtected = false;
                break;
            case "05":
                hitController.SetArmorValue(hitController.ArmorValue - terraArmor);
                break;
            case "06":
                attackController.SetPassiveCritFactor(attackController.PassiveCritFactor - warCritFactor);
                break;
            case "07":
                playerMovement.SetMoveSpeed(playerMovement.MoveSpeed / windSpeed);
                playerMovement.footStepsSound.pitch /= windSpeed;
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") / windSpeed);
                break;
            case "08":
                dodgeController.SetDodgeDistance(dodgeController.DodgeDistance - darkDistanceModifier);
                dodgeController.isTeleport = false;
                break;
        }
    }

    public bool AddItem(Item newItem)
    {
        if (itemCount < 3)
        {
            if (itemArray.Contains(newItem))
            {
                //Debug.Log("cannot have duplicate items");
                return false;
            }
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i] == null)
                {
                    itemArray[i] = newItem;
                    itemCount++;
                    ApplyRuneEffect(newItem);
                    //Debug.Log("item added " + newItem.ItemName + " in pos: " + i);
                    return true;
                }
            }
        }
        //Debug.Log("No space for more items, need to drop one\n" + itemArray.Count() + "/" + itemArray.Length);
        return false;
    }

    private void DropItem(InputAction.CallbackContext context)
    {
        Item itemToRemove = RemoveItem((int)context.ReadValue<float>());
        if (itemToRemove != null)
        {
            string address = "Assets/Prefabs/Pickables/Items/Rune/" + itemToRemove.GetFileName() + ".prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Instantiate(handle.Result, dropLocation.position, Quaternion.identity);
                    //Debug.Log($"Successfully instantiated {itemToRemove.GetFileName()}.");
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
        }
    }

    public Item RemoveItem(int slot)
    {
        if (slot < 1 || slot > 3)
        {
            return null;
        }

        Item removed = itemArray[slot - 1];

        if (removed == null)
        {
            return null;
        }

        itemArray[slot - 1] = null;

        itemCount--;
        RemoveRuneEffect(removed);
        return removed;
    }

    public Item RemoveItemById(string id)
    {
        if (itemArray.Length != 0)
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                Item item = itemArray[i];
                if (item != null && item.ItemId == id)
                {
                    itemArray[i] = null;
                    itemCount--;
                    RemoveRuneEffect(item);
                    return item;
                }
            }
        }
        return null;
    }

    public bool HasItem(string itemId)
    {
        if (itemArray.Length != 0)
        {
            foreach (Item item in itemArray)
            {
                if (item != null && item.ItemId.Equals(itemId))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
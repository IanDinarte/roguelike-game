using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Unused, consumbales are now items
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Consumable")]
public class Consumable : ScriptableObject
{
    [SerializeField] private string consumableId;
    [SerializeField] private string consumableName;
    [SerializeField, TextArea] private string consumableDescription;

    public string ConsumableId => consumableId;
    public string ConsumableName => consumableName;
    public string ConsumableDescription => consumableDescription;

    public string GetFileName() => name;

    public void SetConsumableId(string newId) => consumableId = newId;
    public void SetConsumableName(string newName) => consumableName = newName;
    public void SetConsumableDescription(string newDescription) => consumableDescription = newDescription;
}

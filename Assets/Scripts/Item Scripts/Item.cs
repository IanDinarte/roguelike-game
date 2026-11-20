using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string itemDescription;
    [SerializeField, TextArea] private string itemStats;
    [SerializeField] private string type;
    [SerializeField] private Sprite itemImage;
    [SerializeField] private int dropChance;
    [SerializeField] private int price;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public string ItemStats => itemStats;
    public string Type => type;
    public Sprite ItemImage => itemImage;
    public int DropChance => dropChance;
    public int Price => price;

    public string GetFileName() => name;

    public void SetItemId(string newId) => itemId = newId;
    public void SetItemName(string newName) => itemName = newName;
    public void SetItemDescription(string newDescription) => itemDescription = newDescription;
    public void SetItemStats(string newStats) => itemStats = newStats;
    public void SetType(string newType) => type = newType;
    public void SetItemImage(Sprite newImage) => itemImage = newImage;
    public void SetDropChance(int newDropChance) => dropChance = newDropChance;
    public void SetPrice(int newPrice) => price = newPrice;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Weapon")]
public class Attribute : Item
{
    [SerializeField] private int attributeLevel;
    [SerializeField] private float valueIncrease;

    public int AttributeLevel => attributeLevel;
    public float ValueIncrease => valueIncrease;

    public void SetAttributeLevel(int newAttributeLevel) => attributeLevel = newAttributeLevel;
    public void SetValueIncrease(float newValueIncrease) => valueIncrease = newValueIncrease;
}

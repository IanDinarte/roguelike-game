using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnarmedWeaponController : MeleeWeaponController
{
    public UnarmedWeaponController()
    {
        this.SetDefaultHealthDamage(15); //5 hits to kill FW
        this.SetDefaultPoiseDamage(10); //x hits to stagger FW
        this.SetCritMultiplier(1.5f);
        this.SetCritFactor(0);
    }
}

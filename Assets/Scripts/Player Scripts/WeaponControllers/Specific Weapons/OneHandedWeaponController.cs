using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHandedWeaponController : MeleeWeaponController
{
    public OneHandedWeaponController()
    {
        this.SetDefaultHealthDamage(25); //4 hits to kill FW
        this.SetDefaultPoiseDamage(20); //2 hits to stagger FW
        this.SetCritMultiplier(2f);
        this.SetCritFactor(0);
    }
}

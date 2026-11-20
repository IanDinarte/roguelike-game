using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandedWeaponController : MeleeWeaponController
{
    public TwoHandedWeaponController()
    {
        this.SetDefaultHealthDamage(40); 
        this.SetDefaultPoiseDamage(30); 
        this.SetCritMultiplier(2.5f);
        this.SetCritFactor(0);
    }
}

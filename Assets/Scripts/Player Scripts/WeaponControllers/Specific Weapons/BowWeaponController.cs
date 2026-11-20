using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeaponController : RangedWeaponController
{
    public BowWeaponController()
    {
        this.SetDefaultHealthDamage(20); //4 hits to kill FW
        this.SetDefaultPoiseDamage(20); //3 hits to stagger FW
        this.SetCritMultiplier(2f);
        this.SetCritFactor(5);
        this.SetArrowForce(15f);
    }
}

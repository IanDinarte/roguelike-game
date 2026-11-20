using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XBowWeaponController : RangedWeaponController
{
    public XBowWeaponController()
    {
        this.SetDefaultHealthDamage(30);
        this.SetDefaultPoiseDamage(30);
        this.SetCritMultiplier(2f);
        this.SetCritFactor(0);
        this.SetArrowForce(20f);
    }
}

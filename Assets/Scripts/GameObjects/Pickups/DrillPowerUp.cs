using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DrillPowerUp : Pickup
{
    public DrillPowerUp(Vector2 pos, int drillLevel)
        : base(pos, "drill" + drillLevel.ToString(), "Got Drill Upgrade!\nYour drill does more damage and can\npenetrate harder materials!", (Player p) => { p.setDrillPower(drillLevel); })
    {

    }
}


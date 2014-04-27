using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class JumpBoots : Pickup
{
    public JumpBoots(Vector2 pos)
        : base(pos, "jumpBoots", "Got Pickup Boots!\nYou can now jump TWOOO TIMES!", (Player p) => { p.maxJumpsLeft = 2; })
    {

    }
}


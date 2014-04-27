using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InGamePage : FPage
{
    FCamObject camera;
    World world;

    public InGamePage()
    {
        world = new World();
        Futile.stage.AddChild(world);
        world.loadMap("testMap");
        Player p = new Player();
        world.spawnPlayer(p, "topDoor");
        HealthBar hb = new HealthBar();
        OverheatBar ob = new OverheatBar();
        hb.y = (int)(Futile.screen.halfHeight - hb.Height / 2 - 10);
        hb.x = (int)(-Futile.screen.halfWidth + hb.Width / 2 + 10);

        ob.y = hb.y - hb.Height / 2 - ob.Height / 2 - 5;
        ob.x = hb.x;

        C.getCameraInstance().AddChild(hb);
        C.getCameraInstance().AddChild(ob);
        
        p.setHealthBar(hb);
        p.setOverheatBar(ob);
    }

}


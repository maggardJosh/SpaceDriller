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
        world.loadMap("testMap");
        world.spawnPlayer(new Player());
        Futile.stage.AddChild(world);
    }

}


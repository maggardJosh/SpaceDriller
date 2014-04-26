using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InGamePage : FPage
{
    FCamObject camera;
    public InGamePage()
    {
        FTmxMap tmxMap = new FTmxMap();
        camera= new FCamObject();
        tmxMap.clipNode = camera;
        
        tmxMap.LoadTMX("Maps/testMap");
        camera.setWorldBounds(new Rect(0, -tmxMap.height, tmxMap.width, tmxMap.height));

        Futile.stage.AddChild(tmxMap);
        Player p = new Player();
        Futile.stage.AddChild(p);
        camera.follow(p);
        Futile.stage.AddChild(camera);

        FTilemap collision = (FTilemap)tmxMap.getLayerNamed("Collision");
        
    }

}


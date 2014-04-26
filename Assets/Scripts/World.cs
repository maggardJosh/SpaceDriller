using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    public FTmxMap map;
    public FTilemap collision;

    List<Vector2> spawnPoints = new List<Vector2>();
    FContainer backgroundLayer;
    FContainer playerLayer;
    FContainer foregroundLayer;

    public World()
        : base()
    {
        map = new FTmxMap();
    }

    public void loadMap(string mapName)
    {
        backgroundLayer = new FContainer();
        playerLayer = new FContainer();
        foregroundLayer = new FContainer();

        map = new FTmxMap();
        FCamObject camera = C.getCameraInstance();
        map.clipNode = camera;
        map.LoadTMX("Maps/" + mapName);

        camera.setWorldBounds(new Rect(0, -map.height, map.width, map.height));

        collision = (FTilemap)map.getLayerNamed("Collision");
        FTilemap objects = (FTilemap)map.getLayerNamed("Objects");
        for (int x = 0; x < objects.widthInTiles; x++)
            for (int y = 0; y < objects.heightInTiles; y++)
            {
                if (objects.getFrameNum(x, y) != 0)
                {
                    spawnPoints.Add(new Vector2(x * map.tileWidth + map.tileWidth/2 , -y * map.tileHeight - map.tileHeight/2));
                }
            }

        FTilemap fg = (FTilemap)map.getLayerNamed("FG");
        map.RemoveChild(fg);

        backgroundLayer.AddChild(map);

        this.AddChild(backgroundLayer);
        this.AddChild(playerLayer);
        this.AddChild(fg);

    }

    public void spawnPlayer(Player p)
    {
        C.getCameraInstance().follow(p);
        playerLayer.AddChild(p);
        if (spawnPoints.Count > 0)
            p.SetPosition(spawnPoints[RXRandom.Int(spawnPoints.Count)]);
        p.setWorld(this);
    }
}


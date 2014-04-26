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
    List<Door> doorList = new List<Door>();

    FContainer backgroundLayer;
    FContainer playerLayer;
    FContainer foregroundLayer;

    FSprite whiteOverlaySprite;
    public World()
        : base()
    {
        map = new FTmxMap();

        whiteOverlaySprite = new FSprite(C.whiteElement);
        whiteOverlaySprite.alpha = 0;
        whiteOverlaySprite.color = Color.black;
        whiteOverlaySprite.width = Futile.screen.width;
        whiteOverlaySprite.height = Futile.screen.height;

    }

    public void loadMap(string mapName)
    {
        backgroundLayer = new FContainer();
        playerLayer = new FContainer();
        foregroundLayer = new FContainer();

        doorList = new List<Door>();

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
                    spawnPoints.Add(new Vector2(x * map.tileWidth + map.tileWidth / 2, -y * map.tileHeight - map.tileHeight / 2));
                }
            }

        addDoors(map);

        FTilemap fg = (FTilemap)map.getLayerNamed("FG");
        map.RemoveChild(fg);

        backgroundLayer.AddChild(map);

        this.AddChild(backgroundLayer);
        this.AddChild(playerLayer);
        this.AddChild(fg);
        camera.AddChild(whiteOverlaySprite);
        camera.MoveToFront();


    }

    public void checkDoor(Player p)
    {
        foreach (Door door in doorList)
            if (door.checkPlayer(p))
            {
                C.transitioning = true;
                C.getCameraInstance().MoveToFront();
                Go.to(whiteOverlaySprite, C.transitioningTime, new TweenConfig().floatProp("alpha", 1.0f).setEaseType(EaseType.QuadOut).onComplete((AbstractTween t) =>
                {
                    this.RemoveAllChildren();
                    loadMap(door.toMap);
                    spawnPlayer(p, door.toDoor);
                    Go.to(whiteOverlaySprite, C.transitioningTime, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.QuadIn).onComplete((AbstractTween t2) => { C.transitioning = false; }));
                }));
                break;
            }
    }

    private void addDoors(FTmxMap map)
    {
        foreach (XMLNode node in map.objects)
        {
            if (node.attributes["type"].ToLower().CompareTo("door") == 0)
            {
                //Is a door object
                Vector2 pos = new Vector2(float.Parse(node.attributes["x"]), float.Parse(node.attributes["y"]));
                float width = float.Parse(node.attributes["width"]);
                float height = float.Parse(node.attributes["height"]);
                string toMap = "";
                string toDoor = "";
                foreach (XMLNode property in ((XMLNode)node.children[0]).children)
                {
                    if (property.attributes["name"].ToLower().CompareTo("tomap") == 0)
                        toMap = property.attributes["value"];
                    else
                        if (property.attributes["name"].ToLower().CompareTo("todoor") == 0)
                            toDoor = property.attributes["value"];
                }
                doorList.Add(new Door(pos, width, height, node.attributes["name"], toMap, toDoor));
            }
        }
    }

    public void spawnPlayer(Player p)
    {
        C.getCameraInstance().follow(p);
        playerLayer.AddChild(p);
        if (spawnPoints.Count > 0)
            p.SetPosition(spawnPoints[RXRandom.Int(spawnPoints.Count)]);
        p.setWorld(this);
    }

    public void spawnPlayer(Player p, String toDoor)
    {
        C.getCameraInstance().follow(p);
        playerLayer.AddChild(p);
        foreach (Door door in doorList)
        {
            if (door.Name.ToLower().CompareTo(toDoor.ToLower()) == 0)
            {
                door.spawnPlayer(p);
                break;
            }
        }
        p.setWorld(this);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Door
{
    private Rect bounds;
    public string toMap { get; private set; }
    public string toDoor { get; private set; }
    public string Name { get; private set; }

    bool playerHasLeft = true;

    public Door(Vector2 position, float width, float height, string doorName, string toMap, string toDoor)
    {
        bounds = new Rect(position.x, -position.y - height, width, height);
        this.toMap = toMap;
        this.toDoor = toDoor;
        this.Name = doorName;
    }

    public bool checkPlayer(Player p)
    {
        if (String.IsNullOrEmpty(toMap))
            return false;
        if (playerHasLeft)
            return bounds.Contains(p.GetPosition());
        else
        {
            playerHasLeft = !bounds.Contains(p.GetPosition());  //Wait until player leaves
            return false;
        }
    }

    public void spawnPlayer(Player p)
    {
        playerHasLeft = false;
        p.SetPosition(bounds.x + bounds.width / 2, bounds.y + bounds.width / 2);
    }
}

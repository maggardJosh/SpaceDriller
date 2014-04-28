using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CheckPoint
{
    private Rect bounds;
    private float width = 32;

    bool playerHasLeft = true;

    public CheckPoint(Vector2 position)
    {
        bounds = new Rect(position.x, position.y, width, width);
    }

    public void checkPlayer(Player p)
    {
        if (playerHasLeft)
        {
            if (bounds.Contains(p.GetPosition()))
            {
                p.x = bounds.center.x;
                p.y = bounds.yMin + p.collisionHeight ;
                p.recharge();
                C.transitioning = true;
                playerHasLeft = false;
            }
        }
        else
        {
            playerHasLeft = !bounds.Contains(p.GetPosition());  //Wait until player leaves
        }
    }

    public void spawnPlayer(Player p)
    {
        playerHasLeft = true;
        p.SetPosition(bounds.x + bounds.width / 2, bounds.y + bounds.width / 2);
    }
}

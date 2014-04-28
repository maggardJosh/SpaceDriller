using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tiki : BaseGameObject
{
    public enum State
    {
        IDLE,
        BECOMING_ACTIVE,
        ACTIVE,
        JUMPING
    }

    public State currentState;

    Rect tikiRect;

    List<TikiGuy> tikiGuys = new List<TikiGuy>();

    public Tiki(Vector2 position, int numTikiGuys)
    {
        this.SetPosition(position);
        tikiRect = new Rect();
        currentState = State.IDLE;

        for (int i = 0; i < numTikiGuys; i++)
        {
            TikiGuy newTiki = new TikiGuy(this);
            newTiki.y = i * (newTiki.height);
            tikiGuys.Add(newTiki);
            this.AddChild(newTiki);
        }


        float rectBottom = -tikiGuys[0].height / 2;
        float rectTop = tikiGuys[0].height * tikiGuys.Count - tikiGuys[0].height / 2;
        float rectLeft = -tikiGuys[0].height / 2;
        float rectRight = tikiGuys[0].height / 2;
        tikiRect.Set(rectLeft, rectTop, rectRight - rectLeft, rectTop - rectBottom);


    }

    float xVel;
    float yVel;


    float activeCount = 0;
    float activeBeforeJumpAmount = 1;

    float jumpForce = 190;
    float xJumpForce = 100;
    float gravity = -250;
    float oldY = 0;

    protected override void Update()
    {
        base.Update();
        if (!this.isAlive)
            return;
        switch (currentState)
        {
            case State.IDLE:
                foreach (TikiGuy t in tikiGuys)
                {
                    if (t.collides(world.p))
                    {
                        currentState = State.BECOMING_ACTIVE;
                        foreach (TikiGuy tiki in tikiGuys)
                            tiki.goActive();
                        activeCount = 0;
                    }
                }
                break;
            case State.BECOMING_ACTIVE:
                if (activeCount < activeBeforeJumpAmount)
                    activeCount += UnityEngine.Time.deltaTime;
                else
                {
                    currentState = State.ACTIVE;
                    activeCount = 0;
                }
                break;
            case State.ACTIVE:
                if (activeCount < activeBeforeJumpAmount)
                {
                    activeCount += UnityEngine.Time.deltaTime;
                }
                else
                {
                    currentState = State.JUMPING;
                    yVel = jumpForce;
                    if (world.p.x > this.x)
                        xVel = xJumpForce;
                    else
                        xVel = -xJumpForce;
                    oldY = this.y;
                }
                break;
            case State.JUMPING:

                moveAndDoCollision();

                break;
        }

        for (int i = 0; i < tikiGuys.Count; i++)
        {
            tikiGuys[i].checkCollision();
            if (tikiGuys[i].health <= 0)
            {
                for (int j = i; j < tikiGuys.Count; j++)
                {
                    tikiGuys[j].moveDown(j-1);
                }
                    tikiGuys.RemoveAt(i);
                --i;
                continue;
            }
        }
        if (tikiGuys.Count == 0)
        {
            this.RemoveFromContainer();
            this.health = 0;
        }

    }

    public override void setWorld(World world)
    {
        foreach (TikiGuy tiki in tikiGuys)
            tiki.setWorld(world);
        base.setWorld(world);
    }

    private void moveAndDoCollision()
    {
        float xMove = xVel * UnityEngine.Time.deltaTime;
        this.x += xMove;

        if (xMove > 0)
        {

            int newTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2) / world.map.tileWidth);
            foreach (TikiGuy t in tikiGuys)
            {
                int highTileY = -Mathf.CeilToInt((this.y + t.y + t.height / 2.1f) / world.map.tileWidth);
                int lowTileY = -Mathf.CeilToInt((this.y + t.y - t.height / 2.1f) / world.map.tileWidth);
                if (world.collision.getFrameNum(newTileX, lowTileY) == 1 ||
                    world.collision.getFrameNum(newTileX, highTileY) == 1)
                {
                    this.x = (newTileX * world.map.tileWidth) - world.map.tileWidth / 2;
                    //xVel = 0;
                    break;
                }
            }
        }
        else if (xMove < 0)
        {
            int newTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2) / world.map.tileWidth);
            foreach (TikiGuy t in tikiGuys)
            {
                int highTileY = -Mathf.CeilToInt((this.y + t.y + t.height / 2.1f) / world.map.tileWidth);
                int lowTileY = -Mathf.CeilToInt((this.y + t.y - t.height / 2.1f) / world.map.tileWidth);
                if (world.collision.getFrameNum(newTileX, lowTileY) == 1 ||
                    world.collision.getFrameNum(newTileX, highTileY) == 1)
                {
                    this.x = ((newTileX + 1) * world.map.tileWidth) + world.map.tileWidth / 2;
                    //xVel = 0;
                    break;
                }
            }
        }


        this.y += yVel * UnityEngine.Time.deltaTime;

        if (yVel > 0)
        {

            TikiGuy highestTiki = tikiGuys[tikiGuys.Count - 1];
            int newTileY = -Mathf.CeilToInt((this.y + highestTiki.y + world.map.tileWidth / 2) / world.map.tileHeight);

            int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.1f) / world.map.tileWidth);
            if (world.collision.getFrameNum(leftTileX, newTileY) == 1 ||
                world.collision.getFrameNum(rightTileX, newTileY) == 1)
            {
                this.y = -(newTileY + 1) * world.map.tileHeight - world.map.tileWidth / 2 - highestTiki.y;
            }
        }
        else if (yVel < 0)
        {
            int newTileY = -Mathf.CeilToInt((this.y - world.map.tileWidth / 2) / world.map.tileHeight);

            int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.1f) / world.map.tileWidth);
            if (world.collision.getFrameNum(leftTileX, newTileY) == 1 ||
                world.collision.getFrameNum(rightTileX, newTileY) == 1)
            {
                this.y = -(newTileY) * world.map.tileHeight + world.map.tileWidth / 2;
                currentState = State.ACTIVE;
                activeCount = 0;
            }
        }

        yVel += gravity * UnityEngine.Time.deltaTime;

    }
}

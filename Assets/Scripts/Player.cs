using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FSprite
{
    public World world;
    float speed = 200.0f;
    float yVel = 0;
    float gravity = -20;
    bool grounded = false;
    float jumpForce = 7;

    float disabledTimeLeft = 0;
    bool disabled = false;

    public Player()
        : base("character")
    {

    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromStage();
    }

    private void Update()
    {
        if (world == null)
            return;
        float xMove = 0;
        float yMove = 0;

        yVel += gravity * UnityEngine.Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))            //Space has been pressed. Start jumping
        {
            if (grounded)
                yVel = jumpForce;
        }
        else if (!Input.GetKey(KeyCode.Space))       //Space is not held down... Let the player stop jumping if currently jumping
        {
            if (yVel > 0)
                yVel *= .4f;
        }
        this.grounded = false;
        yVel = Mathf.Clamp(yVel, -6, jumpForce);
        yMove = yVel;
        if (Input.GetKey(KeyCode.A))
            xMove = -Time.deltaTime * speed;
        else
            if (Input.GetKey(KeyCode.D))
                xMove = Time.deltaTime * speed;

        tryMove(xMove, yMove);
    }

    public void disableMovement(float time)
    {
        this.disabledTimeLeft = time;
        this.disabled = false;              //Set to false so we know when to start the countdown.
    }

    private void tryMove(float xMove, float yMove)
    {
        if (disabledTimeLeft > 0)
        {
            if (!disabled)
                disabled = true;
            else
                disabledTimeLeft -= UnityEngine.Time.deltaTime;

            return;     //Disabled don't move
        }

        if (xMove > 0)
            checkRight(xMove);
        else if (xMove < 0)
            checkLeft(-xMove);


       
        if (yMove > 0)
            checkUp(yMove);
        else if (yMove < 0)
            checkDown(-yMove);

        world.checkDoor(this);

    }

    private void checkRight(float xMove)
    {
        while (xMove > 0)
        {
            this.x += Mathf.Min(xMove, world.map.tileWidth-1);
            xMove -= Mathf.Min(xMove, world.map.tileWidth-1);

            int topTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2.1f) / world.map.tileHeight);
            int bottomTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2.1f) / world.map.tileHeight);
            int newTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2) / world.map.tileWidth);

            if (world.collision.getFrameNum(newTileX, topTileY) != 0 ||
                world.collision.getFrameNum(newTileX, bottomTileY) != 0)
            {
                this.x = newTileX * world.map.tileWidth - world.map.tileWidth / 2.0f;
                break;
            }
        }

    }

    private void checkLeft(float xMove)
    {
        while (xMove > 0)
        {
            this.x -= Mathf.Min(xMove, world.map.tileWidth-1);
            xMove -= Mathf.Min(xMove, world.map.tileWidth-1);

            int topTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2.1f) / world.map.tileHeight);
            int bottomTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2.1f) / world.map.tileHeight);
            int newTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2) / world.map.tileWidth);

            if (world.collision.getFrameNum(newTileX, topTileY) != 0 ||
                world.collision.getFrameNum(newTileX, bottomTileY) != 0)
            {
                this.x = (newTileX + 1) * world.map.tileWidth + world.map.tileWidth / 2;
                break;
            }
        }
    }

    private void checkUp(float yMove)
    {
        while (yMove > 0)
        {
            this.y += Mathf.Min(yMove, world.map.tileHeight - 1);
            yMove -= Mathf.Min(yMove, world.map.tileHeight - 1);

            int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int newTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2) / world.map.tileHeight);

            if (world.collision.getFrameNum(rightTileX, newTileY) != 0 ||
                world.collision.getFrameNum(leftTileX, newTileY) != 0)
            {
                this.y = -(newTileY + 1) * world.map.tileHeight - world.map.tileHeight / 2;
                break;
            }
        }
    }

    private void checkDown(float yMove)
    {
        while (yMove > 0)
        {
            this.y -= Mathf.Min(yMove, world.map.tileHeight - 1);
            yMove -= Mathf.Min(yMove, world.map.tileHeight - 1);

            int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.1f) / world.map.tileWidth);
            int newTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2) / world.map.tileHeight);

            if (world.collision.getFrameNum(rightTileX, newTileY) != 0 ||
                world.collision.getFrameNum(leftTileX, newTileY) != 0)
            {
                this.y = -(newTileY) * world.map.tileHeight + world.map.tileHeight / 2;
                this.grounded = true;
                this.yVel = 0;
                break;
            }
        }
    }

    public void setWorld(World world)
    {
        this.world = world;
    }

}

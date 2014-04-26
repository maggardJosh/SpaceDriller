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

    private void tryMove(float xMove, float yMove)
    {


        this.x += xMove;
        if (xMove > 0)
            checkRight();
        else if (xMove < 0)
            checkLeft();


        this.y += yMove;
        if (yMove > 0)
            checkUp();
        else if (yMove < 0)
            checkDown();

        world.checkDoor(this);

    }

    private void checkRight()
    {
        int topTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2.1f) / world.map.tileHeight);
        int bottomTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2.1f) / world.map.tileHeight);
        int newTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2) / world.map.tileWidth);

        if (world.collision.getFrameNum(newTileX, topTileY) != 0 ||
            world.collision.getFrameNum(newTileX, bottomTileY) != 0)
        {
            this.x = newTileX * world.map.tileWidth - world.map.tileWidth / 2.0f;
        }


    }

    private void checkLeft()
    {
        int topTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2.1f) / world.map.tileHeight);
        int bottomTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2.1f) / world.map.tileHeight);
        int newTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2) / world.map.tileWidth);

        if (world.collision.getFrameNum(newTileX, topTileY) != 0 ||
            world.collision.getFrameNum(newTileX, bottomTileY) != 0)
        {
            this.x = (newTileX + 1) * world.map.tileWidth + world.map.tileWidth / 2;
        }
    }

    private void checkUp()
    {
        int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.3f) / world.map.tileWidth);
        int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.3f) / world.map.tileWidth);
        int newTileY = -Mathf.CeilToInt((this.y + world.map.tileHeight / 2) / world.map.tileHeight);

        if (world.collision.getFrameNum(rightTileX, newTileY) != 0 ||
            world.collision.getFrameNum(leftTileX, newTileY) != 0)
        {
            this.y = -(newTileY + 1) * world.map.tileHeight - world.map.tileHeight / 2;
        }
    }

    private void checkDown()
    {
        int rightTileX = Mathf.FloorToInt((this.x + world.map.tileWidth / 2.3f) / world.map.tileWidth);
        int leftTileX = Mathf.FloorToInt((this.x - world.map.tileWidth / 2.3f) / world.map.tileWidth);
        int newTileY = -Mathf.CeilToInt((this.y - world.map.tileHeight / 2) / world.map.tileHeight);

        if (world.collision.getFrameNum(rightTileX, newTileY) != 0 ||
            world.collision.getFrameNum(leftTileX, newTileY) != 0)
        {
            this.y = -(newTileY) * world.map.tileHeight + world.map.tileHeight / 2;
            this.grounded = true;
            this.yVel = 0;
        }
    }

    public void setWorld(World world)
    {
        this.world = world;
    }

}

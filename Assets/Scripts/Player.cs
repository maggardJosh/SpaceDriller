using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : BaseGameObject
{
    public enum AnimState
    {
        IDLE,
        RUN,
        JUMP,
        FALL
    }

    AnimState currentAnimState = AnimState.IDLE;

    public World world;

    private FAnimatedSprite sprite;

    bool isFacingLeft = false;
    float speed = 200.0f;
    float yVel = 0;
    float gravity = -20;
    bool grounded = false;
    float jumpForce = 7;

    public Player()
    {
        sprite = new FAnimatedSprite("character");
        sprite.addAnimation(new FAnimation("leftIDLE", new int[] { 1, 2 }, 100, true));
        sprite.addAnimation(new FAnimation("rightIDLE", new int[] { 1, 2 }, 100, true));
        sprite.addAnimation(new FAnimation("leftRUN", new int[] { 3, 4 }, 100, true));
        sprite.addAnimation(new FAnimation("rightRUN", new int[] { 5, 6 }, 100, true));
        sprite.addAnimation(new FAnimation("leftJUMP", new int[] { 7, 8 }, 100, true));
        sprite.addAnimation(new FAnimation("rightJUMP", new int[] { 7, 8 }, 100, true));
        sprite.addAnimation(new FAnimation("leftFALL", new int[] { 9, 10 }, 100, true));
        sprite.addAnimation(new FAnimation("rightFALL", new int[] { 9, 10 }, 100, true));
        sprite.play("leftIDLE");
        this.AddChild(sprite);
    }


    protected override void Update()
    {
        base.Update();
        if (!isAlive)
            return;
        if (world == null)
            return;
        float xMove = 0;
        float yMove = 0;

        yVel += gravity * UnityEngine.Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))            //Space has been pressed. Start jumping
        {
            if (grounded)
            {
                yVel = jumpForce;
                //State to jump
            }
        }
        else if (!Input.GetKey(KeyCode.Space))       //Space is not held down... Let the player stop jumping if currently jumping
        {
            if (yVel > 0)
                yVel *= .4f;
        }
        if (yVel > 0)
            currentAnimState = AnimState.JUMP;
        else
            if (yVel < 0 && !grounded)
                currentAnimState = AnimState.FALL;
        yVel = Mathf.Clamp(yVel, -6, jumpForce);
        yMove = yVel;
        if (Input.GetKey(KeyCode.A))
        {
            xMove = -Time.deltaTime * speed;
            isFacingLeft = true;
        }
        else
            if (Input.GetKey(KeyCode.D))
            {
                xMove = Time.deltaTime * speed;
                isFacingLeft = false;
            }


        this.grounded = false;
        tryMove(xMove, yMove);

        if (currentAnimState == AnimState.IDLE && xMove != 0)
            currentAnimState = AnimState.RUN;

        sprite.play((isFacingLeft ? "left" : "right") + currentAnimState);
    }


    private void tryMove(float xMove, float yMove)
    {

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
            this.x += Mathf.Min(xMove, world.map.tileWidth - 1);
            xMove -= Mathf.Min(xMove, world.map.tileWidth - 1);

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
            this.x -= Mathf.Min(xMove, world.map.tileWidth - 1);
            xMove -= Mathf.Min(xMove, world.map.tileWidth - 1);

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
                currentAnimState = AnimState.IDLE;
                break;
            }
        }
    }

    public void setWorld(World world)
    {
        this.world = world;
    }

}

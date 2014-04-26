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
        FALL,
        FALL_ATTACK_DOWN
    }

    AnimState currentAnimState = AnimState.IDLE;

    public World world;

    private FAnimatedSprite sprite;
    private FAnimatedSprite attackSprite;

    bool isFacingLeft = false;
    float speed = 200.0f;
    float yVel = 0;
    float gravity = -20;
    float maxYVel = -8;
    bool grounded = false;
    float jumpForce = 7;

    int animSpeed = 100;
    bool isAttacking = false;

    public Player()
    {
        sprite = new FAnimatedSprite("player");
        sprite.addAnimation(new FAnimation("leftIDLE", new int[] { 16 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightIDLE", new int[] { 15 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftRUN", new int[] { 5, 6, 7, 8 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightRUN", new int[] { 1, 2, 3, 4 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftJUMP", new int[] { 11 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightJUMP", new int[] { 9 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftFALL", new int[] { 12 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightFALL", new int[] { 10 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftFALL_ATTACK_DOWN", new int[] { 14 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightFALL_ATTACK_DOWN", new int[] { 13 }, animSpeed, true));
        sprite.play("leftIDLE");
        this.AddChild(sprite);

        attackSprite = new FAnimatedSprite("playerAttack");
        attackSprite.addAnimation(new FAnimation("right", new int[] { 1, 2, 3, 4 }, animSpeed, true));
        attackSprite.addAnimation(new FAnimation("left", new int[] { 5, 6, 7, 8 }, animSpeed, true));
        attackSprite.addAnimation(new FAnimation("down", new int[] { 9, 10, 11, 12 }, animSpeed, true));
        attackSprite.addAnimation(new FAnimation("up", new int[] { 13, 14, 15, 16 }, animSpeed, true));
        this.AddChild(attackSprite);
        attackSprite.alpha = 0;

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
            {
                currentAnimState = AnimState.FALL;
                if (Input.GetKey(KeyCode.S))
                        currentAnimState = AnimState.FALL_ATTACK_DOWN;
            }
        yVel = Mathf.Clamp(yVel, maxYVel, jumpForce);
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

     

        if (Input.GetKeyDown(KeyCode.U))
            isAttacking = !isAttacking;

        if (currentAnimState == AnimState.FALL_ATTACK_DOWN)
            attackSprite.play("down");
        else
        {
            if (isFacingLeft)
                attackSprite.play("left");
            else
                attackSprite.play("right");
        }
        attackSprite.alpha = isAttacking ? 1 : 0;

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
                this.y = -(newTileY + 1) * world.map.tileHeight - world.map.tileWidth / 2;
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
            int newTileY = -Mathf.CeilToInt((this.y - sprite.height / 2) / world.map.tileHeight);

            if (world.collision.getFrameNum(rightTileX, newTileY) != 0 ||
                world.collision.getFrameNum(leftTileX, newTileY) != 0)
            {
                this.y = -(newTileY) * world.map.tileHeight + sprite.height / 2;
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

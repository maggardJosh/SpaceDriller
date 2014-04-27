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
        FALL_ATTACK_DOWN,
        RUN_ATTACK_UP,
        IDLE_ATTACK_UP,
        JUMP_ATTACK_UP,
        FALL_ATTACK_UP
    }

    AnimState currentAnimState = AnimState.IDLE;

    private FAnimatedSprite sprite;

    bool isFacingLeft = false;
    float speed = 200.0f;
    float yVel = 0;
    float gravity = -20;
    float maxYVel = -8;
    bool grounded = false;
    float jumpForce = 7;

    int animSpeed = 100;
    bool isAttacking = false;

    FParticleSystem sparkParticleSystem;
    FParticleDefinition sparkParticleDefinition;

    public float lastX = 0;
    public float lastY = 0;

    float collisionWidth = 20;
    float collisionHeight = 25;

    public Player()
    {
        sparkParticleSystem = new FParticleSystem(100);
        sparkParticleDefinition = new FParticleDefinition(C.whiteElement);
        sparkParticleDefinition.startColor = Color.yellow;
        sparkParticleDefinition.startScale = .05f;
        sparkParticleDefinition.endScale = 0;
        sparkParticleDefinition.endColor = new Color(0, 0, 0, 0);
        sparkParticleSystem.shouldNewParticlesOverwriteExistingParticles = true;
        sparkParticleSystem.accelY = -300;

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
        sprite.addAnimation(new FAnimation("leftRUN_ATTACK_UP", new int[] { 21, 22, 23, 24 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightRUN_ATTACK_UP", new int[] { 17, 18, 19, 20 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftJUMP_ATTACK_UP", new int[] { 27 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightJUMP_ATTACK_UP", new int[] { 25 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftFALL_ATTACK_UP", new int[] { 28 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightFALL_ATTACK_UP", new int[] { 26 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("leftIDLE_ATTACK_UP", new int[] { 30 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("rightIDLE_ATTACK_UP", new int[] { 29 }, animSpeed, true));
        sprite.play("leftIDLE");
        this.AddChild(sprite);
        this.AddChild(sparkParticleSystem);

    }

    int jumpsLeft = 1;

    protected override void Update()
    {
        lastX = this.x;
        lastY = this.y;

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
            else
            {
                if (jumpsLeft > 0)
                {
                    jumpsLeft--;
                    yVel = jumpForce;
                }
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


        this.grounded = false;
        tryMove(xMove, yMove);

        if (currentAnimState == AnimState.IDLE && xMove != 0)
            currentAnimState = AnimState.RUN;

        if (Input.GetKey(KeyCode.W))
        {
            if (currentAnimState == AnimState.IDLE)
                currentAnimState = AnimState.IDLE_ATTACK_UP;
            else if (currentAnimState == AnimState.RUN)
                currentAnimState = AnimState.RUN_ATTACK_UP;
            else if (currentAnimState == AnimState.JUMP)
                currentAnimState = AnimState.JUMP_ATTACK_UP;
            else if (currentAnimState == AnimState.FALL)
                currentAnimState = AnimState.FALL_ATTACK_UP;
        }
        if (isAttacking)
            spawnSparks();

        sprite.play((isFacingLeft ? "left" : "right") + currentAnimState);

        sparkParticleSystem.x = -x;
        sparkParticleSystem.y = -y;
    }

    private void spawnSparks()
    {
        if (RXRandom.Float() < .3f)
            return;
        sparkParticleDefinition.x = this.x;
        sparkParticleDefinition.y = this.y - 10;
        float angle = 90 - RXRandom.Float(90f);

        if (currentAnimState == AnimState.FALL_ATTACK_UP ||
            currentAnimState == AnimState.IDLE_ATTACK_UP ||
            currentAnimState == AnimState.JUMP_ATTACK_UP ||
            currentAnimState == AnimState.RUN_ATTACK_UP)
        {
            sparkParticleDefinition.y += 20;
            angle += 90;
        }
        else if (currentAnimState == AnimState.FALL_ATTACK_DOWN)
        {
            sparkParticleDefinition.y -= 10;
            angle -= 90;
        }else
            if (isFacingLeft)
                sparkParticleDefinition.x -= 15;
            else
                sparkParticleDefinition.x += 15;
        float randomPosDist = 10;
        sparkParticleDefinition.x += RXRandom.Float() * randomPosDist - randomPosDist / 2;
        sparkParticleDefinition.y += RXRandom.Float() * randomPosDist - randomPosDist / 2;

        sparkParticleDefinition.speedX = Mathf.Cos(angle * C.PIOVER180) * (75 + 25 * RXRandom.Float());
        sparkParticleDefinition.speedY = Mathf.Sin(angle * C.PIOVER180) * (75 + 25 * RXRandom.Float());

        if (isFacingLeft)
            sparkParticleDefinition.speedX *= -1;
        sparkParticleSystem.AddParticle(sparkParticleDefinition);
    }

    #region moveFunctions
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

            int topTileY = -Mathf.CeilToInt((this.y + collisionHeight / 2.1f) / world.map.tileHeight);
            int bottomTileY = -Mathf.CeilToInt((this.y - collisionHeight / 2.1f) / world.map.tileHeight);
            int newTileX = Mathf.FloorToInt((this.x + collisionWidth / 2) / world.map.tileWidth);

            if (world.collision.getFrameNum(newTileX, topTileY) == 1 ||
                world.collision.getFrameNum(newTileX, bottomTileY) == 1)
            {
                this.x = newTileX * world.map.tileWidth - collisionWidth / 2.0f;
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

            int topTileY = -Mathf.CeilToInt((this.y + collisionHeight / 2.1f) / world.map.tileHeight);
            int bottomTileY = -Mathf.CeilToInt((this.y - collisionHeight / 2.1f) / world.map.tileHeight);
            int newTileX = Mathf.FloorToInt((this.x - collisionWidth / 2) / world.map.tileWidth);

            if (world.collision.getFrameNum(newTileX, topTileY) == 1 ||
                world.collision.getFrameNum(newTileX, bottomTileY) == 1)
            {
                this.x = (newTileX + 1) * world.map.tileWidth + collisionWidth / 2;
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

            int rightTileX = Mathf.FloorToInt((this.x + collisionWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - collisionWidth / 2.1f) / world.map.tileWidth);
            int newTileY = -Mathf.CeilToInt((this.y + collisionHeight / 2) / world.map.tileHeight);

            if (world.collision.getFrameNum(rightTileX, newTileY) == 1 ||
                world.collision.getFrameNum(leftTileX, newTileY) == 1)
            {
                this.y = -(newTileY + 1) * world.map.tileHeight - collisionHeight / 2;
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

            int rightTileX = Mathf.FloorToInt((this.x + collisionWidth / 2.1f) / world.map.tileWidth);
            int leftTileX = Mathf.FloorToInt((this.x - collisionWidth / 2.1f) / world.map.tileWidth);
            int newTileY = -Mathf.CeilToInt((this.y - sprite.height / 2) / world.map.tileHeight);

            if (world.collision.getFrameNum(rightTileX, newTileY) == 1 ||
                world.collision.getFrameNum(leftTileX, newTileY) == 1)
            {
                this.y = -(newTileY) * world.map.tileHeight + sprite.height / 2;
                this.grounded = true;
                this.yVel = 0;
                this.jumpsLeft = 1;
                currentAnimState = AnimState.IDLE;
                break;
            }
        }
    }
    #endregion

}

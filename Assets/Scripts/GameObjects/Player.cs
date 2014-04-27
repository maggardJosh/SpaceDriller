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

    public float weaponDamageRate = .5f;
    bool isFacingLeft = false;
    float speed = 200.0f;
    float stunCount = 0;            //If this is above zero we've been stunned and shouldn't move
    float invulnerableCount = 0;    //While this is above zero we can't take damage
    float xVel = 0;
    public float yVel { private set; get; }
    float gravity = -28;
    float maxYVel = -14;
    bool grounded = false;
    float jumpForce = 8;

    int animSpeed = 100;
    bool forceBounce = false;

    float overheat = 0;
    bool isOverheated = false;

    FParticleSystem sparkParticleSystem;
    FParticleDefinition sparkParticleDefinition;

    public float lastX = 0;
    public float lastY = 0;

    float collisionWidth = 20;
    float collisionHeight = 25;

    public Player()
    {
        health = 8;
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
        sprite.addAnimation(new FAnimation("leftstun", new int[] { 32, 14 }, animSpeed / 100, true));
        sprite.addAnimation(new FAnimation("rightstun", new int[] { 31, 13 }, animSpeed / 100, true));
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

        this.damage = 1;
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
        if (stunCount <= 0)
        {

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
                if (!forceBounce && yVel > 0)
                    yVel *= .4f;
            }
            if (yVel > 0)
                currentAnimState = AnimState.JUMP;
            else
            {
                forceBounce = false;
                if (yVel < 0 && !grounded)
                {
                    currentAnimState = AnimState.FALL;

                }
            }
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
            if (grounded)
                currentAnimState = AnimState.IDLE;
            if (currentAnimState == AnimState.IDLE && xMove != 0)
                currentAnimState = AnimState.RUN;
            if (Input.GetKey(KeyCode.S))
            {
                if (currentAnimState == AnimState.JUMP || currentAnimState == AnimState.FALL)
                    currentAnimState = AnimState.FALL_ATTACK_DOWN;
            }
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
            if (isAttackDown() && !isOverheated)
            {
                spawnSparks();
                overheat += UnityEngine.Time.deltaTime * .25f;
                if (overheat >= 1)
                {
                    overheat = 1;
                    isOverheated = true;
                }
            }
            else if (isOverheated)
            {
                overheat -= UnityEngine.Time.deltaTime * .5f;
                if (overheat <= 0)
                {
                    isOverheated = false;
                }
            }
            else if (!isAttackDown())
            {
                if (overheat > 0)
                    overheat -= UnityEngine.Time.deltaTime * .5f;
                else
                    overheat = 0;
            }
            overheatBar.setIsOverheated(isOverheated);
            overheatBar.setOverheat(overheat);
        }
        else
        {
            if (xVel == 0)
                stunCount -= UnityEngine.Time.deltaTime;

            if (stunCount <= 0)
                invulnerableCount = C.stunInvulnTime;       //Done being stunned now have control and be invulnerable
        }
        yVel = Mathf.Clamp(yVel, maxYVel, jumpForce);
        if (xVel != 0)
            xMove = xVel;

        yMove = yVel;


        this.grounded = false;
        tryMove(xMove, yMove);

        if (stunCount > 0)
        {
            sprite.alpha = .5f;
            sprite.play((isFacingLeft ? "left" : "right") + "stun");
        }
        else
        {
            sprite.alpha = 1;
            sprite.play((isFacingLeft ? "left" : "right") + currentAnimState);
        }
        if (invulnerableCount > 0)
        {
            invulnerableCount -= UnityEngine.Time.deltaTime;
            sprite.alpha = ((invulnerableCount * 1000) % 10 < 5) ? 0 : 1;
        }

        sparkParticleSystem.x = -x;
        sparkParticleSystem.y = -y;


    }

    private bool isAttackDown()
    {
        return UnityEngine.Input.GetKey(KeyCode.U);
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
        }
        else
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

    public bool isAttackingDown()
    {
        return !isOverheated && (stunCount <= 0) && currentAnimState == AnimState.FALL_ATTACK_DOWN && isAttackDown();
    }
    public bool isAttackingRight()
    {
        return !isOverheated && (stunCount <= 0) && (currentAnimState == AnimState.RUN || currentAnimState == AnimState.IDLE || currentAnimState == AnimState.JUMP || currentAnimState == AnimState.FALL) && !isFacingLeft && isAttackDown();
    }

    public bool isAttackingLeft()
    {
        return !isOverheated && (stunCount <= 0) && (currentAnimState == AnimState.RUN || currentAnimState == AnimState.IDLE || currentAnimState == AnimState.JUMP || currentAnimState == AnimState.FALL) && isFacingLeft && isAttackDown();
    }

    public bool isAttackingUp()
    {
        return !isOverheated && (currentAnimState == AnimState.FALL_ATTACK_UP || currentAnimState == AnimState.IDLE_ATTACK_UP || currentAnimState == AnimState.JUMP_ATTACK_UP || currentAnimState == AnimState.RUN_ATTACK_UP) && isAttackDown();
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

    public void takeDamage(BaseGameObject myObject)
    {
        if (stunCount <= 0 && invulnerableCount <= 0)
        {
            stunCount = .4f;
            if (myObject.x > this.x)
                xVel = -3;
            else
                xVel = 3;
            yVel = jumpForce / 2;

            this.health -= myObject.damage;
            healthBar.setHealth(this.health);
        }
    }

    public void bounce()
    {
        yVel = jumpForce * .8f;
        forceBounce = true;

    }
    HealthBar healthBar;
    public void setHealthBar(HealthBar hb)
    {
        this.healthBar = hb;
    }

    OverheatBar overheatBar;
    public void setOverheatBar(OverheatBar ob)
    {
        this.overheatBar = ob;
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
                this.xVel = 0;
                this.yVel = 0;
                this.jumpsLeft = 1;
                break;
            }
        }
    }
    #endregion

}

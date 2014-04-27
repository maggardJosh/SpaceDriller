using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WallSlime : BaseGameObject
{
    private enum State
    {
        IDLE,
        STARTING_LAUNCH,
        LAUNCHING,
        LANDING
    }

    State currentState;

    FAnimatedSprite sprite;

    Vector2 originalPos;
    Vector2 secondaryPos;

    float yMargin = 10;
    int landAnimSpeed = 50;

    public WallSlime(Vector2 position, float rotation = 0)
    {

        this.SetPosition(position);
        originalPos = this.GetPosition();

        sprite = new FAnimatedSprite("wallSlime/wallSlime");
        sprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100));
        sprite.addAnimation(new FAnimation("start_launch", new int[] { 1, 2, 3, }, landAnimSpeed, false));
        sprite.addAnimation(new FAnimation("launching", new int[] { 4, 5, 6 }, 100, false));
        sprite.addAnimation(new FAnimation("landing", new int[] { 8, 9, 10, 11, 12 }, landAnimSpeed, false));

        sprite.play("idle");
        currentState = State.IDLE;
        sprite.rotation = rotation;
        this.AddChild(sprite);
    }

    public override void setWorld(World world)
    {
        base.setWorld(world);

        int tileX = Mathf.FloorToInt(this.x / world.map.tileWidth);
        int tileY = Mathf.FloorToInt(-this.y / world.map.tileHeight);

        if (sprite.rotation == 0)
        {
            while (world.collision.getFrameNum(tileX - 1, tileY) != 1)
                tileX--;
        }
        else if (sprite.rotation == 180)
        {
            while (world.collision.getFrameNum(tileX + 1, tileY) != 1)
                tileX++;
        }
        else if (sprite.rotation == 90)
        {
            //up
            while (world.collision.getFrameNum(tileX, tileY - 1) != 1)
                tileY--;
        }
        else if (sprite.rotation == 270)
        {
            //Down
            while (world.collision.getFrameNum(tileX, tileY + 1) != 1)
                tileY++;
        }

        secondaryPos = new Vector2(tileX * world.map.tileWidth + world.map.tileWidth / 2, -tileY * world.map.tileHeight - world.map.tileHeight / 2);
        movementTime = (originalPos - secondaryPos).magnitude / 300.0f;

    }
    bool atSecondaryPosition = false;
    EaseType movementEase = EaseType.QuartIn;
    float movementTime = .8f;
    float idleCount = 0;
    float idleMinCount = 1.0f;
    protected override void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (sprite.currentAnim.name != "idle")
                    sprite.play("idle");
                //Player crossed our LoS ATTACK!
                if (idleCount < idleMinCount)
                {
                    idleCount += UnityEngine.Time.deltaTime;
                    break;
                }
                if (((sprite.rotation == 0 || sprite.rotation == 180)
                    && (world.p.y < this.y + yMargin && world.p.y > this.y - yMargin)) ||
                    ((sprite.rotation == 90 || sprite.rotation == 270)
                    && (world.p.x < this.x + yMargin && world.p.x > this.x - yMargin)))
                {
                    sprite.play("start_launch", true);
                    currentState = State.STARTING_LAUNCH;
                }
                break;
            case State.STARTING_LAUNCH:
                if (sprite.IsStopped)
                {
                    Go.killAllTweensWithTarget(this);
                    sprite.play("launching");
                    if (sprite.rotation == 0 || sprite.rotation == 180)
                        Go.to(this, movementTime, new TweenConfig().floatProp("x", atSecondaryPosition ? originalPos.x : secondaryPos.x).setEaseType(movementEase).onComplete((AbstractTween t) => { sprite.play("landing"); currentState = State.LANDING; }));
                    else
                        Go.to(this, movementTime, new TweenConfig().floatProp("y", atSecondaryPosition ? originalPos.y : secondaryPos.y).setEaseType(movementEase).onComplete((AbstractTween t) => { sprite.play("landing"); currentState = State.LANDING; }));
                    currentState = State.LAUNCHING;
                }
                break;
            case State.LAUNCHING:
                //Do nothing Go Kit takes care of this
                break;
            case State.LANDING:
                if (sprite.IsStopped)
                {
                    idleCount = 0;
                    currentState = State.IDLE;
                    sprite.play("idle");
                    sprite.rotation += 180;
                    atSecondaryPosition = !atSecondaryPosition;
                    if (sprite.rotation >= 360)
                        sprite.rotation -= 360;

                }
                break;
        }
        Vector2 playerRelativePos = this.GetPosition() - world.p.GetPosition();
        if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width))
        {
            if (world.p.isAttackingDown() && world.p.yVel < 0 && world.p.y > this.y && world.p.x < this.x + sprite.width / 2 && world.p.x > this.x - sprite.width / 2)
            {
                world.p.bounce();
                if (this.lastDamageCounter > world.p.weaponDamageRate)
                {
                    this.takeDamage(world.p.damage);
                }

            }
            else if (world.p.isAttackingRight() && world.p.x < this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                    this.takeDamage(world.p.damage);
            }
            else if (world.p.isAttackingLeft() && world.p.x > this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                    this.takeDamage(world.p.damage);
            }

            else if (world.p.isAttackingUp() && world.p.y < this.y && world.p.x < this.x + sprite.width / 2 && world.p.x > this.x - sprite.width / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                    this.takeDamage(world.p.damage);
            }
            else
                if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width) / 5)
                {
                    world.p.takeDamage(this);
                }
        }
        base.Update();
    }
}

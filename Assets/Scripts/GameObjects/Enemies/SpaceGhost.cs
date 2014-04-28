using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpaceGhost : BaseGameObject
{
    private enum State
    {
        IDLE,
        CHASE,
        KNOCKBACK
    }

    State currentState;

    FAnimatedSprite sprite;

    Vector2 originalPos;
    int animSpeed = 100;
    Vector2 knockBackVel;
    float chaseSpeed = 40;
    float knockBackCount = 0;
    float knockBackCountDelay = .3f;
    int tilesAwayTakeNotice = 10;

    public SpaceGhost(Vector2 position, int level = 1)
    {

        this.SetPosition(position);
        originalPos = this.GetPosition();
        this.health = 10;

        sprite = new FAnimatedSprite("spaceGhost1/spaceGhost" + level.ToString());
        sprite.addAnimation(new FAnimation("idle", new int[] { 1,2,3,4 }, animSpeed));
        sprite.addAnimation(new FAnimation("chase", new int[] { 5,6,7,8 }, animSpeed, true));
        sprite.addAnimation(new FAnimation("stun", new int[] { 9 }, animSpeed, true));

        sprite.play("idle");
        currentState = State.IDLE;
        this.AddChild(sprite);
    }

    float maxRandomMeander = 32 * 5;
    float meanderTime = 4;
    float minMeanderDelay = .5f;
    float maxMeanderDelay = 3.0f;
    float knockbackStrength = 300;

    protected override void Update()
    {
        base.Update();
        if (!isAlive)
        {
            if (lastIsAlive)
                foreach (AbstractTween tween in Go.tweensWithTarget(this))
                    tween.pause();
            return;
        }
        else
        {
            if (!lastIsAlive)
                foreach (AbstractTween tween in Go.tweensWithTarget(this))
                    tween.play();
        }
        Vector2 playerRelativePos = this.GetPosition() - world.p.GetPosition();
        switch (currentState)
        {
            case State.IDLE:
                sprite.play("idle");
                if (Go.tweensWithTarget(this).Count == 0)       //Not moving around
                {
                    Vector2 newPosition = originalPos + new Vector2(RXRandom.Float() * maxRandomMeander - maxRandomMeander / 2, RXRandom.Float() * maxRandomMeander - maxRandomMeander / 2);
                    Go.to(this, meanderTime, new TweenConfig().setDelay(RXRandom.Float() * (maxMeanderDelay - minMeanderDelay) + minMeanderDelay).floatProp("x", newPosition.x).floatProp("y", newPosition.y).setEaseType(EaseType.CubicInOut));
                }
                if (playerRelativePos.sqrMagnitude <= (world.map.tileWidth * world.map.tileWidth) * tilesAwayTakeNotice * 2)
                {
                    Go.killAllTweensWithTarget(this);
                    currentState = State.CHASE;
                }
                break;
            case State.CHASE:
                sprite.play("chase");
                if (playerRelativePos.sqrMagnitude > (world.map.tileWidth * world.map.tileWidth) * tilesAwayTakeNotice * 2)
                {
                    currentState = State.IDLE;
                    originalPos = this.GetPosition();       //Set new home point
                }
                else
                {
                    Vector2 normVect = playerRelativePos.normalized;
                    x -= normVect.x * UnityEngine.Time.deltaTime * chaseSpeed;
                    if (normVect.x > 0)
                        sprite.scaleX = -1;
                    else
                        sprite.scaleX = 1;
                    y -= normVect.y * UnityEngine.Time.deltaTime * chaseSpeed;
                }
                break;
            case State.KNOCKBACK:
                sprite.play("stun");
                if (knockBackCount >= knockBackCountDelay)
                {
                    x += knockBackVel.x * UnityEngine.Time.deltaTime;
                    y += knockBackVel.y * UnityEngine.Time.deltaTime;
                    knockBackVel *= .94f;
                    if (knockBackVel.sqrMagnitude < 5)
                    {
                        currentState = State.IDLE;
                        knockBackCount = 0;
                    }
                }
                else
                {
                    knockBackCount += UnityEngine.Time.deltaTime;
                }

                break;
        }

        if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width))
        {
            if (world.p.isAttackingDown() && world.p.yVel < 0 && world.p.y > this.y && world.p.x < this.x + Mathf.Abs(sprite.width) / 2 && world.p.x > this.x - Mathf.Abs(sprite.width) / 2)
            {
                world.p.bounce();
                if (this.lastDamageCounter > world.p.weaponDamageRate)
                {
                    this.takeDamage(world.p.damage * 2);
                    if (knockBackCount == 0)
                        currentState = State.KNOCKBACK;

                    knockBackVel = playerRelativePos.normalized * knockbackStrength;
                }

            }
            else if (world.p.isAttackingRight() && world.p.x < this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);
                if (knockBackCount == 0)
                    currentState = State.KNOCKBACK;
                knockBackVel = playerRelativePos.normalized * knockbackStrength;
            }
            else if (world.p.isAttackingLeft() && world.p.x > this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);
                if (knockBackCount == 0)
                    currentState = State.KNOCKBACK;
                knockBackVel = playerRelativePos.normalized * knockbackStrength;
            }

            else if (world.p.isAttackingUp() && world.p.y < this.y && world.p.x < this.x + Mathf.Abs(sprite.width) / 2 && world.p.x > this.x - Mathf.Abs(sprite.width) / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);
                if (knockBackCount == 0)
                    currentState = State.KNOCKBACK;
                knockBackVel = playerRelativePos.normalized * knockbackStrength;
            }
            else
                if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width) / 5)
                {
                    if (currentState != State.KNOCKBACK)
                        world.p.takeDamage(this);
                }

            if (currentState == State.KNOCKBACK)
            {
                if (playerRelativePos.sqrMagnitude < (sprite.width/2) * (sprite.width/2))
                {

                    this.x = world.p.GetPosition().x + playerRelativePos.normalized.x * Mathf.Abs(sprite.width/1.9f );
                    this.y = world.p.GetPosition().y + playerRelativePos.normalized.y * Mathf.Abs(sprite.width/1.9f );

                }
            }
        }
    }

    public void checkOtherGhost(SpaceGhost otherGhost)
    {
        if (currentState == State.IDLE)
            return;
        Vector2 diff = this.GetPosition() - otherGhost.GetPosition();
        if (diff.sqrMagnitude < (this.sprite.width * sprite.width) / 2)
        {
            this.x = Mathf.Lerp(this.x, otherGhost.GetPosition().x + diff.normalized.x * Mathf.Abs(sprite.width / 2), .2f);
            this.y = Mathf.Lerp(this.y, otherGhost.GetPosition().y + diff.normalized.y * Mathf.Abs(sprite.width / 2), .2f);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TikiGuy : BaseGameObject
{
    const int NUM_TIKI_FACES = 1;
    FAnimatedSprite sprite;
    public float height { get { return sprite.height; } }
    Tiki tikiParent;
    Vector2 disp = Vector2.zero;
    public TikiGuy(Tiki tikiParent)
    {
        this.tikiParent = tikiParent;
        sprite = new FAnimatedSprite("Tiki/TikiGuy" + (RXRandom.Int(NUM_TIKI_FACES) + 1).ToString());
        sprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100, true));
        sprite.addAnimation(new FAnimation("active", new int[] { 2 }, 100, true));
        sprite.play("idle");
        this.health = 3;
    }

    public void goActive()
    {
        sprite.play("active");

    }

    public bool collides(Player p)
    {
        Vector2 diff = (p.GetPosition() - tikiParent.GetPosition()) - this.GetPosition();
        return (diff.sqrMagnitude <= world.map.tileWidth * world.map.tileWidth);
    }
    public override void HandleAddedToContainer(FContainer container)
    {
        container.AddChild(sprite);
        base.HandleAddedToContainer(container);
    }

    public override void HandleRemovedFromContainer()
    {
        container.RemoveChild(sprite);
        base.HandleRemovedFromContainer();
    }

    protected override void Update()
    {
        if ((tikiParent.currentState == Tiki.State.BECOMING_ACTIVE) || ( tikiParent.currentState == Tiki.State.ACTIVE ))
            disp.x = RXRandom.Int(2);
        else
            disp.x = 0;

        sprite.SetPosition(this.GetPosition()+ disp);
        base.Update();
    }

    public void moveDown(int newPosition)
    {
        Go.killAllTweensWithTarget(this);
        Go.to(this, .5f, new TweenConfig().floatProp("y", newPosition * (height)).setEaseType(EaseType.CubicIn));
    }

    public void checkCollision()
    {
        Vector2 tikiPos = tikiParent.GetPosition();
        Vector2 playerRelativePos = world.p.GetPosition() - tikiPos;
        Vector2 playerRelativeToUsPos = playerRelativePos - this.GetPosition();

        if (playerRelativeToUsPos.x > -sprite.width &&
            playerRelativeToUsPos.x < sprite.width &&
            playerRelativeToUsPos.y > -sprite.height &&
            playerRelativeToUsPos.y < sprite.height)
        {
            if (world.p.isAttackingDown() && world.p.yVel < 0 && playerRelativePos.y > this.y && playerRelativePos.x < this.x + sprite.width / 2 && playerRelativePos.x > this.x - sprite.width / 2)
            {
                world.p.bounce();
                if (this.lastDamageCounter > world.p.weaponDamageRate)
                {
                    this.takeDamage(world.p.damage * 2, this.GetPosition() + tikiPos);
                }

            }
            else 
            if (world.p.isAttackingRight() && playerRelativePos.x < this.x && playerRelativePos.y > this.y - sprite.height / 2 && playerRelativePos.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage, this.GetPosition() + tikiPos);

            }
            else if (world.p.isAttackingLeft() && playerRelativePos.x > this.x && playerRelativePos.y > this.y - sprite.height / 2 && playerRelativePos.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage, this.GetPosition() + tikiPos);
            }

            else if (world.p.isAttackingUp() && playerRelativePos.y < this.y && playerRelativePos.x < this.x + Mathf.Abs(sprite.width) / 2 && playerRelativePos.x > this.x - Mathf.Abs(sprite.width) / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage, this.GetPosition() + tikiPos);

            }
            else
                if (tikiParent.currentState != Tiki.State.BECOMING_ACTIVE && (playerRelativePos - this.GetPosition()).sqrMagnitude < (sprite.width * sprite.width) / 5)
                {

                    world.p.takeDamage(this);
                }


        }
    }
}


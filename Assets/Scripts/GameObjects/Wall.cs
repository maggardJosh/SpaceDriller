using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Wall : BaseGameObject
{
    FAnimatedSprite sprite;
    int level;
    bool isBroken = false;
    public Wall(Vector2 pos, int level)
    {
        this.SetPosition(pos);
        this.level = level;
        sprite = new FAnimatedSprite("door" + level.ToString());
        sprite.addAnimation(new FAnimation("normal", new int[] { 1 }, 100));
        sprite.addAnimation(new FAnimation("broken", new int[] { 2 }, 100));
        this.AddChild(sprite);
    }

    protected override void Update()
    {
        if (!isBroken)
        {
            Vector2 playerRelativePos = this.GetPosition() - world.p.GetPosition();
            if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width))
            {
                if (world.p.isAttackingRight() && world.p.x < this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
                {
                    this.takeDamage((world.p.drillLevel >= this.level) ? world.p.damage : 0);

                }
                else if (world.p.isAttackingLeft() && world.p.x > this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
                {
                    this.takeDamage((world.p.drillLevel >= this.level) ? world.p.damage : 0);

                }
            }
        }
        base.Update();
    }
    public void breakDoor()
    {
        isBroken = true;
        sprite.play("broken");
    }
    protected override void die()
    {
        //Particles
        sprite.play("broken");
    }
}


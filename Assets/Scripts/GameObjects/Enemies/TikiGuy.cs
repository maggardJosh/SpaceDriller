using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TikiGuy : BaseGameObject
{
    const int NUM_TIKI_FACES = 1;
    FAnimatedSprite sprite;
    public TikiGuy()
    {
        sprite = new FAnimatedSprite("Tiki/TikiGuy" + (RXRandom.Int(NUM_TIKI_FACES) + 1).ToString());
        sprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100, true));
        sprite.addAnimation(new FAnimation("active", new int[] { 2 }, 100, true));
        sprite.play("idle");
        this.health = 30;
    }

    public void checkCollision(Vector2 playerRelativePos)
    {
        if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width))
        {
            if (world.p.isAttackingRight() && world.p.x < this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);

            }
            else if (world.p.isAttackingLeft() && world.p.x > this.x && world.p.y > this.y - sprite.height / 2 && world.p.y < this.y + sprite.height / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);

            }

            else if (world.p.isAttackingUp() && world.p.y < this.y && world.p.x < this.x + Mathf.Abs(sprite.width) / 2 && world.p.x > this.x - Mathf.Abs(sprite.width) / 2 && this.lastDamageCounter > world.p.weaponDamageRate)
            {
                this.takeDamage(world.p.damage);

            }
            else
                if (playerRelativePos.sqrMagnitude < (sprite.width * sprite.width) / 5)
                {

                    world.p.takeDamage(this);
                }


        }
    }
}


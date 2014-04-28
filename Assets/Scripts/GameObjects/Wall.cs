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
    int doorNumber;
    
    public Wall(Vector2 pos, int level, int doorNumber)
    {
        this.doorNumber = doorNumber;
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
                if (playerRelativePos.x > -sprite.width / 2 &&
                    playerRelativePos.x < sprite.width / 2 &&
                    playerRelativePos.y > -sprite.height / 2 &&
                    playerRelativePos.y < sprite.height / 2)
                {
                  
                    if (playerRelativePos.x > 0)
                        world.p.x = this.x - sprite.width/2;
                    else
                        world.p.x = this.x + sprite.width/2;
                    
                }
                
            }
        }
        base.Update();
    }
    public void breakDoor()
    {
        RXDebug.Log("HI");
        isBroken = true;
        sprite.play("broken");
    }
    protected override void die()
    {
        C.doorsBroken.Add(new KeyValuePair<string, int>(world.map.mapName, doorNumber));
        //Particles
        breakDoor();
    }
}


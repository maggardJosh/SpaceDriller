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
        this.health = 15 * level;
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
                        world.p.x = this.x - sprite.width / 2;
                    else
                        world.p.x = this.x + sprite.width / 2;

                }

            }
        }
        base.Update();
    }
    protected override void takeDamage(int damageAmount, Vector2 position)
    {
        base.takeDamage(damageAmount, position);
        FSoundManager.PlaySound("Hit");
    }
    public void breakDoor()
    {
        isBroken = true;
        sprite.play("broken");
    }
    protected override void die()
    {
        FParticleSystem rubbleParticleSystem;
        FParticleDefinition rubbleParticleDefinition;
        
        rubbleParticleSystem = new FParticleSystem(80);
        this.AddChild(rubbleParticleSystem);
        rubbleParticleDefinition = new FParticleDefinition(C.whiteElement);
        rubbleParticleDefinition.startColor = Color.white;
        rubbleParticleDefinition.startScale = 1;
        rubbleParticleDefinition.endScale = 0;
        rubbleParticleDefinition.endColor = new Color(0, 0, 0, 0);
        rubbleParticleDefinition.lifetime = 4;
        rubbleParticleSystem.accelY = -300;
        for (int i = 0; i < 80; i++)
        {
            rubbleParticleDefinition.SetElementByName("doorParticle" + level + "_" + (RXRandom.Int(3) + 1));
            rubbleParticleDefinition.x = RXRandom.Float() * sprite.width - sprite.width / 2;
            rubbleParticleDefinition.y = RXRandom.Float() * sprite.height - sprite.height / 2;
            float angle = 180 * RXRandom.Float();
            rubbleParticleDefinition.speedX = Mathf.Cos(angle * C.PIOVER180) * (75 + 25 * RXRandom.Float());
            rubbleParticleDefinition.speedY = Mathf.Sin(angle * C.PIOVER180) * (75 + 25 * RXRandom.Float());
            rubbleParticleSystem.AddParticle(rubbleParticleDefinition);
        }
        FSoundManager.PlaySound("Totem2");
        C.doorsBroken.Add(new KeyValuePair<string, int>(world.map.mapName, doorNumber));
        //Particles
        breakDoor();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BaseGameObject : FContainer
{
    protected bool isAlive = true;
    protected World world;
    public int health = 2;
    public int damage = 1;

    public float lastDamageCounter = 1;

    public BaseGameObject()
    {

    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromStage();
    }

    protected virtual void Update()
    {
        isAlive = !C.transitioning;
        if (lastDamageCounter < 3)
            lastDamageCounter += UnityEngine.Time.deltaTime;
    }

    public virtual void setWorld(World world)
    {
        this.world = world;
    }

    protected void takeDamage(int damageAmount, Vector2 position)
    {
        this.health -= damageAmount;
        lastDamageCounter = 0;
        for (int i = 0; i < damageAmount; i++)
            Futile.stage.AddChild(new DamageIndicator(position ));
        if (damageAmount == 0)
            Futile.stage.AddChild(new DamageIndicator(position, false));
        if (this.health <= 0)
            this.die();
    }
    protected void takeDamage(int damageAmount)
    {
        this.takeDamage(damageAmount, this.GetPosition());
    }

    protected void die()
    {
        //TODO: actual death 
        this.RemoveFromContainer();
    }
}

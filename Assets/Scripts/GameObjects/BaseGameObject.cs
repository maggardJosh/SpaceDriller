using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BaseGameObject : FContainer
{
    protected bool isAlive = true;
    protected World world;
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
    }

    public virtual void setWorld(World world)
    {
        this.world = world;
    }
}

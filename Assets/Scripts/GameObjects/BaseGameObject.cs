using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BaseGameObject : FContainer
{
    protected bool isAlive = true;
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
}

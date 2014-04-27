using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Pickup : FSprite
{
    Action<Player> pickupAction;
    string pickupMessage;

    FLabel displayMessage;

    public Pickup(Vector2 pos, string elementName, string pickupMessage, Action<Player> pickupAction)
        : base(elementName)
    {
        this.SetPosition(pos);
        this.pickupMessage = pickupMessage;
        this.pickupAction = pickupAction;

        displayMessage = new FLabel(C.NormalFont, pickupMessage);

    }

    public void pickup(Player p)
    {
        pickupAction(p);
        C.transitioning = true;
        Go.killAllTweensWithTarget(this);
        this.RemoveFromContainer();
        C.getCameraInstance().AddChild(this);
        this.x -= C.getCameraInstance().x;
        this.y -= C.getCameraInstance().y;

        Go.to(this, 2.0f, new TweenConfig().floatProp("x", 0).floatProp("y", 0).floatProp("scale", 3).onComplete((AbstractTween t) =>
        {
            C.getCameraInstance().AddChild(displayMessage);
            displayMessage.alpha = 0;
            displayMessage.x = this.x;
            displayMessage.y = this.y - 100;
            Go.to(displayMessage, 2.0f, new TweenConfig().floatProp("alpha", 1.0f).onComplete((AbstractTween t2) =>
            {
                Futile.instance.SignalUpdate += listenForKey;
            }));
        }));
    }

    public void listenForKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            C.transitioning = false;
            this.RemoveFromContainer();
            displayMessage.RemoveFromContainer();
            Futile.instance.SignalUpdate -= listenForKey;
        }
    }
}


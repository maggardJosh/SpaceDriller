using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InGamePage : FPage
{
    FCamObject camera;
    World world;
    FSprite whiteOverlay;

    public InGamePage()
    {
        whiteOverlay = new FSprite(C.whiteElement);
        whiteOverlay.color = Color.black;
        whiteOverlay.width = Futile.screen.width;
        whiteOverlay.height = Futile.screen.height;

        this.addObjectToPage(new FSprite(C.whiteElement), Vector2.up * (Futile.screen.height + 100));
    }
    protected override void transitionOn(AbstractTween tween)
    {
        base.transitionOn(tween);
        
        FSoundManager.PlayMusic("SD1st Area Music");
        world = new World();
        Futile.stage.AddChild(world);
        world.loadMap(C.startMap);
        Player p = new Player(this);
        world.spawnPlayer(p, C.startDoor);
        HealthBar hb = new HealthBar();
        OverheatBar ob = new OverheatBar();
        hb.y = (int)(Futile.screen.halfHeight - hb.Height / 2 - 10);
        hb.x = (int)(-Futile.screen.halfWidth + hb.Width / 2 + 10);

        ob.y = hb.y - hb.Height / 2 - ob.Height / 2 - 5;
        ob.x = hb.x;

        C.getCameraInstance().AddChild(hb);
        C.getCameraInstance().AddChild(ob);
        C.getCameraInstance().AddChild(whiteOverlay);

        p.setHealthBar(hb);
        p.setOverheatBar(ob);

        C.transitioning = true;
        fadeOn().setOnCompleteHandler((AbstractTween t) => { C.transitioning = false; });
    }

    public Tween fadeOn()
    {
        Go.killAllTweensWithTarget(whiteOverlay);
        whiteOverlay.alpha = 1;
        return Go.to(whiteOverlay, 1.0f, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.CubicIn));
    }

    public Tween fadeOut()
    {
        Go.killAllTweensWithTarget(whiteOverlay);
        return Go.to(whiteOverlay, 1.0f, new TweenConfig().floatProp("alpha", 1).setEaseType(EaseType.CubicIn));
    }

}


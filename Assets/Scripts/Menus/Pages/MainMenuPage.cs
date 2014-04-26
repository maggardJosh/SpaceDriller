using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MainMenuPage : FPage
{
    FLabel pressStart;
    public MainMenuPage()
        : base()
    {
        pressStart = new FLabel(C.NormalFont, "Press Space to Start");
        this.addObjectToPage(pressStart, new Vector2(0, 0));
    }

    protected override void transitionOn(AbstractTween tween)
    {
        base.transitionOn(tween);
        Go.killAllTweensWithTarget(pressStart);
        pressStart.alpha = 1;
        Go.to(pressStart, 3.0f, new TweenConfig().floatProp("alpha", .5f).setEaseType(EaseType.CircIn).setIterations(-1, LoopType.PingPong));
    }

    public override void Update()
    {
        if (CurrentState == State.ON)
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                FPageManager.getInstance().transitionOn(new InGamePage());

        base.Update();
    }
}

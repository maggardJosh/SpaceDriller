using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MainMenuPage : FPage
{
    FSprite bg;
    FSprite titleLabel;
    FLabel pressDLabel;
    FLabel ldLabel;
    FSprite[] moltars = new FSprite[30];
    float margin = 10;
    public MainMenuPage()
        : base()
    {
        FSoundManager.PlayMusic("SDMainMenu");
        bg = new FSprite("title");
        this.addObjectToPage(bg, Vector2.zero);

        titleLabel = new FSprite("titleLabel");
        pressDLabel = new FLabel(C.NormalFont, "Press D to Drill");

        ldLabel = new FLabel(C.SmallFont , "Made in 72 hours\nLudum Dare 29");
        this.addObjectToPage(ldLabel, new Vector2(-Futile.screen.halfWidth + ldLabel.textRect.width / 2 + margin, -Futile.screen.halfHeight + ldLabel.textRect.height / 2 + margin));

        for (int i = 0; i < moltars.Length; i++)
        {
            moltars[i] = new FSprite("player_" + String.Format("{0:00}", RXRandom.Int(30)+1));
            this.AddChild(moltars[i]);
            moltars[i].y = Futile.screen.height + 50;
        }
        this.AddChild(titleLabel);
        this.AddChild(pressDLabel);
        titleLabel.alpha = 0;
        titleLabel.x = -Futile.screen.halfWidth + titleLabel.width / 2 + margin / 2;
        titleLabel.y = Futile.screen.halfHeight - titleLabel.height / 2 - margin / 2;
        titleLabel.scale = 0;
        pressDLabel.alpha = 0;
        pressDLabel.x = Futile.screen.halfWidth - pressDLabel.textRect.width / 2 - margin / 2;
        pressDLabel.y = -Futile.screen.halfHeight + pressDLabel.textRect.height / 2 + margin / 2;
    }

    protected override void transitionOn(AbstractTween tween)
    {
        base.transitionOn(tween);
        Go.to(titleLabel, 1.5f, new TweenConfig().floatProp("scale", 1).floatProp("alpha", 1).setEaseType(EaseType.QuadOut).setDelay(.5f).onComplete((AbstractTween t) =>
        {
            Go.to(titleLabel, 8f, new TweenConfig().floatProp("scale", .9f).setEaseType(EaseType.QuadInOut).setIterations(-1, LoopType.PingPong));
            for (int i = 0; i < moltars.Length; i++)
            {
                int moltarIndex = i;
                float randAngle = RXRandom.Float() * 360f;
                moltars[i].x = Mathf.Cos(randAngle * C.PIOVER180) * (Futile.screen.width + 40f);
                moltars[i].y = Mathf.Sin(randAngle * C.PIOVER180) * (Futile.screen.width + 40f);
                Go.to(moltars[i], 25, new TweenConfig().setDelay(RXRandom.Float() * 20).floatProp("x", Mathf.Cos(-randAngle * C.PIOVER180) * (Futile.screen.width + 40f)).floatProp("y", Mathf.Sin(-randAngle * C.PIOVER180) * (Futile.screen.width + 40f)).floatProp("rotation", 360 * 12).onComplete((AbstractTween t2) => { randomTweenMoltar(moltarIndex); }));
            }

        }));
        Go.to(pressDLabel, 2.0f, new TweenConfig().floatProp("alpha", 1).setEaseType(EaseType.QuadInOut).setDelay(4));
    }

    private void randomTweenMoltar(int moltarIndex)
    {
        moltars[moltarIndex].SetElementByName("player_" + String.Format("{0:00}", RXRandom.Int(30) + 1));
        float randAngle = RXRandom.Float() * 360f;
        moltars[moltarIndex].x = Mathf.Cos(randAngle * C.PIOVER180) * (Futile.screen.width + 40f);
        moltars[moltarIndex].y = Mathf.Sin(randAngle * C.PIOVER180) * (Futile.screen.width + 40f);
        Go.to(moltars[moltarIndex], 25, new TweenConfig().setDelay(RXRandom.Float() * 5).floatProp("x", Mathf.Cos(-randAngle * C.PIOVER180) * (Futile.screen.width + 40f)).floatProp("y", Mathf.Sin(-randAngle * C.PIOVER180) * (Futile.screen.width + 40f)).floatProp("rotation", moltars[moltarIndex].rotation + 360 * 12).onComplete((AbstractTween t) => { randomTweenMoltar(moltarIndex); }));
    }

    public override void Update()
    {
        if (CurrentState == State.ON)
            if (UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                FPageManager.getInstance().transitionOn(new InGamePage());
                Go.killAllTweensWithTarget(titleLabel);
                Go.killAllTweensWithTarget(pressDLabel);
                Go.to(titleLabel, animOutTime, new TweenConfig().floatProp("scale", 0).setEaseType(EaseType.CubicOut));
                Go.to(pressDLabel, animOutTime, new TweenConfig().floatProp("alpha", 0).setEaseType(EaseType.CubicOut));
            }

        base.Update();
    }
}


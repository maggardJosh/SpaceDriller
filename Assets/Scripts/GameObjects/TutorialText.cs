using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TutorialText
{
    private Rect bounds;
    private FLabel text;

    bool isPlayerInBounds = false;
    public TutorialText(Vector2 position, float width, float height, string text)
    {
        bounds = new Rect(position.x, -position.y - height, width, height);
        text = text.Replace("\\n", "\n");
        this.text = new FLabel(C.SmallFont, text);
        this.text.y = -Futile.screen.halfHeight + this.text.textRect.height / 2 + 10;
        this.text.alpha = 0;
        C.getCameraInstance().AddChild(this.text);
    }

    public void checkPlayer(Player p)
    {
      
        if(!isPlayerInBounds && bounds.Contains(p.GetPosition()))
        {
            isPlayerInBounds = true;
            Go.killAllTweensWithTarget(text);
            Go.to(text, 1.0f, new TweenConfig().floatProp("alpha", 1.0f).setEaseType(EaseType.CubicOut));
        }else
        if(isPlayerInBounds && !bounds.Contains(p.GetPosition()))
        {
            isPlayerInBounds = false;
            Go.killAllTweensWithTarget(text);
            Go.to(text, .3f, new TweenConfig().floatProp("alpha", 0.0f).setEaseType(EaseType.CubicOut));
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class OverheatBar : FContainer
{
    FLabel label;
    FSprite barWhiteOutline;
    FSprite barBG;
    FWipeSprite filledBar;

    float barWidth = 94;
    float barHeight = 10;
    float blackBarMargin = 2;

    public float Height { get { return barWhiteOutline.height + label.textRect.height + 5; } }
    public float Width { get { return barWhiteOutline.width; } }

    public OverheatBar()
    {

        barBG = new FSprite(C.whiteElement);
        barBG.width = barWidth + blackBarMargin * 2;
        barBG.height = barHeight + blackBarMargin * 2;
        barBG.color = Color.black;

        barWhiteOutline = new FSprite(C.whiteElement);
        barWhiteOutline.width = barBG.width + 4;
        barWhiteOutline.height = barBG.height + 4;

        label = new FLabel(C.SmallFont, "Overheat");
        label.y = barWhiteOutline.height / 2 + 5;

        this.AddChild(label);
        this.AddChild(barWhiteOutline);
        this.AddChild(barBG);

        filledBar = new FWipeSprite(C.whiteElement);
        filledBar.width = barWidth;
        filledBar.height = barHeight;
        filledBar.color = Color.yellow;
        this.AddChild(filledBar);

        filledBar.wipeLeftAmount = 0;

    }

    public void setOverheat(float newOverHeat)
    {
        filledBar.wipeLeftAmount = newOverHeat;
    }

    public void setIsOverheated(bool isOverheated)
    {
        filledBar.color = isOverheated ? new Color(.8f, .8f, 0) : new Color(.3f, .3f, 0);
    }
}


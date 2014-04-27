using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HealthBar : FContainer
{
    const int NUM_CUBES = 8;
    FLabel healthLabel;
    FSprite barWhiteOutline;
    FSprite barBG;
    FSprite[] healthCubes = new FSprite[NUM_CUBES];
    FSprite[] animHealthCubes = new FSprite[NUM_CUBES];

    float healthCubeSize = 10;
    float cubeSideMargin = 2;

    public float Height { get { return barWhiteOutline.height + healthLabel.textRect.height + 5; } }
    public float Width { get { return barWhiteOutline.width; } }

    public HealthBar()
    {

        barBG = new FSprite(C.whiteElement);
        barBG.width = (healthCubeSize + cubeSideMargin) * NUM_CUBES + cubeSideMargin;
        barBG.height = healthCubeSize + cubeSideMargin * 2;
        barBG.color = Color.black;

        barWhiteOutline = new FSprite(C.whiteElement);
        barWhiteOutline.width = barBG.width + 4;
        barWhiteOutline.height = barBG.height + 4;

        healthLabel = new FLabel(C.SmallFont, "Health");
        healthLabel.y = barWhiteOutline.height / 2 + 5;

        this.AddChild(healthLabel);
        this.AddChild(barWhiteOutline);
        this.AddChild(barBG);
        for (int i = 0; i < NUM_CUBES; i++)
        {
            healthCubes[i] = new FSprite(C.whiteElement);
            healthCubes[i].width = healthCubeSize;
            healthCubes[i].height = healthCubeSize;
            healthCubes[i].color = Color.red;

            healthCubes[i].x = (int)(-((healthCubeSize + cubeSideMargin) * NUM_CUBES) / 2 + (cubeSideMargin / 2 + healthCubeSize / 2) + (cubeSideMargin + healthCubeSize) * i);
            this.AddChild(healthCubes[i]);

            animHealthCubes[i] = new FSprite(C.whiteElement);
            animHealthCubes[i].width = healthCubeSize;
            animHealthCubes[i].height = healthCubeSize;
            animHealthCubes[i].color = Color.red;
            animHealthCubes[i].alpha = 0;
            this.AddChild(animHealthCubes[i]);
        }
    }

    public void setHealth(int newHealth)
    {
        for (int i = 0; i < NUM_CUBES; i++)
        {
            if (i >= newHealth)
            {
                if (healthCubes[i].isVisible)
                {
                    healthCubes[i].isVisible = false;
                    Go.killAllTweensWithTarget(animHealthCubes[i]);
                    animHealthCubes[i].SetPosition(healthCubes[i].GetPosition());
                    animHealthCubes[i].alpha = 1;
                    Go.to(animHealthCubes[i], 1.0f, new TweenConfig().floatProp("y", animHealthCubes[i].y - 30).floatProp("alpha", 0).floatProp("x", animHealthCubes[i].x + RXRandom.Float() * 20 - 10).setEaseType(EaseType.QuadOut));
                }
            }
        }
    }

}


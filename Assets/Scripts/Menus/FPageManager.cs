using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FPageManager
{
    private FSprite background;
    private FPage currentPage;
    private FPage nextPage;

    private static FPageManager instance;
    public static FPageManager getInstance()
    {
        if (instance == null)
            instance = new FPageManager();
        return instance;
    }

    private FPageManager()
    {
        currentPage = new InGamePage();
        currentPage.startTransitionOn();
    }

    Action pageOffAction = null;
    public void transitionOn(FPage newPage, Action pageOffAction = null)
    {
        this.pageOffAction = pageOffAction;
        if ( newPage == null)
            return;
        this.nextPage = newPage;
        currentPage.startTransitionOff(handlePageOff);
    }

    public void handlePageOff()
    {
        if (pageOffAction != null)
            pageOffAction.Invoke();

        this.currentPage = this.nextPage;
        this.nextPage = null;
        this.currentPage.startTransitionOn();
    }

    internal void reset(FPage newPage)
    {
        Futile.stage.RemoveAllChildren();
        background = new FSprite("bg_1");
        background.width = Futile.screen.width;
        background.height = Futile.screen.height;
        Futile.stage.AddChild(background);
        Futile.stage.SetPosition(Vector2.zero);
        background.MoveToBack();
        this.transitionOn(newPage);
    }
}


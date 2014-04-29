using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Credits : FPage
{
    FLabel thanks;
    
    
    public Credits()
        : base()
    {
        FSoundManager.PlayMusic("SDMainMenu");
        thanks = new FLabel(C.NormalFont, "Thanks for Playing!\n\nGame made by:\nJosh Maggard - Programming\nDan Konves - Art & Game Design\nTyler Collins - Music");
        this.addObjectToPage(thanks, Vector2.zero);
    }


    public override void Update()
    {
        if (CurrentState == State.ON)
            if (UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                FPageManager.getInstance().transitionOn(new MainMenuPage());
              
            }

        base.Update();
    }
}


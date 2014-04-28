using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Wall : BaseGameObject
{
    FAnimatedSprite sprite;
    public Wall(int level)
    {
        sprite = new FAnimatedSprite("door" + level.ToString());
        sprite.addAnimation(new FAnimation("normal", new int[] { 1 }, 100));
        sprite.addAnimation(new FAnimation("broken", new int[] { 2 }, 100));

    }
}


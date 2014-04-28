using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tiki : BaseGameObject
{
    private enum State
    {
        IDLE,
        ACTIVE
    }

    State currentState;

    List<TikiGuy> tikiGuys = new List<TikiGuy>();

    public Tiki(Vector2 position, int numTikiGuys)
    {
        this.SetPosition(position);
        this.health = 30;
        currentState = State.IDLE;

    }

    protected override void Update()
    {
        base.Update();
        Vector2 playerRelativePos = this.GetPosition() - world.p.GetPosition();
        switch (currentState)
        {
            case State.IDLE:

                break;
            case State.ACTIVE:

                break;
        }

        foreach (TikiGuy t in tikiGuys)
        {
            t.checkCollision(playerRelativePos);
        }
        
    }

}

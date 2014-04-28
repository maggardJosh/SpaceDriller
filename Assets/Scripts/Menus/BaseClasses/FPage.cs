using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FPage : FContainer
{
    public enum State
    {
        OFF,
        TRANS_ON,
        ON,
        TRANS_OFF,
    }

    public enum TransitionType
    {
        INSTANT,
        FADE_IN,
        SLIDE
    }

    private TransitionType transIn = TransitionType.FADE_IN;
    private TransitionType transOut = TransitionType.FADE_IN;

    Dictionary<FNode, Vector2> pageObjectPositions = new Dictionary<FNode, Vector2>();
    private Action transitionOffDoneCallback;

    public State CurrentState { private set; get; }


    public FPage()
        : base()
    {
        CurrentState = State.OFF;

    }

    protected void addObjectToPage(FNode addObject, Vector2 pos)
    {
        addObject.x = pos.x;
        addObject.y = pos.y;
        this.AddChild(addObject);
        pageObjectPositions.Add(addObject, pos);
    }

    protected void moveObject(FNode objectToMove, Vector2 newPos)
    {
        objectToMove.SetPosition(newPos);
        if (pageObjectPositions.ContainsKey(objectToMove))
            pageObjectPositions[objectToMove] = newPos;
        else
            RXDebug.Log("Tried to move object not added to page: " + objectToMove);
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromStage();
    }

    protected void setTransitionType(TransitionType newTransition)
    {
        this.transIn = newTransition;
        this.transOut = newTransition;
    }

    protected float animInTime = 1.5f;
    protected float animOutTime = .5f;
    EaseType easeTypeIn = EaseType.CubicOut;
    EaseType easeTypeOut = EaseType.CubicIn;
    public virtual void startTransitionOff(Action callback)
    {
        if (CurrentState != State.ON)
            return;
        this.transitionOffDoneCallback = callback;
        switch (transOut)
        {
            case TransitionType.INSTANT:
                CurrentState = State.TRANS_OFF;
                transitionOff(null);
                break;
            case TransitionType.FADE_IN:
                float delay = 0;
                foreach (KeyValuePair<FNode, Vector2> myObject in pageObjectPositions)
                {
                    Go.killAllTweensWithTarget(myObject.Key);
                    Tween tween = Go.to(myObject.Key, .5f, new TweenConfig().floatProp("alpha", 0.0f).setDelay(delay).setEaseType(EaseType.CircIn));
                    delay += .1f;
                    if (myObject.Key == pageObjectPositions.ElementAt(pageObjectPositions.Count - 1).Key)
                    {
                        tween.setOnCompleteHandler(transitionOff);
                    }
                }
                CurrentState = State.TRANS_OFF;
                break;
        }
    }

    internal virtual void startTransitionOn()
    {
        if (CurrentState != State.OFF)
            return;

        switch (transIn)
        {
            case TransitionType.INSTANT:
                Futile.stage.AddChild(this);
                CurrentState = State.TRANS_ON;
                foreach (KeyValuePair<FNode, Vector2> i in pageObjectPositions)
                    i.Key.SetPosition(i.Value);

                transitionOn(null);
                break;
            case TransitionType.FADE_IN:
                float delay = 0;
                foreach (KeyValuePair<FNode, Vector2> myObject in pageObjectPositions)
                {
                    myObject.Key.alpha = 0;
                    Tween tween = Go.to(myObject.Key, animInTime, new TweenConfig().floatProp("alpha", 1.0f).setDelay(delay).setEaseType(EaseType.CircInOut));
                    delay += .5f;
                    if (myObject.Key == pageObjectPositions.ElementAt(pageObjectPositions.Count - 1).Key)
                    {
                        tween.setOnCompleteHandler(transitionOn);
                    }
                }                
                CurrentState = State.TRANS_ON;
                Futile.stage.AddChild(this);
                break;
        }

    }

    protected virtual void transitionOn(AbstractTween tween)
    {
        this.SetPosition(Vector2.zero);
        this.CurrentState = State.ON;
    }

    protected virtual void transitionOff(AbstractTween tween)
    {

        this.RemoveFromContainer();
        this.CurrentState = State.OFF;
        transitionOffDoneCallback.Invoke();
    }

    public virtual void Update()
    {
        //Center page on stage
        this.x = -Futile.stage.x;
        this.y = -Futile.stage.y;
    }


}
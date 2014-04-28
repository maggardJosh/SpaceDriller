using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class C
{
    public const string whiteElement = "white";
    public const string NormalFont = "PressStart2P";
    public const string SmallFont = "PressStart2Psmall";
    public const float disableMovementTime = .5f;

    public const string startMap = "room1_1";
    public const string startDoor = "tutorialDoor";

    public static bool transitioning = false;
    public const float transitioningTime = .5f;

    public const float stunInvulnTime = 1.0f;           //Time after stun that you're invulnerable

    public const float PIOVER180 = Mathf.PI / 180.0f;
    public const float PIOVER180_INV = 180.0f / Mathf.PI;
    public static List<KeyValuePair<string, int>> doorsBroken = new List<KeyValuePair<string, int>>();
    //Map name and index of door that was broken

    public static Player.SaveState lastSave = new Player.SaveState();

    private static FCamObject camera;
    public static FCamObject getCameraInstance()
    {
        if (camera == null)
        {
            camera = new FCamObject();
            Futile.stage.AddChild(camera);
        }
        return camera;
    }
}

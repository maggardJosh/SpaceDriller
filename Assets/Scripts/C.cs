using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class C
{
    public const string whiteElement = "white";
    public const string NormalFont = "PressStart2P";
    public const float disableMovementTime = .5f;

    public static bool transitioning = false;
    public const float transitioningTime = .5f;

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

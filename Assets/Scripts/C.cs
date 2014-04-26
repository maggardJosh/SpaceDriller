using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class C
{
    public const string whiteElement = "white";
    public const string NormalFont = "PressStart2P";

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

using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
        FutileParams fparams = new FutileParams(true, false, false, false);
        fparams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");
        fparams.backgroundColor = Color.black;
        fparams.origin = new Vector2(.5f, .5f);
        Futile.instance.Init(fparams);

        Futile.atlasManager.LoadAtlas("Atlases/gameAtlas");

        Futile.stage.AddChild(new Player());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

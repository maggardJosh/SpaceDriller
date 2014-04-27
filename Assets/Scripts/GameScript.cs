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

        Futile.atlasManager.LoadFont(C.NormalFont, "pressStart2P_0", "Atlases/pressStart2P", 0,0);
        Futile.atlasManager.LoadFont(C.SmallFont, "pressStart2Psmall_0", "Atlases/pressStart2Psmall", 0, 0);

        FPageManager.getInstance();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

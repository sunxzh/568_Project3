using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour {
	public GUIStyle style;

	private GameObject boat1;
	private GameObject boat2;
	
	private Boat boatScript1;
	private Boat boatScript2;

	private Vector2 rankTextSize;
	
	// Use this for initialization
	void Start () {
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");
		
		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();
	}

	void OnGUI(){
		GUI.Label(new Rect(Screen.width/4-50.0f, Screen.height*0.01f, 320.0f, 25.0f),"Rank "+boatScript1.rank+"/2",style);
		GUI.Label(new Rect(Screen.width*3/4-50.0f, Screen.height*0.01f, 320.0f, 25.0f),"Rank "+boatScript2.rank+"/2",style);
	}

	// Update is called once per frame
	void Update () {
	
	}
}

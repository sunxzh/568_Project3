using UnityEngine;
using System.Collections;

public class EndSceneScript : MonoBehaviour {
	public GUIStyle RestartStyle;
	public GUIStyle MainStyle;
	public GUIStyle ExitStyle;
	
	private Rect RestartR;
	private Rect MainR;
	private Rect ExitR;
	
	private bool Show;

	public AudioClip EndPause;

	private GameObject Champ;
	// Use this for initialization
	void Start () {
		RestartR = new Rect(Screen.width*0.35f,Screen.height*0.15f,Screen.width*0.4f,Screen.height*0.1f);
		MainR = new Rect(Screen.width*0.35f,Screen.height*0.25f,Screen.width*0.4f,Screen.height*0.1f);
		ExitR = new Rect(Screen.width*0.35f,Screen.height*0.35f,Screen.width*0.4f,Screen.height*0.1f);
		Show = false;

		Champ = GameObject.Find("Champion");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			AudioSource.PlayClipAtPoint(EndPause,transform.position,0.5f);
			Show = !Show;
		}
		
		//pause the camera
		if(Show)
		{
			Champ.GetComponent<SplineInterpolator>().mState = "Stopped";
		}
		else
		{
			Champ.GetComponent<SplineInterpolator>().mState = "Loop";
		}
		
		if(Show)
		{
			Vector3  mousePosition =  Input.mousePosition;
			mousePosition.y = Screen.height - mousePosition.y;
			
			if(RestartR.Contains(mousePosition))
				RestartStyle.normal.textColor = Color.red;
			else
				RestartStyle.normal.textColor = Color.white;
			
			
			if(MainR.Contains(mousePosition))
				MainStyle.normal.textColor = Color.red;
			else
				MainStyle.normal.textColor = Color.white;
			
			
			if(ExitR.Contains(mousePosition))
				ExitStyle.normal.textColor = Color.red;
			else
				ExitStyle.normal.textColor = Color.white;
		}
	}

	void OnGUI(){
		if(Show)
		{
			if(GUI.Button(RestartR,"ReStart",RestartStyle))
				Application.LoadLevel("GameScene");
			
			if(GUI.Button(MainR,"Main Menu",MainStyle))
			    Application.LoadLevel("StartScene");
			
			if(GUI.Button(ExitR,"Exit",ExitStyle))
				Application.Quit();
		}
	}
}

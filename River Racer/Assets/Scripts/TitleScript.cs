using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	public GUIStyle StartStyle;
	public GUIStyle IntroStyle;
	public GUIStyle ExitStyle;

	private Rect StartR;
	private Rect IntroR;
	private Rect ExitR;

	private bool Show;

	public AudioClip TitlePause;
	// Use this for initialization
	void Start () {
		StartR = new Rect(Screen.width*0.35f,Screen.height*0.15f,Screen.width*0.4f,Screen.height*0.1f);
		IntroR = new Rect(Screen.width*0.35f,Screen.height*0.25f,Screen.width*0.4f,Screen.height*0.1f);
		ExitR = new Rect(Screen.width*0.35f,Screen.height*0.35f,Screen.width*0.4f,Screen.height*0.1f);
		Show = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			AudioSource.PlayClipAtPoint(TitlePause,transform.position,0.5f);
			Show = !Show;
		}

		//pause the camera
		if(Show)
		{
			Camera.main.GetComponent<SplineInterpolator>().mState = "Stopped";
		}
		else
		{
			Camera.main.GetComponent<SplineInterpolator>().mState = "Loop";
		}

		if(Show)
		{
			Vector3  mousePosition =  Input.mousePosition;
			mousePosition.y = Screen.height - mousePosition.y;
			
			if(StartR.Contains(mousePosition))
				StartStyle.normal.textColor = Color.red;
			else
				StartStyle.normal.textColor = Color.white;
			
			
			if(IntroR.Contains(mousePosition))
				IntroStyle.normal.textColor = Color.red;
			else
				IntroStyle.normal.textColor = Color.white;
			
			
			if(ExitR.Contains(mousePosition))
				ExitStyle.normal.textColor = Color.red;
			else
				ExitStyle.normal.textColor = Color.white;
		}
	}

	void OnGUI(){
		if(Show)
		{
			if(GUI.Button(StartR,"Start Game",StartStyle))
				Application.LoadLevel("GameScene");
			
			if(GUI.Button(IntroR,"Introduction",IntroStyle))
			    Application.LoadLevel("IntroScene");
			
			if(GUI.Button(ExitR,"Exit",ExitStyle))
				Application.Quit();
		}
	}
}

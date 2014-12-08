using UnityEngine;
using System.Collections;

public class IntroSceneScript : MonoBehaviour {
	public GUITexture guit;
	public Texture[] texs;

	public GUIStyle LeftTex;
	public GUIStyle HomeTex;
	public GUIStyle RightTex;

	private int pageindex = 0;
	private int cppageindex = 0;

	public AudioClip PageTurn;
	// Use this for initialization
	void Start () {
		guit = gameObject.GetComponent<GUITexture>();;
		guit.pixelInset = new Rect(Screen.width/2.0f,-Screen.height/2.0f,0.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(pageindex==4)
			pageindex = 0;

		if(pageindex==-1)
			pageindex = 3;

		if(cppageindex!=pageindex)
			AudioSource.PlayClipAtPoint(PageTurn,transform.position,1.0f);
		guit.texture = texs[pageindex];
		cppageindex = pageindex;
	}


	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width*(0.01f), Screen.height*(0.01f), Screen.width*0.1f, Screen.width*0.1f),"",HomeTex))	
			Application.LoadLevel("StartScene");

		if(GUI.Button(new Rect(Screen.width*(0.01f), Screen.height*(0.45f), Screen.width*0.1f, Screen.width*0.1f),"",LeftTex))	
			pageindex--;

		if(GUI.Button(new Rect(Screen.width*(0.90f), Screen.height*(0.45f), Screen.width*0.1f, Screen.width*0.1f),"",RightTex))
			pageindex++;
	
	}
}

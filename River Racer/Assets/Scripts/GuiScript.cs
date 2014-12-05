using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiScript : MonoBehaviour {
	public GUIStyle style;
	public GUIStyle countdownStyle;
	public GUIStyle resultStyle;

	public Texture powerupBgTex;
	public Texture oilBarrelTex;
	public Texture ufoTex;
	public Texture reverseTex;
	public Texture jetTex;
	public Texture shieldTex;
	public Texture watermineTex;
	public Texture frameTex;
	public Texture rankTimeBgTex;
	public Texture homeBtnTex;
	
	public float elapsedTime;

	public static List<string> finalRank;
	public static List<GameObject> boats;

	public static bool start;
	
	private GameObject boat1;
	private GameObject boat2;
	private GameObject CPU1;
	private GameObject CPU2;
	private GameObject CPU3;
	
	private BoatScript boatScript1;
	private BoatScript boatScript2;
	private Boat playerScript1;
	private Boat playerScript2;
	
	private Vector2 rankTextSize;
	
	private float countdownTime;
	private float resultTime1;
	private float resultTime2;
	
	private string countdownText;
	
	
	// Use this for initialization
	void Start () {
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");
		CPU1 = GameObject.Find ("CPU1");
		CPU2 = GameObject.Find ("CPU2");
		CPU3 = GameObject.Find ("CPU3");
		
		boatScript1=boat1.GetComponent<BoatScript>();
		boatScript2=boat2.GetComponent<BoatScript>();
		playerScript1=boat1.GetComponent<Boat>();
		playerScript2=boat2.GetComponent<Boat>();
		
		countdownTime=4.0f;
		resultTime1=0.0f;
		resultTime2=0.0f;
		elapsedTime=0.0f;

		style.fontSize=Screen.width/35;
		resultStyle.fontSize=Screen.width/4;
		countdownStyle.fontSize=Screen.width/4;

		finalRank=new List<string>();

		boats=new List<GameObject>();
		GameObject[] players=GameObject.FindGameObjectsWithTag("boat");
		GameObject[] cpus=GameObject.FindGameObjectsWithTag("CPUBoat");
		foreach(GameObject obj in players){
			boats.Add(obj);
		}
		foreach(GameObject obj in cpus){
			boats.Add(obj);
		}

		start=false;
	}

	void drawPowerups(List<int> powerups,bool isBoat1){
		for(int i=0;i<powerups.Count;i++){
			Texture powerupTex=jetTex;
			switch(powerups[i]){
			case 0:
				powerupTex=jetTex;
				break;
			case 1:
				powerupTex=reverseTex;
				break;
			case 2:
				powerupTex=ufoTex;
				break;
			case 3:
				powerupTex=shieldTex;
				break;
			case 4:
				powerupTex=watermineTex;
				break;
			case 5:
				powerupTex=oilBarrelTex;
				break;
			}
			
			if(isBoat1){
				GUI.Label(new Rect(Screen.width*(0.01f+0.08f*i), Screen.height*0.9f, Screen.width*0.08f, Screen.width*0.08f),powerupBgTex);
				GUI.Label(new Rect(Screen.width*(0.025f+0.08f*i), Screen.height*0.916f, Screen.width*0.05f, Screen.width*0.05f),powerupTex);
			}else{
				GUI.Label(new Rect(Screen.width*(0.51f+0.08f*i), Screen.height*0.9f, Screen.width*0.08f, Screen.width*0.08f),powerupBgTex);
				GUI.Label(new Rect(Screen.width*(0.525f+0.08f*i), Screen.height*0.916f, Screen.width*0.05f, Screen.width*0.05f),powerupTex);
			}
		}

		if(powerups.Count>0){
			if(isBoat1){
				GUI.Label(new Rect(Screen.width*(0.01f+0.08f*playerScript1.selectedIndex), Screen.height*0.9f, Screen.width*0.08f, Screen.width*0.08f),frameTex);
			}else{
				GUI.Label(new Rect(Screen.width*(0.51f+0.08f*playerScript2.selectedIndex), Screen.height*0.9f, Screen.width*0.08f, Screen.width*0.08f),frameTex);
			}
		}
	}

	void OnGUI(){
		//home button
		if(GUI.Button(new Rect(Screen.width*(0.001f), Screen.height*0.001f, Screen.width*0.08f, Screen.width*0.08f),homeBtnTex,GUIStyle.none)){

		}

		//countdown text
		GUI.Label(new Rect(Screen.width*0.5f-countdownStyle.fontSize*0.5f, Screen.height*0.5f-countdownStyle.fontSize*0.5f, countdownStyle.fontSize, countdownStyle.fontSize),countdownText,countdownStyle);

		//rank bg
		GUI.Label(new Rect(Screen.width*(0.25f-0.075f), 0.0f, Screen.width*0.15f, Screen.width*0.1f),rankTimeBgTex);
		GUI.Label(new Rect(Screen.width*(0.75f-0.075f), 0.0f, Screen.width*0.15f, Screen.width*0.1f),rankTimeBgTex);

		//rank text
		GUI.Label(new Rect(Screen.width*0.25f-style.fontSize*0.5f, Screen.height*0.02f, style.fontSize, style.fontSize),"Rank "+boatScript1.rank+"/"+boats.Count,style);
		GUI.Label(new Rect(Screen.width*0.75f-style.fontSize*0.5f, Screen.height*0.02f, style.fontSize, style.fontSize),"Rank "+boatScript2.rank+"/"+boats.Count,style);

		//timer bg
		GUI.Label(new Rect(Screen.width*(0.5f-0.075f), 0.0f, Screen.width*0.15f, Screen.width*0.1f),rankTimeBgTex);

		//timer text
		string minutes = Mathf.Floor(elapsedTime / 60).ToString("00");
		string seconds = Mathf.Floor(elapsedTime % 60).ToString("00");
		string mseconds = Mathf.Floor((elapsedTime *100)%100).ToString("00");
		GUI.Label(new Rect(Screen.width*0.5f-style.fontSize*0.5f, Screen.height*0.02f, style.fontSize, style.fontSize),minutes+":"+seconds+":"+mseconds,style);
		
		//result text
		if(boatScript1.end){
			if(resultTime1<3.0f){
				resultTime1+=Time.deltaTime;
				GUI.Label(new Rect(Screen.width*0.5f-resultStyle.fontSize*0.5f, Screen.height*0.5f-resultStyle.fontSize*0.5f, resultStyle.fontSize, resultStyle.fontSize),"NO."+boatScript1.rank,resultStyle);
			}
		}
		if(boatScript2.end){
			if(resultTime2<3.0f){
				resultTime2+=Time.deltaTime;
				GUI.Label(new Rect(Screen.width*0.75f-resultStyle.fontSize*0.5f, Screen.height*0.5f-resultStyle.fontSize*0.5f, resultStyle.fontSize, resultStyle.fontSize),"NO."+boatScript2.rank,resultStyle);
			}
		}

		//power up
		drawPowerups(playerScript1.itemnums,true);
		drawPowerups(playerScript2.itemnums,false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!start){
			if(countdownTime>0.0f){
				countdownStyle.fontSize+=1;
				countdownText=Mathf.Floor(countdownTime%4).ToString();
				if(countdownTime<1.0f){
					countdownText="Go!";
				}
				countdownTime-=Time.deltaTime;
			}else{
				countdownText="";
				start=true;
				CPU1.GetComponent<SplineController>().FollowSpline();
				CPU2.GetComponent<SplineController>().FollowSpline();
				CPU3.GetComponent<SplineController>().FollowSpline();
			}
		}else{
			elapsedTime +=Time.deltaTime;
		}
	}
}

using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour {
	public GUIStyle style;
	public GUIStyle countdownStyle;
	public GUIStyle resultStyle;

	public float elapsedTime;

	private GameObject boat1;
	private GameObject boat2;
	
	private Boat boatScript1;
	private Boat boatScript2;

	private Vector2 rankTextSize;

	private float countdownTime;
	private float resultTime1;
	private float resultTime2;

	private string countdownText;

	
	// Use this for initialization
	void Start () {
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");
		
		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();

		countdownTime=4.0f;
		resultTime1=0.0f;
		resultTime2=0.0f;
		elapsedTime=0.0f;
	}

	void OnGUI(){
		//countdown text
		GUI.Label(new Rect(Screen.width/2-150.0f, Screen.height/2-150.0f, 300.0f, 300.0f),countdownText,countdownStyle);

		//rank text
		GUI.Label(new Rect(Screen.width/4-160.0f, Screen.height*0.01f, 320.0f, 25.0f),"Rank "+boatScript1.rank+"/2",style);
		GUI.Label(new Rect(Screen.width*3/4-160.0f, Screen.height*0.01f, 320.0f, 25.0f),"Rank "+boatScript2.rank+"/2",style);

		//timer text
		string minutes = Mathf.Floor(elapsedTime / 60).ToString("00");
		string seconds = Mathf.Floor(elapsedTime % 60).ToString("00");
		GUI.Label(new Rect(Screen.width/2-160.0f, Screen.height*0.01f, 320.0f, 25.0f),"Time "+minutes+":"+seconds,style);

		//result text
		if(boatScript1.end){
			if(resultTime1<3.0f){
				resultTime1+=Time.deltaTime;
				GUI.Label(new Rect(Screen.width/4-100.0f, Screen.height/2-100.0f, 200.0f, 200.0f),"NO."+boatScript1.rank,resultStyle);
			}
		}
		if(boatScript2.end){
			if(resultTime2<3.0f){
				resultTime2+=Time.deltaTime;
				GUI.Label(new Rect(Screen.width*3/4-100.0f, Screen.height/2-100.0f, 200.0f, 200.0f),"NO."+boatScript2.rank,resultStyle);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(!boatScript1.start&&!boatScript2.start){
			if(countdownTime>0.0f){
				countdownStyle.fontSize+=1;
				countdownText=Mathf.Floor(countdownTime%4).ToString();
				if(countdownTime<1.0f){
					countdownText="Go!";
				}
				countdownTime-=Time.deltaTime;
			}else{
				countdownText="";
				boatScript1.start=true;
				boatScript2.start=true;
			}
		}else{
			elapsedTime +=Time.deltaTime;
		}
	}
}

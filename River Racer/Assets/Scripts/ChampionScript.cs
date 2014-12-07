using UnityEngine;
using System.Collections;

public class ChampionScript : MonoBehaviour {
	public Camera mainC;
	
	public GameObject[] boats;
	
	private float timeforrot;
	public float timeforstep;
	private Vector3 randomrot;
	private Vector3 currrandomrot;
	private Vector3 stepforrot = Vector3.zero;
	
	private Vector2 randoml;
	private Vector2 currrandoml;
	private Vector2 stepforl;
	
	// Use this for initialization
	void Start () {
		mainC = Camera.main;
		
		Vector3 campos = transform.rotation * (new Vector3(30.0f,0.0f,0.0f)) + transform.position;
		mainC.transform.position = campos;
		mainC.transform.rotation = transform.rotation;
		
		randomrot = new Vector3(30.0f,0.0f,0.0f);
		currrandomrot = randomrot;
		timeforrot = 0.0f;
		timeforstep = 10.0f;
		
		randoml = new Vector2(10.0f,-35.0f);
		currrandoml = randoml;
		
		boats[0].renderer.enabled = false;
		boats[1].renderer.enabled = false;
		boats[2].renderer.enabled = false;
		
		if(GlobalScript.firstname == "Boat1")
			boats[0].renderer.enabled = true;
		else if(GlobalScript.firstname == "Boat2")
			boats[1].renderer.enabled = true;
		else
			boats[2].renderer.enabled = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		mainC.transform.rotation = transform.rotation;
		
		if(timeforrot == 0.0f)
		{
			float rotx = Random.Range(10.0f,30.0f);
			float roty = Random.Range(10.0f,350.0f);
			
			
			randomrot.x = rotx;
			randomrot.y = roty;
			
			float rotl1 = Random.Range(10.0f,20.0f);
			float rotl2 = Random.Range(-35.0f,-45.0f);
			randoml = new Vector2(rotl1,rotl2);
			
			
			stepforl = (randoml - currrandoml)/timeforstep;
			stepforrot = (randomrot - currrandomrot)/timeforstep;
		}
		
		if(timeforrot < timeforstep)
		{
			timeforrot += Time.deltaTime;
			currrandomrot += stepforrot *Time.deltaTime;
			currrandoml += stepforl *Time.deltaTime;
			
			if( Mathf.Abs(currrandomrot.x - currrandomrot.y)- 90.0f < 1.0f)
				currrandomrot += stepforrot *Time.deltaTime;
			
			mainC.transform.Rotate(currrandomrot);
		}
		else
		{
			timeforrot = 0.0f;
		}
		
		Vector3 campos = mainC.transform.rotation * (new Vector3(0.0f,currrandoml.x,currrandoml.y)) + transform.position;
		mainC.transform.position = campos;
	}
}

using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour {
	public static bool joystick;

	public Water Riverscript;
	public GameObject Underwater;
	//store wave info
	public Vector4 wavecurrspeed;
	public float wavecurrscale;
	
	//time to change wave
	public float timeforwaves;
	public float maxwavespeed;
	public float timeforstep;
	
	
	//Underwater mat
	private Material undermat;
	private Vector4 randomspeed;
	private float randomscale;
	Vector4 stepforspeed = Vector4.zero;
	float stepforscale = 0.0f;
	
	// Use this for initialization
	void Start () {
		joystick = false;

		Riverscript = GameObject.FindGameObjectWithTag("river").GetComponent<Water>();
		timeforwaves = 0.0f;
		timeforstep = 20.0f;
		maxwavespeed = 3.0f;
		undermat = Underwater.GetComponent<MeshRenderer>().materials[0];
		
		//Init
		Riverscript.mat.SetVector("WaveSpeed",new Vector4(0.1f,0.1f,0.5f,0.5f));
		Riverscript.mat.SetFloat("_WaveScale",0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetJoystickNames().Length>0 && Input.GetJoystickNames()[0] == "Controller (Gamepad F310)")
			joystick = true;
		else
			joystick = false;

		wavecurrspeed = Riverscript.mat.GetVector("WaveSpeed");
		wavecurrscale = Riverscript.mat.GetFloat("_WaveScale");
		
		
		if(timeforwaves == 0.0f)
		{
			float vx = Random.Range(-maxwavespeed,maxwavespeed);
			float vy = Random.Range(-maxwavespeed,maxwavespeed);
			randomspeed = new Vector4(vx,vy,0.5f,0.5f);
			
			randomscale = Random.Range(0.05f,0.1f);
			stepforspeed = (randomspeed - wavecurrspeed)/timeforstep;
			stepforscale = (randomscale - wavecurrscale)/timeforstep;
		}
		
		if(timeforwaves < timeforstep)
		{
			timeforwaves += Time.deltaTime;
			wavecurrspeed += stepforspeed*Time.deltaTime;
			wavecurrscale += stepforscale*Time.deltaTime;
			
			Riverscript.mat.SetVector("WaveSpeed",wavecurrspeed);
			Riverscript.mat.SetFloat("_WaveScale",wavecurrscale);
			
			//undermat.color = (wavecurrspeed + stepforspeed*Time.deltaTime  
			// + new Vector4(3.0f,3.0f,3.0f,3.0f))/6.0f + new Vector4(0.2f,0.2f,0.2f,0.2f);
		}
		else
		{
			timeforwaves = 0.0f;
		}
	}
}

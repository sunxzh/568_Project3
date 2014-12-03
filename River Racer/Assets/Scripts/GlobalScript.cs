using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour {
	public Water Riverscript;
	public GameObject Underwater;
	//store wave info
	public Vector4 wavelastspeed;
	public float wavelastscale;
	public Vector4 wavecurrspeed;
	public float wavecurrscale;

	//time to change wave
	public float timeforwaves;
	public float timeforwave;
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
		Riverscript = GameObject.FindGameObjectWithTag("river").GetComponent<Water>();
		timeforwaves = 0.0f;
		timeforwave = 20.0f;
		timeforstep = 40.0f;
		maxwavespeed = 3.0f;
		undermat = Underwater.GetComponent<MeshRenderer>().materials[0];

		//Init
		Riverscript.mat.SetVector("WaveSpeed",new Vector4(1.1f,1.1f,1.1f,1.1f));
		Riverscript.mat.SetFloat("_WaveScale",0.1f);

		wavelastspeed = Riverscript.mat.GetVector("WaveSpeed");
		wavelastscale = Riverscript.mat.GetFloat("_WaveScale");
	}
	
	// Update is called once per frame
	void Update () {
		wavecurrspeed = Riverscript.mat.GetVector("WaveSpeed");
		wavecurrscale = Riverscript.mat.GetFloat("_WaveScale");


		if(timeforwaves == 0.0f)
		{
			randomspeed = new Vector4
				(Random.Range(-maxwavespeed,maxwavespeed),
				 Random.Range(-maxwavespeed,maxwavespeed),
				 Random.Range(-maxwavespeed,maxwavespeed),
				 Random.Range(-maxwavespeed,maxwavespeed));

			randomscale = Random.Range(0.05f,0.1f);
			stepforspeed = (randomspeed - wavelastspeed)/timeforstep;
			stepforscale = (randomscale - wavelastscale)/timeforstep;
		}

		if(timeforwaves<timeforwave)
		{
			timeforwaves += Time.deltaTime;
			wavelastspeed += stepforspeed*Time.deltaTime;
			wavelastscale += stepforscale*Time.deltaTime;

			Riverscript.mat.SetVector("WaveSpeed",wavelastspeed);
			Riverscript.mat.SetFloat("_WaveScale",wavelastscale);

			undermat.color = (wavelastspeed + stepforspeed*Time.deltaTime * 4.0f 
			                  + new Vector4(3.0f,3.0f,3.0f,3.0f))/6.0f + new Vector4(0.2f,0.2f,0.2f,0.2f);
		}
		else
		{
			timeforwaves = 0.0f;
		}
	}
}

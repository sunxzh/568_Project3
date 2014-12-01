using UnityEngine;
using System.Collections;

public class UFOScript : MonoBehaviour {
	public bool onoff;
	public GameObject UFO;
	public GameObject ps;

	public float animspeed;
	public float animlength; 

	private float animtime;
	// Use this for initialization
	void Start () {
		onoff = false;
		animspeed = 0.2f;
		animlength = UFO.GetComponent<Animation>()["UFOAnimation"].length/animspeed;
		animtime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!onoff)
		{
			UFO.GetComponent<Animation>().Stop();
			UFO.GetComponent<MeshRenderer>().enabled = false;		
			ps.GetComponent<ParticleSystem>().Stop();
		}
		else
		{
			animtime += Time.deltaTime;
			if(animtime>=animlength)
			{
				animtime = 0.0f;
				onoff = !onoff;
			}

			UFO.GetComponent<MeshRenderer>().enabled = true;
			UFO.GetComponent<Animation>()["UFOAnimation"].speed = animspeed;
			UFO.GetComponent<Animation>().Play("UFOAnimation",PlayMode.StopAll);
			ps.GetComponent<ParticleSystem>().Play();

		}
	}
}

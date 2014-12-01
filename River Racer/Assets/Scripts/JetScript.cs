using UnityEngine;
using System.Collections;

public class JetScript : MonoBehaviour {
	public bool onoff;
	public GameObject[] particles;
	//public ParticleRenderer[] prenders;
	// Use this for initialization
	void Start () {
		onoff = false;

	}
	
	// Update is called once per frame
	void Update () {
	    if(!onoff)
		{
			for(int i=0;i<particles.Length;i++)
			{
				particles[i].particleEmitter.emit = false;
				particles[i].GetComponent<ParticleRenderer>().enabled = false;
			}

		}
		else
		{
			for(int i=0;i<particles.Length;i++)
			{
				particles[i].GetComponent<ParticleRenderer>().enabled = true;
				particles[i].particleEmitter.emit = true;
			}
		}
	}
}

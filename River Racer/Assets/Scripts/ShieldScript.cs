using UnityEngine;
using System.Collections;

public class ShieldScript : MonoBehaviour {
	public bool onoff;
	// Use this for initialization
	void Start () {
		onoff = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!onoff)
		{
			gameObject.renderer.enabled = false;		
		}
		else
		{
			gameObject.renderer.enabled = true;		
		}
	}
}

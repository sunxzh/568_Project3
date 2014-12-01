using UnityEngine;
using System.Collections;

public class OilScript : MonoBehaviour {
	public bool onoff;
	public GameObject Oilemit;
	// Use this for initialization
	void Start () {
		onoff = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(onoff)
		{
			Destroy(gameObject);
			GameObject oilemit = (GameObject)Instantiate(Oilemit,transform.position,Oilemit.transform.rotation);
			Destroy(oilemit,1.0f);
			onoff = false;
		}
	}
}

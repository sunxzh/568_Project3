using UnityEngine;
using System.Collections;

public class StarScript : MonoBehaviour {
	public bool onoff;
	public GameObject[] stars;
	public GameObject boat;

	private float timer;
	private float shakePeriod;
	private int num;

	// Use this for initialization
	void Start () {
		onoff = false;

		num = 2;
		timer = 0; 
		shakePeriod = 1.0f;	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime; 
		if( timer > shakePeriod ) 
		{
			timer = 0;
			num ++; 
			if(num==4)
				num = 2;
		}
		float ym = Mathf.Pow(-1,num);
		gameObject.transform.position = 
			new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+ym*0.05f,gameObject.transform.position.z);
		gameObject.transform.Rotate(new Vector3(0.0f,4.0f,0.0f));

		if(!onoff)
		{
			for(int i=0;i<stars.Length;i++)
				stars[i].GetComponent<MeshRenderer>().enabled = false;		
		}
		else
		{
			for(int i=0;i<stars.Length;i++)
				stars[i].GetComponent<MeshRenderer>().enabled = true;
		}
	}
}

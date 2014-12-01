using UnityEngine;
using System.Collections;

public class ItemBoxAnimScript : MonoBehaviour {
	private float timer;
	private float shakePeriod;
	private int num;
	// Use this for initialization
	void Start () {
		num = 2;
		timer = 0; 
		shakePeriod = 2.0f;	
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
			new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+ym*0.02f,gameObject.transform.position.z);
		gameObject.transform.Rotate(new Vector3(0.0f,4.0f,0.0f));
	}
}

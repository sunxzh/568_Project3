using UnityEngine;
using System.Collections;

public class MineScript : MonoBehaviour {
	private float timer;
	private float shakePeriod;
	private int num;

	public bool onoff;
	public GameObject Mineemit;
	// Use this for initialization
	void Start () {
		num = 2;
		timer = 0; 
		shakePeriod = 5.0f;	
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
			new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+ym*0.002f,gameObject.transform.position.z);
		gameObject.transform.Rotate(new Vector3(0.0f,ym * 0.1f,0.0f));



		if(onoff)
		{
			Destroy(gameObject);
			GameObject mineemit = (GameObject)Instantiate(Mineemit,transform.position,Mineemit.transform.rotation);
			Destroy(mineemit,2.0f);
			onoff = false;
		}
	}
}

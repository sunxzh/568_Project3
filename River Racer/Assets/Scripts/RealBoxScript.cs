using UnityEngine;
using System.Collections;

public class RealBoxScript : MonoBehaviour {
	public GameObject explode;

	private Boat playerScript;

	private GameObject boat;

	// Use this for initialization
	void Start () {		

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter( Collision collision ) 
	{ 
		Collider collider = collision.collider; 

		if( collider.CompareTag("boat")) 
		{
			boat=GameObject.Find(collider.name);
			playerScript=boat.GetComponent<Boat>();
			playerScript.RandomItem();
			GameObject temp = (GameObject)Instantiate(explode,gameObject.transform.position,explode.transform.rotation);
			Destroy(temp,1.0f);
			Destroy(gameObject);
		}
		else if( collider.CompareTag("CPUBoat")) 
		{

		}
		else 
		{
			if(collider.tag == "Borders")
				Destroy(gameObject);
			else
			    Destroy(collider.gameObject);
		}
	}
}

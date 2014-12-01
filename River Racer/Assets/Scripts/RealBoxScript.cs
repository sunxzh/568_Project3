using UnityEngine;
using System.Collections;

public class RealBoxScript : MonoBehaviour {
	public GameObject explode;

	private Boat boatScript1;
	private Boat boatScript2;

	private GameObject boat1;
	private GameObject boat2;
	// Use this for initialization
	void Start () {
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");
		
		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter( Collision collision ) 
	{ 
		Collider collider = collision.collider; 

		if( collider.name == "Boat1") 
		{
			boatScript1.RandomItem();
			GameObject temp = (GameObject)Instantiate(explode,gameObject.transform.position,explode.transform.rotation);
			Destroy(temp,1.0f);
			Destroy(gameObject);
		}
		else if( collider.name == "Boat2") 
		{
			boatScript2.RandomItem();
			GameObject temp = (GameObject)Instantiate(explode,gameObject.transform.position,explode.transform.rotation);
			Destroy(temp,1.0f);
			Destroy(gameObject);
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

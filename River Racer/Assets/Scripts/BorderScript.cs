using UnityEngine;
using System.Collections;

public class BorderScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionStay(Collision collision){
		Collider collider = collision.collider; 
		if(collider.gameObject.tag == "Trees"||collider.gameObject.tag == "Rocks"){
			Destroy (collider.gameObject);
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.tag == "Trees"||collider.gameObject.tag == "Rocks"){
			Destroy (collider.gameObject);
		}
	}
}

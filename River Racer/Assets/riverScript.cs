using UnityEngine;
using System.Collections;

public class riverScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.name != "Boat"&&collider.gameObject.name != "TrickyPlane"){
			Destroy (collider.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class ObstaclesScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionStay(Collision collision){
		Collider collider = collision.collider; 
		if(collider.CompareTag("Borders"))
			Destroy(gameObject);
	}

	void OnTriggerStay(Collider collider){
		if(collider.CompareTag("Rocks")||
		   collider.CompareTag("Logs")||collider.CompareTag("StartField"))
		{
			Destroy(gameObject);
		}
	}
}

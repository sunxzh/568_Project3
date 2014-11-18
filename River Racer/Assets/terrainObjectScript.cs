using UnityEngine;
using System.Collections;

public class terrainObjectScript : MonoBehaviour {

	private float time = 0;

	// Use this for initialization
	void Start () {
	
	}

	void OnCollisionStay(Collision collision){
		//Debug.LogError (collision.gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
	}
}

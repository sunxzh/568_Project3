﻿using UnityEngine;
using System.Collections;

public class VortexScript : MonoBehaviour {
	public float speed;

	public GameObject[] boats;


	// Use this for initialization
	void Start () {
		boats[0] =GameObject.Find("Boat1");
		boats[1] =GameObject.Find("Boat2");
		boats[2] = GameObject.Find ("CPU1");
		boats[3] = GameObject.Find ("CPU2");
		boats[4] = GameObject.Find ("CPU3");
		speed = Random.Range (-2.0f,2.0f);
		if(speed>=0.0f)
			speed = Mathf.Clamp(speed,0.5f,2.0f);
		else
			speed = Mathf.Clamp(speed,-0.5f,-2.0f);

		animation.Play("VortexAnimation");
		animation["VortexAnimation"].speed = speed;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i=0;i<boats.Length;i++)
		{
			Vector3 dist = gameObject.transform.position - boats[i].transform.position;
			if(dist.magnitude < 15.0f)
			{
				float draw = 0.5f * Mathf.Abs(speed);
				boats[i].transform.position += 	
					new Vector3(draw * dist.x/dist.magnitude,0.0f,draw * dist.z/dist.magnitude);

				//delete vortex after a while:avoid too long stuck
				Destroy(gameObject,5.0f);
			}
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.CompareTag("Rocks")||collider.CompareTag("Logs")||collider.CompareTag("Vortex")){
			Destroy(collider.gameObject);
		}

		if(collider.CompareTag("Borders")||collider.CompareTag("StartField")){
			Destroy (gameObject);
		}
	}
}
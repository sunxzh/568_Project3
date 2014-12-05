using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class waypointScript : MonoBehaviour {
	private GameObject boat;
	private BoatScript boatScript;
	private int index;
	private List<int> waypointList;

	// Use this for initialization
	void Start () {
		index=int.Parse(gameObject.name);

		gameObject.renderer.enabled=false;
	}

	void OnTriggerEnter(Collider collider){
		if(collider.CompareTag("boat")){
			boat=GameObject.Find(collider.name);
			boatScript=boat.GetComponent<BoatScript>();
			waypointList=boatScript.passedWaypoints;

			if(!waypointList.Contains(index)){
				boatScript.passedWaypoints.Add(index);
			}else{
				waypointList.RemoveAt(waypointList.Count-1);
			}

			/*if(waypointList.Count>0){
				Debug.Log(waypointList[waypointList.Count-1]+" "+collider.name);
			}else{
				Debug.Log("empty"+" "+collider.name);
			}*/
		}

		if(collider.CompareTag("CPUBoat")){
			boat=GameObject.Find(collider.name);
			boatScript=boat.GetComponent<BoatScript>();
			waypointList=boatScript.passedWaypoints;
			
			if(!waypointList.Contains(index)){
				boatScript.passedWaypoints.Add(index);
			}else{
				waypointList.RemoveAt(waypointList.Count-1);
			}

			/*if(waypointList.Count>0){
				Debug.Log(waypointList[waypointList.Count-1]+" "+collider.name);
			}else{
				Debug.Log("empty"+" "+collider.name);
			}*/
		}

	}

	// Update is called once per frame
	void Update () {
	
	}
}

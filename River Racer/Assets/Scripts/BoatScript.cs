using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoatScript : MonoBehaviour {
	public List<int> passedWaypoints;

	public int rank;

	public float usedTime;
	public float distToNextWaypoint;

	public bool end;

	// Use this for initialization
	void Start () {
		passedWaypoints=new List<int>();

		rank=0;

		usedTime=0.0f;
		distToNextWaypoint=0.0f;

		end=false;
	}


	// Update is called once per frame
	void Update () {
		if(!end&&GuiScript.start){
			Vector2 boatPos=new Vector2(gameObject.transform.position.x,gameObject.transform.position.z);
			GameObject nextWaypoint=GameObject.Find((passedWaypoints.Count+1).ToString());
			Vector2 waypointPos=new Vector2(nextWaypoint.transform.position.x,nextWaypoint.transform.position.z);
			distToNextWaypoint=(boatPos-waypointPos).magnitude;
		}
	}
}

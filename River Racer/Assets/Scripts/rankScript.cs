using UnityEngine;
using System.Collections;

public class rankScript : MonoBehaviour {
	private GameObject boat1;
	private GameObject boat2;
	private GameObject nextWaypoint;

	private Boat boatScript1;
	private Boat boatScript2;

	// Use this for initialization
	void Start () {
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");

		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();
	}
	
	// Update is called once per frame
	void Update () {
		if(boatScript1.start&&boatScript2.start){
			if(boatScript1.passedWaypoints.Count<boatScript2.passedWaypoints.Count){
				boatScript1.rank=2;
				boatScript2.rank=1;
			}else if(boatScript1.passedWaypoints.Count>boatScript2.passedWaypoints.Count){
				boatScript1.rank=1;
				boatScript2.rank=2;
			}else{
				int nextWaypointIndex=boatScript1.passedWaypoints.Count+1;
				nextWaypoint=GameObject.Find(nextWaypointIndex.ToString());

				Vector2 boatPos1=new Vector2(boat1.transform.position.x,boat1.transform.position.z);
				Vector2 boatPos2=new Vector2(boat2.transform.position.x,boat2.transform.position.z);
				Vector2 nextWayPointPos=new Vector2(nextWaypoint.transform.position.x,nextWaypoint.transform.position.z);
				Vector2 boat1Waypoint=nextWayPointPos-boatPos1;
				Vector2 boatVec12=boatPos2-boatPos1;
				Vector2 boat1Forward=new Vector2(boat1.transform.forward.x,boat1.transform.forward.z);

				if(Vector2.Dot(boat1Waypoint,boat1Forward)<0){
					boatVec12*=-1;
				}

				if(Vector2.Dot(boat1Forward,boatVec12)>0){
					boatScript1.rank=2;
					boatScript2.rank=1;
				}else{
					boatScript1.rank=1;
					boatScript2.rank=2;
				}
			}
		}
	}
}

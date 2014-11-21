using UnityEngine;
using System.Collections;

public class rankScript : MonoBehaviour {
	private GameObject boat1;
	private GameObject boat2;

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
		if(boatScript1.passedWaypoints.Count<boatScript2.passedWaypoints.Count){
			boatScript1.rank=2;
			boatScript2.rank=1;
		}else if(boatScript1.passedWaypoints.Count>boatScript2.passedWaypoints.Count){
			boatScript1.rank=1;
			boatScript2.rank=2;
		}else{
			Vector2 boatPos1=new Vector2(boat1.transform.position.x,boat1.transform.position.z);
			Vector2 boatPos2=new Vector2(boat2.transform.position.x,boat2.transform.position.z);
			Vector2 boatVec12=boatPos2-boatPos1;
			Vector2 boat1Forward=new Vector2(boat1.transform.forward.x,boat1.transform.forward.z);

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

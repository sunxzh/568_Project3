using UnityEngine;
using System.Collections;

public class FinishLineScript : MonoBehaviour {
	private GameObject boat;
	private GameObject global;
	
	private BoatScript boatScript;
	private GuiScript guiScript;

	// Use this for initialization
	void Start () {
		global=GameObject.Find("Global");

		guiScript=global.GetComponent<GuiScript>();
	}

	void OnTriggerEnter(Collider collider){
		if(collider.CompareTag("boat")&&!GuiScript.finalRank.Contains(collider.name)){
			boat=GameObject.Find(collider.name);

			boatScript=boat.GetComponent<BoatScript>();
			boatScript.end=true;
			boatScript.usedTime=guiScript.elapsedTime;

			GuiScript.finalRank.Add(collider.name);
		}

		if(collider.CompareTag("CPUBoat")&&!GuiScript.finalRank.Contains(collider.name)){
			boat=GameObject.Find(collider.name);

			boatScript=boat.GetComponent<BoatScript>();
			boatScript.end=true;
			boatScript.usedTime=guiScript.elapsedTime;

			GuiScript.finalRank.Add(collider.name);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}

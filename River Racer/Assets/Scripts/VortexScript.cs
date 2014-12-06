using UnityEngine;
using System.Collections;

public class VortexScript : MonoBehaviour {
	public float speed;

	public GameObject[] boats;

	// Use this for initialization
	void Start () {
		int index = 0;
		if(GameObject.Find("Boat1"))
			boats[index++] = GameObject.Find("Boat1");

		if(GameObject.Find("Boat2"))
			boats[index++] = GameObject.Find("Boat2");

		if(GameObject.Find("CPU1"))
			boats[index++] = GameObject.Find("CPU1");

		
		if(GameObject.Find("CPU2"))
			boats[index++] = GameObject.Find("CPU2");

		if(GameObject.Find("CPU3"))
			boats[index++] = GameObject.Find("CPU3");


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
			Vector2 dist2D = new Vector2(dist.x,dist.z);
			if(dist2D.magnitude < 15.0f)
			{
				float draw = 0.5f * Mathf.Abs(speed);
				boats[i].transform.position += 	
					new Vector3(draw * dist.x/dist2D.magnitude,0.0f,draw * dist.z/dist2D.magnitude);

				if(boats[i].GetComponent<Boat>())
				{
					if(boats[i].GetComponent<Boat>().onborder)
						Destroy(gameObject);
				}
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

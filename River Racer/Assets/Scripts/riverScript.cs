using UnityEngine;
using System.Collections;

public class riverScript : MonoBehaviour {

	public GameObject[] obstacles;
	public int numObstacles;
	public GameObject Vortex;
	public int numVortex;
	// Use this for initialization
	void Start () {
		Mesh riverMesh = this.gameObject.GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = riverMesh.vertices;

		for (int i = 0; i < numObstacles/2; i++){
			int rndIndex = Random.Range (0,vertices.Length-1);
			Vector3 pos1 = transform.TransformPoint(vertices[rndIndex]);
			Vector3 pos2 = transform.TransformPoint(vertices[rndIndex+1]);
			float rndInterp = Random.Range (0.2f,0.8f);
			Vector3 obstaclePos = (pos2-pos1)*rndInterp + pos1;
			int rndObstacleIdx = 0;

			obstaclePos.y = 0.1f;
			float randomangle = Random.Range (0.0f,360.0f);
			GameObject obs = (GameObject) Instantiate (obstacles[rndObstacleIdx], obstaclePos,obstacles[rndObstacleIdx].transform.rotation* Quaternion.Euler(new Vector3(0.0f,randomangle, 0.0f)));
			float rndScale = Random.Range (1.5f,2.0f);
			obs.transform.localScale = rndScale * obstacles[rndObstacleIdx].transform.localScale;
		}

		for (int i = 0; i < numObstacles/3; i++){
			int rndIndex = Random.Range (0,vertices.Length-1);
			Vector3 pos1 = transform.TransformPoint(vertices[rndIndex]);
			Vector3 pos2 = transform.TransformPoint(vertices[rndIndex+1]);
			float rndInterp = Random.Range (0.2f,0.8f);
			Vector3 obstaclePos = (pos2-pos1)*rndInterp + pos1;
			int rndObstacleIdx = 1;
			
			obstaclePos.y = 0.1f;
			float randomangle = Random.Range (0.0f,360.0f);
			GameObject obs = (GameObject) Instantiate (obstacles[rndObstacleIdx], obstaclePos, obstacles[rndObstacleIdx].transform.rotation* Quaternion.Euler(new Vector3(0.0f,randomangle, 0.0f)));
			float rndScale = Random.Range (1.2f,1.5f);
			obs.transform.localScale = rndScale * obstacles[rndObstacleIdx].transform.localScale;
		}


		for (int i = 0; i < numVortex; i++){
			int rndIndex = Random.Range (0,vertices.Length-1);
			Vector3 pos1 = transform.TransformPoint(vertices[rndIndex]);
			Vector3 pos2 = transform.TransformPoint(vertices[rndIndex+1]);
			float rndInterp = Random.Range (0.2f,0.8f);
			Vector3 obstaclePos = (pos2-pos1)*rndInterp + pos1;
			int rndObstacleIdx = Random.Range (0,obstacles.Length);
			obstaclePos.y = 0.1f;
			GameObject obs = (GameObject) Instantiate (Vortex, obstaclePos, Vortex.transform.rotation);
			float rndScale = Random.Range (0.5f,0.8f);
			obs.transform.localScale = rndScale * Vortex.transform.localScale;
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.tag == "Trees"){
			Destroy (collider.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}

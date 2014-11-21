using UnityEngine;
using System.Collections;

public class riverScript : MonoBehaviour {

	public GameObject[] obstacles;
	public int numObstacles;

	// Use this for initialization
	void Start () {
		Mesh riverMesh = this.gameObject.GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = riverMesh.vertices;

		for (int i = 0; i < numObstacles; i++){
			int rndIndex = Random.Range (0,vertices.Length-1);
			Vector3 pos1 = transform.TransformPoint(vertices[rndIndex]);
			Vector3 pos2 = transform.TransformPoint(vertices[rndIndex+1]);
			float rndInterp = Random.Range (0f,1f);
			Vector3 obstaclePos = (pos2-pos1)*rndInterp + pos1;
			int rndObstacleIdx = Random.Range (0,obstacles.Length);
			GameObject obs = (GameObject) Instantiate (obstacles[rndObstacleIdx], obstaclePos, Quaternion.identity);
			float rndScale = Random.Range (4f,7f);
			obs.transform.localScale = new Vector3(rndScale, rndScale, rndScale);
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.tag != "boat" &&collider.gameObject.name != "TrickyPlane"){
			Destroy (collider.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

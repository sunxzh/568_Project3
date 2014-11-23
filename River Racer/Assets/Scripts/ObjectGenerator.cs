using UnityEngine;
using System.Collections;

public class ObjectGenerator : MonoBehaviour {

	public GameObject[] objects;
	public Transform[] constraints;
	public float sizex = 2;
	public float sizez = 2.2f;
	public GameObject river;
	
	// Use this for initialization
	void Start () {
		river = GameObject.FindGameObjectWithTag ("river");

		// Compute size of grid cells
		Vector3[] objectSizes = new Vector3[objects.Length];
		float stepx = 0;
		float stepz = 0;
		computeStepAndSize(objects, objectSizes, ref stepx, ref stepz);
		
		stepx *= sizex;
		stepz *= sizez;
		
		float minx = constraints[0].position.x;
		float maxx = constraints[1].position.x;
		float minz = constraints[0].position.z;
		float maxz = constraints[1].position.z;
		if (minx > maxx) {
			float temp = minx;
			minx = maxx;
			maxx = temp;
		}
		if (minz > maxz) {
			float temp = minz;
			minz = maxz;
			maxz = temp;
		}
		int xcount = (int)(Mathf.Abs(maxx - minx) / stepx);
		int zcount = (int)(Mathf.Abs(maxz - minz) / stepz);
		
//		for (int i = 0; i < objectSizes.Length; i++) {
//			Debug.Log(objectSizes[i].x);
//			Debug.Log(objectSizes[i].z);
//		}
//		Debug.Log(xcount);
//		Debug.Log(zcount);
		
		// Generate objects
		for (int i = 0; i < xcount; i++) {
			for (int j = 0; j < zcount; j++) {
				int idx = Random.Range(0, objects.Length);
				Vector3 size = objectSizes[idx];
				
				float localminx = minx + stepx * i;
				float localmaxx = minx + stepx * (i + 1);
				float localminz = minz + stepz * j;
				float localmaxz = minz + stepz * (j + 1);
				
				float x = Random.Range(localminx + 0.6f * size.x, localmaxx - 0.6f * size.x);
				float z = Random.Range(localminz + 0.7f * size.z, localmaxz - 0.7f * size.z);
				Instantiate (objects[idx], new Vector3(x, objects[idx].transform.position.y, z), 
				             objects[idx].transform.rotation);
			}
		}
	}
	
	void computeStepAndSize(GameObject[] objects, Vector3[] sizes, ref float stepx, ref float stepz) {
		for (int i = 0; i < objects.Length; i++) {
			GameObject obj = Instantiate(objects[i], new Vector3(0, -10, 0), new Quaternion()) as GameObject;
			Collider collider = obj.collider;
			Vector3 size = 1.5f * collider.bounds.size;
			sizes[i] = size;
			if (stepx < size.x) {
				stepx = size.x;
			}
			if (stepz < size.z) {
				stepz = size.z;
			}
			Destroy(obj);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}

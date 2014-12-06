using UnityEngine;
using System.Collections;

public class SplineOrientation : MonoBehaviour {

	private Transform[] transforms;
	private float radiusOfVariation = 20f;

	// Use this for initialization
	void Start () {
		Debug.Log (gameObject.name);

		transforms = this.gameObject.GetComponentsInChildren<Transform> ();

		for (int i = 1; i < transforms.Length-1; i++){
			float randomAngle = Random.Range (0f, 2*Mathf.PI);
			float x_variance = radiusOfVariation*Mathf.Cos (randomAngle);
			float z_variance = radiusOfVariation*Mathf.Sin (randomAngle);

			if(i != 1)
				transforms[i].position = transforms[i].position + new Vector3(x_variance, 0, z_variance);

			transforms[i].LookAt (transforms[i+1]);
		}
		transforms [transforms.Length - 1].rotation = transforms [transforms.Length - 2].rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

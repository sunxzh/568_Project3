using UnityEngine;
using System.Collections;

public class boatScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (WaitAndCollider (3.0f));
	}

	IEnumerator WaitAndCollider(float time){
		yield return new WaitForSeconds(time);
		CapsuleCollider c = this.gameObject.AddComponent<CapsuleCollider> ();
		c.radius = 0.3f;
		c.height = 8f;
		c.direction = 2;
	}

	// Update is called once per frame
	void Update () {
	
	}
}

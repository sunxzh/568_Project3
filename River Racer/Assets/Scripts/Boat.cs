using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour {
	//Rewrite most parts
	public bool canControl = true;
	public bool autoAcc = true;
	public AudioClip engineSound;
	public Transform engineSpume;
	public float mass = 100.0f;
	public int rudderSensivity = 45;
	public float maxVel = 30.0f;
	public Camera mainC;
	public float CurrVel = 0.0f;
	public float Acc = 1.5f;

	public bool drift = false;

	private float rpmPitch = 0.0f;

	void Start () {
		//Setup rigidbody
		Physics.gravity = new Vector3(0.0f,0.0f,0.0f);
		rigidbody.mass = mass;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.maxAngularVelocity = 5.0f;

		//start engine noise
		if(!audio){
			gameObject.AddComponent<AudioSource>();
		}
		audio.clip = engineSound;
		audio.loop = true;
		audio.Play();

		//Added
		mainC = Camera.main;
		Vector3 campos = transform.rotation * (new Vector3(0.0f, 10.0f, -25.0f)) + transform.position;
		mainC.transform.position = campos;
		mainC.transform.rotation = transform.rotation;

		rudderSensivity = 45;
	}
	
	// Update is called once per frame
	void  FixedUpdate (){
		Vector3 campos = transform.rotation * (new Vector3(0.0f,10.0f, -25.0f)) + transform.position;
		mainC.transform.position = campos;
		mainC.transform.rotation = transform.rotation;

		float motor = 0.0f;
		float steer = 0.0f;
		
		if(canControl){
			if(Input.GetKey("w")||autoAcc)
				motor = 1.0f;
			if(Input.GetKey("s"))
				motor = -1.0f;

			if(Input.GetKey("d"))
				steer = 1.0f;

			if(Input.GetKey("a"))
				steer = -1.0f;

			if( Mathf.Abs(steer)>0.0f && Input.GetKey(KeyCode.LeftShift))
				drift = true;
			else
				drift = false;

				
		}

		if(motor>0.0)
			CurrVel += Acc;
		else
			CurrVel -= 2.0f * Acc;

		CurrVel = Mathf.Clamp(CurrVel,0.0f,maxVel);
		rigidbody.velocity = transform.forward * CurrVel;

		//create particles for propeller
		if(engineSpume!=null)
		{
			engineSpume.particleEmitter.minEmission = Mathf.Abs(0.8f * CurrVel);
			engineSpume.particleEmitter.maxEmission = Mathf.Abs(1.2f * CurrVel);
			engineSpume.particleEmitter.Emit();				
		}


		rigidbody.AddTorque(transform.up*rudderSensivity*steer * 5.0f * CurrVel/maxVel);	

		if(drift&&steer>0.1f)
		{
			transform.Rotate(transform.up* 1.0f);
			CurrVel = maxVel/6.0f;
		}
		else if(drift&&steer<-0.1f)
		{
			transform.Rotate(-transform.up* 1.0f);
			CurrVel = maxVel/6.0f;
		}

		audio.volume = 0.3f + 0.7f * CurrVel/maxVel;
		audio.volume = Mathf.Min(audio.volume,1.0f);

		rpmPitch=Mathf.Lerp(rpmPitch,Mathf.Abs(CurrVel/maxVel),Time.deltaTime*0.4f);
		audio.pitch = 0.3f + 0.7f * rpmPitch;
		audio.pitch = Mathf.Clamp(audio.pitch,0.0f,1.0f);
	}
}

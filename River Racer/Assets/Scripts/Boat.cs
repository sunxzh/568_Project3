using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boat : MonoBehaviour {
	//player1 or player2
	public bool isPlayer1;
	
	//Rewrite most parts
	public bool canControl = false;
	public bool autoAcc = true;
	public AudioClip engineSound;
	public Transform engineSpume;
	public float mass = 100.0f;
	public int rudderSensivity = 45;
	public float maxVel = 30.0f;
	public Camera mainC;
	public float CurrVel = 0.0f;
	public float Acc = 0.1f;
	
	public bool drift = false;
	public GameObject BoatBody;
	private bool blink;
	private float blinks;
	private float blinkp;
	private double INVtimer = 0.0;
	
	private float rpmPitch = 0.0f;
	private List<Vector3> agopos = new List<Vector3>();
	private List<Quaternion> agorot= new List<Quaternion>();
	private float time = 0.0f;

	public List<int> passedWaypoints;
	public int rank;

	private bool OnRiver;

	//Init State
	private Vector3 InitPos;

	public bool start;
	public bool end;

	public float usedTime;

	private GameObject global;
	private GuiScript guiScript;

	//Generate Random Item
	public void RandomItem()
	{

	}

	//boat blink
	void Blink()
	{
		if (Time.time > INVtimer)
		{			
			INVtimer = Time.time + 0.2;
			bool onoff = BoatBody.renderer.enabled;
			BoatBody.renderer.enabled = !onoff;			
		}		
	} 
	
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
		if(isPlayer1)
			mainC = GameObject.Find ("Main Camera1").camera;
		else
			mainC = GameObject.Find ("Main Camera2").camera;
		Vector3 campos = transform.rotation * (new Vector3(0.0f, 15.0f, 35.0f)) + transform.position;
		mainC.transform.position = campos;
		mainC.transform.rotation = transform.rotation;
		
		maxVel = 60.0f;
		Acc = 0.3f;
		
		rudderSensivity = 45;
		blink = false;
		blinks = 0.0f;
		blinkp = 1.0f;
		
		//Init pos and rot
		agopos.Add(transform.position);
		agorot.Add(transform.rotation);

		rank=0;
		OnRiver = true;

		InitPos = transform.position;

		start=false;
		end=false;

		usedTime=0.0f;

		global=GameObject.Find("Global");
		guiScript=global.GetComponent<GuiScript>();
	}

	void Update(){
		canControl=start;
	}

	// Update is called once per frame
	void FixedUpdate (){
		time += Time.deltaTime;
		if(time>1.0f)
		{
			time = 0.0f;
			agopos.Add(transform.position);
			agorot.Add(transform.rotation);
			if(agopos.Count > 5)
				agopos.RemoveAt(0);
			if(agorot.Count > 5)
				agorot.RemoveAt(0);
		}
		
		//if push back blink 
		if(blink)
		{
			blinks += Time.deltaTime;
			if(blinks<blinkp)
				Blink();
			else
			{
				blink = false;
				BoatBody.renderer.enabled = true;		
			}
		}
		
		mainC.transform.rotation = transform.rotation;
		mainC.transform.Rotate(new Vector3(30.0f,180.0f,0.0f));

		Vector3 campos = mainC.transform.rotation * (new Vector3(0.0f,10.0f, -35.0f)) + transform.position;
		mainC.transform.position = campos;


		float motor = 0.0f;
		float steer = 0.0f;
		
		if(canControl){
			//player1
			if(isPlayer1 && Input.GetKey("w")||autoAcc)
				motor = 1.0f;
			if(isPlayer1 && Input.GetKey("s"))
				motor = -1.0f;

			if(isPlayer1 && Input.GetKey("d"))
				steer = 1.0f;
			
			if(isPlayer1 && Input.GetKey("a"))
				steer = -1.0f;

			//player2
			if(!isPlayer1 && Input.GetKey("up")||autoAcc)
				motor = 1.0f;
			if(!isPlayer1 && Input.GetKey("down"))
				motor = -1.0f;
	
			if(!isPlayer1 && Input.GetKey("right"))
				steer = 1.0f;
			
			if(!isPlayer1 && Input.GetKey("left"))
				steer = -1.0f;
			
			if( Mathf.Abs(steer)>0.0f && Input.GetKey(KeyCode.LeftShift))
				drift = true;
			else
				drift = false;
					
		}

		if(canControl && isPlayer1 && Input.GetKey("s"))
			motor = -1.0f;

		if(motor>0.0)
			CurrVel += Acc;
		else
			CurrVel -= 2.0f * Acc;
		
		CurrVel = Mathf.Clamp(CurrVel,0.0f,maxVel);


		//if(OnRiver)
		//{
		    rigidbody.velocity = -transform.forward * CurrVel;
		//}
//	    else 
//		{
//			if(OnBorderR)
//			   rigidbody.velocity = (-transform.forward + 2.0f * transform.right)/3.0f * CurrVel;
//			else if(OnBorderL)
//				rigidbody.velocity = (-transform.forward - 2.0f * transform.right)/3.0f * CurrVel;
//		}


		//Added to avoid weird rotating
		if(rigidbody.angularVelocity.magnitude < 0.5f)
			rigidbody.angularVelocity = new Vector3(0.0f,0.0f,0.0f);
		else
			rigidbody.angularVelocity /= 1.5f;
		
		//create particles for propeller
		if(engineSpume!=null)
		{
			engineSpume.particleEmitter.minEmission = Mathf.Abs(0.8f * CurrVel);
			engineSpume.particleEmitter.maxEmission = Mathf.Abs(1.2f * CurrVel);
			engineSpume.particleEmitter.Emit();				
		}
		

		transform.Rotate(transform.up* steer * 0.3f);
		
		
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
	
	void OnTriggerExit(Collider collider){
		if(collider.CompareTag("river")){
			engineSpume.particleEmitter.emit = false;
			OnRiver = false;
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.CompareTag("river")){
			OnRiver = true;
		}
	}

	void OnTriggerEnter(Collider collider){
		if(collider.CompareTag("FinishLine")){
			end=true;
			start=false;
			usedTime=guiScript.elapsedTime;
			Debug.Log(usedTime);
		}
	}

}




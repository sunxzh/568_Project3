using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour {
	
	public bool canControl = true;
	//Engine sound
	public AudioClip engineSound;
	//Particle system used for foam from the boat's propeller
	public Transform engineSpume;
	//Boat Mass
	public float mass = 3000.0f;
	//Boat motor force
	public float motorForce = 10000.0f;
	//Rudder sensivity
	public int rudderSensivity = 45;
	//Angular drag coefficient
	public float angularDrag = 0.8f;
	//Center of mass offset
	public float cogY = -0.5f;
	//Volume of boat in liters (the higher the volume, the higher the boat will floar)
	public int volume = 9000;
	//Max width, height and length of the boat (used for water dynamics)
	public Vector3 size = new Vector3(3,3,6);

	//Drag coefficients along x,y and z directions
	private Vector3 drag = new Vector3(6.0f,4.0f,0.2f);
	private float rpmPitch = 0.0f;
	private WaterSurface waterSurface;

	// Use this for initialization
	void Start () {
		//Setup rigidbody
		if(!rigidbody){
			gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.mass = mass;
		rigidbody.drag = 0.1f;
		rigidbody.angularDrag = angularDrag;
		rigidbody.centerOfMass = new Vector3(0, cogY, 0);
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		
		//start engine noise
		if(!audio){
			gameObject.AddComponent<AudioSource>();
		}
		audio.clip = engineSound;
		audio.loop = true;
		audio.Play();
	}
	
	// Update is called once per frame
	void  FixedUpdate (){
		//If there is no water surface we are colliding with, no boat physics	
		if(waterSurface==null)
			return;
	
		float motor = 0.0f;
		float steer = 0.0f;
		
		if(canControl){
			motor = Input.GetAxis("Vertical");
			steer = Input.GetAxis("Horizontal");
		}
	
		//Get water level and percent under water
		float waterLevel = waterSurface.collider.bounds.max.y;
		float distanceFromWaterLevel = transform.position.y-waterLevel;
		float percentUnderWater = Mathf.Clamp01((-distanceFromWaterLevel + 0.5f*size.y)/size.y);
	
	
		//BUOYANCY (the force which keeps the boat floating above water)
		//_______________________________________________________________________________________________________
		
		//the point the buoyancy force is applied onto is calculated based 
		//on the boat's picth and roll, so it will always tilt upwards:
		Vector3 buoyancyPos = new Vector3();
		buoyancyPos = transform.TransformPoint(-new Vector3(transform.right.y*size.x*0.5f,0,transform.forward.y*size.z*0.5f));
		
		//then it is shifted arcording to the current waves
		buoyancyPos.x += waterSurface.waveXMotion1 * Mathf.Sin(waterSurface.waveFreq1 * Time.time)
					+waterSurface.waveXMotion2 * Mathf.Sin(waterSurface.waveFreq2 * Time.time)
					+waterSurface.waveXMotion3 * Mathf.Sin(waterSurface.waveFreq3 * Time.time);
		buoyancyPos.z += waterSurface.waveYMotion1 * Mathf.Sin(waterSurface.waveFreq1 * Time.time)
					+waterSurface.waveYMotion2 * Mathf.Sin(waterSurface.waveFreq2 * Time.time)
					+waterSurface.waveYMotion3 * Mathf.Sin(waterSurface.waveFreq3 * Time.time);
		
		//apply the force
		rigidbody.AddForceAtPosition(- volume * percentUnderWater * Physics.gravity, buoyancyPos);
		
		//ENGINE
		//_______________________________________________________________________________________________________
		
		//calculate propeller position
		Vector3 propellerPos = new Vector3(0,-size.y*0.5f,-size.z*0.5f);
		Vector3 propellerPosGlobal = transform.TransformPoint(propellerPos);
		
		//apply force only if propeller is under water
		if(propellerPosGlobal.y<waterLevel)
		{
			//direction propeller force is pointing to.
			//mostly forward, rotated a bit according to steering angle
			float steeringAngle = steer * rudderSensivity/100 * Mathf.Deg2Rad;
			Vector3 propellerDir = transform.forward*Mathf.Cos(steeringAngle) - transform.right*Mathf.Sin(steeringAngle);
			
			//apply propeller force
			rigidbody.AddForceAtPosition(propellerDir * motorForce * motor , propellerPosGlobal);
			
			//create particles for propeller
			if(engineSpume!=null)
			{
				//engineSpume.position = propellerPosGlobal;
				//engineSpume.position.y = waterLevel-0.5f;
				//engineSpume.particleEmitter.worldVelocity = rigidbody.velocity*0.5f-propellerDir*10*motor+Vector3.up*3*Mathf.Clamp01(motor);
				engineSpume.particleEmitter.minEmission = Mathf.Abs(motor);
				engineSpume.particleEmitter.maxEmission = Mathf.Abs(motor);
				engineSpume.particleEmitter.Emit();				
			}
		}
		
		//DRAG
		//_______________________________________________________________________________________________________
		
		//calculate drag force
		Vector3 dragDirection = transform.InverseTransformDirection(rigidbody.velocity);
		Vector3 dragForces = - Vector3.Scale(dragDirection,drag);
		
		//depth of the boat under water (used to find attack point for drag force)
		float depth = Mathf.Abs(transform.forward.y)*size.z*0.5f+Mathf.Abs(transform.up.y)*size.y*0.5f;
		
		//apply force
		Vector3 dragAttackPosition = new Vector3(transform.position.x,waterLevel-depth,transform.position.z);
		rigidbody.AddForceAtPosition(transform.TransformDirection(dragForces)*rigidbody.velocity.magnitude*(1+percentUnderWater*(waterSurface.waterDragFactor-1)),dragAttackPosition);
		
		//linear drag (linear to velocity, for low speed movement)
		rigidbody.AddForce(transform.TransformDirection(dragForces)*500);
		
		//rudder torque for steering (square to velocity)
		float forwardVelo = Vector3.Dot(rigidbody.velocity,transform.forward);
		rigidbody.AddTorque(transform.up*forwardVelo*forwardVelo*rudderSensivity*steer);	
		
		//SOUND
		//_______________________________________________________________________________________________________
		
		audio.volume = 0.3f + Mathf.Abs(motor);
	
		//slowly adjust pitch to power input
		rpmPitch=Mathf.Lerp(rpmPitch,Mathf.Abs(motor),Time.deltaTime*0.4f);
		audio.pitch = 0.3f + 0.7f * rpmPitch;
	
		//reset water surface, so we have to stay in contact for boat physics.
		waterSurface = null; 
	}
	
	//Check if we inside water area
	void OnTriggerStay(Collider col){
		if(col.GetComponent<WaterSurface>()!=null)
			waterSurface=col.GetComponent<WaterSurface>();
	}

}

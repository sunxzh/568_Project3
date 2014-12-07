using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boat : MonoBehaviour {
	//Add arrow 
	public GameObject Arrow;
	private Vector3 ArrowtoBoat;
	
	//player1 or player2
	public bool isPlayer1;
	
	//Rewrite most parts
	public bool stop = false;
	public bool canControl = true;
	public bool autoAcc = true;
	public AudioClip engineSound;
	public Transform engineSpume;

	public float mass = 100.0f;
	public int rudderSensivity = 45;
	public float maxVel = 40.0f;
	public float maxVel_copy = 30.0f;
	public float minVel = -15.0f;

	public Camera mainC;
	public float CurrVel = 0.0f;
	public float Acc = 0.1f;
	public float Acc_copy = 0.1f;

	float motor = 0.0f;
	float steer = 0.0f;
	float reverse = 1.0f;
	float slippery = 1.0f;

	public bool drift = false;
	public GameObject SphereBody;
	private bool blink;
	private float blinks;
	private float blinkp;
	private double INVtimer = 0.0;
	
	private float rpmPitch = 0.0f;
	private List<Vector3> agopos = new List<Vector3>();
	private List<Quaternion> agorot= new List<Quaternion>();
	private float time = 0.0f;

	//Init State
	private Vector3 InitPos;
	
	private GameObject global;
	//Get info from global
	GlobalScript globalscript;


	//effect on each other
	private Boat boatScript1;
	private Boat boatScript2;
	private BoatCPU CPUBoatScript1;
	private BoatCPU CPUBoatScript2;
	private BoatCPU CPUBoatScript3;

	private GameObject boat1;
	private GameObject boat2;
	private GameObject CPUboat1;
	private GameObject CPUboat2;
	private GameObject CPUboat3;

	//List to store items
	public GameObject[] Items;
	public List<int> itemnums; //item players collect,3 at most
	public List<float> itemstarttimes; //items start time
	public List<float> itemtimes; //items last time
	private bool shield;

	//sounds for items
	public AudioClip[] itemSounds;

	//if collide with border, stop wavespeed
	public bool onborder = false;
	private float onborderlag = 0.0f;

	public int selectedIndex;

	//Get Random Item
	public void RandomItem()
	{
		int itemid=Random.Range(0,1000)%6;
		itemnums.Add(itemid);
		if(itemnums.Count > 3)
			itemnums.RemoveAt(0);
	}

	//Item effect
	public void ItemEffect(int itemid)
	{
		//0 boost;
		if(itemid == 0 || Items[0].GetComponent<JetScript>().onoff)
		{
			int tempid = 0;
			Items[tempid].GetComponent<JetScript>().onoff = true;

			//Add acc and max vel
			Acc = 1.5f * Acc_copy;
			maxVel = 1.5f * maxVel_copy;

			if(itemstarttimes[tempid]<0.01f)
			{
				AudioSource.PlayClipAtPoint(itemSounds[tempid], transform.position ,1.0f); 
			}
			
			itemstarttimes[tempid] += Time.deltaTime;

			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
				Acc = Acc_copy;
				maxVel = maxVel_copy;
				itemstarttimes[tempid] = 0.0f;
				Items[tempid].GetComponent<JetScript>().onoff = false;
			}
		}

		//1 reverse control
		if(itemid == 1 || Items[1].GetComponent<StarScript>().onoff)
		{
			int tempid = 1;
			Items[tempid].GetComponent<StarScript>().onoff = true;

			if(itemstarttimes[tempid]<0.01f)
			{
				AudioSource.PlayClipAtPoint(itemSounds[tempid], transform.position ,0.3f); 
			}
			
			//Reverse control
			reverse = -1.0f;
			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid]+ 1.0f;
				shield = false;
			}
			
			itemstarttimes[tempid] += Time.deltaTime;
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
				reverse = 1.0f;
				itemstarttimes[tempid] = 0.0f;
				Items[tempid].GetComponent<StarScript>().onoff = false;
			}
		}

		//2 UFO
		if(itemid == 2 || Items[2].GetComponent<UFOScript>().onoff)
		{
			int tempid = 2;
			Items[tempid].GetComponent<UFOScript>().onoff = true;
			
			//UFO make u slower
			Acc = 0.5f * Acc_copy;
			maxVel = 0.5f * maxVel_copy;

			if(itemstarttimes[tempid]<0.01f)
			{
				AudioSource.PlayClipAtPoint(itemSounds[tempid], transform.position ,0.5f); 
			}
			
			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid] + 1.0f;
				shield = false;
			}
			
			itemstarttimes[tempid] += Time.deltaTime;

			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
				Acc = Acc_copy;
				maxVel = maxVel_copy;
				itemstarttimes[tempid] = 0.0f;
				Items[tempid].GetComponent<UFOScript>().onoff = false;
			}
		}


		//3 Shield
		if(itemid == 3 || Items[3].GetComponent<ShieldScript>().onoff)
		{
			int tempid = 3;
			Items[tempid].GetComponent<ShieldScript>().onoff = true;
			
			//shield works one time
			if(itemstarttimes[tempid]<0.01f)
			{
				AudioSource.PlayClipAtPoint(itemSounds[tempid], transform.position ,1.0f); 
				shield = true;
			}

			itemstarttimes[tempid] += Time.deltaTime;
			if(itemstarttimes[tempid]>itemtimes[tempid]||shield == false)
			{
				itemstarttimes[tempid] = 0.0f;
				Items[tempid].GetComponent<ShieldScript>().onoff = false;
			}
		}

		//4 WaterMine generate a water mine after the boat
		if(itemid == 4)
		{
			AudioSource.PlayClipAtPoint(itemSounds[4], transform.position ,0.5f); 
			Vector3 gpos = transform.position + 8.0f * transform.forward;
			gpos.y += 2.0f;
			GameObject temp = (GameObject)Instantiate(Items[itemid],gpos,transform.rotation);
			temp.rigidbody.velocity = 0.8f * rigidbody.velocity  + transform.right * Random.Range(-25.0f,25.0f);
		}

		//If hit mine, trace back to the pos 5 seconds ago
		if(itemid == 6)
		{
			AudioSource.PlayClipAtPoint(itemSounds[5], transform.position ,1.0f);
			if(shield)
				shield = false;
			else
			{
				transform.position = agopos[0];
				transform.rotation = agorot[0];
				engineSpume.particleEmitter.emit = false;
				CurrVel = 0.0f;
				blink = true;
				blinks = 0.0f;
			}
		}

		//5 Oilbarrel generate a Oilbarrel after the boat
		if(itemid == 5)
		{
			AudioSource.PlayClipAtPoint(itemSounds[4], transform.position ,0.5f); 
			Vector3 gpos = transform.position + 8.0f * transform.forward;
			gpos.y += 2.0f;
			GameObject temp = (GameObject)Instantiate(Items[itemid],gpos,transform.rotation* Quaternion.Euler(new Vector3(90.0f,0.0f, 0.0f)));
			temp.rigidbody.velocity = 0.8f * rigidbody.velocity + transform.right * Random.Range(-25.0f, 25.0f);
		}

		//If hit Oilbarrel, become slippery
		if(itemid == 7 || itemstarttimes[5]!=0.0f)
		{
			int tempid = 5;
		    //mine makes u slippery
			slippery = 20.0f;

			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid] + 1.0f;
				shield = false;
			}

			itemstarttimes[tempid] += Time.deltaTime;
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
				slippery = 1.0f;
				itemstarttimes[tempid] = 0.0f;
			}
		}
	}

	//boat blink
	void Blink()
	{
		if (Time.time > INVtimer)
		{			
			INVtimer = Time.time + 0.2;
			bool onoff = SphereBody.renderer.enabled;
			SphereBody.renderer.enabled = !onoff;			
		}		
	} 
	
	void Start () {
 
		boat1=GameObject.Find("Boat1");
		boat2=GameObject.Find("Boat2");
		CPUboat1 = GameObject.Find ("CPU1");
		CPUboat2 = GameObject.Find ("CPU2");
		CPUboat3 = GameObject.Find ("CPU3");
		

		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();
		CPUBoatScript1 = CPUboat1.GetComponent<BoatCPU> ();
		CPUBoatScript2 = CPUboat2.GetComponent<BoatCPU> ();
		CPUBoatScript3 = CPUboat3.GetComponent<BoatCPU> ();
		

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
		
		maxVel = 70.0f;
		maxVel_copy = maxVel;
		minVel = -maxVel/2.0f;
		Acc = 0.3f;
		Acc_copy = Acc;

		rudderSensivity = 45;
		blink = false;
		blinks = 0.0f;
		blinkp = 4.0f;
		
		//Init pos and rot
		agopos.Add(transform.position);
		agorot.Add(transform.rotation);
		
		InitPos = transform.position;
		
		for(int i=0;i<6;i++)
			itemstarttimes.Add(0.0f);

		float timescale = 1.5f;
		itemtimes.Add(6.0f*timescale);  //jet time 6s
		itemtimes.Add(5.0f*timescale);  //dizzy time 5s
		itemtimes.Add(Items[2].GetComponent<UFOScript>().animlength);  //UFO time
		itemtimes.Add(5.0f*timescale);  //shield time 5s
		itemtimes.Add(0.0f*timescale);  //mine time 0s
		itemtimes.Add(5.0f*timescale);  //oil time 5s
				
		global=GameObject.Find("Global");
		globalscript = global.GetComponent<GlobalScript>();

		selectedIndex=0;

		//Init speed
		rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
		
		//arrow to boat
		InitPos = transform.position;

//		Arrow.transform.position = new Vector3(InitPos.x,InitPos.y+5.0f,InitPos.z);
//		Arrow.transform.rotation = transform.rotation;

		ArrowtoBoat = Arrow.transform.position - transform.position;
		Arrow.renderer.enabled = false;

		stop = false;
	}

	// Update is called once per frame
	void  Update (){
		//Add joystick control
        bool joysticks= GlobalScript.joystick;

		canControl=GuiScript.start;
		ItemEffect(-1);

		//update arrow pos
		Arrow.transform.position = transform.position + ArrowtoBoat;
		
		Vector4 wavespeed = globalscript.wavecurrspeed;
		float wavescale = globalscript.wavecurrscale;
		Vector2 riverspeed = 5.0f/3.0f * new Vector2(wavespeed.x,wavespeed.y);

		//Avoid huge wavespeed
		float angle = Mathf.Atan2(riverspeed.y,riverspeed.x);
		Arrow.transform.rotation = Quaternion.Euler(new Vector3(0.0f,Mathf.Rad2Deg * angle,0.0f));


		//lag 2s to guarantee boat leave the border
		if(onborder)
		{
			onborderlag+= 0.01f;
			riverspeed = new Vector2(0.0f,0.0f);
		}

		if(onborderlag>0.0f&&onborderlag<1.0f)
		{
			riverspeed = new Vector2(0.0f,0.0f);
			onborderlag+= Time.deltaTime;
		}
		else
		{
			onborder = false;
			onborderlag = 0.0f;
		}



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
				blinks = 0.0f;
				blink = false;
				SphereBody.renderer.enabled = false;		
			}
		}
		
		mainC.transform.rotation = transform.rotation;
		mainC.transform.Rotate(new Vector3(30.0f,180.0f,0.0f));

		Vector3 campos = mainC.transform.rotation * (new Vector3(0.0f,10.0f, -35.0f)) + transform.position;
		mainC.transform.position = campos;


		motor = 0.0f;
		steer = 0.0f;
		rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
		//player1
		if(canControl&&!stop)
		{
			//add wavespeed
			rigidbody.velocity = - Arrow.transform.forward * riverspeed.magnitude;
			if(isPlayer1)
			{
				if(joysticks)
				{
					if(Input.GetAxis("Horizontal")==1.0f)
						steer = 1.0f;
					else if(Input.GetAxis("Horizontal")==-1.0f)
						steer = -1.0f;
					
					if(Input.GetAxis("ZAxis")<0.0f)
						drift = true;
					else
						drift = false;
					
					
					//separate turn and acc
					if(Mathf.Abs(steer)<0.1f) 
					{
						if(autoAcc||Input.GetAxis("Vertical")==1.0f)
							motor = 1.0f;
						else if(autoAcc||Input.GetAxis("Vertical")== -1.0f)
							motor = -1.0f;
					}
					
					if(Input.GetButtonDown("Fire2"))
						Arrow.renderer.enabled = !Arrow.renderer.enabled;
		

					if(itemnums.Count>0)
					{
						//select powerup
						if(Input.GetButtonDown("Fire3")){
							selectedIndex=(selectedIndex+1)%itemnums.Count;
						}
						
						//use powerup
						if(Input.GetButtonDown("Fire1")){
							int tempnum = itemnums[selectedIndex];
							if(tempnum == 1 || tempnum == 2){
								Debug.Log (gameObject.name);
								boatScript2.ItemEffect(tempnum);
								CPUBoatScript1.ItemEffect (tempnum);
								CPUBoatScript2.ItemEffect (tempnum);
								CPUBoatScript3.ItemEffect (tempnum);
							}
							else 
								ItemEffect(tempnum);
							
							itemnums.RemoveAt(selectedIndex);
							
							if(selectedIndex>=itemnums.Count){
								selectedIndex=Mathf.Max(0,itemnums.Count-1);
							}
						}
					}
				}
				else
				{
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
					
					if(Input.GetKeyDown(KeyCode.Z))
						Arrow.renderer.enabled = !Arrow.renderer.enabled;
					
					if(itemnums.Count>0)
					{
						//select powerup
						if(Input.GetKeyDown(KeyCode.LeftAlt)){
							selectedIndex=(selectedIndex+1)%itemnums.Count;
						}
						
						//use powerup
						if(Input.GetKeyDown(KeyCode.LeftControl)){
							int tempnum = itemnums[selectedIndex];
							if(tempnum == 1 || tempnum == 2){
								boatScript2.ItemEffect(tempnum);
								CPUBoatScript1.ItemEffect (tempnum);
								CPUBoatScript2.ItemEffect (tempnum);
								CPUBoatScript3.ItemEffect (tempnum);
							}
							else 
								ItemEffect(tempnum);
							
							itemnums.RemoveAt(selectedIndex);
							
							if(selectedIndex>=itemnums.Count){
								selectedIndex=Mathf.Max(0,itemnums.Count-1);
							}
						}
					}
				}
			}
			else
			{
				if(joysticks)
				{
					if(Input.GetKey("up")||autoAcc)
						motor = 1.0f;
					if(Input.GetKey("down"))
						motor = -1.0f;
					
					if(Input.GetKey("right"))
						steer = 1.0f;
					
					if(Input.GetKey("left"))
						steer = -1.0f;
					
					if( Mathf.Abs(steer)>0.0f && (Input.GetKey(KeyCode.RightShift)||Input.GetKey(KeyCode.LeftShift)))
						drift = true;
					else
						drift = false;	
					
					
					if(Input.GetKeyDown(KeyCode.Slash))
						Arrow.renderer.enabled = !Arrow.renderer.enabled;
					
					if(itemnums.Count>0)
					{
						//select powerup
						if(Input.GetKeyDown(KeyCode.RightAlt)||Input.GetKeyDown(KeyCode.LeftAlt)){
							selectedIndex=(selectedIndex+1)%itemnums.Count;
						}
						
						//use powerup
						if(Input.GetKeyDown(KeyCode.RightControl)||Input.GetKeyDown(KeyCode.LeftControl)){
							int tempnum = itemnums[selectedIndex];
							if(tempnum == 1 || tempnum == 2){
								boatScript1.ItemEffect(tempnum);
								CPUBoatScript1.ItemEffect (tempnum);
								CPUBoatScript2.ItemEffect (tempnum);
								CPUBoatScript3.ItemEffect (tempnum);
							}else 
								ItemEffect(tempnum);
							
							itemnums.RemoveAt(selectedIndex);
							
							if(selectedIndex>=itemnums.Count){
								selectedIndex=Mathf.Max(0,itemnums.Count-1);
							}
						}
					}
				}
				else
				{
					if(Input.GetKey("up")||autoAcc)
						motor = 1.0f;
					if(Input.GetKey("down"))
						motor = -1.0f;
					
					if(Input.GetKey("right"))
						steer = 1.0f;
					
					if(Input.GetKey("left"))
						steer = -1.0f;
					
					if( Mathf.Abs(steer)>0.0f && Input.GetKey(KeyCode.RightShift))
						drift = true;
					else
						drift = false;	
					
					if(Input.GetKeyDown(KeyCode.Slash))
						Arrow.renderer.enabled = !Arrow.renderer.enabled;
					
					if(itemnums.Count>0)
					{
						//select powerup
						if(Input.GetKeyDown(KeyCode.RightAlt)){
							selectedIndex=(selectedIndex+1)%itemnums.Count;
						}
						
						//use powerup
						if(Input.GetKeyDown(KeyCode.RightControl)){
							int tempnum = itemnums[selectedIndex];
							if(tempnum == 1 || tempnum == 2){
								boatScript1.ItemEffect(tempnum);
								CPUBoatScript1.ItemEffect (tempnum);
								CPUBoatScript2.ItemEffect (tempnum);
								CPUBoatScript3.ItemEffect (tempnum);
							}else 
								ItemEffect(tempnum);
							
							itemnums.RemoveAt(selectedIndex);
							
							if(selectedIndex>=itemnums.Count){
								selectedIndex=Mathf.Max(0,itemnums.Count-1);
							}
						}
					}
				}
			}
		}

		//Add for reserve
		motor *= reverse;
		steer *= reverse;
		steer *= slippery;
		
		if(motor>0.0f)
			CurrVel += Acc;
		else if(motor<0.0f)
			CurrVel -= 1.5f * Acc;
		
		CurrVel = Mathf.Clamp(CurrVel,minVel,maxVel);
		
		
		rigidbody.velocity += -transform.forward * CurrVel;
		
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
		
		
		transform.Rotate(transform.up* steer * 0.8f);
		
		
		if(drift&&steer>0.1f)
		{
			transform.Rotate(transform.up* 2.0f);
			CurrVel = CurrVel * 0.98f;
			CurrVel = Mathf.Max(CurrVel,0.1f*maxVel);
		}
		else if(drift&&steer<-0.1f)
		{
			transform.Rotate(-transform.up* 3.0f);
			CurrVel = CurrVel * 0.98f;
			CurrVel = Mathf.Max(CurrVel,0.1f*maxVel);
		}
		
		audio.volume = 0.3f + 0.7f * CurrVel/maxVel;
		audio.volume = Mathf.Min(audio.volume,1.0f)/2.0f;
		
		rpmPitch=Mathf.Lerp(rpmPitch,Mathf.Abs(CurrVel/maxVel),Time.deltaTime*0.4f);
		audio.pitch = 0.3f + 0.7f * rpmPitch;
		audio.pitch = Mathf.Clamp(audio.pitch,0.0f,1.0f);
	}

	void OnCollisionExit(Collision collision)
	{
		if(collider.CompareTag("Borders")){
			onborder = false;
		}
	}

	void OnCollisionStay(Collision collision){
		Collider collider = collision.collider; 
		if(collider.CompareTag("Borders")){
			onborder = true;
			CurrVel *= 0.98f; //slow down if collide with borders
			CurrVel = Mathf.Min(0.7f* maxVel,CurrVel); //if collide with borders, maxvel is lower
		}
	}

	void OnCollisionEnter(Collision collision){
		Collider collider = collision.collider; 
		if(collider.CompareTag("Rocks")){
			if(!shield)
				CurrVel *= 1f; //0.3f;
			else
				shield = false; 
		}

		if(collider.CompareTag("Logs")){
				CurrVel *= 0.8f;
		}

		if(collider.CompareTag("Borders")){
			onborder = true;
			CurrVel *= 0.9f;
		}

		if(collider.CompareTag("OilBarrels")){
			ItemEffect(7);
			collider.GetComponent<OilScript>().onoff = true;
		}

		if(collider.CompareTag("Mines")){
			ItemEffect(6);
			collider.GetComponent<MineScript>().onoff = true;
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.CompareTag("FinishLine")){		
			//Stop
			rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
			canControl = false;
			stop = true;
			if(CurrVel>=0.0f)
			    CurrVel -= 2.0f;
			else
				CurrVel = 0.0f;
		}
	}

}




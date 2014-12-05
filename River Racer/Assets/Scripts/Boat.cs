using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boat : MonoBehaviour {

	//Add joystick control
	public bool joysticks;
	//player1 or player2
	public bool isPlayer1;
	
	//Rewrite most parts
	public bool canControl = true;
	public bool autoAcc = true;
	public AudioClip engineSound;
	public Transform engineSpume;

	public float mass = 100.0f;
	public int rudderSensivity = 45;
	public float maxVel = 40.0f;
	public float maxVel_copy = 30.0f;
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
	private GameObject boat1;
	private GameObject boat2;

	//List to store items
	public GameObject[] Items;
	public List<int> itemnums; //item players collect,3 at most
	public List<float> itemstarttimes; //items start time
	public List<float> itemtimes; //items last time
	private bool shield;

	//if collide with border, stop wavespeed
	private bool onborder = false;
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
				shield = true;
			
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

			GameObject temp = (GameObject)Instantiate(Items[itemid],transform.position + 8.0f * transform.forward,transform.rotation);
			temp.rigidbody.velocity = 0.3f * rigidbody.velocity  + transform.right * Random.Range(-15, 15);
		}

		//If hit mine, trace back to the pos 5 seconds ago
		if(itemid == 6)
		{
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
			GameObject temp = (GameObject)Instantiate(Items[itemid],transform.position + 8.0f * transform.forward,transform.rotation* Quaternion.Euler(new Vector3(0.0f,90.0f, 0.0f)));
			temp.rigidbody.velocity = 0.4f * rigidbody.velocity + transform.right * Random.Range(-15, 15);
		}

		//If hit Oilbarrel, become slippery
		if(itemid == 7 || itemstarttimes[5]!=0.0f)
		{
			int tempid = 5;
		    //mine makes u slippery
			slippery = 50.0f;

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
		
		boatScript1=boat1.GetComponent<Boat>();
		boatScript2=boat2.GetComponent<Boat>();

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
		audio.volume = 0.5f;
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
		maxVel_copy = maxVel;
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
	}

	// Update is called once per frame
	void  Update (){
		canControl=GuiScript.start;
		ItemEffect(-1);

		Vector4 wavespeed = globalscript.wavecurrspeed;
		float wavescale = globalscript.wavecurrscale;
		Vector2 riverspeed =  wavescale/0.05f*(new Vector2(wavespeed.x,wavespeed.y)+new Vector2(wavespeed.z ,wavespeed.w));

		//Avoid huge wavespeed
		riverspeed.x = Mathf.Clamp(riverspeed.x,-5.0f,5.0f);
		riverspeed.y = Mathf.Clamp(riverspeed.y,-5.0f,5.0f);

		//Debug.Log(riverspeed);

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
		if(canControl && isPlayer1)
		{
			//add wavespeed
			rigidbody.velocity = new Vector3(riverspeed.x,0.0f,riverspeed.y);

			if(joysticks)
			{
				if(autoAcc||Input.GetAxis("Vertical")>0.0f)
					motor = 1.0f;
				else
					motor = -1.0f;
				
				steer = Input.GetAxis("Horizontal");
				if(Input.GetAxis("ZAxis")<0.0f)
					drift = true;
				else
					drift = false;
				
				
				if(itemnums.Count>0 &&Input.GetButtonDown("Fire1"))
				{
					int tempnum = itemnums[selectedIndex];
					if(tempnum == 1 || tempnum == 2)
						boatScript2.ItemEffect(tempnum);
					else 
						ItemEffect(tempnum);
					
					itemnums.RemoveAt(selectedIndex);
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
				
				if(itemnums.Count>0)
				{
					//select powerup
					if(Input.GetKeyDown(KeyCode.LeftAlt)){
						selectedIndex=(selectedIndex+1)%itemnums.Count;
					}

					//use powerup
					if(Input.GetKeyDown(KeyCode.LeftControl)){
						int tempnum = itemnums[selectedIndex];
						if(tempnum == 1 || tempnum == 2)
							boatScript2.ItemEffect(tempnum);
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


		//player2
		if(canControl && !isPlayer1){
			//add wavespeed
			rigidbody.velocity = - new Vector3(riverspeed.y,0.0f,riverspeed.x);

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
			
			if(itemnums.Count>0)
			{
				//select powerup
				if(Input.GetKeyDown(KeyCode.RightAlt)){
					selectedIndex=(selectedIndex+1)%itemnums.Count;
				}
				
				//use powerup
				if(Input.GetKeyDown(KeyCode.RightControl)){
					int tempnum = itemnums[selectedIndex];
					if(tempnum == 1 || tempnum == 2)
						boatScript1.ItemEffect(tempnum);
					else 
						ItemEffect(tempnum);
					
					itemnums.RemoveAt(selectedIndex);
					
					if(selectedIndex>=itemnums.Count){
						selectedIndex=Mathf.Max(0,itemnums.Count-1);
					}
				}
			}
		}

		//Add for reserve
		motor *= reverse;
		steer *= reverse;
		steer *= slippery;

		if(motor>0.0)
			CurrVel += Acc;
		else 
			CurrVel -= 1.5f * Acc;
		
		CurrVel = Mathf.Clamp(CurrVel,0.0f,maxVel);


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
		

		transform.Rotate(transform.up* steer * 0.3f);
		
		
		if(drift&&steer>0.1f)
		{
			transform.Rotate(transform.up* 1.0f);
			CurrVel = CurrVel * 0.98f;
			CurrVel = Mathf.Max(CurrVel,0.1f*maxVel);
		}
		else if(drift&&steer<-0.1f)
		{
			transform.Rotate(-transform.up* 1.0f);
			CurrVel = CurrVel * 0.98f;
			CurrVel = Mathf.Max(CurrVel,0.1f*maxVel);
		}
		
		audio.volume = 0.3f + 0.7f * CurrVel/maxVel;
		audio.volume = Mathf.Min(audio.volume,1.0f);
		
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
			   CurrVel *= 0.3f;
			else
				shield = false; 
		}

		if(collider.CompareTag("Borders")){
			onborder = true;
			CurrVel *= 0.9f;
		}

		if(collider.CompareTag("OilBarrels")){
			collider.GetComponent<OilScript>().onoff = true;
			ItemEffect(7);
		}

		if(collider.CompareTag("Mines")){
			collider.GetComponent<MineScript>().onoff = true;
			ItemEffect(6);
		}
	}



}




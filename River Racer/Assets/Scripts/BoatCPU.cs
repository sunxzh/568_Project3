using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoatCPU : MonoBehaviour {
	
	public Transform engineSpume;
	public bool isMoving = false;

	public float mass;
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
			
			itemstarttimes[tempid] += Time.deltaTime;
			
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
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
			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid]+ 1.0f;
				shield = false;
			}
			
			itemstarttimes[tempid] += Time.deltaTime;
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
				itemstarttimes[tempid] = 0.0f;
				Items[tempid].GetComponent<StarScript>().onoff = false;
			}
		}
		
		//2 UFO
		if(itemid == 2 || Items[2].GetComponent<UFOScript>().onoff)
		{
			int tempid = 2;
			Items[tempid].GetComponent<UFOScript>().onoff = true;
			
			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid] + 1.0f;
				shield = false;
			}
			
			itemstarttimes[tempid] += Time.deltaTime;
			
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
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
			
			GameObject temp = (GameObject)Instantiate(Items[itemid],transform.position - 8.0f * transform.forward,transform.rotation);
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
				blink = true;
				blinks = 0.0f;
			}
		}
		
		//5 Oilbarrel generate a Oilbarrel after the boat
		if(itemid == 5)
		{
			GameObject temp = (GameObject)Instantiate(Items[itemid],transform.position - 8.0f * transform.forward,transform.rotation* Quaternion.Euler(new Vector3(0.0f,90.0f, 0.0f)));
			temp.rigidbody.velocity = 0.4f * rigidbody.velocity + transform.right * Random.Range(-15, 15);
		}
		
		//If hit Oilbarrel, become slippery
		if(itemid == 7 || itemstarttimes[5]!=0.0f)
		{
			int tempid = 5;
			if(shield)
			{
				itemstarttimes[tempid] = itemtimes[tempid] + 1.0f;
				shield = false;
			}
			
			itemstarttimes[tempid] += Time.deltaTime;
			if(itemstarttimes[tempid]>itemtimes[tempid])
			{
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

	
		//Added
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
		
		
		
		//Added to avoid weird rotating
		if(rigidbody.angularVelocity.magnitude < 0.5f)
			rigidbody.angularVelocity = new Vector3(0.0f,0.0f,0.0f);
		else
			rigidbody.angularVelocity /= 1.5f;
		
		//create particles for propeller
		if(engineSpume!=null && isMoving)
		{
			engineSpume.particleEmitter.minEmission = Mathf.Abs(0.8f * 15f );
			engineSpume.particleEmitter.maxEmission = Mathf.Abs(1.2f * 15f );
			engineSpume.particleEmitter.Emit();				
		}


		//AI using items
		if (itemnums.Count > 0){
			int tempnum = itemnums[0];
			if(tempnum == 1 || tempnum == 2){
				boatScript1.ItemEffect(tempnum);
				boatScript2.ItemEffect(tempnum);
				CPUBoatScript1.ItemEffect (tempnum);
				CPUBoatScript2.ItemEffect (tempnum);
				CPUBoatScript3.ItemEffect (tempnum);
			}
			else 
				ItemEffect(tempnum);
			
			itemnums.RemoveAt(selectedIndex);
		}
	
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
			Debug.Log (gameObject.name + "hit border");
		}
	}
	
	void OnCollisionEnter(Collision collision){
		Collider collider = collision.collider; 
		if(collider.CompareTag("Rocks")){
			if(shield)
				shield = false; 
		}
		
		if(collider.CompareTag("Borders")){
			onborder = true;
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
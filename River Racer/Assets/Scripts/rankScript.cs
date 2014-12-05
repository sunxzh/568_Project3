using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rankScript : MonoBehaviour {
	private GameObject nextWaypoint;

	// Use this for initialization
	void Start () {

	}

	public sealed class ReverseComparer<T> : IComparer<T> {
		private readonly IComparer<T> inner;
		public ReverseComparer() : this(null) { }
		public ReverseComparer(IComparer<T> inner) {
			this.inner = inner ?? Comparer<T>.Default;
		}
		int IComparer<T>.Compare(T x, T y) { return inner.Compare(y, x); }
	}
	
	public class CustomComparer:IComparer<BoatScript>{
		public int Compare(BoatScript boatScript1,BoatScript boatScript2){
			if(boatScript1.distToNextWaypoint<boatScript2.distToNextWaypoint){
				return -1;
			}else{
				return 1;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		SortedDictionary<int, List<BoatScript> >ranking=new SortedDictionary<int,List<BoatScript> >(new ReverseComparer<int>());

		//key is the number of passed waypoints, value is the list of boats with the same key
		foreach(GameObject boat in GuiScript.boats){
			BoatScript boatScript=boat.GetComponent<BoatScript>();
			if(!boatScript.end&&GuiScript.start){
				int passedWaypointsNum=boatScript.passedWaypoints.Count;

				if(!ranking.ContainsKey(passedWaypointsNum)){
					List<BoatScript> boatsInSegment=new List<BoatScript>(){boatScript};
					ranking.Add(passedWaypointsNum,boatsInSegment);
				}else{
					ranking[passedWaypointsNum].Add(boatScript);
					ranking[passedWaypointsNum].Sort(new CustomComparer());
				}
			}
		}

		//assign rank to each boat
		int rank=GuiScript.finalRank.Count;
		foreach(KeyValuePair<int,List<BoatScript> > pair in ranking){
			foreach(BoatScript boatScript in pair.Value){
				boatScript.rank=(++rank);
			}
		}
	}
}

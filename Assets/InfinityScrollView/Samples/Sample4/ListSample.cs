using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OneP.InfinityScrollView;
//this is sample script to create list string
public class ListSample : MonoBehaviour {
	//define a infinity scollview
	public InfinityScrollView verticleScroll;

	// define your list string-> that is your list
	public List<string> listString=new List<string>();
	// Use this for initialization
	void Start () {
		
		//setup total item of verticle
		verticleScroll.Setup (listString.Count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}

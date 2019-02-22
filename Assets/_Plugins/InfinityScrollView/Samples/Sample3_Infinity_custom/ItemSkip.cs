using UnityEngine;
using System.Collections;

public class ItemSkip : MonoBehaviour {
	public string skipName;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnClickSkipItem(){
		Sample3.Instance.OnClickSkipItem (skipName); 
	}
}

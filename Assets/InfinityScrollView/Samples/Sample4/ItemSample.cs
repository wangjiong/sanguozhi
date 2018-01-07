using UnityEngine;
using System.Collections;
using OneP.InfinityScrollView;
using UnityEngine.UI;

// inherit from InfinityBaseItem
// this script will control a item in scrollview
public class ItemSample : InfinityBaseItem {
	public ListSample listSample;
	public Text text;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//reload is autocall
	public override void Reload(InfinityScrollView _infinity, int _index){
		base.Reload(_infinity,_index);
		if (listSample != null) {
			if (listSample.listString.Count > Index) {
				text.text = listSample.listString [Index];
			}
		}
	}
}

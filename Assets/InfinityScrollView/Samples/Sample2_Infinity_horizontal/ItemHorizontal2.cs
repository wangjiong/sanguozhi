using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneP.InfinityScrollView;
public class ItemHorizontal2 : InfinityBaseItem {
	public Image image;
	public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void Reload(InfinityScrollView _infinity,int _index){
		base.Reload(_infinity,_index);
		text.text = (Index+1).ToString();
	}
	
	public void OnClick()
	{
		Sample2.Instance.OnClickItem (Index + 1);
	}
}

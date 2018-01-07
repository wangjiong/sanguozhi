using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneP.InfinityScrollView;

public class ItemHorizontal1 : InfinityBaseItem {
	public GameObject item1;
	public GameObject item2;
	public Text text1;
	public Text text2;

	public override void Reload(InfinityScrollView _infinity,int _index){
		base.Reload(_infinity, _index);
		text1.text =  (Index*2+1).ToString();
		text2.text = (Index*2+2).ToString();

		int total = Sample2.Instance.GetTotalItem ();
		if (_index*2+1< total) {
			item2.SetActive (true);
		} else {
			item2.SetActive (false);	
		}
	}
	
	public void OnClick1()
	{
		Sample2.Instance.OnClickItem (Index*2 + 1);
	}
	public void OnClick2()
	{
		Sample2.Instance.OnClickItem (Index*2 + 2);
	}
}

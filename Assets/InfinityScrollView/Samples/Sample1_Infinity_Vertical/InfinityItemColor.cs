using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneP.InfinityScrollView;
public class InfinityItemColor : InfinityBaseItem {
	public Image image;
	public Text text;

	public override void Reload(InfinityScrollView _infinity, int _index){
		// Index is order item in scrollview
		base.Reload (_infinity, _index);
	}
	public void OnClick()
	{
		Sample1.Instance.OnClickItem (Index + 1);
	}
}

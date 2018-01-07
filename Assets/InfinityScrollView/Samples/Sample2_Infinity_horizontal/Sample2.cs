using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OneP.InfinityScrollView;
using OneP.Samples;
public class Sample2 : SingletonMono<Sample2>  {
	public InputField input;
	public InfinityScrollView infinityScroll1;
	public InfinityScrollView infinityScroll2;
	public Text txtInfo;
	private int totalItem=100;
	public int GetTotalItem(){
		return totalItem;
	}
	void Update () {
		
	}
	public void Reload(){
		string text = input.text;
		int valueData = 0;
		if (!int.TryParse (text, out valueData)) {
			valueData = 100;
		}
		if (valueData < 0)
			valueData = 100;
		totalItem = valueData;
		input.text = valueData.ToString ();

		int total1 = valueData / 2;
		if (valueData%2 ==1) {
			total1+=1;
		}
		infinityScroll1.Setup (total1);
		if (valueData/2 > 0) {
			infinityScroll1.InternalReload ();
		}
		infinityScroll2.Setup (valueData);
		if (valueData > 0) {
			infinityScroll2.InternalReload ();
		}
	}
	public void OnClickItem(int index){
		txtInfo.text="Click On Item:"+index;
	}
	public void NextSample(){
		SampleGlobalValue.GoToNextSample ();
	}
	public void ClickAds(){
		txtInfo.text="Click On Ads";
	}
}

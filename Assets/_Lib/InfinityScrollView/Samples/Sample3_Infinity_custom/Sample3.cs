using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OneP.InfinityScrollView;
using OneP.Samples;
public class Sample3 : SingletonMono<Sample3> {
	public InputField input;
	public InfinityScrollView verticleScroll;
	public TextAsset textAsset;
	public Text txtInfo;

	private List<string> listIdUser=new List<string>();

	// Use this for initialization
	void Start () {
		string text = textAsset.ToString ();
		string[] str = text.Split (new string[]{"\n"}, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i=0; i<str.Length; i++) {
			listIdUser.Add(str[i]);
		}
		verticleScroll.Setup ();
	}

	public void Reload(){
		string text = input.text;
		int valueData = 0;
		if (!int.TryParse (text, out valueData)) {
			valueData = 100;
		}
		if (valueData < 0)
			valueData = 100;
		input.text = valueData.ToString ();
		verticleScroll.Setup (valueData);
		if (valueData > 0) {
			verticleScroll.InternalReload ();
		}
	}

	public void NextSample(){
		SampleGlobalValue.GoToNextSample ();
	}
	public string GetIDUser(int index){
		if (listIdUser.Count > index&&index>-1) {
			return listIdUser[index];	
		}
		return "";
	}

	public void OnClickItem(int index){
		txtInfo.text="Click On User:"+index;
	}
	public void OnClickSkipItem(string skipInfo){
		txtInfo.text="Click On :"+skipInfo;
	}
	// Update is called once per frame
	void Update () {
	
	}
}

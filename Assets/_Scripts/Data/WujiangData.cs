using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WujiangBean {
	public int id;
	public string name;
	public string zi;
	public string place;
	public string teji;

	public string image;

	public string qiangbin;
	public string jibin;
	public string nubin;
	public string qibin;
	public string binqi;
	public string shuijun;

	public string tongshuai;
	public string wuli;
	public string zhili;
	public string zhengzhi;
	public string meili;

	public string liezhuang;
}

// Json解析为该对象
public class Response<T> {
	public List<T> list;
}

public class WujiangData {
	
	static string WUJIANG_DATA = "WujiangData/data";
		
	public void LoadData() {
		//Debug.Log("WujiangData LoadData");

		TextAsset wujiangJSON = Resources.Load<TextAsset>(WUJIANG_DATA);
		string text = wujiangJSON.text;
		Loom.RunAsync(() => {
			// 读取武将数据
			ReadData(text);
		});
	}

	private void ReadData(string text) {
		//Debug.Log("WujiangData ReadData");

		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start(); //  开始监视代码运行时间

		Response<WujiangBean> response = JsonUtility.FromJson<Response<WujiangBean>>(text);
		BattleGameManager.msAllWujiangs = response.list;

		stopwatch.Stop(); //  停止监视
		System.TimeSpan timespan = stopwatch.Elapsed;
		double milliseconds = timespan.TotalSeconds;  //  总毫秒数
		//Debug.Log(milliseconds);

		//Debug.Log("ReadData sGenerals:"+ sGenerals.Count);
		//foreach (General g in sGenerals) {
		//    Debug.Log(g.place);
		//}
	}
}

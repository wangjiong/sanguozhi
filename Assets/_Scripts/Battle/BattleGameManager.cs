using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class BattleGameManager : MonoBehaviour {
    static string TAG = "GameManager==";

	static BattleGameManager msBattleGameManager = null;

	private void Awake(){
		msBattleGameManager = this;
	}

	public static BattleGameManager GetInstance() {
		return msBattleGameManager;
	}

	// 1.加载的城池数据
	private CityData mCityData;
	// 2.加载的武将数据
	private WujiangData mWujiangData;

	void Start(){
		LoadData ();
	}

	void LoadData(){
		mCityData = new CityData();
		mCityData.LoadData();

		mWujiangData = new WujiangData();
		mWujiangData.LoadData();
	}

	// 1.武将
	public static List<WujiangBean> msAllWujiangs = new List<WujiangBean>();
	// 1-1 当前选择的武将
	public static List<WujiangBean> msCurrentCityWujiangs = new List<WujiangBean>();
    public static void SetCurrentCityWujiangs() {
		//Debug.Log(TAG + "SetCurrentGenerals sCityName:" + msCurrentCityName + " msAllWujiangs:" + msAllWujiangs.Count );
        BattleGameManager.msCurrentCityWujiangs.Clear();
		foreach (WujiangBean g in BattleGameManager.msAllWujiangs) {
            if (g.place.Equals(BattleGameManager.msCurrentCityName)) {
                BattleGameManager.msCurrentCityWujiangs.Add(g);
            }
        }
    }

	// 2.当前选择的城市
    public static string msCurrentCityName;
    public static GameObject msCurrentCityGameObject;


}
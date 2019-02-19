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
        // 加载城池数据
        mCityData = new CityData();
		mCityData.LoadData();
        // 加载城池数据
        mWujiangData = new WujiangData();
		mWujiangData.LoadData();
        // 归属武将
        mCityData.AllocateWujiangData(mWujiangData);

    }

    public CityData GetCityData() {
        return mCityData;
    }

    public WujiangData GetWujiangData() {
        return mWujiangData;
    }
}
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

    // 当前选择的城市
    City mCurrentCity;

    public void SetCurrentCity(City city) {
        mCurrentCity = city;
        //Debug.Log("SetCurrentCity:" + mCurrentCity.GetCityBean().name + " GetWujiangBeans:" + mCurrentCity.GetWujiangBeans().Count);
    }

    public City GetCurrentCity() {
        return mCurrentCity;
    }

    public List<WujiangBean> GetCurrentCityWujiangs() {
        //Debug.Log("GetCurrentCityWujiangs:" + mCurrentCity.GetCityBean().name + " GetWujiangBeans:" + mCurrentCity.GetWujiangBeans().Count);
        return mCurrentCity.GetWujiangBeans();
    }

}
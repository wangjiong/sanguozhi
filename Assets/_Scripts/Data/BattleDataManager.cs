using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager {
    static BattleDataManager msDataManager = null;

    public static BattleDataManager GetInstance() {
        if (msDataManager == null) {
            msDataManager = new BattleDataManager();
        }
        return msDataManager;
    }

    private BattleDataManager() {}

    CityData mCityData;

    public void LoadCityData() {
        mCityData = new CityData();
        mCityData.LoadData();
    }

    public CityData GetCityData() {
        return mCityData;
    }

    public void ReadWujiangData() {

    }





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    CityBean mCityBean;
    List<WujiangBean> mWujiangBean = new List<WujiangBean>();

    public void SetCityBean(CityBean cityBean) {
        mCityBean = cityBean;
    }

    public CityBean GetCityBean() {
        return mCityBean;
    }

    public List<WujiangBean> GetWujiangBeans() {
        return mWujiangBean;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CityBean {

    public string name;
    public int x, y;
    public int passType = -1;

    public City city;
}

public class CityData {
	
	static string CITY_MODEL = "Citys/City";
	static string PASS_MODEL = "Citys/Pass";
	static string PORT_MODEL = "Citys/Port";

    static string[] CITYS = {
        "武威","8","40",
        "安定","27","53",
        "陇西","9","69",
        "长安","52","72",
        "汉中","26","88",
        "梓潼","15","108",
        "成都","9","130",
        "江州","29","145",
        "建寧","26","173",
        "云南","4","190",
        "永安","45","126",

        "新野","86","108",
        "襄阳","76","116",
        "江陵","81","135",
        "江夏","114","120",
        "武陵","68","154",
        "零陵","69","183",
        "桂阳","98","187",
        "长沙","107","162",

        "柴桑","125","131",
        "庐江","145","116",
        "金陵","161","102",
        "吴","181","107",
        "会稽","183","123",

        "上庸","53","100",
        "宛","76","91",
        "洛阳","78","75",
        "陈留","107","77",
        "开封","119","69",
        "许昌","98","91",
        "汝南","112","106",

        "寿春","139","94",
        "小沛","135","75",
        "彭城","155","75",
        "临淄","159","49",

        "邺","108","52",
        "晋阳","90","38",
        "蓟","121","14",
        "南皮","137","35",
        "平原","128","53",
        "北平","148","19",
        "襄平","173","14",
    };

    static string[] PASSES = {
        "绵竹关","8","124","0",
        "涪水关","10","119","1",
        "葭萌关","23","101","0",
        "剑阁","14","96","0",
        "阳平关","27","82","0",
        "潼关","60","77","1",
        "函谷关","67","77","1",
        "武关","67","85","1",
        "虎牢关","89","76","1",
        "壶关","91","56","1",
    };

    static string[] PORTS = {
        "巫林港","54","129",
        "江津港","73","135",
        "公安港","74","145",
        "洞庭港","79","154",
        "罗阳港","100","147",
        "乌林港","99","132",
        "汉津港","89","124",
        "夏口港","108","122",
        "中庐港","70","109",
        "湖阳港","81","108",

        "房陵港","59","104",
        "夏阳港","59","54",
        "解阳港","65","64",
        "新丰港","64","73",
        "孟津港","79","69",
        "官渡港","101","71",
        "顿丘港","123","65",

        "陆口港","116","136",
        "九江港","132","128",
        "鄱阳港","146","141",
        "庐陵港","138","166",
        "皖口港","143","121",
        "芜湖港","163","108",
        "虎林港","161","121",
        "曲阿港","177","100",
        "句章港","187","117",

        "濡须港","148","109",
        "江都港","173","86",
        "海陵港","166","77",
        "冒阳港","176","47",
        "临济港","145","53",
        "高唐港","134","56",
        "白马港","110","62",
        "西河港","63","48",
        "安平港","168","27",
    };

    List<CityBean> mCitys = new List<CityBean>(); // 城市
    List<CityBean> mPasses = new List<CityBean>(); // 关口
    List<CityBean> mPorts = new List<CityBean>(); // 港口

    Dictionary<string, City> mAllCitys = new Dictionary<string, City>();

    public List<CityBean> GetCitys() {
        return mCitys;
    }

    public List<CityBean> GePasses() {
        return mPasses;
    }

    public List<CityBean> GetPorts() {
        return mPorts;
    }

    public Dictionary<string, City> GetAllCitys() {
        return mAllCitys;
    }

    public void LoadData() {
		GameObject cityRoot = new GameObject("Citys");
		cityRoot.transform.position = new Vector3 (0,0,200);
        // 1.城市
        for (int i = 0; i < CITYS.Length;) {
            CityBean city = new CityBean();
            city.name = CITYS[i++];
            city.x = int.Parse(CITYS[i++]);
            city.y = int.Parse(CITYS[i++]);
            mCitys.Add(city);
        }
        // 1-1 创建模型
        foreach (CityBean cityBean in mCitys) {
			GameObject o = GameObject.Instantiate(Resources.Load(CITY_MODEL)) as GameObject;
            o.transform.SetParent(cityRoot.transform);
            if (cityBean.x % 2 == 0) {
                o.transform.localPosition = new Vector3(cityBean.x + 1.5f, 0, -cityBean.y - 1.5f);
            } else {
                o.transform.localPosition = new Vector3(cityBean.x + 1.5f, 0, -cityBean.y - 2f);
            }
            o.GetComponentInChildren<TextMesh>().text = cityBean.name;

            // cityComponent
            City cityComponent = o.AddComponent<City>();
            cityComponent.SetCityBean(cityBean);

            cityBean.city = cityComponent;
            mAllCitys.Add(cityBean.name, cityComponent);

            // 修改地形数据
            MapManager.GetInstance().GetMapDatas()[cityBean.x, cityBean.y] = (int)TerrainType.TerrainType_Dushi;
        }
        // 2.关口
        for (int i = 0; i < PASSES.Length;) {
            CityBean city = new CityBean();
            city.name = PASSES[i++];
            city.x = int.Parse(PASSES[i++]);
            city.y = int.Parse(PASSES[i++]);
            city.passType = int.Parse(PASSES[i++]);
            mPasses.Add(city);
        }
        // 2-1 创建模型
        foreach (CityBean cityBean in mPasses) {
			GameObject o = GameObject.Instantiate(Resources.Load(PASS_MODEL)) as GameObject;
            o.transform.SetParent(cityRoot.transform);
            o.transform.localPosition = new Vector3(cityBean.x + 0.5f, 0, -cityBean.y - 1f);
            if (cityBean.passType == 1) {
                // 竖
                if (cityBean.x % 2 == 0) {
                    o.transform.localPosition = new Vector3(cityBean.x + 1.5f, 0, -cityBean.y - 0.5f);
                } else {
                    o.transform.localPosition = new Vector3(cityBean.x + 1.5f, 0, -cityBean.y - 1f);
                }
                o.transform.Find("Model").localRotation = Quaternion.Euler(0, -90, 0);
            } else {
                // 横
                if (cityBean.x % 2 == 0) {
                    o.transform.localPosition = new Vector3(cityBean.x + 0.5f, 0, -cityBean.y - 1f);
                } else {
                    o.transform.localPosition = new Vector3(cityBean.x + 0.5f, 0, -cityBean.y - 1.5f);
                }
            }
            o.GetComponentInChildren<TextMesh>().text = cityBean.name;
            // cityComponent
            City cityComponent = o.AddComponent<City>();
            cityComponent.SetCityBean(cityBean);

            cityBean.city = cityComponent;
            mAllCitys.Add(cityBean.name, cityComponent);

            // 修改地形数据
            MapManager.GetInstance().GetMapDatas()[cityBean.x, cityBean.y] = (int)TerrainType.TerrainType_Guansuo;
        }
        // 3.港口
        for (int i = 0; i < PORTS.Length;) {
            CityBean city = new CityBean();
            city.name = PORTS[i++];
            city.x = int.Parse(PORTS[i++]);
            city.y = int.Parse(PORTS[i++]);
            mPorts.Add(city);
        }
        // 3-1 创建模型
        foreach (CityBean cityBean in mPorts) {
			GameObject o = GameObject.Instantiate(Resources.Load(PORT_MODEL)) as GameObject;
            o.transform.SetParent(cityRoot.transform);
            if (cityBean.x % 2 == 0) {
                o.transform.localPosition = new Vector3(cityBean.x + 0.5f, 0, -cityBean.y - 1f);
            } else {
                o.transform.localPosition = new Vector3(cityBean.x + 0.5f, 0, -cityBean.y - 1.5f);
            }

            o.GetComponentInChildren<TextMesh>().text = cityBean.name;

            // cityComponent
            City cityComponent = o.AddComponent<City>();
            cityComponent.SetCityBean(cityBean);

            cityBean.city = cityComponent;
            mAllCitys.Add(cityBean.name, cityComponent);

            // 修改地形数据
            MapManager.GetInstance().GetMapDatas()[cityBean.x, cityBean.y] = (int)TerrainType.TerrainType_Gang;
        }
    }

    public void AllocateWujiangData(WujiangData wujiangData){
        foreach (WujiangBean wujiangBean in wujiangData.GetAllWujiangs()) {
            if (mAllCitys.ContainsKey(wujiangBean.place)) {
                mAllCitys[wujiangBean.place].GetWujiangBeans().Add(wujiangBean);
            }
        }
    }
}
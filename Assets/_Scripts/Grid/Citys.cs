using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City {

    public string name;
    public int x, y;

    override public string ToString() {
        return "name:" + name + "  (" + x + " , " + y + ")";
    }
}

public class Citys : MonoBehaviour {
    string TAG = "City==";

    string[] CITYS = {
        "武威","8","40",
        "安定","27","53",
        "陇西","9","69",
        "长安","52","72",
        "汉中","26","88",
        "梓潼","15","108",
        "成都","9","130",
        "江州","29","145",
        "建宁","26","173",
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

        "邺","108","52",
        "晋阳","90","38",
        "蓟","121","14",
        "南皮","137","35",
        "平原","128","53",
        "北平","148","19",
        "襄平","173","14",
    };

    public GameObject mCity;

    List<City> mCitys = new List<City>();

    void Start() {
        print(TAG + "Start");
        for (int i = 0; i < CITYS.Length;) {
            City city = new City();
            city.name = CITYS[i++];
            city.x = int.Parse(CITYS[i++]);
            city.y = int.Parse(CITYS[i++]);
            mCitys.Add(city);
        }
        foreach (City city in mCitys) {
            print(city);
            GameObject o = Instantiate(mCity);
            o.transform.SetParent(this.transform);
            o.transform.localPosition = new Vector3(city.x, 0, -city.y);
            o.GetComponentInChildren<TextMesh>().text = city.name;
        }
    }

}
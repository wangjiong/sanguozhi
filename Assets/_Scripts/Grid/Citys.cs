using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City {

    public string name;
    public int x, y;
    public int passType = -1;

    override public string ToString() {
        return "name:" + name + "  (" + x + " , " + y + ")" + " passType:" + passType;
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

    string[] PASSES = {
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

    string[] PORTS = {
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

    public GameObject mCity;
    public GameObject mPasse;
    public GameObject mPort;

    List<City> mCitys = new List<City>(); // 城市
    List<City> mPasses = new List<City>(); // 关口
    List<City> mPorts = new List<City>(); // 港口


    void Start() {
        // 城市
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
        // 关口
        for (int i = 0; i < PASSES.Length;) {
            City city = new City();
            city.name = PASSES[i++];
            city.x = int.Parse(PASSES[i++]);
            city.y = int.Parse(PASSES[i++]);
            city.passType = int.Parse(PASSES[i++]);
            mPasses.Add(city);
        }
        foreach (City city in mPasses) {
            print(city);
            GameObject o = Instantiate(mPasse);
            o.transform.SetParent(this.transform);
            o.transform.localPosition = new Vector3(city.x, 0, -city.y);
            if (city.passType == 1) {
                o.transform.Find("Model").localRotation = Quaternion.Euler(0, 90, 0);
            }
            o.GetComponentInChildren<TextMesh>().text = city.name;
        }
        // 港口
        for (int i = 0; i < PORTS.Length;) {
            City city = new City();
            city.name = PORTS[i++];
            city.x = int.Parse(PORTS[i++]);
            city.y = int.Parse(PORTS[i++]);
            mPorts.Add(city);
        }
        foreach (City city in mPorts) {
            print(city);
            GameObject o = Instantiate(mPort);
            o.transform.SetParent(this.transform);
            o.transform.localPosition = new Vector3(city.x, 0, -city.y);
            o.GetComponentInChildren<TextMesh>().text = city.name;
        }
    }

}
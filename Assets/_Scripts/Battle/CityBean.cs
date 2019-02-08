using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBean {

    public string name;
    public int x, y;
    public int passType = -1;

	public List<WujiangBean> generals = new List<WujiangBean>();

    override public string ToString() {
        return "name:" + name + "  (" + x + " , " + y + ")" + " passType:" + passType;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City {

    public string name;
    public int x, y;
    public int passType = -1;

    public List<General> generals = new List<General>();

    override public string ToString() {
        return "name:" + name + "  (" + x + " , " + y + ")" + " passType:" + passType;
    }
}

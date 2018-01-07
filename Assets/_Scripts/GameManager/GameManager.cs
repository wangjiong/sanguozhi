using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 武将
[System.Serializable]
public class General {
    public int id;
    public string name;
    public string zi;
    public string place;
    public string teji;

    public string image;

    public string qiangbin;
    public string jibin;
    public string nubin;
    public string qibin;
    public string binqi;
    public string shuijun;

    public string tongshuai;
    public string wuli;
    public string zhili;
    public string zhengzhi;
    public string meili;

    public string liezhuang;
}

// Json解析为该对象
public class Response<T> {
    public List<T> list;
}


public class GameManager : MonoBehaviour {
    public static List<General> sGenerals = new List<General>();

    public TextAsset mData;

    void Start() {
        string text = mData.text;
        Loom.RunAsync(() => {
            ReadData(text);
        });
    }

    private void ReadData(string text) {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start(); //  开始监视代码运行时间

        Response<General> response = JsonUtility.FromJson<Response<General>>(text);
        sGenerals = response.list;

        stopwatch.Stop(); //  停止监视
        System.TimeSpan timespan = stopwatch.Elapsed;
        double milliseconds = timespan.TotalSeconds;  //  总毫秒数
        //Debug.Log(milliseconds);

        foreach (General g in sGenerals) {
            Debug.Log(g.name);
        }
    }
}
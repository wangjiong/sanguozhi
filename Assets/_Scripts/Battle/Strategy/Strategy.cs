using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy {

    public static List<WujiangBean> GetExpeditionGenerals(List<WujiangBean> wujiangs) {
        WujiangBean general01 = null;
        WujiangBean general02 = null;
        WujiangBean general03 = null;

        if (wujiangs.Count >= 3) {
            general01 = wujiangs[Random.Range(0, wujiangs.Count)];
            while (true) {
                general02 = wujiangs[Random.Range(0, wujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
            while (true) {
                general03 = wujiangs[Random.Range(0, wujiangs.Count)];
                if (general03 != general01 && general03 != general02) {
                    break;
                }
            }
        } else if (wujiangs.Count == 2) {
            general01 = wujiangs[Random.Range(0, wujiangs.Count)];
            while (true) {
                general02 = wujiangs[Random.Range(0, wujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
        } else if (wujiangs.Count == 1) {
            general01 = wujiangs[0];
        }
        List<WujiangBean> temp = new List<WujiangBean>();
        temp.Add(general01);
        temp.Add(general02);
        temp.Add(general03);
        return temp;
    }
}
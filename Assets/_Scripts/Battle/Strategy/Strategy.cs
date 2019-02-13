using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy : MonoBehaviour {
    public static List<WujiangBean> GetExpeditionGenerals(List<WujiangBean> list) {
        WujiangBean general01 = null;
        WujiangBean general02 = null;
        WujiangBean general03 = null;

        List<WujiangBean> currentCityWujiangs = BattleGameManager.GetInstance().GetCurrentCityWujiangs();
        if (currentCityWujiangs.Count >= 3) {
            general01 = currentCityWujiangs[Random.Range(0, currentCityWujiangs.Count)];
            while (true) {
                general02 = currentCityWujiangs[Random.Range(0, currentCityWujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
            while (true) {
                general03 = currentCityWujiangs[Random.Range(0, currentCityWujiangs.Count)];
                if (general03 != general01 && general03 != general02) {
                    break;
                }
            }
        } else if (currentCityWujiangs.Count == 2) {
            general01 = currentCityWujiangs[Random.Range(0, currentCityWujiangs.Count)];
            while (true) {
                general02 = currentCityWujiangs[Random.Range(0, currentCityWujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
        } else if (currentCityWujiangs.Count == 1) {
            general01 = currentCityWujiangs[0];
        }
        List<WujiangBean> temp = new List<WujiangBean>();
        temp.Add(general01);
        temp.Add(general02);
        temp.Add(general03);
        return temp;
    }
}
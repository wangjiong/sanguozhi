using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy : MonoBehaviour {
	public static List<WujiangBean> GetExpeditionGenerals(List<WujiangBean> list) {
		WujiangBean general01 = null;
		WujiangBean general02 = null;
		WujiangBean general03 = null;
        if (BattleGameManager.msCurrentCityWujiangs.Count >= 3) {
            general01 = BattleGameManager.msCurrentCityWujiangs[Random.Range(0, BattleGameManager.msCurrentCityWujiangs.Count)];
            while (true) {
                general02 = BattleGameManager.msCurrentCityWujiangs[Random.Range(0, BattleGameManager.msCurrentCityWujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
            while (true) {
                general03 = BattleGameManager.msCurrentCityWujiangs[Random.Range(0, BattleGameManager.msCurrentCityWujiangs.Count)];
                if (general03 != general01 && general03 != general02) {
                    break;
                }
            }
        } else if (BattleGameManager.msCurrentCityWujiangs.Count == 2) {
            general01 = BattleGameManager.msCurrentCityWujiangs[Random.Range(0, BattleGameManager.msCurrentCityWujiangs.Count)];
            while (true) {
                general02 = BattleGameManager.msCurrentCityWujiangs[Random.Range(0, BattleGameManager.msCurrentCityWujiangs.Count)];
                if (general02 != general01) {
                    break;
                }
            }
        } else if (BattleGameManager.msCurrentCityWujiangs.Count == 1) {
            general01 = BattleGameManager.msCurrentCityWujiangs[0];
        }
		List<WujiangBean> temp = new List<WujiangBean>();
        temp.Add(general01);
        temp.Add(general02);
        temp.Add(general03);
        return temp;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNubing {

    // 1-火矢
    public static List<Coordinates> ShowSkillTarget_Nubing01(Wujiang myWujiang) {
        List<Coordinates> targets = new List<Coordinates>();
        Coordinates center = MapManager.GetInstance().TerrainPositionToCorrdinate(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
        List<Coordinates> neighbours = new List<Coordinates>();
        MapManager.GetInstance().GetAroundN(neighbours, center, 2);
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();

        foreach (Coordinates c in neighbours) {
            if (wujiangExpeditions.ContainsKey(c)) {
                // 不能是自己
                if (!myWujiang.GetCoordinates().Equals(c)) {
                    targets.Add(c);
                }
            }
        }
        return targets;
    }

    // 2-贯射
    public static List<Coordinates> ShowSkillTarget_Nubing02(Wujiang myWujiang) {
        List<Coordinates> targets = new List<Coordinates>();
        Coordinates center = MapManager.GetInstance().TerrainPositionToCorrdinate(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
        List<Coordinates> neighbours = new List<Coordinates>();
        MapManager.GetInstance().GetAroundN(neighbours, center, 2);
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();

        foreach (Coordinates c in neighbours) {
            if (wujiangExpeditions.ContainsKey(c)) {
                // 不能是自己
                if (!myWujiang.GetCoordinates().Equals(c)) {
                    // 必须是一条直线上
                    if (MapManager.GetInstance().CheckInLine(c, myWujiang.GetCoordinates())) {
                        targets.Add(c);
                    }
                }
            }
        }
        return targets;
    }

    // 3-乱射
    public static List<Coordinates> ShowSkillTarget_Nubing03(Wujiang myWujiang) {
        return ShowSkillTarget_Nubing01(myWujiang);
    }

    public static void Skill_Nubing01(Wujiang wujiang, Coordinates target) {
    }

    public static void Skill_Nubing02(Wujiang wujiang, Coordinates target) {
    }

    public static void Skill_Nubing03(Wujiang wujiang, Coordinates target) {
    }
}

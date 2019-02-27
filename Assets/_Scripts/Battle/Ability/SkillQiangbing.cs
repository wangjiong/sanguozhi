using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillQiangbing {
    public static List<Coordinates> ShowSkillTarget_Qiangbing01(Wujiang wujiang) {
        List<Coordinates> targets = new List<Coordinates>();
        List<Coordinates> neighbour = MapManager.GetInstance().GetNeighbours(MapManager.GetInstance().TerrainPositionToCorrdinate(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position));
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        foreach (Coordinates c in neighbour) {
            if (wujiangExpeditions.ContainsKey(c)) {
                // 不能是自己
                if (!wujiang.GetCoordinates().Equals(c)) {
                    targets.Add(c);
                }
            }
        }
        return targets;
    }

    public static List<Coordinates> ShowSkillTarget_Qiangbing02(Wujiang wujiang) {
        return ShowSkillTarget_Qiangbing01(wujiang);
    }

    public static List<Coordinates> ShowSkillTarget_Qiangbing03(Wujiang wujiang) {
        return ShowSkillTarget_Qiangbing01(wujiang);
    }

    public static void Skill_Qiangbing01(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];

        Coordinates c1 = wujiang.GetCoordinates();
        Coordinates c2 = targetWujiang.GetCoordinates();
        int dx = c2.HexX - c1.HexX;
        int dy = c2.HexY - c1.HexY;
        Coordinates otherCoordinates = new Coordinates();
        otherCoordinates.SetHexXY(c2.HexX + dx, c2.HexY + dy);

        if (!Skills.CanMoveIn(otherCoordinates)) {
            return;
        }

        targetWujiang.Move(otherCoordinates);
    }

    public static void Skill_Qiangbing02(Wujiang wujiang, Coordinates target) {
    }

    public static void Skill_Qiangbing03(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];
        Coordinates c1 = wujiang.GetCoordinates();
        Coordinates c2 = targetWujiang.GetCoordinates();
        int dx = (c2.HexX - c1.HexX) * 2;
        int dy = (c2.HexY - c1.HexY) * 2;
        Coordinates myCoordinates = new Coordinates();
        Coordinates otherCoordinates = new Coordinates();
        myCoordinates.SetHexXY(c1.HexX + dx, c1.HexY + dy);
        otherCoordinates.SetHexXY(c2.HexX + dx, c2.HexY + dy);

        if (!Skills.CanMoveIn(myCoordinates)) {
            return;
        } else if (!Skills.CanMoveIn(otherCoordinates)) {
            targetWujiang.Move(myCoordinates);
        } else {
            targetWujiang.Move(otherCoordinates);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJibing {

    public static List<Coordinates> ShowSkillTarget_Jibing01(Wujiang wujiang) {
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

    public static List<Coordinates> ShowSkillTarget_Jibing02(Wujiang wujiang) {
        return ShowSkillTarget_Jibing01(wujiang);
    }

    public static List<Coordinates> ShowSkillTarget_Jibing03(Wujiang wujiang) {
        return ShowSkillTarget_Jibing01(wujiang);
    }

    public static void Skill_Jibing01(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];

        // 1.伤害
        targetWujiang.OnDamage(3000);
        // 2.移动
        Coordinates c1 = wujiang.GetCoordinates();
        Coordinates c2 = targetWujiang.GetCoordinates();
        int dx = c1.HexX - c2.HexX;
        int dy = c1.HexY - c2.HexY;
        Coordinates myCoordinates = new Coordinates();
        myCoordinates.SetHexXY(c2.HexX + dx, c2.HexY + dy);

        if (!Skills.CanMoveIn(myCoordinates)) {
            return;
        }

        wujiang.Move(myCoordinates);
        targetWujiang.Move(c1);
    }

    public static void Skill_Jibing02(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        List<Coordinates> neighbors = MapManager.GetInstance().GetNeighbours(wujiang.GetCoordinates());

        if (wujiangExpeditions.ContainsKey(target)) {
            wujiangExpeditions[target].OnDamage(4000);
        }
        foreach (Coordinates c in neighbors) {
            if (MapManager.GetInstance().CheckInNeighbors(target, c)) {
                if (wujiangExpeditions.ContainsKey(c)) {
                    wujiangExpeditions[c].OnDamage(4000);
                }
            }
        }
    }

    public static void Skill_Jibing03(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        List<Coordinates> neighbors = MapManager.GetInstance().GetNeighbours(wujiang.GetCoordinates());

        foreach (Coordinates c in neighbors) {
            if (wujiangExpeditions.ContainsKey(c)) {
                wujiangExpeditions[c].OnDamage(5000);
            }
        }
    }

}
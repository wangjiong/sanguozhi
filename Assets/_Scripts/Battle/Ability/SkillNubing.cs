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
        Coordinates c1 = MapManager.GetInstance().TerrainPositionToCorrdinate(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
        List<Coordinates> neighbours = new List<Coordinates>();
        MapManager.GetInstance().GetAroundN(neighbours, c1, 2);
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();

        foreach (Coordinates c in neighbours) {
            if (wujiangExpeditions.ContainsKey(c)) {
                // 不能是自己
                if (!myWujiang.GetCoordinates().Equals(c)) {
                    // 必须是一条直线上
                    if (MapManager.GetInstance().CheckInLine(c, c1)) {
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
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];
        // 1.伤害
        targetWujiang.OnDamage(3000);
    }

    public static void Skill_Nubing02(Wujiang myWujiang, Coordinates target) {
        Coordinates c1 = myWujiang.GetCoordinates();
        Coordinates c2 = target;
        List<Coordinates> targets = new List<Coordinates>();
        if (c1.HexX == c2.HexX) {
            int count = c2.HexY - c1.HexY;
            int step = count > 0 ? 1 : -1;
            for (int i= c2.HexY ; i!= c1.HexY; i -= step) {
                Coordinates c = new Coordinates();
                c.SetHexXY(c1.HexX , i);
                targets.Add(c);
            }
        }else if (c1.HexY == c2.HexY) {
            int count = c2.HexX - c1.HexX;
            int step = count > 0 ? 1 : -1;
            for (int i = c2.HexX; i != c1.HexX; i -= step) {
                Coordinates c = new Coordinates();
                c.SetHexXY(i, c1.HexY);
                targets.Add(c);
            }
        } else if (c1.HexZ == c2.HexZ) {
            int count = c2.HexY - c1.HexY;
            int step = count > 0 ? 1 : -1;
            for (int i = c2.HexY; i != c1.HexY; i -= step) {
                Coordinates c = new Coordinates();
                c.SetHexXY(-c1.HexZ-i, i);
                targets.Add(c);
            }
        }
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        foreach (Coordinates c in targets) {
            if (wujiangExpeditions.ContainsKey(c)) {
                wujiangExpeditions[c].OnDamage(4000);
            }
        }
    }

    public static void Skill_Nubing03(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        List<Coordinates> neighbors = MapManager.GetInstance().GetNeighbours(target);
        neighbors.Add(target);

        foreach (Coordinates c in neighbors) {
            if (wujiangExpeditions.ContainsKey(c)) {
                wujiangExpeditions[c].OnDamage(5000);
            }
        }
    }
}

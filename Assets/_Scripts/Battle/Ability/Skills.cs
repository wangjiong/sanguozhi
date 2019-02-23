using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmType {
    ArmType_Qiangbing,
    ArmType_jibing,
    ArmType_Nubing,
    ArmType_Qibing,
};


public delegate void Skill(Wujiang wujiang, Coordinates target);

public delegate List<Coordinates> ShowSkillTarget(Wujiang wujiang);

public class Skills {

    public List<Skill> mSkills = new List<Skill>();
    public List<ShowSkillTarget> mShowSkillTargets = new List<ShowSkillTarget>();

    public Skills(Wujiang wujiang) {
        ArmType armType = wujiang.mArmType;
        int armAbility = wujiang.mArmAbility;
        if (armType == ArmType.ArmType_Qiangbing && armAbility >= 3) {
            mSkills.Add(Skill_Qibing01);
            mSkills.Add(Skill_Qibing02);
            mSkills.Add(Skill_Qibing03);

            mShowSkillTargets.Add(ShowSkillTarget_Qibing01);
            mShowSkillTargets.Add(ShowSkillTarget_Qibing02);
            mShowSkillTargets.Add(ShowSkillTarget_Qibing03);
        }
    }

    List<Coordinates> ShowSkillTarget_Qibing01(Wujiang wujiang) {
        List<Coordinates> targets = new List<Coordinates>();
        List<Coordinates> neighbour = MapManager.GetInstance().GetNeighbours(wujiang.GetCoordinates());
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        foreach (Coordinates c in neighbour) {
            if (wujiangExpeditions.ContainsKey(c)) {
                targets.Add(c);
            }
        }
        return targets;
    }

    List<Coordinates> ShowSkillTarget_Qibing02(Wujiang wujiang) {
        return ShowSkillTarget_Qibing01(wujiang);
    }

    List<Coordinates> ShowSkillTarget_Qibing03(Wujiang wujiang) {
        return ShowSkillTarget_Qibing01(wujiang);
    }

    void Skill_Qibing01(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];
        Coordinates c1 = wujiang.GetCoordinates();
        Coordinates c2 = targetWujiang.GetCoordinates();
        int dx = c2.HexX - c1.HexX;
        int dy = c2.HexY - c1.HexY;
        Coordinates myCoordinates = new Coordinates();
        myCoordinates.SetHexXY(c2.HexX + dx, c2.HexY + dy);

        // 这里必须先移动target
        targetWujiang.Move(myCoordinates);
        wujiang.Move(c2);

    }

    void Skill_Qibing02(Wujiang wujiang, Coordinates target) {
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        Wujiang targetWujiang = wujiangExpeditions[target];
        Coordinates c2 = targetWujiang.GetCoordinates();
        wujiang.Move(c2);
    }

    void Skill_Qibing03(Wujiang wujiang, Coordinates target) {
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
        wujiang.Move(myCoordinates);
        targetWujiang.Move(otherCoordinates);
    }
}

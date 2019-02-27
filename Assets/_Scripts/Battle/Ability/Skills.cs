using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmType {
    ArmType_Qiangbing = 0,
    ArmType_jibing,
    ArmType_Nubing,
    ArmType_Qibing,
};


public delegate void Skill(Wujiang wujiang, Coordinates target);

public delegate List<Coordinates> ShowSkillTarget(Wujiang wujiang);

public class Skills {

    public List<Skill> mSkills = new List<Skill>();
    public List<ShowSkillTarget> mShowSkillTargets = new List<ShowSkillTarget>();
    public List<List<Coordinates>> mAllTargets = new List<List<Coordinates>>();
    public List<string> mSkillNames = new List<string>();

    public Skills(Wujiang wujiang , string[] skillNames) {
        if (wujiang.mArmAbility == 1) {
            if (wujiang.mArmType == ArmType.ArmType_Qiangbing) {
                mSkills.Add(SkillQiangbing.Skill_Qiangbing01);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing01);
            } else if (wujiang.mArmType == ArmType.ArmType_jibing) {
                mSkills.Add(SkillJibing.Skill_Jibing01);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing01);
            } else if (wujiang.mArmType == ArmType.ArmType_Nubing) {
                mSkills.Add(SkillNubing.Skill_Nubing01);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing01);
            } else if (wujiang.mArmType == ArmType.ArmType_Qibing) {
                mSkills.Add(SkillQibing.Skill_Qibing01);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing01);
            }
        } else if (wujiang.mArmAbility == 2) {
            if (wujiang.mArmType == ArmType.ArmType_Qiangbing) {
                mSkills.Add(SkillQiangbing.Skill_Qiangbing01);
                mSkills.Add(SkillQiangbing.Skill_Qiangbing02);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing01);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing02);
            } else if (wujiang.mArmType == ArmType.ArmType_jibing) {
                mSkills.Add(SkillJibing.Skill_Jibing01);
                mSkills.Add(SkillJibing.Skill_Jibing02);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing01);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing02);
            } else if (wujiang.mArmType == ArmType.ArmType_Nubing) {
                mSkills.Add(SkillNubing.Skill_Nubing01);
                mSkills.Add(SkillNubing.Skill_Nubing02);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing01);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing02);
            } else if (wujiang.mArmType == ArmType.ArmType_Qibing) {
                mSkills.Add(SkillQibing.Skill_Qibing01);
                mSkills.Add(SkillQibing.Skill_Qibing02);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing01);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing02);
            }
        } else if (wujiang.mArmAbility > 2) {
            if (wujiang.mArmType == ArmType.ArmType_Qiangbing) {
                mSkills.Add(SkillQiangbing.Skill_Qiangbing01);
                mSkills.Add(SkillQiangbing.Skill_Qiangbing02);
                mSkills.Add(SkillQiangbing.Skill_Qiangbing03);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing01);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing02);
                mShowSkillTargets.Add(SkillQiangbing.ShowSkillTarget_Qiangbing03);
            } else if (wujiang.mArmType == ArmType.ArmType_jibing) {
                mSkills.Add(SkillJibing.Skill_Jibing01);
                mSkills.Add(SkillJibing.Skill_Jibing02);
                mSkills.Add(SkillJibing.Skill_Jibing03);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing01);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing02);
                mShowSkillTargets.Add(SkillJibing.ShowSkillTarget_Jibing03);
            } else if (wujiang.mArmType == ArmType.ArmType_Nubing) {
                mSkills.Add(SkillNubing.Skill_Nubing01);
                mSkills.Add(SkillNubing.Skill_Nubing02);
                mSkills.Add(SkillNubing.Skill_Nubing03);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing01);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing02);
                mShowSkillTargets.Add(SkillNubing.ShowSkillTarget_Nubing03);
            } else if (wujiang.mArmType == ArmType.ArmType_Qibing) {
                mSkills.Add(SkillQibing.Skill_Qibing01);
                mSkills.Add(SkillQibing.Skill_Qibing02);
                mSkills.Add(SkillQibing.Skill_Qibing03);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing01);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing02);
                mShowSkillTargets.Add(SkillQibing.ShowSkillTarget_Qibing03);
            }
        }
        if (wujiang.mArmAbility == 1) {
            mSkillNames.Add(skillNames[0]);
        } else if (wujiang.mArmAbility == 2) {
            mSkillNames.Add(skillNames[0]);
            mSkillNames.Add(skillNames[1]);
        } else if (wujiang.mArmAbility > 2) {
            mSkillNames.Add(skillNames[0]);
            mSkillNames.Add(skillNames[1]);
            mSkillNames.Add(skillNames[2]);
        }
    }

    public void ShowAllTargets(Wujiang wujiang) {
        for (int i = 0; i < mShowSkillTargets.Count; i++) {
            mAllTargets.Add(mShowSkillTargets[i](wujiang));
        }
    }

    public static bool CanMoveIn(Coordinates coordinates) {
        if (MapManager.GetInstance().ContainTerrainType(coordinates, TerrainType.TerrainType_Wujiang) ||
            MapManager.GetInstance().ContainTerrainType(coordinates, TerrainType.TerrainType_Dushi) ||
            MapManager.GetInstance().ContainTerrainType(coordinates, TerrainType.TerrainType_Guansuo) ||
            MapManager.GetInstance().ContainTerrainType(coordinates, TerrainType.TerrainType_Gang)
            ) {
            return false;
        }
        return true;
    }
}
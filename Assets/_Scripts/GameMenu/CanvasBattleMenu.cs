using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void StartSkillDelegate(Coordinates coordinates);

public class CanvasBattleMenu : MonoBehaviour {
    string TAG = "CanvasBattleMenu==";

    public static bool msCanStartSkill = true;

    public GameObject mFirstMenu;
    public GameObject[] mMenuFirstBtns;

    public GameObject mSecondMenu;
    public GameObject[] mMenuSecondBtns;

    bool mPositionFlag = true; // 第一次ShowPosition有问题

    Wujiang mWujiang;

    List<Coordinates> mTargets;
    int mSkillIndex = 0;
    List<GameObject> mTargetCache = new List<GameObject>();
    int mCacheIndex = 0;
    static string WUJIANG_TARGET = "WujiangData/Target";

    GameObject mTargetPrefab;
    GameObject mTargetParent;

    void Start() {
        mTargetPrefab = Resources.Load(WUJIANG_TARGET) as GameObject;
        mTargetParent = new GameObject("TargetParent");
        mTargetParent.transform.position = new Vector3(0, 0, 0);
        // 第一级菜单监听，显示第二级菜单位置
        for (int i = 0; i < mMenuFirstBtns.Length; i++) {
            int index = i;
            mMenuFirstBtns[index].GetComponent<Button>().onClick.AddListener(delegate () {
                if (index == 0) {
                    // 隐藏整个菜单
                    gameObject.SetActive(false);
                    // 待机（目前为移动）
                    mWujiang.Move(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
                    mWujiang.Seclet(false);
                } else if (index == 3) {
                    // 显示技能菜单
                    Vector3 scale = mMenuFirstBtns[index].transform.lossyScale;
                    Vector2 sizeDelta = mMenuFirstBtns[index].GetComponent<RectTransform>().sizeDelta;
                    mSecondMenu.transform.position = mMenuFirstBtns[index].transform.position + new Vector3(sizeDelta.x * scale.x / 2, sizeDelta.y * scale.y / 2, 0);
                } else {
                    // 隐耳机菜单
                    mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
                }
            });

        }
        // 第二级菜单监听
        for (int i = 0; i < mMenuSecondBtns.Length; i++) {
            int index = i;
            mMenuSecondBtns[i].GetComponent<Button>().onClick.AddListener(delegate () {
                // 显示技能能攻击的目标集合
                mSkillIndex = index;
                mTargets = mWujiang.mSkills.mShowSkillTargets[index](mWujiang);
                mWujiang.SetWujiangState(WujiangState.WujiangState_Prepare_Attack);
                //mWujiang.HidePath();
                ShowSkillTarget(mTargets);
                gameObject.SetActive(false);
            });
        }
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }

    void OnDisable() {
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }

    // 显示攻击目标
    void ShowSkillTarget(List<Coordinates> targets) {
        foreach (Coordinates coordinates in targets) {
            GameObject g;
            if (mCacheIndex < mTargetCache.Count) {
                g = mTargetCache[mCacheIndex];
            } else {
                g = Instantiate(mTargetPrefab) as GameObject;
                g.transform.parent = mTargetParent.transform;
                mTargetCache.Add(g);
            }
            mCacheIndex++;
            Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
            g.transform.position = new Vector3(p.x, 0.51f, p.z);
        }
        msCanStartSkill = false;
    }

    // 移动和放技能
    public void MoveAndStartSkill(Coordinates attackCoordinates) {
        //  1.先移动，再放技能
        // 注意这里有两个坐标概念:
        // 1.要移动到的位置
        // 2.点击攻击的目标coordinates
        mWujiang.Move(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position, StartSkill , attackCoordinates);
        BattleGameManager.GetInstance().GetWujiangTransparent().SetActive(false);
    }

    // 放技能
    public void StartSkill(Coordinates coordinates) {
        // 只有点击Targets集合中的才能放技能
        foreach (Coordinates c in mTargets) {
            if (c.Equals(coordinates)) {
                mWujiang.mSkills.mSkills[mSkillIndex](mWujiang, c);
                break;
            }
        }
        // 隐藏Targets
        foreach (GameObject g in mTargetCache) {
            g.transform.position = new Vector3(0, 0, 10000);
        }
        mCacheIndex = 0;
        // 取消Select
        mWujiang.SetWujiangState(WujiangState.WujiangState_Battle);
        mWujiang.Seclet(false);
    }


    public void ShowMenu(Vector2 screenPosition) {
        gameObject.SetActive(true);
        if (mPositionFlag) {
            StartCoroutine(SetPosition(screenPosition));
            mPositionFlag = false;
        } else {
            mFirstMenu.transform.position = screenPosition;
            mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        }
    }

    public void SetWujiang(Wujiang wujiang) {
        mWujiang = wujiang;
    }

    IEnumerator SetPosition(Vector2 screenPosition) {
        yield return null;
        mFirstMenu.transform.position = screenPosition;
    }
}
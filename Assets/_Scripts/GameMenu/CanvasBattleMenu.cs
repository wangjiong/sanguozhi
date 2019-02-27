using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void StartSkillDelegate(Coordinates coordinates);

public class CanvasBattleMenu : MonoBehaviour {
    string TAG = "CanvasBattleMenu==";

    public GameObject mFirstMenu;
    public GameObject[] mMenuFirstBtns;

    public GameObject mSecondMenu;
    public GameObject[] mMenuSecondBtns;

    public Sprite mEnableSprite;
    public Sprite mDisableSprite;

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
                    // 隐藏整个菜单(一级菜单)
                    gameObject.SetActive(false);
                    // 待机（目前为移动）
                    mWujiang.Move(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
                    mWujiang.Seclet(false);
                } else if (index == 3) {
                    // 显示技能菜单(二级菜单)
                    Vector3 scale = mMenuFirstBtns[index].transform.lossyScale;
                    Vector2 sizeDelta = mMenuFirstBtns[index].GetComponent<RectTransform>().sizeDelta;
                    mSecondMenu.transform.position = mMenuFirstBtns[index].transform.position + new Vector3(sizeDelta.x * scale.x / 2, sizeDelta.y * scale.y / 2, 0);
                    // 根据武将适性显示技能个数
                    for (int j = 0; j < mMenuSecondBtns.Length; j++) {
                        if (j < mWujiang.mSkills.mSkillNames.Count) {
                            mMenuSecondBtns[j].SetActive(true);
                            mMenuSecondBtns[j].GetComponentInChildren<Text>().text = mWujiang.mSkills.mSkillNames[j];
                            // 根据是否可以释放技能来决定是否只用按钮
                            if (mWujiang.mSkills.mAllTargets[j].Count > 0) {
                                DisabelBtn(mMenuSecondBtns[j]);
                            } else {
                                EnabelBtn(mMenuSecondBtns[j]);
                            }
                        } else {
                            mMenuSecondBtns[j].SetActive(false);
                        }
                    }
                } else {
                    // 隐藏二级菜单
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
                mTargets = mWujiang.mSkills.mAllTargets[index];
                mWujiang.SetWujiangState(WujiangState.WujiangState_Prepare_Attack);
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
        BattleGameManager.msIgnoreRaycast = true;
    }

    // 移动和放技能
    public void MoveAndStartSkill(Coordinates attackCoordinates) {
        //  1.先移动，再放技能
        // 注意这里有两个坐标概念:
        // 1.要移动到的位置
        // 2.点击攻击的目标coordinates
        mWujiang.Move(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position, StartSkill, attackCoordinates);
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

    // 显示战斗菜单
    public void ShowMenu(Vector2 screenPosition) {
        gameObject.SetActive(true);
        if (mPositionFlag) {
            StartCoroutine(SetPosition(screenPosition));
            mPositionFlag = false;
        } else {
            mFirstMenu.transform.position = screenPosition;
            mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        }
        // 检测是否显示选项
        CheckFirstBtn();
    }

    public void SetWujiang(Wujiang wujiang) {
        mWujiang = wujiang;
    }

    IEnumerator SetPosition(Vector2 screenPosition) {
        yield return null;
        mFirstMenu.transform.position = screenPosition;
    }

    // 检测一级菜单按钮
    void CheckFirstBtn() {
        for (int i = 0; i < mMenuFirstBtns.Length; i++) {
            if (i == 0) {
                // 待机
            } else if (i == 1) {
                // 攻击
                DisabelBtn(mMenuFirstBtns[i]);
            } else if (i == 2) {
                // 齐攻
                DisabelBtn(mMenuFirstBtns[i]);
            } else if (i == 3) {
                // 战法
                if (CheckShowSkill()) {
                    EnabelBtn(mMenuFirstBtns[i]);
                } else {
                    DisabelBtn(mMenuFirstBtns[i]);
                }
            } else {
                DisabelBtn(mMenuFirstBtns[i]);
            }
        }
    }

    // 检测释放可以释放战法
    bool CheckShowSkill() {
        mWujiang.mSkills.ShowAllTargets(mWujiang);
        foreach (List<Coordinates> targets in mWujiang.mSkills.mAllTargets) {
            if (targets.Count > 0) {
                return true;
            }
        }
        return false;
    }

    void DisabelBtn(GameObject g) {
        Button btn = g.GetComponent<Button>();
        btn.image.sprite = mDisableSprite;
        btn.GetComponentInChildren<Text>().color = Color.black;
        btn.interactable = false;
    }

    void EnabelBtn(GameObject g) {
        Button btn = g.GetComponent<Button>();
        btn.image.sprite = mEnableSprite;
        btn.GetComponentInChildren<Text>().color = Color.white;
        btn.interactable = true;
    }
}
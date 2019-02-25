using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WujiangState {
    WujiangState_Prepare_Expedition,
    WujiangState_Prepare_Move,
    WujiangState_Prepare_Attack,
    WujiangState_Battle,
    WujiangState_Fallback,
}

public class Wujiang : MonoBehaviour {
    static string TAG = "Wujiang==";

    static Wujiang msCurrentWujiang;
    public static bool msCanShowBattleMenu = true;
    public static bool msCanShowCityMenu = true;

    // 属性
    Coordinates mCoordinates;
    City mCity;
    // 面板属性
    WujiangBean[] mWujiangBeans;// 武将最多三个，主将一定为第一个
    public Image mAvatar;
    public Text mHealth;
    public Text mName;
    public ArmType mArmType = ArmType.ArmType_Qiangbing;
    public int mArmAbility = 3;
    public Skills mSkills;
    // 路径相关
    float mWujiangPathfindingCost = 6;
    Dictionary<Coordinates, Node> mPathfindingResult;

    // 其他
    bool mSelected;
    HighlightableObject mHighlightableObjecto;
    WujiangState mWujiangState;

    void Start() {
        mCoordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(transform.position);
        BattleGameManager.GetInstance().GetWujiangData().SetWujiangExpeditionCorrdinates(mCoordinates, this);
    }

    void OnDestroy() {
        BattleGameManager.GetInstance().GetWujiangData().SetWujiangExpeditionCorrdinates(mCoordinates, null);
    }

    public static void SetCurrentWujiang(Wujiang wujiang) {
        msCurrentWujiang = wujiang;
        msCurrentWujiang.Seclet(true);
    }

    public static Wujiang GetCurrentWujiang() {
        return msCurrentWujiang;
    }

    public void SetWujiangState(WujiangState WujiangState) {
        mWujiangState = WujiangState;
    }

    public WujiangState GetWujiangState() {
        return mWujiangState;
    }

    public void SetCity(City city) {
        mCity = city;
    }

    public City SetCity() {
        return mCity;
    }

    public void HideLight() {
        mSelected = false;
        mHighlightableObjecto.Off();
    }

    public float GetWujiangPathfindingCost() {
        return mWujiangPathfindingCost;
    }

    public void OnMouseDown() {
        // 当前选中的武将准备移动，那么不能点击其他武将
        if (msCurrentWujiang) {
            if (msCurrentWujiang.GetWujiangState() == WujiangState.WujiangState_Prepare_Expedition ||
                msCurrentWujiang.GetWujiangState() == WujiangState.WujiangState_Prepare_Move ||
                msCurrentWujiang.GetWujiangState() == WujiangState.WujiangState_Prepare_Attack
                )
                return;
        }
        // 防止移动过程中点击武将
        if (!msCanShowCityMenu) {
            return;
        }
        mSelected = !mSelected;
        Seclet(mSelected);

        if (mSelected) {
            ShowPath();
            mWujiangState = WujiangState.WujiangState_Prepare_Move;
            msCanShowBattleMenu = false;
        }
    }

    public void Seclet(bool seclet) {
        mSelected = seclet;
        if (mSelected) {
            // 1.选中
            msCurrentWujiang = this;
            if (mHighlightableObjecto == null) {
                mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
            }
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
        } else {
            // 2.不选中
            msCurrentWujiang = null;
            mHighlightableObjecto.Off();
        }
    }

    // 显示路径
    public void ShowPath() {
        mPathfindingResult = Pathfinding.GetInstance().ShowPath(this);
    }

    // 隐藏路径
    public void HidePath() {
        Pathfinding.GetInstance().ClearNode();
    }

    public bool ShowBattleMeun(Vector3 position) {
        Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(position);
        foreach (KeyValuePair<Coordinates, Node> node in mPathfindingResult) {
            if (node.Value.nodeCurrentCosted <= mWujiangPathfindingCost) {
                if (coordinates.Equals(node.Key)) {
                    // 如果移动的目标点为都市、关口、港口，那么让武将进城
                    City city = BattleGameManager.GetInstance().GetCityData().GetCity(coordinates);
                    // 不能回当前的城池
                    if (mWujiangState == WujiangState.WujiangState_Prepare_Expedition) {
                        if (city == mCity) {
                            return true;
                        }
                    }
                    // 显示战斗菜单
                    BattleGameManager.GetInstance().GetCanvasBattleMenu().ShowMenu(Input.mousePosition);
                    BattleGameManager.GetInstance().GetCanvasBattleMenu().SetWujiang(this);
                    // 透明武将
                    GameObject wujiangTransparent = BattleGameManager.GetInstance().GetWujiangTransparent();
                    wujiangTransparent.SetActive(true);
                    wujiangTransparent.transform.position = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
                }
            }
        }
        return false;
    }

    public void Move(Vector3 position , StartSkillDelegate StartSkillDelegate = null , Coordinates attackCoordinates = null) {
        Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(position);
        Node node = mPathfindingResult[coordinates];
        City city = BattleGameManager.GetInstance().GetCityData().GetCity(coordinates);
        // 计算路径
        List<Vector3> waypoints = new List<Vector3>();
        while (node != null) {
            waypoints.Insert(0, MapManager.GetInstance().CorrdinateToTerrainPosition(node.nodeCoordinates));
            node = node.nodeParent;
        }
        // 播放动画
        if (waypoints.Count < 2) {
            // 如果小于2个点，那么直接隐藏路径即可
            HidePath();
            // 如果有技能的话，放技能
            if (StartSkillDelegate != null) {
                StartSkillDelegate(attackCoordinates);
            }
        } else {
            msCanShowCityMenu = false;
            Tween t = transform.DOPath(waypoints.ToArray(), 0.1f * (waypoints.Count-1), PathType.CatmullRom).SetEase(Ease.Linear);
            t.onComplete = delegate () {
                if (city) {
                    // 1.回到城市
                    foreach (WujiangBean wujiangBean in mWujiangBeans) {
                        city.GetWujiangBeans().Add(wujiangBean);
                    }
                    Destroy(gameObject);
                } else {
                    // 2.正常移动
                    transform.position = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
                    BattleGameManager.GetInstance().GetWujiangData().UpdateWujiangExpeditionCorrdinates(mCoordinates, coordinates);
                    mCoordinates = coordinates;
                }
                SetWujiangState(WujiangState.WujiangState_Battle);
                msCanShowCityMenu = true;
                HidePath();
                // 如果有技能的话，放技能
                if (StartSkillDelegate!=null) {
                    StartSkillDelegate(attackCoordinates);
                }
            };
        }
    }

    public void Move(Coordinates targetCoordinates) {
        List<Vector3> waypoints = new List<Vector3>();
        waypoints.Add(transform.position);
        Vector3 targetPosition = MapManager.GetInstance().CorrdinateToTerrainPosition(targetCoordinates);
        waypoints.Add(targetPosition);

        Tween t = transform.DOPath(waypoints.ToArray(), 0.02f * (waypoints.Count - 1), PathType.CatmullRom).SetEase(Ease.Linear);
        t.onComplete = delegate () {
            // 正常移动
            transform.position = targetPosition;
            BattleGameManager.GetInstance().GetWujiangData().UpdateWujiangExpeditionCorrdinates(mCoordinates, targetCoordinates);
            mCoordinates = targetCoordinates;
        };
    }

    public void SetWujiangBeans(WujiangBean[] wujiangBeans) {
        mWujiangBeans = wujiangBeans;
    }

    public WujiangBean[] GetWujiangBeans() {
        return mWujiangBeans;
    }

    public Coordinates GetCoordinates() {
        return mCoordinates;
    }
}
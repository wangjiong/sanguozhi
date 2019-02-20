using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WujiangState {
    WujiangState_Prepare_Expedition,
    WujiangState_Prepare_Move,
    WujiangState_Battle,
    WujiangState_Fallback,
}

public class Wujiang : MonoBehaviour {
    static string TAG = "Wujiang==";

    static Wujiang msCurrentWujiang;

    // 属性
    Coordinates mCoordinates;
    City mCity;
    // 面板属性
    WujiangBean[] mWujiangBeans;// 武将最多三个，主将一定为第一个
    public Image mAvatar;
    public Text mHealth;
    public Text mName;

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
        if (msCurrentWujiang && msCurrentWujiang.GetWujiangState() != WujiangState.WujiangState_Battle) {
            return;
        }
        mSelected = !mSelected;
        Seclet(mSelected);
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
            HidePath();
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

    public void Move(Vector3 position) {
        Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(position);
        foreach (KeyValuePair<Coordinates, Node> node in mPathfindingResult) {
            if (node.Value.nodeCurrentCosted <= mWujiangPathfindingCost) {
                if (coordinates.Equals(node.Key)) {
                    // 如果移动的目标点为都市、关口、港口，那么让武将进城
                    City city = BattleGameManager.GetInstance().GetCityData().GetCity(coordinates);
                    // 不能回当前的城池
                    if (mWujiangState == WujiangState.WujiangState_Prepare_Expedition) {
                        if (city == mCity) {
                            return;
                        }
                    }
                    if (city) {
                        // 1.回到城市
                        foreach (WujiangBean wujiangBean in mWujiangBeans) {
                            city.GetWujiangBeans().Add(wujiangBean);
                        }
                        Destroy(gameObject);
                    } else {
                        // 2.正常移动
                        transform.position = position;
                        // Update WujiangExpeditions
                        BattleGameManager.GetInstance().GetWujiangData().UpdateWujiangExpeditionCorrdinates(mCoordinates, coordinates);
                        mCoordinates = coordinates;
                    }
                    HidePath();
                    Seclet(false);
                    mWujiangState = WujiangState.WujiangState_Battle;
                    return;
                }
            }
        }
    }

    public void SetWujiangBeans(WujiangBean[] wujiangBeans) {
        mWujiangBeans = wujiangBeans;
    }

    public WujiangBean[] GetWujiangBeans() {
        return mWujiangBeans;
    }
}
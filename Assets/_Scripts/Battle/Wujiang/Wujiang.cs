using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node {
    public Coordinates nodeCoordinates;
    public float nodeCost = 1;
    public float nodeCurrentCosted = float.MaxValue;
    public Node(Coordinates coordinates) {
        nodeCoordinates = coordinates;
        // 根据地形信息获取权重
        uint terrainType = MapManager.GetInstance().GetTerrainType(coordinates);
        // 暂时只考虑低8为地表地形
        terrainType = MapManager.ToLowTerrainType(terrainType);
        nodeCost = MapConfig.msTerrainWight[terrainType];
    }
}

public enum WujiangState {
    WujiangState_Prepare_Expedition,
    WujiangState_Prepare_Move,
    WujiangState_Battle,
    WujiangState_Fallback,
}

public class Wujiang : MonoBehaviour {
    static string TAG = "Wujiang==";

    static Wujiang msCurrentWujiang;

    public Image mAvatar;
    public Text mHealth;
    public Text mName;
    bool mSelected;
    HighlightableObject mHighlightableObjecto;

    WujiangBean[] mWujiangBeans;// 武将最多三个，主将一定为第一个

    // Path
    public GameObject mPrefabPathGrid;
    GameObject mPathNodesParent;

    WujiangState mWujiangState;

    Coordinates mCoordinates;

    City mCity;

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

    public void OnMouseDown() {
        // 当前选中的武将准备移动，那么不能点击其他武将
        if (msCurrentWujiang && msCurrentWujiang.GetWujiangState() != WujiangState.WujiangState_Battle) {
            return;
        }
        mSelected = !mSelected;
        Seclet(mSelected);
    }

    Dictionary<Coordinates, Node> mNodesCache = new Dictionary<Coordinates, Node>();
    Dictionary<Coordinates, Node> mResult = new Dictionary<Coordinates, Node>();
    List<GameObject> mPathGameObjectCache = new List<GameObject>();
    int mPathGridsCacheIndex = 0;
    float mWujiangAllCost = 6;

    public void Move(Vector3 position) {
        Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(position);
        foreach (KeyValuePair<Coordinates, Node> node in mResult) {
            if (node.Value.nodeCurrentCosted <= mWujiangAllCost) {
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
        // 所有武将
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        // 所有地形

        if (mPrefabPathGrid) {
            ClearNode();
            Coordinates current = MapManager.GetInstance().TerrainPositionToCorrdinate(transform.position);
            Queue<Node> queue = new Queue<Node>();
            Node startNode = GetNode(current);
            startNode.nodeCurrentCosted = 1;
            queue.Enqueue(startNode);
            mResult[startNode.nodeCoordinates] = startNode;
            while (queue.Count > 0) {
                Node currentNode = queue.Dequeue();
                List<Coordinates> neighbours = MapManager.GetInstance().GetNeighbours(currentNode.nodeCoordinates);
                foreach (Coordinates c in neighbours) {
                    // 检查是否越界
                    if (MapManager.GetInstance().CheckBoundary(c)) {
                        Node node = GetNode(c); // 创建node
                        float newCost = currentNode.nodeCurrentCosted + node.nodeCost;
                        if (newCost < node.nodeCurrentCosted) {
                            node.nodeCurrentCosted = newCost;
                            // 1.当前点的cost小于总cost
                            if (node.nodeCurrentCosted <= mWujiangAllCost) {
                                // 2.不能移动到其他武将的点上
                                if (MapManager.GetInstance().GetTerrainType(node.nodeCoordinates) == (uint)TerrainType.TerrainType_Wujiang) {
                                    if (wujiangExpeditions[node.nodeCoordinates] != this) {
                                        continue;
                                    }
                                }
                                queue.Enqueue(node);
                                mResult[node.nodeCoordinates] = node;
                            }
                        }
                    }
                }
            }
            // 显示可走路径的网格
            foreach (KeyValuePair<Coordinates, Node> node in mResult) {
                GameObject g = GetGridNode();
                g.SetActive(true);
                g.transform.position = MapManager.GetInstance().CorrdinateToTerrainPosition(node.Key);
            }
        }
    }

    // 隐藏路径
    public void HidePath() {
        ClearNode();
    }

    private void ClearNode() {
        mResult.Clear();
        mNodesCache.Clear();
        mPathGridsCacheIndex = 0;
        foreach (GameObject g in mPathGameObjectCache) {
            g.SetActive(false);
        }
    }

    private Node GetNode(Coordinates c) {
        if (!mNodesCache.ContainsKey(c)) {
            mNodesCache.Add(c, new Node(c));
        }
        return mNodesCache[c];
    }

    private GameObject GetGridNode() {
        if (mPathGridsCacheIndex >= mPathGameObjectCache.Count) {
            GameObject g = Instantiate(mPrefabPathGrid);
            if (mPathNodesParent == null) {
                mPathNodesParent = new GameObject("PathNodes");
                mPathNodesParent.transform.position = new Vector3(0, 0, 0);
            }
            g.transform.SetParent(mPathNodesParent.transform);
            mPathGameObjectCache.Add(g);
        }
        return mPathGameObjectCache[mPathGridsCacheIndex++];
    }

    public void SetWujiangBeans(WujiangBean[] wujiangBeans) {
        mWujiangBeans = wujiangBeans;
    }

    public WujiangBean[] GetWujiangBeans() {
        return mWujiangBeans;
    }
}
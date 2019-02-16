using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node {
    public Coordinates nodeCoordinates;
    public float nodeCost = 1;
    public float nodeCurrentCosted = float.MaxValue;
    public Node(Coordinates coordinates) {
        nodeCoordinates = coordinates;
        int terrainType = MapManager.GetInstance().GetMapDatas()[coordinates.x, coordinates.y];
        nodeCost = MapConfig.msTerrainWight[terrainType];
    }
}

public enum WujiangState {
    WujiangState_Prepare,
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

    City mCity;

    void Start() {
        //mWujiangState = WujiangState.WujiangState_Battle;
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
        if (mWujiangState == WujiangState.WujiangState_Prepare) {
            return;
        }
        mSelected = !mSelected;
        Seclet(mSelected);
    }

    Dictionary<Coordinates, Node> mNodesCache = new Dictionary<Coordinates, Node>();
    List<GameObject> mPathGameObjectCache = new List<GameObject>();
    int mPathGridsCacheIndex = 0;
    float mWujiangAllCost = 6;

    public void SetPosition(Vector3 position) {
        Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(position);
        foreach (KeyValuePair<Coordinates, Node> node in mNodesCache) {
            if (node.Value.nodeCurrentCosted <= mWujiangAllCost) {
                if (coordinates.Equals(node.Key)) {
                    // 如果移动的目标点为都市、关口、港口，那么让武将进城
                    City city = BattleGameManager.GetInstance().GetCityData().GetCity(coordinates);
                    // 不能回当前的城池
                    if (mWujiangState == WujiangState.WujiangState_Prepare) {
                        if ( city == mCity) {
                            return;
                        }else {
                            mWujiangState = WujiangState.WujiangState_Battle;
                        }
                    }
                    if (city) {
                        foreach (WujiangBean wujiangBean in mWujiangBeans) {
                            city.GetWujiangBeans().Add(wujiangBean);
                        }
                        Destroy(gameObject);
                    } else {
                        transform.position = position;
                    }
                    HidePath();
                    Seclet(false);
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
        if (mPrefabPathGrid) {
            ClearNode();
            Coordinates current = MapManager.GetInstance().TerrainPositionToCorrdinate(transform.position);
            Queue<Node> queue = new Queue<Node>();
            Node n = GetNode(current);
            n.nodeCurrentCosted = 1;
            queue.Enqueue(n);
            while (queue.Count > 0) {
                Node currentNode = queue.Dequeue();
                List<Coordinates> neighbours = MapManager.GetInstance().GetNeighbours(currentNode.nodeCoordinates);
                foreach (Coordinates c in neighbours) {
                    // 检查是否越界
                    if (MapManager.GetInstance().CheckBoundary(c)) {
                        Node node = GetNode(c);
                        float newCost = currentNode.nodeCurrentCosted + node.nodeCost;
                        if (newCost < node.nodeCurrentCosted) {
                            node.nodeCurrentCosted = newCost;
                            if (node.nodeCurrentCosted <= mWujiangAllCost) {
                                queue.Enqueue(node);
                            }
                        }
                    }
                }
            }
            // 显示可走路径的网格
            foreach (KeyValuePair<Coordinates, Node> node in mNodesCache) {
                if (node.Value.nodeCurrentCosted <= mWujiangAllCost) {
                    GameObject g = GetGridNode();
                    g.SetActive(true);
                    g.transform.position = MapManager.GetInstance().CorrdinateToTerrainPosition(node.Key);
                }
            }
        }
    }

    // 隐藏路径
    public void HidePath() {
        ClearNode();
    }

    // 是否显示路径
    public bool IsShowPath() {
        return mPathGridsCacheIndex > 0;
    }

    private void ClearNode() {
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
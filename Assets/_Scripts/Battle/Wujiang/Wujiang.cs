using System.Collections;
using System.Collections.Generic;
using TGS;
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
public class Wujiang : MonoBehaviour {
    public static Wujiang msCurrentWujiang;

    static string TAG = "Wujiang==";

    WujiangBean mWujiangBean;

    public Image mAvatar;
    public Text mHealth;
    public Text mName;
    bool mSelected;
    HighlightableObject mHighlightableObjecto;

    // Path
    public GameObject mPrefabPathGrid;
    GameObject mPathNodesParent;

    void Start() {
        mPathNodesParent = GameObject.Find("PathNodes");
    }

    public void HideLight() {
        mSelected = false;
        mHighlightableObjecto.Off();
    }

    public void OnMouseDown() {
        if (mHighlightableObjecto == null) {
            mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
        }
        mSelected = !mSelected;
        Seclet(mSelected);
        if (!mSelected) {
            HidePath();
        }
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
                    // 只有在可行走的区域内才可以移动
                    transform.position = position;
                    // 如果移动的目标点为都市、关口、港口，那么让武将进城
                    City city = BattleGameManager.GetInstance().GetCityData().GetCity(coordinates);
                    if (city) {
                        city.GetWujiangBeans().Add(mWujiangBean);
                        Destroy(gameObject);
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
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
        } else {
            // 2.不选中
            msCurrentWujiang = null;
            mHighlightableObjecto.Off();
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
            //Debug.Log( "mPathGridsCacheIndex:"+ mPathGridsCacheIndex + "mNodesCache:" + mPathGameObjectCache.Count);
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
            g.transform.SetParent(mPathNodesParent.transform);
            mPathGameObjectCache.Add(g);
        }
        return mPathGameObjectCache[mPathGridsCacheIndex++];
    }
}
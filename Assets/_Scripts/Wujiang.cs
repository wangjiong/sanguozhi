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
        int terrainType = MapManager.GetInstance().mMapDatas[coordinates.x, coordinates.y];
        nodeCost = MapConfig.msTerrainWight[terrainType];
    }
}
public class Wujiang : MonoBehaviour {
    public static Wujiang sCurrentWujiang;

    static string TAG = "Wujiang==";
    public Image mAvatar;
    public Text mHealth;
    public Text mName;
    bool mSelected;
    HighlightableObject mHighlightableObjecto;

    // Path
    public GameObject mPrefabPathGrid;
    GameObject mPathNodesParent;

    void OnEnable() {
    }

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
        if (mSelected) {
            // 1.选中
            sCurrentWujiang = this;
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
            // 显示移动的范围
            //ShowPath();
        } else {
            // 2.不选中
            sCurrentWujiang = null;
            mHighlightableObjecto.Off();
            Clear();
        }
    }

    Dictionary<Coordinates, Node> mNodesCache = new Dictionary<Coordinates, Node>();
    List<GameObject> mPathGameObjectCache = new List<GameObject>();
    int mPathGridsCacheIndex = 0;
    float mWujiangAllCost = 6;
    public void ShowPath() {
        if (mPrefabPathGrid) {
            Clear();
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
            Debug.Log( "mPathGridsCacheIndex:"+ mPathGridsCacheIndex + "mNodesCache:" + mPathGameObjectCache.Count);
        }
    }

    private void Clear() {
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
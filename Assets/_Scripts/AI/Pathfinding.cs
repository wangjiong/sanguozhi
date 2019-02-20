using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Pathfinding {
    static string PATH_GRID = "AI/PathGrid";

    // Path
    GameObject mPrefabPathGrid;
    GameObject mPathNodesParent;
    Dictionary<Coordinates, Node> mNodesCache = new Dictionary<Coordinates, Node>();
    Dictionary<Coordinates, Node> mResult = new Dictionary<Coordinates, Node>();
    List<GameObject> mPathGameObjectCache = new List<GameObject>();
    int mPathGridsCacheIndex = 0;

    static Pathfinding msPathfinding = null;

    public static Pathfinding GetInstance() {
        if (msPathfinding == null) {
            msPathfinding = new Pathfinding();
        }
        return msPathfinding;
    }

    Pathfinding() {
        mPrefabPathGrid = GameObject.Instantiate(Resources.Load(PATH_GRID)) as GameObject;
        mPathNodesParent = new GameObject("PathNodes");
        mPathNodesParent.transform.position = new Vector3(0, 0, 0);
    }

    public Dictionary<Coordinates, Node> ShowPath(Wujiang wujiang) {
        // 所有武将
        Dictionary<Coordinates, Wujiang> wujiangExpeditions = BattleGameManager.GetInstance().GetWujiangData().GetWujiangExpeditions();
        ClearNode();
        Coordinates current = MapManager.GetInstance().TerrainPositionToCorrdinate(wujiang.transform.position);
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
                        if (node.nodeCurrentCosted <= wujiang.GetWujiangPathfindingCost()) {
                            // 2.不能移动到其他武将的点上
                            if (wujiangExpeditions.ContainsKey(node.nodeCoordinates)) {
                                Wujiang wujiangNode = wujiangExpeditions[node.nodeCoordinates];
                                if (wujiangNode && wujiangNode != wujiang) {
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
        return mResult;
    }

    public void ClearNode() {
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
            GameObject g = GameObject.Instantiate(mPrefabPathGrid);

            g.transform.SetParent(mPathNodesParent.transform);
            mPathGameObjectCache.Add(g);
        }
        return mPathGameObjectCache[mPathGridsCacheIndex++];
    }
}
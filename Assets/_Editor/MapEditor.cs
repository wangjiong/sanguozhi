using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour {
    public GameObject mTerrainTypePrefab;

    // 鼠标指示点
    GameObject mPointCube;
    Vector3 mOriginalPosition;

    string[] mTerrainTypeNames = { "草地", "土", "沙地", "湿地", "毒泉", "森", "川", "河",
        "海", "荒地", "主径", "栈道", "渡所", "浅滩", "岸", "涯", "都市", "关所", "港", "小径"};

    List<Toggle> mTerrainTypeToggles = new List<Toggle>();
    int mIndex = 0;

    // 地图
    int[,] mMapDatas = new int[200, 200];

    void Start() {
        // 鼠标指示点
        mPointCube = GameObject.Find("Point");
        mOriginalPosition = mPointCube.transform.position;
        // 初始化地形类型按钮
        GameObject terrainTypes = GameObject.Find("TerrainTypes");
        for (int i = 0; i < mTerrainTypeNames.Length; i++) {
            GameObject g = Instantiate(mTerrainTypePrefab);
            g.GetComponentInChildren<Text>().text = mTerrainTypeNames[i];
            g.transform.SetParent(terrainTypes.transform);

            Toggle toggle = g.GetComponent<Toggle>();
            int index = i;
            toggle.onValueChanged.AddListener(delegate (bool isOn) { ToggleEvent(isOn, index); });
            mTerrainTypeToggles.Add(toggle);
        }
        mTerrainTypeToggles[mIndex].isOn = true;

        Load();
    }

    void ToggleEvent(bool isOn, int index) {
        if (isOn) {
            mIndex = index;
            for (int i = 0; i < mTerrainTypeToggles.Count; i++) {
                if (i != index) {
                    mTerrainTypeToggles[i].isOn = false;
                }
            }
        }
    }


    void Update() {
        RaycastHit hit;
        // 从鼠标所在的位置发射
        Vector2 screenPosition = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out hit)) {
            // 格子
            Vector3 pointCubePosition = MapManager.GetInstance().TerrainPositionToCorrdinatePosition(hit.point);
            pointCubePosition.y = mOriginalPosition.y;
            mPointCube.transform.position = pointCubePosition;

            // Debug
            Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(hit.point);
            Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
            p.y = mOriginalPosition.y;

            if (coordinates.x >= 0 && coordinates.x < 200 && coordinates.y >= 0 && coordinates.y < 200) {
                // 按住鼠标刷地图
                if (Input.GetMouseButton(1)) {
                    mMapDatas[coordinates.x, coordinates.y] = mIndex;
                }

                // 放开地图自动保存地图
                if (Input.GetMouseButtonUp(1)) {
                    Save();
                }

                int terrainType = mMapDatas[coordinates.x, coordinates.y];
                GameObject.Find("DebugPosition").GetComponent<Text>().text = coordinates.ToString() + " terrainType:" + mTerrainTypeNames[terrainType];
            }
        }
    }

    void Load() {
        Debug.Log("Load");
        FileStream fs = new FileStream(Application.dataPath + "/mapdata.txt", FileMode.Open);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();
        //将读取到的二进制转换成字符串
        string s = new UTF8Encoding().GetString(bytes);
        //将字符串按照'|'进行分割得到字符串数组
        string[] itemIds = s.Split(';');
        Debug.Log(itemIds.Length);
        Debug.Log(itemIds[itemIds.Length-1]);
        // 初始化地图
        for (int i = 0; i < 200; i++) {
            for (int j = 0; j < 200; j++) {
                mMapDatas[i, j] = int.Parse(itemIds[i * 200 + j]);
                //mMapDatas[i, j] = 0;
            }
        }
        Save();
    }

    void Save() {
        Debug.Log("Save");
        StringBuilder mapData = new StringBuilder();
        for (int i = 0; i < 200; i++) {
            for (int j = 0; j < 200; j++) {
                mapData.Append(mMapDatas[i, j] + ";");
            }
        }
        //写文件 文件名为save.text
        //这里的FileMode.create是创建这个文件,如果文件名存在则覆盖重新创建
        FileStream fs = new FileStream(Application.dataPath + "/mapdata.txt", FileMode.Create);
        //存储时时二进制,所以这里需要把我们的字符串转成二进制
        byte[] bytes = new UTF8Encoding().GetBytes(mapData.ToString());
        fs.Write(bytes, 0, bytes.Length);
        //每次读取文件后都要记得关闭文件
        fs.Close();
    }
}

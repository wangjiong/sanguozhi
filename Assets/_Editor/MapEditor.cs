using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BitBenderGames;
using System;

public class MapEditor : MonoBehaviour {
    public GameObject mTerrainTypePrefab;

    // 鼠标指示点
    GameObject mPointerIndicator;
    Vector3 mOriginalPosition;
    List<GameObject> mPointerIndicatorList = new List<GameObject>();

    string[] mTerrainTypeNames = { "无效地形" , "草地", "土", "沙地", "湿地", "毒泉", "森", "川", "河",
        "海", "荒地", "主径", "栈道", "渡所", "浅滩", "岸", "涯", "都市", "关所", "港", "小径"};
    List<string> mTerrainTypeName = new List<string>();
    List<Color> mTerrainTypColor = new List<Color>();
    GameObject mTerrain;

    // 1.地形类型
    List<Toggle> mTerrainTypeToggles = new List<Toggle>();
    int mTerrainTypeIndex = 0;
    // 2.显示地形
    List<Toggle> mShowTerrainTypeToggles = new List<Toggle>();
    Dictionary<Coordinates, GameObject> mShowTerrainTypeDictionary = new Dictionary<Coordinates, GameObject>(); // 存储相应位置的地形类型的显示的物体

    // 3.地形大小
    Slider mSlider;
    int mSliderValue = 0;

    // 地图
    int[,] mMapDatas;
    // 相机
    MobileTouchCamera mMobileTouchCamera;

    void Start() {
        // 鼠标指示点
        mPointerIndicator = GameObject.Find("Point");
        mOriginalPosition = mPointerIndicator.transform.position;
        mTerrain = GameObject.Find("Terrain");
        // 枚举
        foreach (TerrainType item in Enum.GetValues(typeof(TerrainType))) {
            mTerrainTypeName.Add(item.ToString());
        }
        // 颜色
        System.Random random = new System.Random(9527);
        for (int i = 0; i < mTerrainTypeName.Count; i++) {
            float r = random.Next(0, 255) / 255f;
            float g = random.Next(0, 255) / 255f;
            float b = random.Next(0, 255) / 255f;
            Color color = new Color(r, g, b);
            mTerrainTypColor.Add(color);
        }

        // 1.地形类型
        GameObject terrainTypes = GameObject.Find("TerrainTypes");
        for (int i = 0; i < mTerrainTypeNames.Length; i++) {
            GameObject item = Instantiate(mTerrainTypePrefab);
            item.name = mTerrainTypeName[i];
            // 名字
            item.GetComponentInChildren<Text>().text = mTerrainTypeNames[i];
            // 颜色
            item.transform.Find("Color").GetComponent<Image>().color = mTerrainTypColor[i];
            item.transform.SetParent(terrainTypes.transform);

            // 1-1.改变地形
            Toggle changeTerrainTypeToggle = item.transform.Find("ChangeTerrainType").GetComponent<Toggle>();
            int index = i;
            changeTerrainTypeToggle.onValueChanged.AddListener(delegate (bool isOn) { ChangeTerrainTypeTogglsEvent(isOn, index); });
            mTerrainTypeToggles.Add(changeTerrainTypeToggle);
            // 2.显示地形
            Toggle showTerrainTypeToggle = item.transform.Find("ShowTerrainType").GetComponent<Toggle>();
            showTerrainTypeToggle.onValueChanged.AddListener(delegate (bool isOn) { ShowTerrainTypeTogglsEvent(isOn, index); });
            mShowTerrainTypeToggles.Add(showTerrainTypeToggle);
        }
        mTerrainTypeToggles[mTerrainTypeIndex].isOn = true;
        // 3.地形大小
        mSlider = GameObject.Find("Slider").GetComponent<Slider>();
        mSlider.onValueChanged.AddListener(delegate (float value) { ChangeSizeSliderEvent(value); });
        // 加载地图文件
        Load();
        // 相机
        mMobileTouchCamera = GameObject.Find("Main Camera").GetComponent<MobileTouchCamera>();
    }

    // 1.改变地形
    void ChangeTerrainTypeTogglsEvent(bool isOn, int terrainTypeIndex) {
        if (isOn) {
            mTerrainTypeIndex = terrainTypeIndex;
            for (int i = 0; i < mTerrainTypeToggles.Count; i++) {
                if (i != terrainTypeIndex) {
                    // 改变刷子的地形类型
                    mTerrainTypeToggles[i].isOn = false;
                }
            }
        }
    }

    // 2.显示地形
    void ShowTerrainTypeTogglsEvent(bool isOn, int terrainTypeIndex) {
        if (isOn) {
            mTerrainTypeToggles[terrainTypeIndex].isOn = true;
            if (terrainTypeIndex == (int)TerrainType.TerrainType_Invalid) {
                return;
            }
            // 创建特定地形
            for (int i = 0; i < 200; i++) {
                for (int j = 0; j < 200; j++) {
                    if (mMapDatas[i, j] == terrainTypeIndex) {
                        GameObject g = Instantiate(mPointerIndicator);
                        Coordinates coordinates = new Coordinates(i, j);
                        Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
                        p.y = mOriginalPosition.y - 0.00001f;
                        g.transform.position = p;
                        g.name = mTerrainTypeName[terrainTypeIndex];
                        g.GetComponent<Renderer>().material.color = mTerrainTypColor[terrainTypeIndex];
                        g.transform.SetParent(mTerrain.transform);

                        mShowTerrainTypeDictionary.Add(coordinates, g);
                    }
                }
            }
        } else {
            if (terrainTypeIndex == (int)TerrainType.TerrainType_Invalid) {
                return;
            }
            // 销毁特定地形
            foreach (Transform child in mTerrain.transform) {
                if (child.name.Equals(mTerrainTypeName[terrainTypeIndex])) {
                    Destroy(child.gameObject);
                    Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(child.position);
                    mShowTerrainTypeDictionary.Remove(coordinates);
                }
            }
        }
    }

    // 3.地形大小
    void ChangeSizeSliderEvent(float value) {
        if (mSliderValue == (int)value) {
            return;
        }
        mSliderValue = (int)value;
        // 1.先销毁之前的Pointer
        foreach (GameObject g in mPointerIndicatorList) {
            Destroy(g);
        }
        mPointerIndicatorList.Clear();
        // 2.再创建现在的Pointer
        List<Coordinates> list = MapManager.GetInstance().GetAllAroundN(MapManager.GetInstance().TerrainPositionToCorrdinate(mPointerIndicator.transform.position), mSliderValue);
        for (int i = 0; i < list.Count; i++) {
            GameObject pointer = Instantiate(mPointerIndicator);
            pointer.transform.SetParent(mPointerIndicator.transform.parent);
            mPointerIndicatorList.Add(pointer);
        }
        // 刷新刷子中的所有格子的位置
        RefreshAllPointersPosition();
    }

    // 3-1 刷新刷子中的所有格子的位置
    void RefreshAllPointersPosition() {
        List<Coordinates> list = MapManager.GetInstance().GetAllAroundN(MapManager.GetInstance().TerrainPositionToCorrdinate(mPointerIndicator.transform.position), mSliderValue);
        for (int i = 0; i < mPointerIndicatorList.Count; i++) {
            if (MapManager.GetInstance().CheckBoundary(list[i])) {
                Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(list[i]);
                p.y = mPointerIndicator.transform.position.y;
                mPointerIndicatorList[i].SetActive(true);
                mPointerIndicatorList[i].transform.position = p;
            } else {
                mPointerIndicatorList[i].SetActive(false);
            }
        }
    }

    // 处理点击事件
    void Update() {
        // 如果点击UI，那么不移动相机
        if (IsPointerOverGameObject(Input.mousePosition)) {
            if (mMobileTouchCamera.enabled) {
                mMobileTouchCamera.enabled = false;
            }
            return;
        }
        if (!mMobileTouchCamera.enabled) {
            mMobileTouchCamera.enabled = true;
        }

        RaycastHit hit;
        // 从鼠标所在的位置发射
        Vector2 screenPosition = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out hit)) {
            // 鼠标的位置
            Vector3 pointCubePosition = MapManager.GetInstance().TerrainPositionToCenterPosition(hit.point);
            pointCubePosition.y = mOriginalPosition.y;
            mPointerIndicator.transform.position = pointCubePosition;
            // 刷子中的所有格子的位置
            RefreshAllPointersPosition();
            // Debug
            Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(hit.point);
            Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
            p.y = mOriginalPosition.y;

            if (coordinates.x >= 0 && coordinates.x < 200 && coordinates.y >= 0 && coordinates.y < 200) {
                // 右键鼠标刷地图
                if (Input.GetMouseButton(1)) {
                    if (mPointerIndicatorList.Count == 0) {
                        // 检查当前的地形类型是否有显示
                        ChangeShowTerrainType(coordinates, mTerrainTypeIndex);
                    } else {
                        for (int i = 0; i < mPointerIndicatorList.Count; i++) { // 改变所有的地形
                            GameObject g = mPointerIndicatorList[i];
                            Coordinates c = MapManager.GetInstance().TerrainPositionToCorrdinate(g.transform.position);
                            // 检查当前的地形类型是否有显示
                            ChangeShowTerrainType(c, mTerrainTypeIndex);
                        }
                    }
                }

                // 放开地图自动保存地图
                if (Input.GetMouseButtonUp(1)) {
                    Save();
                }
                if (mMapDatas!=null) {
                    int terrainType = mMapDatas[coordinates.x, coordinates.y];
                    GameObject.Find("DebugPosition").GetComponent<Text>().text = coordinates.ToString() + " terrainType:" + mTerrainTypeNames[terrainType];
                }
            }
        }
    }

    // 4.改变地形的显示
    void ChangeShowTerrainType(Coordinates coordinates, int terrainTypeIndex) {
        if (mMapDatas[coordinates.x, coordinates.y] != terrainTypeIndex) {
            mMapDatas[coordinates.x, coordinates.y] = terrainTypeIndex;
            string newName = mTerrainTypeName[mMapDatas[coordinates.x, coordinates.y]];
            if (mShowTerrainTypeDictionary.ContainsKey(coordinates)) {
                // 4-1 如果之前的位置有显示，那么直接改变颜色即可
                if (terrainTypeIndex == (int)TerrainType.TerrainType_Invalid) {
                    // 如果是无效地形，那么直接销毁
                    GameObject g = mShowTerrainTypeDictionary[coordinates];
                    Destroy(g);
                    mShowTerrainTypeDictionary.Remove(coordinates);
                } else {
                    mShowTerrainTypeDictionary[coordinates].name = newName;
                    mShowTerrainTypeDictionary[coordinates].GetComponent<Renderer>().material.color = mTerrainTypColor[terrainTypeIndex];
                }
            } else {
                // 4-2 如果之前的位置没有显示，那么直接
                if (terrainTypeIndex == (int)TerrainType.TerrainType_Invalid) {
                    // 如果是无效地形，那么直接return
                    return;
                } else {
                    GameObject g = Instantiate(mPointerIndicator);
                    Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
                    p.y = mOriginalPosition.y - 0.00001f;
                    g.transform.position = p;
                    g.name = newName;
                    g.GetComponent<Renderer>().material.color = mTerrainTypColor[terrainTypeIndex];
                    g.transform.SetParent(mTerrain.transform);
                    mShowTerrainTypeDictionary.Add(coordinates, g);
                }
            }
        }
    }

    void Load() {
        mMapDatas = MapManager.GetInstance().GetMapDatas();
        Debug.Log("Load");
        FileStream fs = new FileStream(Application.dataPath + "/mapdata.txt", FileMode.Open);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();
        string s = new UTF8Encoding().GetString(bytes);
        string[] itemIds = s.Split(';');
        // 初始化地图
        for (int i = 0; i < 200; i++) {
            for (int j = 0; j < 200; j++) {
                int terrainType = int.Parse(itemIds[i * 200 + j]);
                if (terrainType != (int)TerrainType.TerrainType_Dushi &&
                    terrainType != (int)TerrainType.TerrainType_Guansuo &&
                    terrainType != (int)TerrainType.TerrainType_Gang) {
                    mMapDatas[i, j] = terrainType;
                }
            }
        }
        Save();
    }

    void Save() {
        Debug.Log("Save " + mTerrainTypeNames[mTerrainTypeIndex]);
        StringBuilder mapData = new StringBuilder();
        for (int i = 0; i < 200; i++) {
            for (int j = 0; j < 200; j++) {
                mapData.Append(mMapDatas[i, j] + ";");
            }
        }
        FileStream fs = new FileStream(Application.dataPath + "/mapdata.txt", FileMode.Create);
        byte[] bytes = new UTF8Encoding().GetBytes(mapData.ToString());
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }

    public bool IsPointerOverGameObject(Vector2 screenPosition) {
        //实例化点击事件  
        PointerEventData eventDataCurrentPosition = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //将点击位置的屏幕坐标赋值给点击事件  
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        //向点击处发射射线  
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}

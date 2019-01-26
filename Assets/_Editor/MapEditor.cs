using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BitBenderGames;

public class MapEditor : MonoBehaviour {
    public GameObject mTerrainTypePrefab;

    // 鼠标指示点
    GameObject mPointerIndicator;
    Vector3 mOriginalPosition;
    List<GameObject> mPointerIndicatorList = new List<GameObject>();

    // 地形类型
    string[] mTerrainTypeNames = { "草地", "土", "沙地", "湿地", "毒泉", "森", "川", "河",
        "海", "荒地", "主径", "栈道", "渡所", "浅滩", "岸", "涯", "都市", "关所", "港", "小径"};
    List<Toggle> mTerrainTypeToggles = new List<Toggle>();
    int mTerrainTypeIndex = 0;
    // 地形大小
    Slider mSlider;
    int mSliderValue = 0;

    // 地图
    int[,] mMapDatas = new int[200, 200];
    // 相机
    MobileTouchCamera mMobileTouchCamera;

    void Start() {
        // 鼠标指示点
        mPointerIndicator = GameObject.Find("Point");
        mOriginalPosition = mPointerIndicator.transform.position;
        // 地形类型
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
        mTerrainTypeToggles[mTerrainTypeIndex].isOn = true;
        // 地形大小
        mSlider = GameObject.Find("Slider").GetComponent<Slider>();
        mSlider.onValueChanged.AddListener(delegate (float value) { SliderEvent(value); });
        // 加载地图文件
        Load();
        // 相机
        mMobileTouchCamera = GameObject.Find("Main Camera").GetComponent<MobileTouchCamera>();

    }

    void ToggleEvent(bool isOn, int index) {
        if (isOn) {
            mTerrainTypeIndex = index;
            for (int i = 0; i < mTerrainTypeToggles.Count; i++) {
                if (i != index) {
                    mTerrainTypeToggles[i].isOn = false;
                }
            }
        }
    }

    void SliderEvent(float value) {
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
        RefreshPointers();
    }

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
            RefreshPointers();
            // Debug
            Coordinates coordinates = MapManager.GetInstance().TerrainPositionToCorrdinate(hit.point);
            Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(coordinates);
            p.y = mOriginalPosition.y;

            if (coordinates.x >= 0 && coordinates.x < 200 && coordinates.y >= 0 && coordinates.y < 200) {
                // 按住鼠标刷地图
                if (Input.GetMouseButton(1)) {
                    mMapDatas[coordinates.x, coordinates.y] = mTerrainTypeIndex;
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

    void RefreshPointers() {
        // 刷子中的所有格子的位置
        List<Coordinates> list = MapManager.GetInstance().GetAllAroundN(MapManager.GetInstance().TerrainPositionToCorrdinate(mPointerIndicator.transform.position), mSliderValue);
        for (int i=0;i < mPointerIndicatorList.Count;i++) {
            if (MapManager.GetInstance().CheckBoundary(list[i])) {
                Vector3 p = MapManager.GetInstance().CorrdinateToTerrainPosition(list[i]);
                p.y = mPointerIndicator.transform.position.y;
                mPointerIndicatorList[i].SetActive(true);
                mPointerIndicatorList[i].transform.position = p;
            }else {
                mPointerIndicatorList[i].SetActive(false);
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
        Debug.Log(itemIds[itemIds.Length - 1]);
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

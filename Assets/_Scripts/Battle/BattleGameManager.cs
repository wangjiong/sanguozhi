using BitBenderGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleGameManager : MonoBehaviour {
    static string TAG = "GameManager==";

    static string WUJIANG_TRANSPARENT = "WujiangData/WujiangTransparent";

    static BattleGameManager msBattleGameManager = null;

    public static bool msIgnoreRaycast = false;

    public CanvasGameMenu mCanvasGameMenu;
    public CanvasBattleMenu mCanvasBattleMenu;
    public MobileTouchCamera mMobileTouchCamera;

    GameObject mWujiangTransparent;

    GameObject mPointCube;
    Vector3 mOriginalPosition;

    private void Awake() {
        msBattleGameManager = this;
    }

    public static BattleGameManager GetInstance() {
        return msBattleGameManager;
    }

    // 1.加载的城池数据
    private CityData mCityData;
    // 2.加载的武将数据
    private WujiangData mWujiangData;


    void Start() {
        // 加载数据
        LoadData();

        // 初始化
        mPointCube = GameObject.Find("Point");
        mOriginalPosition = mPointCube.transform.position;

        mWujiangTransparent = Instantiate(Resources.Load(WUJIANG_TRANSPARENT)) as GameObject;
        mWujiangTransparent.SetActive(false);
    }

    void Update() {
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
            // 1.点击格子，主要用于行军
            Vector3 pointCubePosition = MapManager.GetInstance().TerrainPositionToCenterPosition(hit.point);
            pointCubePosition.y = mOriginalPosition.y;
            mPointCube.transform.position = pointCubePosition;
            if (Input.GetMouseButtonUp(0)) {
                // 这个主要是因为防止刚刚点击武将，战斗菜单就显示出来了
                if (msIgnoreRaycast) {
                    msIgnoreRaycast = false;
                    return;
                }
                Wujiang currentWujiang = Wujiang.GetCurrentWujiang();
                if (currentWujiang != null) {
                    // 1.当前处于要攻击状态
                    // 这个主要是因为防止刚刚点击武将，战斗菜单就显示出来了
                    if (currentWujiang.GetWujiangState() == WujiangState.WujiangState_Prepare_Attack) {
                        // 释放技能
                        mCanvasBattleMenu.MoveAndStartSkill(MapManager.GetInstance().TerrainPositionToCorrdinate(pointCubePosition));
                        return;
                    }
                    // 2.显示战斗菜单
                    currentWujiang.ShowBattleMeun(new Vector3(mPointCube.transform.position.x, currentWujiang.transform.position.y, mPointCube.transform.position.z));
                    return;
                }
                // 2.点击城市
                if (hit.collider.CompareTag("City")) {
                    if (currentWujiang != null || Pathfinding.GetInstance().IsShowingPath()) {
                        return;
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        GameObject city;
                        if (hit.collider.gameObject.name.Equals("Model")) {
                            // 关隘
                            city = hit.collider.transform.parent.gameObject;
                        } else {
                            // 港口
                            city = hit.collider.transform.gameObject;
                        }
                        // 显示小菜单
                        mCanvasGameMenu.SetCity(city.GetComponent<City>());
                        mCanvasGameMenu.ShowCanvasGameMenu(screenPosition);
                    }
                }
            }
        }
        // 3.点击空白
        if (Input.GetMouseButtonDown(0)) {
            mCanvasGameMenu.gameObject.SetActive(false);
            mCanvasBattleMenu.gameObject.SetActive(false);
        }
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

    void LoadData() {
        // 1.加载地形数据
        MapManager.GetInstance().LoadData();
        // 2.加载城池数据
        mCityData = new CityData();
        mCityData.LoadData();
        // 3.加载城池数据
        mWujiangData = new WujiangData();
        mWujiangData.LoadData();

        // 归属武将
        mCityData.AllocateWujiangData(mWujiangData);
    }

    public CityData GetCityData() {
        return mCityData;
    }

    public WujiangData GetWujiangData() {
        return mWujiangData;
    }

    public CanvasBattleMenu GetCanvasBattleMenu() {
        return mCanvasBattleMenu;
    }

    public GameObject GetWujiangTransparent() {
        return mWujiangTransparent;
    }


}
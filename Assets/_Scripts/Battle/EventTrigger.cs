using BitBenderGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTrigger : MonoBehaviour {
    string TAG = "EventTrigger==";
    GameObject mPointCube;

    public CanvasGameMenu mCanvasGameMenu;

    Vector3 mOriginalPosition;

    MobileTouchCamera mMobileTouchCamera;

    void Start () {
        mPointCube = GameObject.Find("Point");
        mOriginalPosition = mPointCube.transform.position;
        mMobileTouchCamera = GetComponent<MobileTouchCamera>();
    }
	
	void Update () {
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
                Wujiang currentWujiang = Wujiang.GetCurrentWujiang();
                if (currentWujiang != null) {
                    if (currentWujiang.GetWujiangState() == WujiangState.WujiangState_Prepare_Expedition) {
                        // 当前武将处于出征状态
                        currentWujiang.SetPosition(new Vector3(mPointCube.transform.position.x, currentWujiang.transform.position.y, mPointCube.transform.position.z));
                        return;
                    } else {
                        if (currentWujiang.GetWujiangState() != WujiangState.WujiangState_Prepare_Move) {
                            currentWujiang.SetWujiangState(WujiangState.WujiangState_Prepare_Move);
                            currentWujiang.ShowPath();
                        } else {
                            currentWujiang.SetPosition(new Vector3(mPointCube.transform.position.x, currentWujiang.transform.position.y, mPointCube.transform.position.z));
                            return;
                        }
                    }
                }
            }
            // 2.点击城市
            if (hit.collider.CompareTag("City")) {
                
                if (Input.GetMouseButtonUp(0)) {
					GameObject city;
					if (hit.collider.gameObject.name.Equals ("Model")) {
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
        // 3.点击空白
        if (Input.GetMouseButtonDown(0)) {
            mCanvasGameMenu.gameObject.SetActive(false);
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
}

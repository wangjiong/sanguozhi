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
            // 1.点击格子
            Vector3 pointCubePosition = MapManager.GetInstance().TerrainPositionToCenterPosition(hit.point);
            pointCubePosition.y = mOriginalPosition.y;
            mPointCube.transform.position = pointCubePosition;
            if (Input.GetMouseButtonUp(0)) {
                if (Wujiang.msCurrentWujiang != null) {
                    if (!Wujiang.msCurrentWujiang.IsShowPath()) {
                        // 如果没有显示路径
                        Wujiang.msCurrentWujiang.ShowPath();
                    } else {
                        // 如果显示路径
                        Wujiang.msCurrentWujiang.SetPosition(new Vector3(mPointCube.transform.position.x, Wujiang.msCurrentWujiang.transform.position.y, mPointCube.transform.position.z));
                        return;
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

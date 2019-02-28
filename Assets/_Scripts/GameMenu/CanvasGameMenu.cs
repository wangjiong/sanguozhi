using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGameMenu : MonoBehaviour {
    string TAG = "CanvasGameMenu==";

    public GameObject mFirstMenu;

    public GameObject[] mMenuFirstBtns;

    public GameObject mSecondMenu;

    public GameObject[] mMenuSecondBtns;

    public GameObject mCanvasExpedition;

    public GameObject mCanvasSearch;

    public GameObject mCanvasDevelop;

    City mCity; // 属于菜单的城池

    float mFirstHeight = 290;
    float mWidth = 100;
    float mSecondHeight = 145;

    public void SetCity(City city) {
        mCity = city;
    }

    public City GetCity() {
        return mCity;
    }

    public Vector2 FilterFirstScreenPosition(Vector2 screenPosition) {
        if (screenPosition.y < mFirstHeight) {
            screenPosition.y = mFirstHeight;
        }
        if (screenPosition.y > Screen.height) {
            screenPosition.y = Screen.height;
        }
        if (screenPosition.x < 0) {
            screenPosition.x = 0;
        }
        if (screenPosition.x > Screen.width - mWidth) {
            screenPosition.x = Screen.width - mWidth;
        }
        return screenPosition;
    }

    // 显示菜单
    public void ShowCanvasGameMenu(Vector2 screenPosition) {
        gameObject.SetActive(true);
        mFirstMenu.transform.position = FilterFirstScreenPosition(screenPosition);
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }

    void Start() {
        // 第一级菜单监听，显示第二级菜单位置
        for (int i = 0; i < mMenuFirstBtns.Length; i++) {
            int index = i;
            mMenuFirstBtns[index].GetComponent<Button>().onClick.AddListener(delegate () {
                Vector3 scale = mMenuFirstBtns[index].transform.lossyScale;
                Vector2 sizeDelta = mMenuFirstBtns[index].GetComponent<RectTransform>().sizeDelta;
                mSecondMenu.transform.position = mMenuFirstBtns[index].transform.position + new Vector3(sizeDelta.x * scale.x / 2, sizeDelta.y * scale.y / 2, 0);
            });
        }
        // 第二级菜单监听
        for (int i = 0; i < mMenuSecondBtns.Length; i++) {
            int index = i;
            mMenuSecondBtns[i].GetComponent<Button>().onClick.AddListener(delegate () {
                if (index == 0) {
                    // 开发界面
                    mCanvasDevelop.SetActive(true);
                } else if (index == 1) {
                    // 出征界面
                    mCanvasExpedition.SetActive(true);
                    mCanvasExpedition.GetComponent<CanvasExpedition>().SetCity(mCity);
                } else if (index == 2) {
                    // 搜索界面
                    mCanvasSearch.SetActive(true);
                }
            });
        }

        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }

    void OnDisable() {
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }
}
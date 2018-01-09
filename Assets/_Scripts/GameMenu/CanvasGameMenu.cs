using System.Collections;
using System.Collections.Generic;
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

    bool mPositionFlag = true; // 第一次ShowPosition有问题

    

    public void ShowCanvasGameMenu(Vector2 screenPosition) {
        this.gameObject.SetActive(true);
        if (mPositionFlag) {
            StartCoroutine(SetPosition(screenPosition));
            mPositionFlag = false;
        } else {
            mFirstMenu.transform.position = screenPosition;
            mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        }
    }

    public void SetCityName(string cityName) {
        print(TAG + "SetCityName cityName:" + cityName);
        GameManager.sCityName = cityName;
        GameManager.SetCurrentGenerals();
    }

    IEnumerator SetPosition(Vector2 screenPosition) {
        yield return null;
        mFirstMenu.transform.position = screenPosition;
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
                    mCanvasDevelop.SetActive(true);
                } else if (index == 1) {
                    mCanvasExpedition.SetActive(true);
                } else if (index == 2) {
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
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

    bool mPositionFlag = true; // 第一次ShowPosition有问题

    public void ShowCanvasGameMenu(Vector2 screenPosition) {
        gameObject.SetActive(true);
        if (mPositionFlag) {
            StartCoroutine(SetPosition(screenPosition));
            mPositionFlag = false;
        } else {
            mFirstMenu.transform.position = screenPosition;
            mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        }
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
                    // 开发界面
                    mCanvasDevelop.SetActive(true);
                } else if (index == 1) {
                    // 出征界面
                    mCanvasExpedition.SetActive(true);
                    mCanvasExpedition.GetComponent<CanvasExpedition>().SetCity(BattleGameManager.GetInstance().GetCurrentCity());
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
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBattleMenu : MonoBehaviour {
    string TAG = "CanvasBattleMenu==";

    public GameObject mFirstMenu;
    public GameObject[] mMenuFirstBtns;

    public GameObject mSecondMenu;
    public GameObject[] mMenuSecondBtns;

    bool mPositionFlag = true; // 第一次ShowPosition有问题

    Wujiang mWujiang;

    void Start() {
        // 第一级菜单监听，显示第二级菜单位置
        for (int i = 0; i < mMenuFirstBtns.Length; i++) {
            int index = i;
            mMenuFirstBtns[index].GetComponent<Button>().onClick.AddListener(delegate () {
                if (index == 0) {
                    // 隐藏战斗菜单
                    gameObject.SetActive(false);
                    // 待机（目前为移动）
                    mWujiang.Move(BattleGameManager.GetInstance().GetWujiangTransparent().transform.position);
                    mWujiang.SetWujiangState(WujiangState.WujiangState_Battle);
                    mWujiang.Seclet(false);
                } else if (index == 3) {
                    Vector3 scale = mMenuFirstBtns[index].transform.lossyScale;
                    Vector2 sizeDelta = mMenuFirstBtns[index].GetComponent<RectTransform>().sizeDelta;
                    mSecondMenu.transform.position = mMenuFirstBtns[index].transform.position + new Vector3(sizeDelta.x * scale.x / 2, sizeDelta.y * scale.y / 2, 0);
                } else {
                    mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
                }
            });

        }
        // 第二级菜单监听
        for (int i = 0; i < mMenuSecondBtns.Length; i++) {
            int index = i;
            mMenuSecondBtns[i].GetComponent<Button>().onClick.AddListener(delegate () {
                if (index == 0) {
                } else if (index == 1) {
                } else if (index == 2) {
                }
            });
        }
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
    }

    void OnDisable() {
        mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        BattleGameManager.GetInstance().GetWujiangTransparent().SetActive(false);
    }

    public void ShowMenu(Vector2 screenPosition) {
        gameObject.SetActive(true);
        if (mPositionFlag) {
            StartCoroutine(SetPosition(screenPosition));
            mPositionFlag = false;
        } else {
            mFirstMenu.transform.position = screenPosition;
            mSecondMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 1000);
        }
    }

    public void SetWujiang(Wujiang wujiang) {
        mWujiang = wujiang;
    }

    IEnumerator SetPosition(Vector2 screenPosition) {
        yield return null;
        mFirstMenu.transform.position = screenPosition;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OneP.InfinityScrollView;

public class CanvasExecutive : MonoBehaviour {
    static string TAG = "CanvasExecutive==";
    public GameObject mEnableBtn;
    public GameObject mCloseBtn;
    public InfinityScrollView mInfinityScrollView;

    public CanvasExpedition mCanvasExpedition;

    public Button[] mButtons;

    int mIndexClick = 0;

    void Start() {
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            // 出征武将
            List<General> temp = Strategy.GetExpeditionGenerals(GameManager.sCurrentGenerals);
            mCanvasExpedition.SetGeneral(temp[0], temp[1], temp[2]);
            this.gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        // 武将表格
        for (int i = 1; i < mButtons.Length; i++) {
            int index = i;
            mButtons[i].onClick.AddListener(delegate () {
                if (mIndexClick == index) {
                    mIndexClick = 0;
                    Sort(index, true);
                } else {
                    mIndexClick = index;
                    Sort(index, false);
                }
                mInfinityScrollView.Setup(GameManager.sCurrentGenerals.Count);
                mInfinityScrollView.InternalReload();
            });
        }
    }

    private void Sort(int index, bool revert) {
        if (revert) {
            switch (index) {
                case 1:
                    GameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从大到小排序
                    break;
                case 2:
                    GameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从大到小排序
                    break;
                case 3:
                    GameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从大到小排序
                    break;
                case 4:
                    GameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从大到小排序
                    break;
                case 5:
                    GameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.meili) - int.Parse(a.meili)); // 从大到小排序
                    break;
            }
        } else {
            switch (index) {
                case 1:
                    GameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从大到小排序
                    break;
                case 2:
                    GameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从大到小排序
                    break;
                case 3:
                    GameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从大到小排序
                    break;
                case 4:
                    GameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从大到小排序
                    break;
                case 5:
                    GameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.meili) - int.Parse(a.meili)); // 从大到小排序
                    break;
            }
        }
    }

    public void Show() {
        print(TAG + "Show:" + GameManager.sCurrentGenerals.Count);
        if (GameManager.sCurrentGenerals.Count > 0) {
            mInfinityScrollView.Setup(GameManager.sCurrentGenerals.Count);
            mInfinityScrollView.InternalReload();
            gameObject.SetActive(true);
        }
    }
}
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

    int mSelectTotalCount = 3;
    int mCurrentSelectCount = 0;
    HashSet<int> mSelectItems = new HashSet<int>();

    void Start() {
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            // 出征武将
            List<General> temp = Strategy.GetExpeditionGenerals(GameManager.sCurrentGenerals);
            mCanvasExpedition.SetGeneral(temp[0], temp[1], temp[2]);
            gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            gameObject.SetActive(false);
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
        if (GameManager.sCurrentGenerals.Count > 0) {
            gameObject.SetActive(true);

            mInfinityScrollView.Setup(GameManager.sCurrentGenerals.Count);
            mInfinityScrollView.InternalReload();
        }
    }

    public bool CanSelect() {
        if (mCurrentSelectCount >= mSelectTotalCount) {
            return false;
        }
        return true;
    }

    public bool CantainIndex(int index) {
        if (mSelectItems.Contains(index)) {
            return true;
        }
        return false;
    }

    public void SelectItem(bool isOn, int index) {
        if (isOn) {
            mCurrentSelectCount++;
            mSelectItems.Add(index);
        } else {
            mCurrentSelectCount--;
            mSelectItems.Remove(index);
        }
    }
}
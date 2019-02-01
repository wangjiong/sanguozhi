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
    HashSet<int> mSelectItems = new HashSet<int>();

    void Start() {
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            // 出征武将
            //List<General> temp = Strategy.GetExpeditionGenerals(GameManager.sCurrentGenerals);
            //mCanvasExpedition.SetGeneral(temp[0], temp[1], temp[2]);
            General[] wujiangs = new General[3];
            int i = 0;
            foreach (int index in mSelectItems) {
                if (i < 3) {
                    wujiangs[i++] = BattleGameManager.sCurrentGenerals[index];
                }
            }
            mCanvasExpedition.SetGeneral(wujiangs[0], wujiangs[1], wujiangs[2]);
            mSelectItems.Clear();
            gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            mSelectItems.Clear();
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
                mInfinityScrollView.Setup(BattleGameManager.sCurrentGenerals.Count);
                mInfinityScrollView.InternalReload();
            });
        }
    }

    private void Sort(int index, bool revert) {
        if (revert) {
            switch (index) {
                case 1:
                    BattleGameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从大到小排序
                    break;
                case 2:
                    BattleGameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从大到小排序
                    break;
                case 3:
                    BattleGameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从大到小排序
                    break;
                case 4:
                    BattleGameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从大到小排序
                    break;
                case 5:
                    BattleGameManager.sCurrentGenerals.Sort((b, a) => int.Parse(b.meili) - int.Parse(a.meili)); // 从大到小排序
                    break;
            }
        } else {
            switch (index) {
                case 1:
                    BattleGameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从小到小排序
                    break;
                case 2:
                    BattleGameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从小到小排序
                    break;
                case 3:
                    BattleGameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从小到小排序
                    break;
                case 4:
                    BattleGameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从小到小排序
                    break;
                case 5:
                    BattleGameManager.sCurrentGenerals.Sort((a, b) => int.Parse(b.meili) - int.Parse(a.meili)); // 从小到小排序
                    break;
            }
        }
    }

    public void Show() {
        if (BattleGameManager.sCurrentGenerals.Count > 0) {
            gameObject.SetActive(true);

            mInfinityScrollView.Setup(BattleGameManager.sCurrentGenerals.Count);
            mInfinityScrollView.InternalReload();
        }
    }

    public bool CanSelect() {
        if (mSelectItems.Count >= mSelectTotalCount) {
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
            mSelectItems.Add(index);
        } else {
            mSelectItems.Remove(index);
        }
    }
}
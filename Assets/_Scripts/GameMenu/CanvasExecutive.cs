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
    HashSet<int> mSelectWujiangIds = new HashSet<int>();

    City mCity;

    void Start() {
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            // 出征武将
			WujiangBean[] wujiangs = new WujiangBean[3];
            int i = 0;
            foreach (int index in mSelectItems) {
                if (i < 3) {
                    wujiangs[i++] = mCity.GetWujiangBeans()[index];
                }
            }
            mCanvasExpedition.SetGeneral(wujiangs[0], wujiangs[1], wujiangs[2]);
            mSelectItems.Clear();
            mSelectWujiangIds.Clear();
            gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            mSelectItems.Clear();
            mSelectWujiangIds.Clear();
            gameObject.SetActive(false);
        });
        // 武将表格
        for (int i = 1; i < mButtons.Length; i++) {
            int index = i;
            mButtons[i].onClick.AddListener(delegate () {
                if (mIndexClick == index) {
                    // 反复点击重排
                    mIndexClick = 0;
                    Sort(index, true);
                } else {
                    // 点击其他的
                    mIndexClick = index;
                    Sort(index, false);
                }
                mInfinityScrollView.Setup(mCity.GetWujiangBeans().Count);
                mInfinityScrollView.InternalReload();
            });
        }
    }

    private void Sort(int index, bool revert) {
        List<WujiangBean> currentCityWujiangs = mCity.GetWujiangBeans();
        if (revert) {
            switch (index) {
                case 1:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从大到小排序
                    break;
                case 2:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从大到小排序
                    break;
                case 3:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从大到小排序
                    break;
                case 4:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从大到小排序
                    break;
                case 5:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.meili) - int.Parse(a.meili)); // 从大到小排序
                    break;
            }
        } else {
            switch (index) {
                case 1:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从小到小排序
                    break;
                case 2:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从小到小排序
                    break;
                case 3:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从小到小排序
                    break;
                case 4:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从小到小排序
                    break;
                case 5:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.meili) - int.Parse(a.meili)); // 从小到小排序
                    break;
            }
        }
    }

    public void Show() {
        gameObject.SetActive(true);

        mInfinityScrollView.Setup(mCity.GetWujiangBeans().Count);
        mInfinityScrollView.InternalReload();
    }

    public void SetCity(City city) {
        mCity = city;
    }

    public City GetCity() {
        return mCity;
    }

    public bool CanSelect() {
        if (mSelectItems.Count >= mSelectTotalCount) {
            return false;
        }
        return true;
    }

    public bool CantainWujiangId(int wujiangId) {
        if (mSelectWujiangIds.Contains(wujiangId)) {
            return true;
        }
        return false;
    }

    public void SelectItem(bool isOn, int index) {
        WujiangBean general = mCity.GetWujiangBeans()[index];
        if (isOn) {
            mSelectItems.Add(index);
            mSelectWujiangIds.Add(general.id);
        } else {
            mSelectItems.Remove(index);
            mSelectWujiangIds.Remove(general.id);
        }
    }
}
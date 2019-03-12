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
    HashSet<int> mSelectWujiangIds = new HashSet<int>();

    City mCity;

    void OnEnable() {
        //Debug.Log(TAG + "OnEnable");
    }

    void Start() {
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            // 出征武将
            WujiangBean[] wujiangs = new WujiangBean[3];
            int i = 0;
            List<WujiangBean> currentCityWujiangs = mCity.GetWujiangBeans();
            foreach (int wujiangId in mSelectWujiangIds) {
                if (i < 3) {
                    foreach (WujiangBean wujiangBean in currentCityWujiangs) {
                        if (wujiangBean.id == wujiangId) {
                            wujiangs[i++] = wujiangBean;
                        }
                    }
                } else {
                    break;
                }
            }
            mCanvasExpedition.SetGeneral(wujiangs);
            this.gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        // 武将表格(统帅、武力、智力、政治、魅力)
        for (int i = 0; i < mButtons.Length; i++) {
            if ( i==1 ) { // 1为武将，0为选中状态，2、3、4、5、6 ： 统帅、武力、智力、政治、魅力
                continue;
            }
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
                // List列表
                mInfinityScrollView.Setup(mCity.GetWujiangBeans().Count);
                mInfinityScrollView.InternalReload();
            });
        }
    }

    private void Sort(int index, bool revert) {
        List<WujiangBean> currentCityWujiangs = mCity.GetWujiangBeans();
        if (revert) {
            switch (index) {
                case 0:
                    // 选中
                    //int first = 0;
                    //int count = mSelectWujiangIds.Count;
                    //for (int i = 0; i < currentCityWujiangs.Count; i++) {
                    //    if (mSelectWujiangIds.Contains(currentCityWujiangs[i].id)) {
                    //        WujiangBean temp = currentCityWujiangs[first];
                    //        currentCityWujiangs[first] = currentCityWujiangs[i];
                    //        currentCityWujiangs[i] = temp;
                    //        first++;
                    //    }
                    //}
                    break;
                case 2:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 从大到小排序
                    break;
                case 3:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从大到小排序
                    break;
                case 4:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从大到小排序
                    break;
                case 5:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从大到小排序
                    break;
                case 6:
                    currentCityWujiangs.Sort((b, a) => int.Parse(b.meili) - int.Parse(a.meili)); // 从大到小排序
                    break;
            }
        } else {
            switch (index) {
                case 0:
                    // 选中
                    //int first = 0;
                    //int count = mSelectWujiangIds.Count;
                    //for (int i = 0; i < currentCityWujiangs.Count; i++) {
                    //    if (mSelectWujiangIds.Contains(currentCityWujiangs[i].id)) {
                    //        WujiangBean temp = currentCityWujiangs[first++];
                    //        currentCityWujiangs[first++] = currentCityWujiangs[i];
                    //        currentCityWujiangs[i] = temp;
                    //    }
                    //}
                    break;
                case 2:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.tongshuai) - int.Parse(a.tongshuai)); // 选中
                    break;
                case 3:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.wuli) - int.Parse(a.wuli)); // 从小到小排序
                    break;
                case 4:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.zhili) - int.Parse(a.zhili)); // 从小到小排序
                    break;
                case 5:
                    currentCityWujiangs.Sort((a, b) => int.Parse(b.zhengzhi) - int.Parse(a.zhengzhi)); // 从小到小排序
                    break;
                case 6:
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
        if (mSelectWujiangIds.Count >= mSelectTotalCount) {
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
            mSelectWujiangIds.Add(general.id);
        } else {
            mSelectWujiangIds.Remove(general.id);
        }
    }

    public void ClearSelectWujiang() {
        mSelectWujiangIds.Clear();
    }

    public void SetSelectGeneral(WujiangBean[] wujiangs) {
        ClearSelectWujiang();
        foreach (WujiangBean wujiangBean in wujiangs) {
            if (wujiangBean != null) {
                mSelectWujiangIds.Add(wujiangBean.id);
            }
        }
    }
}
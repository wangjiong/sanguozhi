using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneP.InfinityScrollView;

public class ItemHorizontal3 : InfinityBaseItem {
    static string TAG = "ItemHorizontal3==";

    CanvasExecutive mCanvasExecutive;

    Button mSelect;
    GameObject mSelectImage;
    Text mWujiang;
    Text mTongshuai;
    Text mWuli;
    Text mZhili;
    Text mZhengzhi;
    Text mMeili;

    int mIndex;

    bool mIsSeclet = false;

    void OnEnable() {
        if (!mCanvasExecutive) {
            mCanvasExecutive = GameObject.Find("Canvas General").GetComponent<CanvasExecutive>();

            mSelect = transform.Find("Select").GetComponent<Button>();
            mSelectImage = mSelect.transform.Find("SelectImage").gameObject;

            mWujiang = transform.Find("Wujiang").GetComponentInChildren<Text>();
            mTongshuai = transform.Find("Tongshuai").GetComponentInChildren<Text>();
            mWuli = transform.Find("Wuli").GetComponentInChildren<Text>();
            mZhili = transform.Find("Zhili").GetComponentInChildren<Text>();
            mZhengzhi = transform.Find("Zhengzhi").GetComponentInChildren<Text>();
            mMeili = transform.Find("Meili").GetComponentInChildren<Text>();

            mSelect.onClick.AddListener(delegate () {
                if (mIsSeclet) {
                    // 之前已经选中那么现在不选中了
                    mIsSeclet = false;
                } else {
                    // 之前没有选中那么现在选中
                    if (mCanvasExecutive.CanSelect()) {
                        mIsSeclet = true;
                    }else {
                        return;
                    }
                }
                mSelectImage.SetActive(mIsSeclet);
                mCanvasExecutive.SelectItem(mIsSeclet, mIndex);
            });
        }
    }

    public override void Reload(InfinityScrollView _infinity, int _index) {
        base.Reload(_infinity, _index);
        mIndex = _index;

        General general = BattleGameManager.sCurrentGenerals[_index];
        if (mCanvasExecutive.CantainIndex(_index)) {
            mIsSeclet = true;
            mSelectImage.SetActive(true);
        }else {
            mIsSeclet = false;
            mSelectImage.SetActive(false);
        }
        mWujiang.text = general.name;
        mTongshuai.text = general.tongshuai;
        mWuli.text = general.wuli;
        mZhili.text = general.zhili;
        mZhengzhi.text = general.zhengzhi;
        mMeili.text = general.meili;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasExpedition : MonoBehaviour {
    static string TAG = "CanvasExpedition==";
    public GameObject mExecutiveBtn;
    public GameObject mEnableBtn;
    public GameObject mCloseBtn;
    public GameObject mCanvasExecutive;

    public Button[] mWeapons;
    public Sprite[] mSprite01; // 普通
    public Sprite[] mSprite02; // 高亮
    public int mWeaponIndex = 0;

    void Start() {
        mExecutiveBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            mCanvasExecutive.GetComponent<CanvasExecutive>().Show();
        });
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        // 默认第一个亮
        mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
        for (int i = 0; i < mWeapons.Length; i++) {
            int index = i;
            mWeapons[index].onClick.AddListener(delegate () {
                if (mWeaponIndex == index) {
                    return;
                }else {
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite01[mWeaponIndex];
                    mWeaponIndex = index;
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
                };
            });
        }
    }

}

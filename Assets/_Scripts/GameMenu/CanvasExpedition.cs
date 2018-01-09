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

    // Arms 兵种
    public Button[] mArms;

    // Image
    public Image mChiefImg;
    public Image mJunior01Img;
    public Image mJunior02Img;

    public Text[] mNamesAndSkillTexts;

    // Weapon
    public Button[] mWeapons;
    public Sprite[] mSprite01; // 普通
    public Sprite[] mSprite02; // 高亮
    public int mWeaponIndex = 0;

    void OnEnable() {
        mChiefImg.color = new Color(1, 1, 1, 0);
        mJunior01Img.color = new Color(1, 1, 1, 0);
        mJunior02Img.color = new Color(1, 1, 1, 0);
        foreach (Text text in mNamesAndSkillTexts) {
            text.text = "";
        }
    }

    void Start() {
        // 兵种
        for (int i = 0; i < mArms.Length; i++) {
            int index = i;
            mArms[index].onClick.AddListener(delegate () {
                List<General> temp = Strategy.GetExpeditionGenerals(GameManager.sCurrentGenerals);
                SetGeneral(temp[0], temp[1], temp[2]);

                mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite01[mWeaponIndex];
                mWeaponIndex = index;
                mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
            });
        }
        // 显示武将界面
        mExecutiveBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            mCanvasExecutive.GetComponent<CanvasExecutive>().Show();
        });
        // 确定、返回
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
                } else {
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite01[mWeaponIndex];
                    mWeaponIndex = index;
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
                };
            });
        }
    }

    // 设置武将信息
    public void SetGeneral(General chief, General junior01, General junior02) {
        SetNameAndSkill(chief, junior01, junior02);
        SetImage(chief, junior01, junior02);
    }

    // 设置名字和特技
    void SetNameAndSkill(General chief, General junior01, General junior02) {
        if (chief != null) {
            mNamesAndSkillTexts[0].text = chief.name;
            mNamesAndSkillTexts[1].text = chief.teji;
        }
        if (junior01 != null) {
            mNamesAndSkillTexts[2].text = junior01.name;
            mNamesAndSkillTexts[3].text = junior01.teji;
        }
        if (junior02 != null) {
            mNamesAndSkillTexts[4].text = junior02.name;
            mNamesAndSkillTexts[5].text = junior02.teji;
        }
    }

    // 设置图片
    void SetImage(General chief, General junior01, General junior02) {
        string PATH = Application.streamingAssetsPath + "/img/img";
        if (chief != null) {
            string url = PATH + chief.id + ".jpg";
            StartCoroutine(LoadImage(url, mChiefImg));
        }
        if (junior01 != null) {
            string url = PATH + junior01.id + ".jpg";
            StartCoroutine(LoadImage(url, mJunior01Img));
        }
        if (junior02 != null) {
            string url = PATH + junior02.id + ".jpg";
            StartCoroutine(LoadImage(url, mJunior02Img));
        }
    }

    IEnumerator LoadImage(string url, Image image) {
        print(TAG + "url:" + url);
        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error) || !www.isDone) {
            // 1 www下载失败
            Debug.Log(TAG + "DownloadImage www.error:" + www.error + " www.isDone:" + www.isDone);
        } else {
            Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
            image.sprite = sprite;
            image.color = new Color(1, 1, 1, 1);
        }
    }
}

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

    // 设置武将信息
	WujiangBean mChief, mJunior01, mJunior02;

    public GameObject mWujiangPrefab;

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

    // Slider
    public int mCurrentSoldier;
    public Slider[] mSlider;
    public Text[] mSliderText;
    public Text mSliderDayText;

    public Text[] mTotalText;
    public Text[] mWeaponText;

    // Ability
    public Text[] mAbilityText;
    public Text mArmText;
    public Text mArmAbilityText;
    int mAttack; // 攻击力
    int mDefence; // 防御力
    int mIntelligence; // 智力
    int mBuild; // 建设力
    int mMove; // 移动能力
    // 战法
    public Text[] mSkillText;
    // 能力5边型
    public AbilityMesh mAbilityMesh;

    // 城池
    City mCity;

    void OnEnable() {
        mChiefImg.color = new Color(1, 1, 1, 0);
        mJunior01Img.color = new Color(1, 1, 1, 0);
        mJunior02Img.color = new Color(1, 1, 1, 0);
        foreach (Text text in mNamesAndSkillTexts) {
            text.text = "";
        }
        // Slider
        foreach (Slider slider in mSlider) {
            slider.value = 0.5f;
        }
        for (int index = 0; index < mSliderText.Length; index++) {
            if (index == 0) {
                int max = 20000;
                mSliderText[index].text = (int)(max * mSlider[index].value) + "/" + max;
            } else if (index == 1) {
                int max = 10000;
                mSliderText[index].text = (int)(max * mSlider[index].value) + "/" + max;
            } else if (index == 2) {
                int max = 50000;
                mSliderText[index].text = (int)(max * mSlider[index].value) + "/" + max;
            }
        }
        mSliderDayText.text = "0日";
        foreach (Text text in mTotalText) {
            text.text = "1000000";
        }
        foreach (Text text in mTotalText) {
            text.text = "1000000";
        }
        // Ability
        foreach (Text text in mAbilityText) {
            text.text = "";
        }
        mArmText.text = "";
        mArmAbilityText.text = "";
        // 战法
        foreach (Text text in mSkillText) {
            text.text = "";
        }
        mChief = null;
        // 设置能力五边形
        // 攻击力
        mAttack = 0;
        // 防御力
        mDefence = 0;
        // 智力
        mIntelligence = 0;
        // 建设力
        mBuild = 0;
        // 移动能力
        mMove = 0;
        mAbilityMesh.SetPointOffset(mAttack / 120f, mDefence / 120f, mIntelligence / 120f, mBuild / 120f, mMove / 120f);
    }

    void Start() {
        // 兵种
        for (int i = 0; i < mArms.Length; i++) {
            int index = i;
            mArms[index].onClick.AddListener(delegate () {
                mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite01[mWeaponIndex];
                mWeaponText[mWeaponIndex].text = 1000000 + "";
                mWeaponIndex = index;
                mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
                mWeaponText[mWeaponIndex].text = 1000000 - mCurrentSoldier + "";

				List<WujiangBean> temp = Strategy.GetExpeditionGenerals(mCity.GetWujiangBeans());
                SetGeneral(temp[0], temp[1], temp[2]);
            });
        }
        // 显示武将界面
        mExecutiveBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            CanvasExecutive canvasExecutive = mCanvasExecutive.GetComponent<CanvasExecutive>();
            canvasExecutive.SetCity(mCity);
            canvasExecutive.Show();
        });
        // 确定、返回
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
            GameObject canvasGameMenu = GameObject.Find("Canvas GameMenu");
            if (canvasGameMenu!=null) {
                canvasGameMenu.SetActive(false);
            }
            if (mChief != null) {
                // 出兵
                GameObject wujiangGameObject = Instantiate(mWujiangPrefab);
                wujiangGameObject.transform.position = new Vector3(mCity.transform.position.x, 0.05f , mCity.transform.position.z);
                Wujiang wujiang = wujiangGameObject.GetComponent<Wujiang>();
                wujiang.mAvatar.sprite = mChiefImg.sprite;
                wujiang.mHealth.text = mSliderText[0].text.Split('/')[0];
                wujiang.mName.text = mChief.name;
                WujiangBean[] wujiangBeans = new WujiangBean[3];
                wujiangBeans[0] = mChief;
                wujiangBeans[1] = mJunior01;
                wujiangBeans[2] = mJunior02;
                wujiang.SetWujiangBeans(wujiangBeans);
                // 显示路径
                Wujiang.SetCurrentWujiang(wujiang);
                wujiang.SetWujiangState(WujiangState.WujiangState_Prepare_Expedition);
                wujiang.SetCity(mCity);
                wujiang.ShowPath();
                // 减少城池里面的武将
                List<WujiangBean> allWujiangs = mCity.GetWujiangBeans();
                for (int i = allWujiangs.Count - 1; i >= 0; i--) {
                    for (int j = 0; j < wujiangBeans.Length; j++) {
                        if (allWujiangs[i] == wujiangBeans[j]) {
                            allWujiangs.Remove(allWujiangs[i]);
                            break;
                        }
                    }
                }
            }
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        // 武器，默认第一个亮
        mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
        for (int i = 0; i < mWeapons.Length; i++) {
            int index = i;
            mWeapons[index].onClick.AddListener(delegate () {
                if (mWeaponIndex == index && mChief == null) {
                    return;
                } else {
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite01[mWeaponIndex];
                    mWeaponText[mWeaponIndex].text = 1000000 + "";
                    mWeaponIndex = index;
                    mWeapons[mWeaponIndex].GetComponent<Image>().sprite = mSprite02[mWeaponIndex];
                    mWeaponText[mWeaponIndex].text = 1000000 - mCurrentSoldier + "";

                    SetAbilitysValue(mChief, mJunior01, mJunior02);
                };
            });
        }
        // 滑动条
        for (int i = 0; i < mSlider.Length; i++) {
            int index = i;
            mSlider[index].onValueChanged.AddListener(delegate (float value) {
                int max = 0;
                int current = 0;
                if (index == 0) {
                    // 士兵
                    max = 20000;
                    mCurrentSoldier = (int)(max * value);
                    mWeaponText[mWeaponIndex].text = 1000000 - mCurrentSoldier + "";
                } else if (index == 1) {
                    max = 10000;
                    int temp = (int)(max * value);
                } else if (index == 2) {
                    max = 50000;
                    mSliderDayText.text = (int)(200 * value) / 10 * 10 + "日";
                }
                current = (int)(max * value);
                mSliderText[index].text = current + "/" + max;
                mTotalText[index].text = 1000000 - current + "";
            });
        }
        // Test
        //string PATH = Application.streamingAssetsPath + "/img/img";
        //string url = PATH + BattleGameManager.msWujiangs[0].id + ".jpg";
        //StartCoroutine(LoadImage(url, null));
    }

    
	public void SetGeneral(WujiangBean chief, WujiangBean junior01, WujiangBean junior02) {
        mChief = chief;
        mJunior01 = junior01;
        mJunior02 = junior02;
        SetNameAndSkill(chief, junior01, junior02);
        SetImage(chief, junior01, junior02);
        SetAbilitysValue(chief, junior01, junior02);
    }

    // 设置攻击防御
	public void SetAbilitysValue(WujiangBean chief, WujiangBean junior01, WujiangBean junior02) {
        if (chief == null) {
            return;
        }
        junior01 = chief;
        junior02 = chief;
        float a1 = 0; // 兵种
        float a2 = 0;
        float a5 = 100;

        float b1 = 0; // 兵种适性
        if (mWeaponIndex == 0) {
            a1 = 1.2f;
            a2 = 1.2f;
            b1 = GetMaxAdaptability(chief.qiangbin, junior01.qiangbin, junior02.qiangbin);
            mArmText.text = "枪兵";
        } else if (mWeaponIndex == 1) {
            a1 = 1.0f;
            a2 = 1.5f;
            b1 = GetMaxAdaptability(chief.jibin, junior01.jibin, junior02.jibin);
            mArmText.text = "戟兵";
        } else if (mWeaponIndex == 2) {
            a1 = 1.0f;
            a2 = 1.0f;
            b1 = GetMaxAdaptability(chief.nubin, junior01.nubin, junior02.nubin);
            mArmText.text = "弩兵";
        } else if (mWeaponIndex == 3) {
            a1 = 1.5f;
            a2 = 1.2f;
            a5 = 120;
            b1 = GetMaxAdaptability(chief.qibin, junior01.qibin, junior02.qibin);
            mArmText.text = "骑兵";
        }
        float c1 = 0.8f; // 偏移值

        // 攻击力
        mAttack = (int)(GetMaxAbility(chief.tongshuai, junior01.tongshuai, junior02.tongshuai) * a1 * b1 * c1);
        // 防御力
        mDefence = (int)(GetMaxAbility(chief.tongshuai, junior01.tongshuai, junior02.tongshuai) * a2 * b1 * c1);
        // 智力
        mIntelligence = GetMaxAbility(chief.zhili, junior01.zhili, junior02.zhili);
        // 建设力
        mBuild = GetMaxAbility(chief.zhengzhi, junior01.zhengzhi, junior02.zhengzhi);
        // 移动能力
        mMove = (int)a5;

        mAbilityText[0].text = mAttack + "";
        mAbilityText[1].text = mDefence + "";
        mAbilityText[2].text = mIntelligence + "";
        mAbilityText[3].text = mBuild + "";
        mAbilityText[4].text = mMove + "";

        // 设置能力五边形
        mAbilityMesh.SetPointOffset(mAttack / 120f, mDefence / 120f, mIntelligence / 120f, mBuild / 120f, mMove / 120f);
    }

    private int GetMaxAbility(string m, string n, string k) {
        int a = int.Parse(m);
        int b = int.Parse(n);
        int c = int.Parse(k);

        int max = a;
        if (max < b) {
            max = b;
        }
        if (max < c) {
            max = c;
        }
        return max;
    }

    private float GetMaxAdaptability(string m, string n, string k) {
        //print("GetMaxAdaptability " + m + " " + n + " " + k);
        //print("GetMaxAdaptability " + m + " " + n.Length + " " + k.Length);
        float max = 0;
        string S2 = "S2";
        if (m.Equals(S2) || n.Equals(S2) || k.Equals(S2)) {
            max = 1.3f;
            mArmAbilityText.text = "圣";
            SetWeaponSkill(4);
            return max;
        }
        string S1 = "S1";
        if (m.Equals(S1) || n.Equals(S1) || k.Equals(S1)) {
            max = 1.2f;
            mArmAbilityText.text = "神";
            SetWeaponSkill(4);
            return max;
        }
        string S = "S";
        if (m.Equals(S) || n.Equals(S) || k.Equals(S)) {
            max = 1.1f;
            mArmAbilityText.text = "极";
            SetWeaponSkill(4);
            return max;
        }
        string A = "A";
        if (m.Equals(A) || n.Equals(A) || k.Equals(A)) {
            max = 1.0f;
            mArmAbilityText.text = "精";
            SetWeaponSkill(3);
            return max;
        }
        string B = "B";
        if (m.Equals(B) || n.Equals(B) || k.Equals(B)) {
            max = 0.9f;
            mArmAbilityText.text = "熟";
            SetWeaponSkill(2);
            return max;
        }
        string C = "C";
        if (m.Equals(C) || n.Equals(C) || k.Equals(C)) {
            max = 0.8f;
            mArmAbilityText.text = "初";
            SetWeaponSkill(1);
            return max;
        }
        return 1.25f;
    }

    private void SetWeaponSkill(int type) {
        mSkillText[0].text = "";
        mSkillText[1].text = "";
        mSkillText[2].text = "";
        if (type >= 2) {
            if (mWeaponIndex == 0) {
                // 枪兵
                mSkillText[0].text = "突刺";
            } else if (mWeaponIndex == 1) {
                // 戟兵
                mSkillText[0].text = "熊手";
            } else if (mWeaponIndex == 2) {
                // 弩兵
                mSkillText[0].text = "火矢";
            } else if (mWeaponIndex == 3) {
                // 骑兵
                mSkillText[0].text = "突击";
            }
        }
        if (type >= 3) {
            if (mWeaponIndex == 0) {
                // 枪兵
                mSkillText[1].text = "螺旋突刺";
            } else if (mWeaponIndex == 1) {
                // 戟兵
                mSkillText[1].text = "横扫";
            } else if (mWeaponIndex == 2) {
                // 弩兵
                mSkillText[1].text = "贯箭";
            } else if (mWeaponIndex == 3) {
                // 骑兵
                mSkillText[1].text = "突破";
            }
        }
        if (type >= 4) {
            if (mWeaponIndex == 0) {
                // 枪兵
                mSkillText[2].text = "二段突刺";
            } else if (mWeaponIndex == 1) {
                // 戟兵
                mSkillText[2].text = "旋风";
            } else if (mWeaponIndex == 2) {
                // 弩兵
                mSkillText[2].text = "乱射";
            } else if (mWeaponIndex == 3) {
                // 骑兵
                mSkillText[2].text = "突进";
            }
        }
    }

    // 设置名字和特技
	void SetNameAndSkill(WujiangBean chief, WujiangBean junior01, WujiangBean junior02) {
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
	void SetImage(WujiangBean chief, WujiangBean junior01, WujiangBean junior02) {
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

#if UNITY_IOS || UNITY_ANDROID
        url = "file:://" + url;
#else
        url = "file:://" + url;
#endif

        //Debug.Log(TAG + "url:" + url);
        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error) || !www.isDone) {
            // 1 www下载失败
            Debug.Log(TAG + "DownloadImage www.error:" + www.error + " www.isDone:" + www.isDone);
        } else {
            if (image != null) {
                Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                www.LoadImageIntoTexture(texture);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
                image.sprite = sprite;
                image.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void SetCity(City city) {
        mCity = city;
    }
}

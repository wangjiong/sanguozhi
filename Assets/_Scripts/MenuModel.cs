using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuModel : MonoBehaviour {

    public Image mDefaultBtn;
    public Image mPKBtn;

    public Sprite mDefaultSprite;
    public Sprite mSelectedSprite;

    public GameObject[] mDefaultModels;
    public GameObject[] mPKModels;

    public void Select(bool isDefault) {
        if (isDefault) {
            mDefaultBtn.sprite = mSelectedSprite;
            mPKBtn.sprite = mDefaultSprite;

            for (int i = 0; i < mDefaultModels.Length; i++) {
                mDefaultModels[i].SetActive(true);
            }
            for (int i = 0; i < mPKModels.Length; i++) {
                mPKModels[i].SetActive(false);
            }
        } else {
            mDefaultBtn.sprite = mDefaultSprite;
            mPKBtn.sprite = mSelectedSprite;

            for (int i = 0; i < mDefaultModels.Length; i++) {
                mDefaultModels[i].SetActive(false);
            }
            for (int i = 0; i < mPKModels.Length; i++) {
                mPKModels[i].SetActive(true);
            }
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("Game02");
    }
}

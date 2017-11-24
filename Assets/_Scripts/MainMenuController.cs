using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    string TAG = "MainMenuController";

    public GameObject mMenuModel;
    public GameObject mAnim;

    void OnEnable() {
        mAnim.SetActive(true);
    }

    void OnDisable() {
        mAnim.SetActive(false);
    }

    void Update() {

    }

    public void StartGame() {
        this.gameObject.SetActive(false);
        if (mMenuModel != null) {
            mMenuModel.SetActive(true);
        }
    }
}

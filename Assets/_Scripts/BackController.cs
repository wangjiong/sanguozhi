using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BackController : MonoBehaviour, IPointerClickHandler {
    public GameObject mMenuMain;
    public GameObject mMenuModel;

    public void OnPointerClick(PointerEventData eventData) {
        if (mMenuModel != null) {
            mMenuModel.SetActive(false);
        }
        if (mMenuMain != null) {
            mMenuMain.SetActive(true);
        }
    }
}

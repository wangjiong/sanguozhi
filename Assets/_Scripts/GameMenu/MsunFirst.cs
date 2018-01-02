using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsunFirst : MonoBehaviour {

    public GameObject mSecondMeun;

    public void ShowSecondMeun(int index) {
        mSecondMeun.SetActive(true);
        mSecondMeun.GetComponent<RectTransform>().anchoredPosition = new Vector3(130, -60 * (index - 1));
    }
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDevelop : MonoBehaviour {
    public GameObject mCanvasDevelop;

    public void ShowDevelop() {
        mCanvasDevelop.SetActive(true);
    }

    public void HideDevelop() {
        mCanvasDevelop.SetActive(false);
    }
}

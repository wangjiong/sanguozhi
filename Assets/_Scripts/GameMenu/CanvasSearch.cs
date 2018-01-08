using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSearch : MonoBehaviour {
    public GameObject mExecutiveBtn;
    public GameObject mEnableBtn;
    public GameObject mCloseBtn;
    public GameObject mCanvasExecutive;

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
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSearch : MonoBehaviour {
    public GameObject mExecutiveBtn;
    public GameObject mEnableBtn;
    public GameObject mCloseBtn;


    void Start() {
        mExecutiveBtn.GetComponent<Button>().onClick.AddListener(delegate () {

        });
        mEnableBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
        mCloseBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            this.gameObject.SetActive(false);
        });
    }

    void Update() {

    }
}

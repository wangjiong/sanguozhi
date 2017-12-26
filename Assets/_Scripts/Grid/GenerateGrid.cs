using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour {
    private int N = 200;

    public GameObject mGrid;
    bool mFlag = false;

    GameObject mGrids;

    void Start() {
        mGrids = GameObject.Find("Grids");
        for (int i = 0; i < N; i++) {
            GameObject grid = Instantiate(mGrid);
            grid.transform.localScale = new Vector3(1, 200 , 1);
            mFlag = !mFlag;
            if (mFlag) {
                grid.transform.position = new Vector3(0.5f + i, 0, 100f);
            } else {
                grid.transform.position = new Vector3(0.5f + i, 0, 100.5f);
            }
            grid.transform.transform.SetParent(mGrids.transform);
        }
    }

    void Update() {

    }
}

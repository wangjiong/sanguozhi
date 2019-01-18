using System.Collections;
using System.Collections.Generic;
using TGS;
using UnityEngine;
using UnityEngine.UI;

public class Wujiang : MonoBehaviour {
    public static Wujiang sCurrentWujiang;

    static string TAG = "Wujiang==";
    public Image mAvatar;
    public Text mHealth;
    public Text mName;
    bool mSelected;
    HighlightableObject mHighlightableObjecto;

    // Path
    public GameObject mPrefabPathGrid;
    int N = 3;

    void OnEnable() {
    }

    public void HideLight() {
        mSelected = false;
        mHighlightableObjecto.Off();
    }

    public void OnMouseDown() {
        if (mHighlightableObjecto == null) {
            mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
        }
        mSelected = !mSelected;
        if (mSelected) {
            // 1.选中
            sCurrentWujiang = this;
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
            // 显示移动的范围
            ShowPath();
        } else {
            // 2.不选中
            sCurrentWujiang = null;
            mHighlightableObjecto.Off();
        }
    }


    void ShowPath() {
        if (mPrefabPathGrid) {
            Debug.Log(transform.position);
            int x = (int)(transform.position.x + 0.5f);
            int y = (int)(transform.position.z);

            Debug.Log("x:" + x + " y:" + y);

            List<Coordinates> neighbours = MapManager.GetInstance().GetNeighbours(transform.position);

            foreach (Coordinates c in neighbours) {
                GameObject g = Instantiate(mPrefabPathGrid);
                g.transform.position = MapManager.GetInstance().CorrdinateToTerrainPosition(c);
            }
        }
    }

}
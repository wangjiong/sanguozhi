using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTrigger : MonoBehaviour {
    string TAG = "GridTrigger";

    public GameObject mPointCube;

    Vector3 mOriginalPosition;

    void Start() {
        mOriginalPosition = mPointCube.transform.position;
    }

    void OnMouseEnter() {
        print(TAG + "OnMouseEnter");
    }

    void OnMouseOver() {
        print(TAG + "OnMouseOver");
        RaycastHit hit;
        // 从鼠标所在的位置发射
        Vector2 screenPosition = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out hit)) {
            print(hit.transform.name + " point:" + hit.point);
            float x = Mathf.Ceil(hit.point.x);
            float z = 0;
            if (x % 2 == 0) {
                z = Mathf.Ceil(hit.point.z - 0.5f);
                mPointCube.transform.position = new Vector3(x - 0.5f, mOriginalPosition.y, z);
            } else {
                z = Mathf.Ceil(hit.point.z);
                mPointCube.transform.position = new Vector3(x - 0.5f, mOriginalPosition.y, z - 0.5f);
            }

            print("x:" + x + " z:" + z);
        }
    }
    void OnMouseExit() {
        print(TAG + "OnMouseExit");
    }
}

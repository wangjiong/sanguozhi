using System.Collections;
using System.Collections.Generic;
using TGS;
using UnityEngine;

public class Wujiang : MonoBehaviour {
    HighlightableObject mHighlightableObjecto;
    bool mSelected;

    TerrainGridSystem tgs;
    public int index = 0;
    public Vector2 location = Vector2.zero;

    float timer = 0.2f;

    void Start() {
        tgs = TerrainGridSystem.instance;
        location.x = tgs.columnCount / 2;
        location.y = tgs.rowCount / 2;

        location.x++;
        if (location.x >= tgs.columnCount) {
            location.x = 0;
            location.y++;
            if (location.y >= tgs.rowCount) {
                location.y = 0;
            }
        }
        Cell cell = tgs.CellGetAtPosition((int)location.x, (int)location.y);
        int cellIndex = tgs.CellGetIndex(cell);
        if (cellIndex >= 0) {
            Vector3 temp = tgs.CellGetPosition(cellIndex);
            transform.position = new Vector3(temp.x, transform.position.y, temp.z);
        }
        timer = 2.0f;
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {

            //Use these lines to trace by column and row.
            location.x++;
            if (location.x >= tgs.columnCount) {
                location.x = 0;
                location.y++;
                if (location.y >= tgs.rowCount) {
                    location.y = 0;
                }
            }
            Cell cell = tgs.CellGetAtPosition((int)location.x, (int)location.y);
            int cellIndex = tgs.CellGetIndex(cell);
            if (cellIndex >= 0)
                transform.position = tgs.CellGetPosition(cellIndex);


            timer = 2.0f;
        }
    }

    void OnMouseDown() {
        if (mHighlightableObjecto == null) {
            mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
        }
        mSelected = !mSelected;
        if (mSelected) {
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
        } else {
            mHighlightableObjecto.Off();
        }
    }
}
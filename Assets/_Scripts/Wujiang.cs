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

    void OnEnable() {
        //mSelected = true;
        //mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
        //mHighlightableObjecto.ConstantOnImmediate(Color.red);
        //sCurrentWujiang = this;
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
            sCurrentWujiang = this;
            mHighlightableObjecto.ConstantOnImmediate(Color.red);
        } else {
            sCurrentWujiang = null;
            mHighlightableObjecto.Off();
        }
    }
        //HighlightableObject mHighlightableObjecto;
        //bool mSelected;

        //TerrainGridSystem tgs;
        //public int index = 0;
        //public Vector2 location = Vector2.zero;

        //float timer = 0.2f;

        //bool isSelectingStart;
        //int cellStartIndex;

        //void Start() {
        //    tgs = TerrainGridSystem.instance;
        //    location.x = tgs.columnCount / 2;
        //    location.y = tgs.rowCount / 2;

        //    location.x++;
        //    if (location.x >= tgs.columnCount) {
        //        location.x = 0;
        //        location.y++;
        //        if (location.y >= tgs.rowCount) {
        //            location.y = 0;
        //        }
        //    }
        //    Cell cell = tgs.CellGetAtPosition((int)location.x, (int)location.y);
        //    int cellIndex = tgs.CellGetIndex(cell);
        //    if (cellIndex >= 0) {
        //        Vector3 temp = tgs.CellGetPosition(cellIndex);
        //        transform.position = new Vector3(temp.x, transform.position.y, temp.z);
        //    }
        //    timer = 2.0f;

        //    tgs.OnCellClick += (cellIndex1, buttonIndex) => BuildPath(cellIndex1);
        //}

        //void Update() {
        //    timer -= Time.deltaTime;
        //    if (timer <= 0) {

        //        //Use these lines to trace by column and row.
        //        location.x++;
        //        if (location.x >= tgs.columnCount) {
        //            location.x = 0;
        //            location.y++;
        //            if (location.y >= tgs.rowCount) {
        //                location.y = 0;
        //            }
        //        }
        //        Cell cell = tgs.CellGetAtPosition((int)location.x, (int)location.y);
        //        int cellIndex = tgs.CellGetIndex(cell);
        //        if (cellIndex >= 0)
        //            transform.position = tgs.CellGetPosition(cellIndex);


        //        timer = 200.0f;
        //    }
        //}

        //void OnMouseDown() {
        //    if (mHighlightableObjecto == null) {
        //        mHighlightableObjecto = gameObject.AddComponent<HighlightableObject>();
        //    }
        //    mSelected = !mSelected;
        //    if (mSelected) {
        //        mHighlightableObjecto.ConstantOnImmediate(Color.red);
        //    } else {
        //        mHighlightableObjecto.Off();
        //    }

        //    // 寻路
        //    Cell sphereCell = tgs.CellGetAtPosition(transform.position, true);

        //    cellStartIndex = tgs.CellGetIndex(sphereCell);
        //    tgs.CellToggleRegionSurface(cellStartIndex, true, Color.yellow);

        //    isSelectingStart = false;
        //    print("OnMouseDown cellStartIndex:" + cellStartIndex);

        //    //sphereCell.mValue = 0;
        //    //Debug.Log("Sphere Cell Row = " + sphereCell.row + ", Col = " + sphereCell.column);
        //    //mQuene.Enqueue(sphereCell);
        //    //FindPath();
        //    //Debug.Log("mList:" + mList.Count);
        //    //foreach (Cell cell in mList) {
        //    //    if (cell.mValue < 3) {
        //    //        tgs.CellFadeOut(cell, Color.red, 2.0f);
        //    //    }
        //    //}
        //    //sphereCell.mValue = int.MaxValue;
        //    //foreach (Cell cell in mList) {
        //    //    cell.mValue = int.MaxValue;
        //    //}
        //    //mList.Clear();
        //}

        ////List<Cell> mList = new List<Cell>();
        ////Queue<Cell> mQuene = new Queue<Cell>();

        ////private void FindPath() {

        ////    while (mQuene.Count > 0) {
        ////        Cell parent = mQuene.Dequeue();
        ////        List<Cell> neighbours = tgs.CellGetNeighbours(parent);
        ////        foreach (Cell cell in neighbours) {
        ////            if (cell.mValue > parent.mValue + 1) {
        ////                cell.mValue = parent.mValue + 1;
        ////                if (!mList.Contains(cell)) {
        ////                    mList.Add(cell);
        ////                }
        ////                if (cell.mValue < 3) { // 大于3的就不去查找了
        ////                    mQuene.Enqueue(cell);
        ////                }
        ////            }
        ////        }
        ////    }
        ////}



        //void BuildPath(int clickedCellIndex) {
        //    print("BuildPath:" + clickedCellIndex);
        //    if (isSelectingStart) {
        //        // Selects start cell
        //        cellStartIndex = clickedCellIndex;
        //        tgs.CellToggleRegionSurface(cellStartIndex, true, Color.yellow);
        //        isSelectingStart = false;
        //    } else {
        //        // Clicked on the end cell, then show the path
        //        // First clear color of start cell
        //        tgs.CellToggleRegionSurface(cellStartIndex, false, Color.white);
        //        // Get Path
        //        List<int> path = tgs.FindPath(cellStartIndex, clickedCellIndex);
        //        // Color the path
        //        if (path != null) {
        //            for (int k = 0; k < path.Count; k++) {
        //                tgs.CellFadeOut(path[k], Color.green, 1f);
        //            }
        //        }
        //        isSelectingStart = true;
        //    }

        //}


    }
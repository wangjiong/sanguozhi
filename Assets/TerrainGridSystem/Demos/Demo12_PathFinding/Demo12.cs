using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TGS
{
	public class Demo12 : MonoBehaviour
	{
	
		TerrainGridSystem tgs;
		bool isSelectingStart;
		int cellStartIndex;
		GUIStyle labelStyle;

		// Use this for initialization
		void Start ()
		{
			tgs = TerrainGridSystem.instance;

			// setup GUI resizer - only for the demo
			GUIResizer.Init (800, 500); 
			labelStyle = new GUIStyle ();
			labelStyle.alignment = TextAnchor.MiddleLeft;
			labelStyle.normal.textColor = Color.white;

			isSelectingStart = true;

			#if UNITY_5_4_OR_NEWER
			Random.InitState(2);
			#else
			Random.seed = 2;
			#endif

			// Draw some blocked areas
			for (int i=0;i<25;i++) {

				int row = Random.Range(2, tgs.rowCount - 3);
				int col = Random.Range(2, tgs.columnCount - 3);

				for (int j=-2;j<=2;j++) {
					for (int k=-2;k<=2;k++) {
						int cellIndex = tgs.CellGetIndex(row+j, col+k);
						tgs.CellSetCanCross(cellIndex, false);
						tgs.CellToggleRegionSurface(cellIndex, true, Color.white);
					}
				}
			}

			// Hook into cell click event to toggle start selection or draw a computed path using A* path finding algorithm
			tgs.OnCellClick += (cellIndex, buttonIndex) => BuildPath(cellIndex);
		}


		void BuildPath(int clickedCellIndex) {
			if (isSelectingStart) {
				// Selects start cell
				cellStartIndex = clickedCellIndex;
				tgs.CellToggleRegionSurface(cellStartIndex, true, Color.yellow);
			} else {
				// Clicked on the end cell, then show the path
				// First clear color of start cell
				tgs.CellToggleRegionSurface(cellStartIndex, false, Color.white);
				// Get Path
				List<int> path = tgs.FindPath(cellStartIndex, clickedCellIndex);
				// Color the path
				if (path!=null) {
					for (int k=0;k<path.Count;k++) {
						tgs.CellFadeOut(path[k], Color.green, 1f);
					}
				}
			}
			isSelectingStart = !isSelectingStart;
		}

		void OnGUI() {
			// Do autoresizing of GUI layer
			GUIResizer.AutoResize ();
			
			if (isSelectingStart) {
				GUI.Label (new Rect (10, 10, 160, 30), "Select the starting cell", labelStyle);
			} else {
				GUI.Label (new Rect (10, 10, 160, 30), "Select the ending cell", labelStyle);
			}

		}
	
	}
}
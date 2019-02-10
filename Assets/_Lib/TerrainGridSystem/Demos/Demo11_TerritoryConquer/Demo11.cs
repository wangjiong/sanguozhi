using UnityEngine;
using System.Collections;

namespace TGS {
	public class Demo11 : MonoBehaviour {

		TerrainGridSystem tgs;
		GUIStyle labelStyle;

		void Start () {
			// setup GUI styles
			labelStyle = new GUIStyle ();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.black;

			// Get a reference to Terrain Grid System's API
			tgs = TerrainGridSystem.instance;

			// Set colors for frontiers
			tgs.territoryDisputedFrontierColor = Color.yellow;
			tgs.TerritorySetFrontierColor(0, Color.red);
			tgs.TerritorySetFrontierColor(1, Color.blue);

			// listen to events
			tgs.OnCellClick += (cellIndex, buttonIndex) => changeCellOwner(cellIndex);
		}

		void OnGUI () {
			GUI.Label (new Rect (0, 5, Screen.width, 30), "Click on any cell to transfer it to the opposite territory.", labelStyle);
			GUI.Label (new Rect (0, 25, Screen.width, 30), "Note that territories can't be split between two or more areas.", labelStyle);
			GUI.Label (new Rect (0, 45, Screen.width, 30), "If you need separate areas, just color cells with same 'territory color' and don't use territories.", labelStyle);
		}

		void changeCellOwner(int cellIndex) {
			int currentTerritory = tgs.cells[cellIndex].territoryIndex;
			if (currentTerritory == 0) {
				currentTerritory = 1;
			} else {
				currentTerritory = 0;
			}
			tgs.CellSetTerritory(cellIndex, currentTerritory);
			tgs.Redraw();
		}


	}
}

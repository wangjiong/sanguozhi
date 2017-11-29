using UnityEngine;
using System.Collections;

namespace TGS {
	public class Demo3 : MonoBehaviour {

		TerrainGridSystem tgs;
		GUIStyle labelStyle;

		void Start () {
			// setup GUI styles
			labelStyle = new GUIStyle ();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.black;

			// hide all cells
			tgs = TerrainGridSystem.instance;
			tgs.cells.ForEach( (cell) => cell.visible = false );
			tgs.Redraw();
			
			// listen to events
			tgs.OnCellClick += (cellIndex, buttonIndex) => toggleCellVisible(cellIndex);

		}

		void OnGUI () {
			GUI.Label (new Rect (0, 5, Screen.width, 30), "Click on any position to toggle cell visibility.", labelStyle);
		}

		void toggleCellVisible(int cellIndex) {
			tgs.CellSetVisible(cellIndex, !tgs.CellIsVisible(cellIndex));
			tgs.Redraw();
		}


	}
}

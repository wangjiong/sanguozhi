using UnityEngine;
using System.Collections;

namespace TGS {
	public class Demo2 : MonoBehaviour {

		TerrainGridSystem tgs;
		GUIStyle labelStyle;

		void Start () {
			tgs = TerrainGridSystem.instance;

			// setup GUI styles
			labelStyle = new GUIStyle ();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.black;
		}

		void OnGUI () {
			GUI.Label (new Rect (0, 5, Screen.width, 30), "Try changing the grid properties in Inspector. You can click a cell to merge it.", labelStyle);
		}

		void Update() {
			if (tgs.cellHighlighted!=null) {
				if (Input.GetMouseButtonDown(0)) {
					MergeCell(tgs.cellHighlighted);
				} else if (Input.GetMouseButtonDown(1)) {
					Debug.Log ("Right clicked on cell #" + tgs.CellGetIndex(tgs.cellHighlighted));
				}
			}
		}

		/// <summary>
		/// Merge cell example. This function will make cell1 marge with a random cell from its neighbours.
		/// </summary>
		void MergeCell(Cell cell1) {
			int neighbourCount = cell1.region.neighbours.Count;
			if (neighbourCount==0) return;
			Cell cell2 = (Cell)cell1.region.neighbours[Random.Range(0, neighbourCount)].entity;
			tgs.CellMerge(cell1, cell2);
			tgs.Redraw();
		}

	}
}

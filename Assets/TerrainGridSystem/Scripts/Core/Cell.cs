using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TGS.Geom;

namespace TGS {

	public class Cell: IAdmin {
		/// <summary>
		/// Optional cell name.
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// The territory to which this cell belongs to. You can change it using CellSetTerritory method.
		/// </summary>
		public int territoryIndex = -1;

		public Region region { get; set; }
		public Polygon polygon { get; set; }
		public Vector2 center;
		public bool visible { get; set; }

		/// <summary>
		/// Optional value that can be set with CellSetTag. You can later get the cell quickly using CellGetWithTag method.
		/// </summary>
		public int tag;


		public int row, column;

		/// <summary>
		/// If this cell blocks path finding.
		/// </summary>
		public bool canCross = true;

		public Cell(): this("", Vector2.zero) { }

		public Cell (string name): this(name, Vector2.zero) { }

		public Cell(string name, Vector2 center) {
			this.name = name;
			this.center = center;
			visible = true;
		}

	}
}


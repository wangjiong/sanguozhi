using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TGS.Geom;
using TGS.PathFinding;

namespace TGS {
	
	/* Event definitions */
	public delegate void CellEvent(int cellIndex);
	public delegate void CellHighlightEvent(int cellIndex, ref bool cancelHighlight);
	public delegate void CellClickEvent(int cellIndex, int buttonIndex);

	public partial class TerrainGridSystem : MonoBehaviour {

		public event CellEvent OnCellEnter;
		public event CellEvent OnCellExit;
		public event CellClickEvent OnCellClick;
		public event CellHighlightEvent OnCellHighlight;

		/// <summary>
		/// Complete array of states and cells and the territory name they belong to.
		/// </summary>
		[NonSerialized]
		public List<Cell> cells;


		[SerializeField]
		int _numCells = 3;

		/// <summary>
		/// Gets or sets the desired number of cells in irregular topology.
		/// </summary>
		public int numCells { 
			get { return _numCells; } 
			set { if (_numCells!=value) {
					_numCells = Mathf.Clamp(value, 1, MAX_CELLS);
					GenerateMap();
					isDirty = true;
				}
			}
		}


		[SerializeField]
		bool _showCells = true;
		/// <summary>
		/// Toggle cells frontiers visibility.
		/// </summary>
		public bool showCells { 
			get {
				return _showCells; 
			}
			set {
				if (value != _showCells) {
					_showCells = value;
					isDirty = true;
					if (cellLayer != null) {
						cellLayer.SetActive (_showCells);
						ClearLastOver();
					} else if (_showCells) {
						Redraw ();
					}
				}
			}
		}

		[SerializeField]
		Color
			_cellBorderColor = new Color (0, 1, 0, 1.0f);
		
		/// <summary>
		/// Cells border color
		/// </summary>
		public Color cellBorderColor {
			get {
				if (cellsMat != null) {
					return cellsMat.color;
				} else {
					return _cellBorderColor;
				}
			}
			set {
				if (value != _cellBorderColor) {
					_cellBorderColor = value;
					isDirty = true;
					if (cellsMat != null && _cellBorderColor != cellsMat.color) {
						cellsMat.color = _cellBorderColor;
					}
				}
			}
		}

		public float cellBorderAlpha {
			get {
				return _cellBorderColor.a;
			}
			set {
				if (_cellBorderColor.a!=value) {
					cellBorderColor = new Color(_cellBorderColor.r, _cellBorderColor.g, _cellBorderColor.b, Mathf.Clamp01(value));
				}
			}
		}


		[SerializeField]
		Color
			_cellHighlightColor = new Color (1, 0, 0, 0.7f);
		
		/// <summary>
		/// Fill color to use when the mouse hovers a cell's region.
		/// </summary>
		public Color cellHighlightColor {
			get {
				return _cellHighlightColor;
			}
			set {
				if (value != _cellHighlightColor) {
					_cellHighlightColor = value;
					isDirty = true;
					if (hudMatCellOverlay != null && _cellHighlightColor != hudMatCellOverlay.color) {
						hudMatCellOverlay.color = _cellHighlightColor;
					}
					if (hudMatCellGround != null && _cellHighlightColor != hudMatCellGround.color) {
						hudMatCellGround.color = _cellHighlightColor;
					}
				}
			}
		}

		[SerializeField]
		bool _cellHighlightNonVisible = true;

		/// <summary>
		/// Gets or sets whether invisible cells should also be highlighted when pointer is over them
		/// </summary>
		public bool cellHighlightNonVisible {
			get { return _cellHighlightNonVisible; }
			set { if (_cellHighlightNonVisible!=value) { _cellHighlightNonVisible = value; isDirty = true; } }
		}

		/// <summary>
		/// Returns Cell under mouse position or null if none.
		/// </summary>
		public Cell cellHighlighted { get { return _cellHighlighted; } }
		
		/// <summary>
		/// Returns current highlighted cell index.
		/// </summary>
		public int cellHighlightedIndex { get { return _cellHighlightedIndex; } }

		/// <summary>
		/// Returns Cell index which has been clicked
		/// </summary>
		public int cellLastClickedIndex { get { return _cellLastClickedIndex; } }


		#region Public Cell Functions

		
		/// <summary>
		/// Returns the_numCellsrovince in the cells array by its reference.
		/// </summary>
		public int CellGetIndex (Cell cell) {
			if (cell==null) return -1;
			if (cellLookup.ContainsKey(cell)) 
				return _cellLookup[cell];
			else
				return -1;
		}

		/// <summary>
		/// Returns the_numCellsrovince in the cells array by its reference.
		/// </summary>
		/// <returns>The get index.</returns>
		/// <param name="row">Row.</param>
		/// <param name="column">Column.</param>
		/// <param name="clampToBorders">If set to <c>true</c> row and column values will be clamped inside current grid size (in case their values exceed the number of rows or columns). If set to false, it will wrap around edges.</param>
		public int CellGetIndex (int row, int column, bool clampToBorders = true) {
			if (_gridTopology != GRID_TOPOLOGY.Box && _gridTopology != GRID_TOPOLOGY.Hexagonal) {
				Debug.LogWarning("Grid topology does not support row/column indexing.");
				return -1;
			}

			if (clampToBorders) {
				row = Mathf.Clamp(row, 0, _cellRowCount-1);
				column = Mathf.Clamp(column, 0, _cellColumnCount-1);
			} else {
				row = (row + _cellRowCount) % _cellRowCount;
				column = (column + _cellColumnCount) % _cellColumnCount;
			}

			return row * _cellColumnCount + column;
		}



		public GameObject CellToggleRegionSurface (int cellIndex, bool visible, Texture2D texture) {
			return CellToggleRegionSurface(cellIndex, visible, Color.white, false, texture, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a cell by indexes.
		/// </summary>
		public GameObject CellToggleRegionSurface (int cellIndex, bool visible, Color color, bool refreshGeometry = false) {
			return CellToggleRegionSurface(cellIndex, visible, color, refreshGeometry, null, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a cell by indexes.
		/// </summary>
		public GameObject CellToggleRegionSurface (int cellIndex, bool visible, Color color,  bool refreshGeometry, int textureIndex) {
			Texture2D texture = null;
			if (textureIndex>=0 && textureIndex < textures.Length) texture = textures[textureIndex];
			return CellToggleRegionSurface(cellIndex, visible, color, refreshGeometry, texture, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a cell by indexes.
		/// </summary>
		public GameObject CellToggleRegionSurface (int cellIndex, bool visible, Color color,  bool refreshGeometry, Texture2D texture) {
			return CellToggleRegionSurface(cellIndex, visible, color, refreshGeometry, texture, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a cell by indexes.
		/// </summary>
		public GameObject CellToggleRegionSurface (int cellIndex, bool visible, Color color,  bool refreshGeometry, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
			if (cellIndex<0 || cellIndex>=cells.Count) return null;

			if (!visible) {
				CellHideRegionSurface (cellIndex);
				return null;
			}
			int cacheIndex = GetCacheIndexForCellRegion (cellIndex); 
			bool existsInCache = surfaces.ContainsKey (cacheIndex);
			if (existsInCache && surfaces[cacheIndex]==null) {
				surfaces.Remove(cacheIndex);
				existsInCache = false;
			}
			if (refreshGeometry && existsInCache) {
				GameObject obj = surfaces [cacheIndex];
				surfaces.Remove(cacheIndex);
				DestroyImmediate(obj);
				existsInCache = false;
			}
			GameObject surf = null;
			Region region = cells [cellIndex].region;
			if (existsInCache) 
				surf = surfaces [cacheIndex];
			
			// Should the surface be recreated?
			Material surfMaterial;
			if (surf != null) {
				surfMaterial = surf.GetComponent<Renderer> ().sharedMaterial;
				if (texture != null && (region.customMaterial == null || textureScale != region.customTextureScale || textureOffset != region.customTextureOffset || 
				                        textureRotation != region.customTextureRotation || !region.customMaterial.name.Equals (texturizedMat.name))) {
					surfaces.Remove (cacheIndex);
					DestroyImmediate (surf);
					surf = null;
				}
			}
			// If it exists, activate and check proper material, if not create surface
			bool isHighlighted = cellHighlightedIndex == cellIndex;
			if (surf != null) {
				if (!surf.activeSelf)
					surf.SetActive (true);
				// Check if material is ok
				surfMaterial = surf.GetComponent<Renderer> ().sharedMaterial;
				if ((texture == null && !surfMaterial.name.Equals (coloredMat.name)) || (texture != null && !surfMaterial.name.Equals (texturizedMat.name)) 
				    || (surfMaterial.color != color && !isHighlighted) || (texture != null && region.customMaterial.mainTexture != texture)) {
					Material goodMaterial = GetColoredTexturedMaterial (color, texture);
					region.customMaterial = goodMaterial;
					ApplyMaterialToSurface (surf, goodMaterial);
				}
			} else {
				surfMaterial = GetColoredTexturedMaterial (color, texture);
				surf = GenerateCellRegionSurface (cellIndex, surfMaterial, textureScale, textureOffset, textureRotation);
				region.customMaterial = surfMaterial;
				region.customTextureOffset = textureOffset;
				region.customTextureRotation = textureRotation;
				region.customTextureScale = textureScale;
			}
			// If it was highlighted, highlight it again
			if (region.customMaterial != null && isHighlighted && region.customMaterial.color != hudMatTerritory.color) {
				Material clonedMat = Instantiate (region.customMaterial);
				clonedMat.hideFlags = HideFlags.DontSave;
				clonedMat.name = region.customMaterial.name;
				clonedMat.color = hudMatTerritory.color;
				surf.GetComponent<Renderer> ().sharedMaterial = clonedMat;
				_highlightedObj = surf;
			}

			if (!cells[cellIndex].visible) surf.SetActive(false);
			return surf;
		}


		/// <summary>
		/// Uncolorize/hide specified cell by index in the cells collection.
		/// </summary>
		public void CellHideRegionSurface (int cellIndex) {
			if (_cellHighlightedIndex != cellIndex) {
				int cacheIndex = GetCacheIndexForCellRegion (cellIndex);
				if (surfaces.ContainsKey (cacheIndex)) {
					if (surfaces[cacheIndex] == null) {
						surfaces.Remove(cacheIndex);
					} else {
						surfaces [cacheIndex].SetActive (false);
					}
				}
			}
			cells [cellIndex].region.customMaterial = null;
		}

		
		/// <summary>
		/// Uncolorize/hide specified all cells.
		/// </summary>
		public void CellHideRegionSurfaces () {
			for (int k=0;k<cells.Count;k++) {
				CellHideRegionSurface(k);
			}
		}

		/// <summary>
		/// Colors a cell and fades it out during "duration" in seconds.
		/// </summary>
		public void CellFadeOut(Cell cell, Color color, float duration) {
			int cellIndex = CellGetIndex(cell);
			CellFadeOut(cellIndex, color, duration);
		}


		/// <summary>
		/// Colors a cell and fades it out during "duration" in seconds.
		/// </summary>
		public void CellFadeOut(int cellIndex, Color color, float duration) {
			if (cellIndex<0 || cellIndex>=cells.Count) return;
			CellToggleRegionSurface(cellIndex, true, color, false);
			int cacheIndex = GetCacheIndexForCellRegion (cellIndex);
			if (surfaces.ContainsKey (cacheIndex)) {
				GameObject cellSurface = surfaces[cacheIndex];
				Material mat = Instantiate(cellSurface.GetComponent<Renderer>().sharedMaterial);
				mat.color = color;
				mat.hideFlags = HideFlags.DontSave;
				cellSurface.GetComponent<Renderer>().sharedMaterial = mat;
				cells[cellIndex].region.customMaterial = mat;
				SurfaceFader.FadeOut(this, cellSurface, cells[cellIndex].region, color, duration);
			}
		}

		/// <summary>
		/// Gets the cell's center position in world space.
		/// </summary>
		public Vector3 CellGetPosition(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count) return Vector3.zero;
			Vector2 cellGridCenter = cells[cellIndex].center;
			return GetWorldSpacePosition(cellGridCenter);
		}

		/// <summary>
		/// Returns the number of vertices of the cell
		/// </summary>
		public int CellGetVertexCount(int cellIndex) {
			return cells[cellIndex].region.points.Count;
		}

		/// <summary>
		/// Returns the world space position of the vertex
		/// </summary>
		public Vector3 CellGetVertexPosition(int cellIndex, int vertexIndex) {
			Vector2 localPosition = cells[cellIndex].region.points[vertexIndex];
			return GetWorldSpacePosition(localPosition);
		}


		/// <summary>
		/// Returns a list of neighbour cells for specificed cell.
		/// </summary>
		public List<Cell> CellGetNeighbours(Cell cell) {
			int cellIndex = CellGetIndex(cell);
			return CellGetNeighbours(cellIndex);
		}

		/// <summary>
		/// Returns a list of neighbour cells for specificed cell index.
		/// </summary>
		public List<Cell> CellGetNeighbours(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count) return null;
			List<Cell>neighbours = new List<Cell>();
			Region region = cells[cellIndex].region;
			for (int k=0;k<region.neighbours.Count;k++) {
				neighbours.Add ( (Cell)region.neighbours[k].entity);
			}
			return neighbours;
		}

		/// <summary>
		/// Returns cell's territory index to which it belongs to.
		/// </summary>
		public int CellGetTerritoryIndex(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count) return -1;
			return cells[cellIndex].territoryIndex;
		}

		/// <summary>
		/// Returns current cell's fill color
		/// </summary>
		public Color CellGetColor(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count || cells[cellIndex].region.customMaterial==null) return new Color(0,0,0,0);
			return cells[cellIndex].region.customMaterial.color;
		}

		/// <summary>
		/// Returns current cell's fill texture
		/// </summary>
		public Texture CellGetTexture(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count || cells[cellIndex].region.customMaterial==null) return null;
			return cells[cellIndex].region.customMaterial.mainTexture;
		}

		/// <summary>
		/// Returns current cell's fill texture index (if texture exists in textures list).
		/// Texture index is from 1..32. It will return 0 if texture does not exist or it does not match any texture in the list of textures.
		/// </summary>
		public int CellGetTextureIndex(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count || cells[cellIndex].region.customMaterial==null) return 0;
			Texture2D tex = (Texture2D)cells[cellIndex].region.customMaterial.mainTexture;
			for (int k=1;k<textures.Length;k++) {
				if (tex == textures[k]) return k;
			}
			return 0;
		}

								public bool CellIsBorder(int cellIndex) {

												if (cellIndex<0 || cellIndex>=cells.Count) return false;
												Cell cell = cells[cellIndex];
			return (cell.column == 0 || cell.column == _cellColumnCount-1 || cell.row == 0 || cell.row == _cellRowCount-1);
								}


								/// <summary>
								/// Returns the_numCellsrovince in the cells array by its reference.
								/// </summary>
								public int TerritoryGetIndex (Territory territory) {
												if (territory==null) return -1;
												if (territoryLookup.ContainsKey(territory)) 
																return _territoryLookup[territory];
												else
																return -1;
								}

		/// <summary>
		/// Returns true if cell is visible
		/// </summary>
		public bool CellIsVisible(int cellIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count) return false;
			return cells[cellIndex].visible;
		}


		/// <summary>
		/// Merges cell2 into cell1. Cell2 is removed.
		/// Only cells which are neighbours can be merged.
		/// </summary>
		public bool CellMerge(Cell cell1, Cell cell2) {
			if (cell1==null || cell2==null) return false;
			if (!cell1.region.neighbours.Contains(cell2.region)) return false;
			cell1.center = (cell2.center + cell1.center)/2.0f;
			// Polygon UNION operation between both regions
			PolygonClipper pc = new PolygonClipper(cell1.polygon, cell2.polygon);
			pc.Compute(PolygonOp.UNION);
			// Remove cell2 from lists
			CellRemove(cell2);
			// Updates geometry data on cell1
			UpdateCellGeometry(cell1, pc.subject);
			return true;
		}

		/// <summary>
		/// Removes a cell from the cells and territories lists. Note that this operation only removes cell structure but does not affect polygons - mostly internally used
		/// </summary>
		public void CellRemove(Cell cell) {
			if (cell == _cellHighlighted) HideCellRegionHighlight();
			if (cell == _cellLastOver) {
				ClearLastOver();
			}
			int territoryIndex = cell.territoryIndex;
			if (territoryIndex>=0) {
				if (territories[territoryIndex].cells.Contains(cell)) {
					territories[territoryIndex].cells.Remove(cell);
				}
			}
			// remove cell from global list
			if (cells.Contains(cell)) cells.Remove(cell);

			needRefreshRouteMatrix = true;
			needUpdateTerritories = true;
		}

		/// <summary>
		/// Tags a cell with a user-defined integer tag. Cell can be later retrieved very quickly using CellGetWithTag.
		/// </summary>
		public void CellSetTag(Cell cell, int tag) {
			if (!cellTagged.ContainsKey(tag)) cellTagged.Add(tag, cell);
			cell.tag = tag;
		}

		/// <summary>
		/// Retrieves Cell object with associated tag.
		/// </summary>
		public Cell CellGetWithTag(int tag) {
			if (cellTagged.ContainsKey(tag)) return cellTagged[tag];
			return null;
		}

		
		/// <summary>
		/// Specifies if a given cell can be crossed by using the pathfinding engine.
		/// </summary>
		public void CellSetCanCross(int cellIndex, bool canCross) {
			if (cellIndex<0 || cellIndex>=cells.Count) return;
			cells[cellIndex].canCross = canCross;
			needRefreshRouteMatrix = true;
		}

		/// <summary>
		/// Specifies if a given cell is visible.
		/// </summary>
		public void CellSetVisible(int cellIndex, bool visible) {
			if (cellIndex<0 || cellIndex>=cells.Count) return;
			cells[cellIndex].visible = visible;
			if (cellIndex == _cellLastOverIndex) {
				ClearLastOver();
			}
			needRefreshRouteMatrix = true;
		}


		/// <summary>
		/// Returns the cell object under position in local coordinates
		/// </summary>
		public Cell CellGetAtPosition(Vector3 position) {
			return GetCellAtPoint(position, false);
		}

		/// <summary>
		/// Returns the cell object under position in local or worldSpace coordinates
		/// </summary>
		public Cell CellGetAtPosition(Vector3 position, bool worldSpace) {
			return GetCellAtPoint(position, worldSpace);
		}

		/// <summary>
		/// Sets the territory of a cell triggering territory boundary recalculation
		/// </summary>
		/// <returns><c>true</c>, if cell was transferred., <c>false</c> otherwise.</returns>
		public bool CellSetTerritory(int cellIndex, int territoryIndex) {
			if (cellIndex<0 || cellIndex>=cells.Count) return false;
			Cell cell = cells[cellIndex];
			if (cell.territoryIndex>=0 && cell.territoryIndex<territories.Count && territories[cell.territoryIndex].cells.Contains(cell)) {
				territories[cell.territoryIndex].cells.Remove(cell);
			}
			cells[cellIndex].territoryIndex = territoryIndex;
			needUpdateTerritories = true;
			return true;
		}

		/// <summary>
		/// Returns a string-packed representation of current cells settings.
		/// Each cell separated by ;
		/// Individual settings mean:
		/// Position	Meaning
		/// 0			Visibility (0 = invisible, 1 = visible)
		/// 1			Territory Index
		/// 2			Color R (0..1)
		/// 3			Color G (0..1)
		/// 4			Color B (0..1)
		/// 5			Color A (0..1)
		/// 6			Texture Index
		/// </summary>
		/// <returns>The get configuration data.</returns>
		public string CellGetConfigurationData() {
			StringBuilder sb = new StringBuilder();
			for (int k=0;k<cells.Count;k++) {
				if (k>0) sb.Append(";");
				// 0
				Cell cell = cells[k];
				if (cell.visible) {
					sb.Append("1");
				} else {
					sb.Append("0");
				}
				// 1
				sb.Append(",");
				sb.Append(cell.territoryIndex);
				// 2
				sb.Append(",");
				Color color = CellGetColor(k);
				sb.Append(color.a.ToString("F3"));
				// 3
				sb.Append(",");
				sb.Append(color.r.ToString("F3"));
				// 4
				sb.Append(",");
				sb.Append(color.g.ToString("F3"));
				// 5
				sb.Append(",");
				sb.Append(color.b.ToString("F3"));
				// 6
				sb.Append(",");
				sb.Append(CellGetTextureIndex(k));
			}
			return sb.ToString();
		}

		public void CellSetConfigurationData(string cellData) {
			if (cells==null) return;
			string[] cellsInfo = cellData.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries);
			char[] separators = new char[] {','};
			for (int k=0;k<cellsInfo.Length && k<cells.Count;k++) {
				string[] cellInfo = cellsInfo[k].Split(separators, StringSplitOptions.RemoveEmptyEntries);
				int length = cellInfo.Length;
				if (length>0) {
					if (cellInfo[0].Length>0) {
						cells[k].visible = cellInfo[0][0]!='0'; // cellInfo[0].Equals("0");
					}
				}
				if (length>1) {
					cells[k].territoryIndex = FastConvertToInt(cellInfo[1]);
				}
				Color color = new Color(0,0,0,0);
				if (length>5) {
					Single.TryParse(cellInfo[5], out color.a);
					if (color.a>0) {
						Single.TryParse(cellInfo[2], out color.r);
						Single.TryParse(cellInfo[3], out color.g);
						Single.TryParse(cellInfo[4], out color.b);
					}
				} 
				int textureIndex = -1;
				if (length>6) {
					textureIndex = FastConvertToInt(cellInfo[6]);
				}
				if (color.a>0 || textureIndex>=1) {
					CellToggleRegionSurface(k, true, color, false, textureIndex);
				}
			}
			needUpdateTerritories = true;
			needRefreshRouteMatrix = true;
			Redraw();
			isDirty = true;
		}

		/// <summary>
		/// Returns the cell located at given row and column
		/// </summary>
		public Cell CellGetAtPosition(int column, int row) {
			int index = row * _cellColumnCount + column;
			if (index>=0 && index<cells.Count) return cells[index];
			return null;
		}

		#endregion


	
	}
}


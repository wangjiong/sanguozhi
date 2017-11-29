using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TGS.Geom;
using TGS.PathFinding;

namespace TGS {
	
	public enum HIGHLIGHT_MODE {
		None = 0,
		Territories = 1,
		Cells = 2
	}

	public enum OVERLAY_MODE {
		Overlay = 0,
		Ground = 1
	}

	public enum GRID_TOPOLOGY {
		Irregular = 0,
		Box = 1,
//		Rectangular = 2,	// deprecated: use Box
		Hexagonal = 3
	}

	/* Event definitions */

	public delegate int OnPathFindingCrossCell (int cellIndex);


	public partial class TerrainGridSystem : MonoBehaviour {


		[SerializeField]
		Terrain _terrain;
		/// <summary>
		/// Terrain reference. Assign a terrain to this property to fit the grid to terrain height and dimensions
		/// </summary>
		public Terrain terrain {
			get {
				return _terrain;
			}
			set {
				if (_terrain!=value)  {
					_terrain = value;
					isDirty = true;
					Redraw ();
				}
			}
		}

		/// <summary>
		/// Returns the terrain center in world space.
		/// </summary>
		public Vector3 terrainCenter {
			get {
				return _terrain.transform.position + new Vector3(terrainWidth * 0.5f, 0, terrainDepth * 0.5f);
			}
		}

		public Texture2D canvasTexture;

		[SerializeField]
		Texture2D _gridMask;
		/// <summary>
		/// Gets or sets the grid mask. The alpha component of this texture is used to determine cells visibility (0 = cell invisible)
		/// </summary>
		public Texture2D gridMask {
			get { return _gridMask; }
			set { if (_gridMask!=value) { _gridMask = value; isDirty = true; ReloadMask(); } }
		}


		[SerializeField]
		GRID_TOPOLOGY _gridTopology = GRID_TOPOLOGY.Irregular;

		/// <summary>
		/// The grid type (boxed, hexagonal or irregular)
		/// </summary>
		public GRID_TOPOLOGY gridTopology { 
			get { return _gridTopology; } 
			set { if (_gridTopology!=value) {
					_gridTopology = value;
					GenerateMap();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		int _seed = 1;

		/// <summary>
		/// Randomize seed used to generate cells. Use this to control randomization.
		/// </summary>
		public int seed { 
			get { return _seed; } 
			set { if (_seed!=value) {
					_seed = value;
					GenerateMap();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		int _gridRelaxation = 1;

		/// <summary>
		/// Sets the relaxation iterations used to normalize cells sizes in irregular topology.
		/// </summary>
		public int gridRelaxation { 
			get { return _gridRelaxation; } 
			set { if (_gridRelaxation!=value) {
					_gridRelaxation = value;
					GenerateMap();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		float _gridCurvature = 0.0f;

		/// <summary>
		/// Gets or sets the grid's curvature factor.
		/// </summary>
		public float gridCurvature { 
			get { return _gridCurvature; } 
			set { if (_gridCurvature!=value) {
					_gridCurvature = value;
					GenerateMap();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		HIGHLIGHT_MODE _highlightMode = HIGHLIGHT_MODE.Cells;
		
		public HIGHLIGHT_MODE highlightMode {
			get {
				return _highlightMode;
			}
			set {
				if (_highlightMode != value) {
					_highlightMode = value;
					isDirty = true;
					HideCellRegionHighlight();
					HideTerritoryRegionHighlight();
					CheckCells();
					CheckTerritories();
				}
			}
		}

		[SerializeField]
		float _highlightFadeAmount = 0.5f;

		public float highlightFadeAmount {
			get {
				return _highlightFadeAmount;
			}
			set {
				if (_highlightFadeAmount!=value) {
					_highlightFadeAmount = value;
					isDirty = true;
				}
			}
		}

		
		[SerializeField]
		float _highlightMinimumTerrainDistance = 35f;

		/// <summary>
		/// Minimum distance from camera for cells to be highlighted on terrain
		/// </summary>
		public float highlightMinimumTerrainDistance {
			get {
				return _highlightMinimumTerrainDistance;
			}
			set {
				if (_highlightMinimumTerrainDistance!=value) {
					_highlightMinimumTerrainDistance = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		OVERLAY_MODE _overlayMode = OVERLAY_MODE.Overlay;
		
		public OVERLAY_MODE overlayMode {
			get {
				return _overlayMode;
			}
			set {
				if (_overlayMode != value) {
					_overlayMode = value;
					isDirty = true;
				}
			}
		}

		[SerializeField]
		Vector2 _gridCenter = new Vector2(0,0);

		/// <summary>
		/// Center of the grid on the Terrain (by default, 0,0, which means center of terrain)
		/// </summary>
		public Vector2 gridCenter { 
			get { return _gridCenter; } 
			set { if (_gridCenter!=value) {
					_gridCenter = value;
					isDirty = true;
					CellsUpdateBounds();
					Redraw();
				}
			}
		}

		[SerializeField]
		Vector2 _gridScale = new Vector2(1,1);

		/// <summary>
		/// Scale of the grid on the Terrain (by default, 1,1, which means occupy entire terrain)
		/// </summary>
		public Vector2 gridScale { 
			get { return _gridScale; } 
			set { if (_gridScale!=value) {
					_gridScale.x = Mathf.Clamp(value.x, 0.0001f, 1f);
					_gridScale.y = Mathf.Clamp(value.y, 0.0001f, 1f);
					isDirty = true;
					CellsUpdateBounds();
					Redraw();
				}
			}
		}

		
		[SerializeField]
		float _gridElevation = 0;
		public float gridElevation { 
			get { return _gridElevation; } 
			set { if (_gridElevation!=value) {
					_gridElevation = value;
					isDirty = true;
					FitToTerrain();
				}
			}
		}

		[SerializeField]
		float _gridElevationBase = 0;
		public float gridElevationBase { 
			get { return _gridElevationBase; } 
			set { if (_gridElevationBase!=value) {
					_gridElevationBase = value;
					isDirty = true;
					FitToTerrain();
				}
			}
		}

		public float gridElevationCurrent { get { return _gridElevation + _gridElevationBase; } }

		[SerializeField]
		float _gridCameraOffset = 0;
		public float gridCameraOffset { 
			get { return _gridCameraOffset; } 
			set { if (_gridCameraOffset!=value) {
					_gridCameraOffset = value;
					isDirty = true;
					FitToTerrain();
				}
			}
		}

		
		[SerializeField]
		float _gridNormalOffset = 0;
		public float gridNormalOffset { 
			get { return _gridNormalOffset; } 
			set { if (_gridNormalOffset!=value) {
					_gridNormalOffset = value;
					isDirty = true;
					Redraw ();
				}
			}
		}

		
		[SerializeField]
		int _gridDepthOffset = -1;
		public int gridDepthOffset { 
			get { return _gridDepthOffset; } 
			set { if (_gridDepthOffset!=value) {
					_gridDepthOffset = value;
					UpdateMaterialDepthOffset ();
					isDirty = true;
				}
			}
		}
		
		[SerializeField]
		float _gridRoughness = 0.01f;
		public float gridRoughness { 
			get { return _gridRoughness; } 
			set { if (_gridRoughness!=value) {
					_gridRoughness = value;
					isDirty = true;
					Redraw ();
				}
			}
		}

		[SerializeField]
		int _cellRowCount = 8;
		/// <summary>
		/// Returns the number of rows for box and hexagonal grid topologies
		/// </summary>
		public int rowCount { 
			get {
				return _cellRowCount;
			}
			set {
				if (value != _cellRowCount) {
					_cellRowCount = value;
					isDirty = true;
					GenerateMap();
				}
			}

		}

		/// <summary>
		/// Returns the number of rows for box and hexagonal grid topologies
		/// </summary>
		[Obsolete("Use rowCount instead.")]
		public int cellRowCount { 
			get {
				return rowCount;
			}
			set {
				rowCount = value;
			}
			
		}

					
		[SerializeField]
		int _cellColumnCount = 8;
		/// <summary>
		/// Returns the number of columns for box and hexagonal grid topologies
		/// </summary>
		public int columnCount { 
			get {
				return _cellColumnCount;
			}
			set {
				if (value != _cellColumnCount) {
					_cellColumnCount = value;
					isDirty = true;
					GenerateMap();
				}
			}
		}

		/// <summary>
		/// Returns the number of columns for box and hexagonal grid topologies
		/// </summary>
		[Obsolete("Use columnCount instead.")]
		public int cellColumnCount { 
			get {
				return columnCount;
			}
			set {
				columnCount = value;
			}
		}

		public Texture2D[] textures;

		
		[SerializeField]
		bool
			_respectOtherUI = true;
		
		/// <summary>
		/// When enabled, will prevent interaction if pointer is over an UI element
		/// </summary>
		public bool	respectOtherUI {
			get { return _respectOtherUI; }
			set {
				if (value != _respectOtherUI) {
					_respectOtherUI = value;
					isDirty = true;
				}
			}
		}


		[SerializeField]
		float _nearClipFade = 25f;
		public float nearClipFade { 
			get { return _nearClipFade; } 
			set { if (_nearClipFade!=value) {
					_nearClipFade = value;
					isDirty = true;
					UpdateMaterialNearClipFade();
				}
			}
		}

		[SerializeField]
		float _nearClipFadeFallOff = 50f;
		public float nearClipFadeFallOff { 
			get { return _nearClipFadeFallOff; } 
			set { if (_nearClipFadeFallOff!=value) {
					_nearClipFadeFallOff = Mathf.Max (value, 0.001f);
					isDirty = true;
					UpdateMaterialNearClipFade();
				}
			}
		}

		public static TerrainGridSystem instance { get {
				if (_instance==null) {
					GameObject o = GameObject.Find ("TerrainGridSystem");
					if (o!=null) {
						_instance = o.GetComponentInChildren<TerrainGridSystem>();
					} else {
						Debug.LogWarning("TerrainGridSystem gameobject not found in the scene!");
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// Returns a reference of the currently highlighted gameobject (cell or territory)
		/// </summary>
		public GameObject highlightedObj { get { return _highlightedObj; } }


		#region Public General Functions

		/// <summary>
		/// Used to cancel highlighting on a given gameobject. This call is ignored if go is not currently highlighted.
		/// </summary>
		public void HideHighlightedObject(GameObject go) {
			if (go!=_highlightedObj) return;
			_cellHighlightedIndex = -1;
			_cellHighlighted = null;
			_territoryHighlightedIndex = -1;
			_territoryHighlighted = null;
			_territoryLastOver = null;
			_territoryLastOverIndex = -1;
			_highlightedObj = null;
			ClearLastOver();
		}

		#endregion


	
	}
}


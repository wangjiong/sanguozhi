using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TGS;
using TGS.PathFinding;

namespace TGS_Editor
{
	[CustomEditor(typeof(TerrainGridSystem))]
	public class TGSInspector : Editor
	{

		TerrainGridSystem tgs;
		Texture2D _headerTexture, _blackTexture;
		string[] selectionModeOptions, topologyOptions, overlayModeOptions;
		int[] topologyOptionsValues;
		GUIStyle blackStyle, titleLabelStyle, infoLabelStyle;
		int cellSelectedIndex, cellHighlightedIndex = -1, cellTerritoryIndex, cellTextureIndex;
		Color colorSelection, cellColor;
		int textureMode;
		static GUIStyle toggleButtonStyleNormal = null;
		static GUIStyle toggleButtonStyleToggled = null;
		SerializedProperty isDirty;

		void OnEnable ()
		{

			_blackTexture = MakeTex (4, 4, EditorGUIUtility.isProSkin ? new Color (0.18f, 0.18f, 0.18f) : new Color (0.88f, 0.88f, 0.88f));
			_blackTexture.hideFlags = HideFlags.DontSave;
			_headerTexture = Resources.Load<Texture2D> ("EditorHeader");
			blackStyle = new GUIStyle ();
			blackStyle.normal.background = _blackTexture;

			selectionModeOptions = new string[] { "None", "Territories", "Cells" };
			overlayModeOptions = new string[] { "Overlay", "Ground" };
			topologyOptions = new string[] { "Irregular", "Box", "Hexagonal" };
			topologyOptionsValues = new int[] {
				(int)GRID_TOPOLOGY.Irregular,
				(int)GRID_TOPOLOGY.Box,
				(int)GRID_TOPOLOGY.Hexagonal
			};

			tgs = (TerrainGridSystem)target;
			if (tgs.territories == null) {
				tgs.Init ();
			}
			colorSelection = new Color (1, 1, 0.5f, 0.85f);
			cellColor = Color.white;
			cellSelectedIndex = -1;
			isDirty = serializedObject.FindProperty("isDirty");

			HideEditorMesh ();
		}

		public override void OnInspectorGUI ()
		{
			EditorGUILayout.Separator ();
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;  
			GUILayout.Label (_headerTexture, GUILayout.ExpandWidth (true));
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;  


			EditorGUILayout.BeginVertical (blackStyle);

			EditorGUILayout.BeginHorizontal ();
			DrawTitleLabel ("Grid Configuration");
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Help")) {
				EditorUtility.DisplayDialog ("Terrain Grid System", "TGS is an advanced grid generator for Unity terrain. It can also work as a standalone 2D grid.\n\nFor a complete description of the options, please refer to the documentation guide (PDF) included in the asset.\nWe also invite you to visit and sign up on our support forum on kronnect.com where you can post your questions/requests.\n\nThanks for purchasing! Please rate Terrain Grid System on the Asset Store! Thanks.", "Close");
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Topology", GUILayout.Width (120));
			tgs.gridTopology = (GRID_TOPOLOGY)EditorGUILayout.IntPopup ((int)tgs.gridTopology, topologyOptions, topologyOptionsValues);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Territories", GUILayout.Width (120));
			tgs.numTerritories = EditorGUILayout.IntSlider (tgs.numTerritories, 1, Mathf.Min (tgs.numCells, TerrainGridSystem.MAX_TERRITORIES));
			EditorGUILayout.EndHorizontal ();

			if (tgs.gridTopology == GRID_TOPOLOGY.Irregular) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Cells (aprox.)", GUILayout.Width (120));
				tgs.numCells = EditorGUILayout.IntSlider (tgs.numCells, 2, TerrainGridSystem.MAX_CELLS);
				EditorGUILayout.EndHorizontal ();
			} else {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Columns", GUILayout.Width (120));
				tgs.columnCount = EditorGUILayout.IntSlider (tgs.columnCount, 2, TerrainGridSystem.MAX_CELLS_SQRT);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Rows", GUILayout.Width (120));
				tgs.rowCount = EditorGUILayout.IntSlider (tgs.rowCount, 2, TerrainGridSystem.MAX_CELLS_SQRT);
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Curvature", GUILayout.Width (120));
			if (tgs.numCells > TerrainGridSystem.MAX_CELLS_FOR_CURVATURE) {
				DrawInfoLabel ("not available with >" + TerrainGridSystem.MAX_CELLS_FOR_CURVATURE + " cells");
			} else {
				tgs.gridCurvature = EditorGUILayout.Slider (tgs.gridCurvature, 0, 0.1f);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Relaxation", GUILayout.Width (120));
			if (tgs.gridTopology != GRID_TOPOLOGY.Irregular) {
				DrawInfoLabel ("only available with irregular topology");
			} else if (tgs.numCells > TerrainGridSystem.MAX_CELLS_FOR_RELAXATION) {
				DrawInfoLabel ("not available with >" + TerrainGridSystem.MAX_CELLS_FOR_RELAXATION + " cells");
			} else {
				tgs.gridRelaxation = EditorGUILayout.IntSlider (tgs.gridRelaxation, 1, 32);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Roughness", GUILayout.Width (120));
			tgs.gridRoughness = EditorGUILayout.Slider (tgs.gridRoughness, 0.01f, 0.2f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Seed", GUILayout.Width (120));
			tgs.seed = EditorGUILayout.IntSlider (tgs.seed, 1, 10000);
			if (GUILayout.Button ("Redraw")) {
				tgs.Redraw ();
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent ("Mask", "Alpha channel is used to determine cell visibility (0 = cell is not visible)"), GUILayout.Width (120));
			tgs.gridMask = (Texture2D)EditorGUILayout.ObjectField (tgs.gridMask, typeof(Texture2D), true);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent ("Territories Texture", "Quickly create territories assigning a color texture in which each territory corresponds to a color."), GUILayout.Width (120));
			tgs.territoriesTexture = (Texture2D)EditorGUILayout.ObjectField (tgs.territoriesTexture, typeof(Texture2D), true);
			if (tgs.territoriesTexture!=null) {
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label (new GUIContent ("  Neutral Color", "Color to be ignored."), GUILayout.Width (120));
				tgs.territoriesTextureNeutralColor = EditorGUILayout.ColorField(tgs.territoriesTextureNeutralColor, GUILayout.Width(50));
				EditorGUILayout.Space();
				if (GUILayout.Button("Generate Territories", GUILayout.Width(120))) {
					tgs.CreateTerritories(tgs.territoriesTexture, tgs.territoriesTextureNeutralColor);
				}
			}
			EditorGUILayout.EndHorizontal ();

			int cellsCreated = tgs.cells == null ? 0 : tgs.cells.Count;
			int territoriesCreated = tgs.territories == null ? 0 : tgs.territories.Count;

			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			DrawInfoLabel ("Cells Created: " + cellsCreated + " / Territories Created: " + territoriesCreated + " / Vertex Count: " + tgs.lastVertexCount);
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginVertical (blackStyle);

			DrawTitleLabel ("Grid Positioning");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Hide Objects", GUILayout.Width (120));
			if (tgs.terrain != null && GUILayout.Button ("Toggle Terrain")) {
				tgs.terrain.enabled = !tgs.terrain.enabled;
			}
			if (GUILayout.Button ("Toggle Grid")) {
				tgs.gameObject.SetActive (!tgs.gameObject.activeSelf);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Center", GUILayout.Width (120));
			tgs.gridCenter = EditorGUILayout.Vector2Field ("", tgs.gridCenter);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Scale", GUILayout.Width (120));
			tgs.gridScale = EditorGUILayout.Vector2Field ("", tgs.gridScale);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Depth Offset", GUILayout.Width (120));
			tgs.gridDepthOffset = EditorGUILayout.IntSlider (tgs.gridDepthOffset, -10, -1);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Elevation", GUILayout.Width (120));
			tgs.gridElevation = EditorGUILayout.Slider (tgs.gridElevation, 0f, 5f);
			EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Elevation Base", GUILayout.Width (120));
			tgs.gridElevationBase = EditorGUILayout.FloatField (tgs.gridElevationBase, GUILayout.Width (60));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Camera Offset", GUILayout.Width (120));
			tgs.gridCameraOffset = EditorGUILayout.Slider (tgs.gridCameraOffset, 0, 0.1f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Normal Offset", GUILayout.Width (120));
			tgs.gridNormalOffset = EditorGUILayout.Slider (tgs.gridNormalOffset, 0.00f, 5f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginVertical (blackStyle);

			DrawTitleLabel ("Grid Appearance");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Show Territories", GUILayout.Width (120));
			tgs.showTerritories = EditorGUILayout.Toggle (tgs.showTerritories);
			GUILayout.Label ("Frontier Color");
			tgs.territoryFrontiersColor = EditorGUILayout.ColorField (tgs.territoryFrontiersColor, GUILayout.Width (50));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  Highlight Color", GUILayout.Width (120));
			tgs.territoryHighlightColor = EditorGUILayout.ColorField (tgs.territoryHighlightColor, GUILayout.Width (50));
			GUILayout.FlexibleSpace();
			GUILayout.Label (new GUIContent("Disputed Frontier", "Color for common frontiers between two territories."));
			tgs.territoryDisputedFrontierColor = EditorGUILayout.ColorField (tgs.territoryDisputedFrontierColor, GUILayout.Width (50));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  Colorize Territories", GUILayout.Width (120));
			tgs.colorizeTerritories = EditorGUILayout.Toggle (tgs.colorizeTerritories);
			GUILayout.Label ("Alpha");
			tgs.colorizedTerritoriesAlpha = EditorGUILayout.Slider (tgs.colorizedTerritoriesAlpha, 0.0f, 1.0f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  Outer Borders", GUILayout.Width (120));
			tgs.showTerritoriesOuterBorders = EditorGUILayout.Toggle (tgs.showTerritoriesOuterBorders);
			GUILayout.Label(new GUIContent("Internal Territories", "Allows territories to be contained by other territories."));
			tgs.allowTerritoriesInsideTerritories = EditorGUILayout.Toggle (tgs.allowTerritoriesInsideTerritories);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Show Cells", GUILayout.Width (120));
			tgs.showCells = EditorGUILayout.Toggle (tgs.showCells);
			if (tgs.showCells) {
				GUILayout.Label ("Border Color", GUILayout.Width (120));
				tgs.cellBorderColor = EditorGUILayout.ColorField (tgs.cellBorderColor, GUILayout.Width (50));
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  Highlight Color", GUILayout.Width (120));
			tgs.cellHighlightColor = EditorGUILayout.ColorField (tgs.cellHighlightColor, GUILayout.Width (50));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Highlight Fade", GUILayout.Width (120));
			tgs.highlightFadeAmount = EditorGUILayout.Slider (tgs.highlightFadeAmount, 0.0f, 1.0f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent("Near Clip Fade", "Fades out the cell and territories lines near to the camera."), GUILayout.Width (120));
			tgs.nearClipFade = EditorGUILayout.Slider (tgs.nearClipFade, 0.0f, 100.0f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  FallOff", GUILayout.Width (120));
			tgs.nearClipFadeFallOff = EditorGUILayout.Slider (tgs.nearClipFadeFallOff, 0.001f, 100.0f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Canvas Texture", GUILayout.Width (120));
			tgs.canvasTexture = (Texture2D)EditorGUILayout.ObjectField (tgs.canvasTexture, typeof(Texture2D), true);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginVertical (blackStyle);
				
			DrawTitleLabel ("Grid Behaviour");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Terrain", GUILayout.Width (120));
			Terrain prevTerrain = tgs.terrain;
			tgs.terrain = (Terrain)EditorGUILayout.ObjectField (tgs.terrain, typeof(Terrain), true);
			if (tgs.terrain != prevTerrain) {
				GUIUtility.ExitGUI ();
				return;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Selection Mode", GUILayout.Width (120));
			tgs.highlightMode = (HIGHLIGHT_MODE)EditorGUILayout.Popup ((int)tgs.highlightMode, selectionModeOptions);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("  Include Invisible Cells", GUILayout.Width (120));
			tgs.cellHighlightNonVisible = EditorGUILayout.Toggle (tgs.cellHighlightNonVisible, GUILayout.Width (40));
			GUILayout.Label (new GUIContent ("Minimum Distance", "Minimum distance of cell/territory to camera to be selectable. Useful in first person view to prevent selecting cells already under character."), GUILayout.Width (120));
			tgs.highlightMinimumTerrainDistance = EditorGUILayout.FloatField (tgs.highlightMinimumTerrainDistance, GUILayout.Width (60));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Overlay Mode", GUILayout.Width (120));
			tgs.overlayMode = (OVERLAY_MODE)EditorGUILayout.Popup ((int)tgs.overlayMode, overlayModeOptions);
			EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Respect Other UI", GUILayout.Width (120));
			tgs.respectOtherUI = EditorGUILayout.Toggle (tgs.respectOtherUI);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginVertical (blackStyle);
			
			DrawTitleLabel ("Path Finding");
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Algorithm", GUILayout.Width (120));
			tgs.pathFindingHeuristicFormula = (TGS.PathFinding.HeuristicFormula)EditorGUILayout.EnumPopup (tgs.pathFindingHeuristicFormula);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Max Search Cost", GUILayout.Width (120));
			tgs.pathFindingMaxCost = EditorGUILayout.IntField (tgs.pathFindingMaxCost, GUILayout.Width (100));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Diagonals", GUILayout.Width (120));
			tgs.pathFindingUseDiagonals = EditorGUILayout.Toggle (tgs.pathFindingUseDiagonals, GUILayout.Width (40));
			GUILayout.Label ("Heavy Diagonals", GUILayout.Width (120));
			tgs.pathFindingHeavyDiagonals = EditorGUILayout.Toggle (tgs.pathFindingHeavyDiagonals, GUILayout.Width (40));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Separator ();

			if (!Application.isPlaying) {
				EditorGUILayout.BeginVertical (blackStyle);
				EditorGUILayout.BeginHorizontal ();
				DrawTitleLabel ("Grid Editor");
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Export Settings")) {
					if (EditorUtility.DisplayDialog ("Export Grid Settings", "This option will add a TGS Config component to this game object with current cell settings. You can restore this configuration just enabling this new component.", "Ok", "Cancel")) {
						CreatePlaceholder ();
					}
				}
				if (GUILayout.Button ("Reset")) {
					if (EditorUtility.DisplayDialog ("Reset Grid", "Reset cells to their default values?", "Ok", "Cancel")) {
						ResetCells ();
						GUIUtility.ExitGUI ();
						return;
					}
				}
				EditorGUILayout.EndHorizontal ();

				if (cellSelectedIndex < 0 || cellSelectedIndex >= tgs.cells.Count) {
					GUILayout.Label ("Click on a cell in Scene View to edit its properties.");
				} else {
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Selected Cell", GUILayout.Width (120));
					GUILayout.Label (cellSelectedIndex.ToString (), GUILayout.Width (120));
					EditorGUILayout.EndHorizontal ();
			
					bool needsRedraw = false;
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("  Visible", GUILayout.Width (120));
					bool cellVisible = tgs.cells [cellSelectedIndex].visible;
					tgs.cells [cellSelectedIndex].visible = EditorGUILayout.Toggle (cellVisible);
					if (tgs.cells [cellSelectedIndex].visible != cellVisible) {
						needsRedraw = true;
					}
					EditorGUILayout.EndHorizontal ();
			
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("  Territory Index", GUILayout.Width (120));
					cellTerritoryIndex = EditorGUILayout.IntField (cellTerritoryIndex, GUILayout.Width (40));
					if (GUILayout.Button ("Set Territory", GUILayout.Width (100)) && cellTerritoryIndex != tgs.cells [cellSelectedIndex].territoryIndex) {
						tgs.CellSetTerritory (cellSelectedIndex, cellTerritoryIndex);
						needsRedraw = true;
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("  Color", GUILayout.Width (120));
					cellColor = EditorGUILayout.ColorField (cellColor, GUILayout.Width (40));
					GUILayout.Label ("  Texture", GUILayout.Width (60));
					cellTextureIndex = EditorGUILayout.IntField (cellTextureIndex, GUILayout.Width (40));
					if (GUILayout.Button ("Set", GUILayout.Width (40))) {
						tgs.CellToggleRegionSurface (cellSelectedIndex, true, cellColor, false, cellTextureIndex);
						needsRedraw = true;
					}
					if (GUILayout.Button ("Clear", GUILayout.Width (40))) {
						tgs.CellHideRegionSurface (cellSelectedIndex);
						needsRedraw = true;
					}
					EditorGUILayout.EndHorizontal ();

					if (needsRedraw) {
						RefreshGrid ();
						GUIUtility.ExitGUI ();
						return;
					}
				}

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Textures", GUILayout.Width (120));
				EditorGUILayout.EndHorizontal ();

				if (toggleButtonStyleNormal == null) {
					toggleButtonStyleNormal = "Button";
					toggleButtonStyleToggled = new GUIStyle (toggleButtonStyleNormal);
					toggleButtonStyleToggled.normal.background = toggleButtonStyleToggled.active.background;
				}

				int textureMax = tgs.textures.Length - 1;
				while (textureMax>=1 && tgs.textures[textureMax]==null) {
					textureMax--;
				}
				textureMax++;
				if (textureMax >= tgs.textures.Length)
					textureMax = tgs.textures.Length - 1;

				for (int k=1; k<=textureMax; k++) {
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("  " + k.ToString (), GUILayout.Width (40));
					tgs.textures [k] = (Texture2D)EditorGUILayout.ObjectField (tgs.textures [k], typeof(Texture2D), false);
					if (tgs.textures [k] != null) {
						if (GUILayout.Button (new GUIContent ("T", "Texture mode - if enabled, you can paint several cells just clicking over them."), textureMode == k ? toggleButtonStyleToggled : toggleButtonStyleNormal, GUILayout.Width (20))) {
							textureMode = textureMode == k ? 0 : k;
						}
						if (GUILayout.Button (new GUIContent ("X", "Remove texture"), GUILayout.Width (20))) {
							if (EditorUtility.DisplayDialog ("Remove texture", "Are you sure you want to remove this texture?", "Yes", "No")) {
								tgs.textures [k] = null;
								GUIUtility.ExitGUI ();
								return;
							}
						}
					}
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.Separator ();

			if (tgs.isDirty) {
				serializedObject.UpdateIfDirtyOrScript();
				isDirty.boolValue = false;
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty (target);

				// Hide mesh in Editor
				HideEditorMesh ();

				SceneView.RepaintAll ();

			}
		}

		void OnSceneGUI ()
		{
			if (tgs == null || Application.isPlaying)
				return;
			Event e = Event.current;
			tgs.CheckRay (HandleUtility.GUIPointToWorldRay (e.mousePosition));
			if (cellHighlightedIndex != tgs.cellHighlightedIndex) {
				cellHighlightedIndex = tgs.cellHighlightedIndex;
				SceneView.RepaintAll ();
			}
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			if (e.GetTypeForControl (controlID) == EventType.MouseDown) {
				if (cellHighlightedIndex != cellSelectedIndex) {
					cellSelectedIndex = cellHighlightedIndex;
					if (textureMode > 0) {
						tgs.CellToggleRegionSurface (cellSelectedIndex, true, Color.white, false, textureMode);
						SceneView.RepaintAll ();
					}
					if (cellSelectedIndex >= 0) {
						cellTerritoryIndex = tgs.CellGetTerritoryIndex (cellSelectedIndex);
						cellColor = tgs.CellGetColor (cellSelectedIndex);
						if (cellColor.a == 0)
							cellColor = Color.white;
						cellTextureIndex = tgs.CellGetTextureIndex (cellSelectedIndex);
					}
					EditorUtility.SetDirty (target);
				}
			}
			if (cellSelectedIndex >= 0 && cellSelectedIndex < tgs.cells.Count) {
				Vector3 pos = tgs.CellGetPosition (cellSelectedIndex);
				Handles.color = colorSelection;
				Handles.DrawSolidDisc (pos, tgs.transform.forward, HandleUtility.GetHandleSize (pos) * 0.075f);
			}
		}

		#region Utility functions

		void HideEditorMesh ()
		{
			Renderer[] rr = tgs.GetComponentsInChildren<Renderer> (true);
			for (int k=0; k<rr.Length; k++) {
				#if UNITY_5_5_OR_NEWER
				EditorUtility.SetSelectedRenderState(rr[k], EditorSelectedRenderState.Hidden);
				#else
				EditorUtility.SetSelectedWireframeHidden (rr [k], true);
				#endif			
			}
		}
			
		Texture2D MakeTex (int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];
			
			for (int i = 0; i < pix.Length; i++)
				pix [i] = col;
			
			Texture2D result = new Texture2D (width, height);
			result.SetPixels (pix);
			result.Apply ();
			
			return result;
		}

		void DrawTitleLabel (string s)
		{
			if (titleLabelStyle == null)
				titleLabelStyle = new GUIStyle (GUI.skin.label);
			titleLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color (0.52f, 0.66f, 0.9f) : new Color (0.22f, 0.36f, 0.6f);
			titleLabelStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label (s, titleLabelStyle);
		}

		void DrawInfoLabel (string s)
		{
			if (infoLabelStyle == null)
				infoLabelStyle = new GUIStyle (GUI.skin.label);
			infoLabelStyle.normal.textColor = new Color (0.76f, 0.52f, 0.52f);
			GUILayout.Label (s, infoLabelStyle);
		}

		void ResetCells ()
		{
			cellSelectedIndex = -1;
			cellColor = Color.white;
			tgs.GenerateMap ();
			RefreshGrid ();
		}

		void RefreshGrid ()
		{
			tgs.Redraw ();
			HideEditorMesh ();
			EditorUtility.SetDirty (target);
			SceneView.RepaintAll ();
		}

		void CreatePlaceholder ()
		{
			TGSConfig configComponent = tgs.gameObject.AddComponent<TGSConfig> ();
			configComponent.textures = tgs.textures;
			configComponent.config = tgs.CellGetConfigurationData ();
			configComponent.enabled = false;
		}

		#endregion

	}

}
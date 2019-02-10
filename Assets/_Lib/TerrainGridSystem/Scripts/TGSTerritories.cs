using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TGS.Geom;
using TGS.PathFinding;

namespace TGS {
	
	/* Event definitions */
	public delegate void TerritoryEvent(int territoryIndex);
	public delegate void TerritoryHighlightEvent(int territoryIndex, ref bool cancelHighlight);
	public delegate void TerritoryClickEvent(int territoryIndex, int buttonIndex);

	public partial class TerrainGridSystem : MonoBehaviour {

		public event TerritoryEvent OnTerritoryEnter;
		public event TerritoryEvent OnTerritoryExit;
		public event TerritoryClickEvent OnTerritoryClick;
		public event TerritoryHighlightEvent OnTerritoryHighlight;

		[NonSerialized]
		public List<Territory> territories;

		public Texture2D territoriesTexture;
		public Color territoriesTextureNeutralColor;


		[SerializeField]
		int _numTerritories = 3;

		/// <summary>
		/// Gets or sets the number of territories.
		/// </summary>
		public int numTerritories { 
			get { return _numTerritories; } 
			set { if (_numTerritories!=value) {
					_numTerritories = Mathf.Clamp(value, 1, MAX_TERRITORIES);
					GenerateMap();
					isDirty = true;
				}
			}
		}

		[SerializeField]
		bool
			_showTerritories = false;
		
		/// <summary>
		/// Toggle frontiers visibility.
		/// </summary>
		public bool showTerritories { 
			get {
				return _showTerritories; 
			}
			set {
				if (value != _showTerritories) {
					_showTerritories = value;
					isDirty = true;
					if (!_showTerritories && territoryLayer != null) {
						territoryLayer.SetActive (false);
						ClearLastOver();
					} else {
						Redraw ();
					}
				}
			}
		}

		[SerializeField]
		bool _colorizeTerritories = false;
		/// <summary>
		/// Toggle colorize countries.
		/// </summary>
		public bool colorizeTerritories { 
			get {
				return _colorizeTerritories; 
			}
			set {
				if (value != _colorizeTerritories) {
					_colorizeTerritories = value;
					isDirty = true;
					if (!_colorizeTerritories && surfacesLayer!=null) {
						DestroySurfaces();
					} else {
						Redraw();
					}
				}
			}
		}

		[SerializeField]
		float _colorizedTerritoriesAlpha = 0.7f;
		public float colorizedTerritoriesAlpha { 
			get { return _colorizedTerritoriesAlpha; } 
			set { if (_colorizedTerritoriesAlpha!=value) {
					_colorizedTerritoriesAlpha = value;
					isDirty = true;
					UpdateColorizedTerritoriesAlpha();
				}
			}
		}


		[SerializeField]
		Color
			_territoryHighlightColor = new Color (1, 0, 0, 0.7f);
		
		/// <summary>
		/// Fill color to use when the mouse hovers a territory's region.
		/// </summary>
		public Color territoryHighlightColor {
			get {
				return _territoryHighlightColor;
			}
			set {
				if (value != _territoryHighlightColor) {
					_territoryHighlightColor = value;
					isDirty = true;
					if (hudMatTerritoryOverlay != null && _territoryHighlightColor != hudMatTerritoryOverlay.color) {
						hudMatTerritoryOverlay.color = _territoryHighlightColor;
					}
					if (hudMatTerritoryGround != null && _territoryHighlightColor != hudMatTerritoryGround.color) {
						hudMatTerritoryGround.color = _territoryHighlightColor;
					}
				}
			}
		}

		
		[SerializeField]
		Color
			_territoryFrontierColor = new Color (0, 1, 0, 1.0f);
		
		/// <summary>
		/// Territories border color
		/// </summary>
		public Color territoryFrontiersColor {
			get {
				if (territoriesMat != null) {
					return territoriesMat.color;
				} else {
					return _territoryFrontierColor;
				}
			}
			set {
				if (value != _territoryFrontierColor) {
					_territoryFrontierColor = value;
					isDirty = true;
					if (territoriesMat != null && _territoryFrontierColor != territoriesMat.color) {
						territoriesMat.color = _territoryFrontierColor;
					}
				}
			}
		}

		public float territoryFrontiersAlpha {
			get {
				return _territoryFrontierColor.a;
			}
			set {
				if (_territoryFrontierColor.a!=value) {
					_territoryFrontierColor = new Color(_territoryFrontierColor.r, _territoryFrontierColor.g, _territoryFrontierColor.b, value);
				}
				if (_territoryDisputedFrontierColor.a!=value) {
					_territoryDisputedFrontierColor = new Color(_territoryDisputedFrontierColor.r, _territoryDisputedFrontierColor.g, _territoryDisputedFrontierColor.b, value);
				}
			}
		}

		
		[SerializeField]
		Color
			_territoryDisputedFrontierColor;
		
		/// <summary>
		/// Territories disputed borders color
		/// </summary>
		public Color territoryDisputedFrontierColor {
			get {
				if (territoriesDisputedMat != null) {
					return territoriesDisputedMat.color;
				} else {
					return _territoryDisputedFrontierColor;
				}
			}
			set {
				if (value != _territoryDisputedFrontierColor) {
					_territoryDisputedFrontierColor = value;
					isDirty = true;
					if (territoriesDisputedMat != null && _territoryDisputedFrontierColor != territoriesDisputedMat.color) {
						territoriesDisputedMat.color = _territoryDisputedFrontierColor;
					}
				}
			}
		}


		[SerializeField]
		bool _showTerritoriesOuterBorder = true;

		/// <summary>
		/// Shows perimetral/outer border of territories?
		/// </summary>
		/// <value><c>true</c> if show territories outer borders; otherwise, <c>false</c>.</value>
		public bool showTerritoriesOuterBorders {
			get { return _showTerritoriesOuterBorder; }
			set { if (_showTerritoriesOuterBorder!=value) { _showTerritoriesOuterBorder = value; isDirty = true; Redraw (); } }
		}

		
		[SerializeField]
		bool _allowTerritoriesInsideTerritories = false;
		
		/// <summary>
		/// Set this property to true to allow territories to be surrounded by other territories.
		/// </summary>
		public bool allowTerritoriesInsideTerritories {
			get { return _allowTerritoriesInsideTerritories; }
			set { if (_allowTerritoriesInsideTerritories!=value) { _allowTerritoriesInsideTerritories = value; isDirty = true; } }
		}

		/// <summary>
		/// Returns Territory under mouse position or null if none.
		/// </summary>
		public Territory territoryHighlighted { get { return _territoryHighlighted; } }
		
		/// <summary>
		/// Returns currently highlighted territory index in the countries list.
		/// </summary>
		public int territoryHighlightedIndex { get { return _territoryHighlightedIndex; } }

		/// <summary>
		/// Returns Territory index which has been clicked
		/// </summary>
		public int territoryLastClickedIndex { get { return _territoryLastClickedIndex; } }


		#region Public Territories Functions

		/// <summary>
		/// Uncolorize/hide specified territory by index in the territories collection.
		/// </summary>
		public void TerritoryHideRegionSurface (int territoryIndex) {
			if (_territoryHighlightedIndex != territoryIndex) {
				int cacheIndex = GetCacheIndexForTerritoryRegion (territoryIndex);
				if (surfaces.ContainsKey (cacheIndex)) {
					if (surfaces[cacheIndex] == null) {
						surfaces.Remove(cacheIndex);
					} else {
						surfaces [cacheIndex].SetActive (false);
					}
				}
			}
			territories [territoryIndex].region.customMaterial = null;
		}

		public void TerritoryToggleRegionSurface (int territoryIndex, bool visible, Color color) {
			TerritoryToggleRegionSurface (territoryIndex, visible, color, null, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a territory by indexes.
		/// </summary>
		public void TerritoryToggleRegionSurface (int territoryIndex, bool visible, Color color,  bool refreshGeometry, Texture2D texture) {
			TerritoryToggleRegionSurface(territoryIndex, visible, color, texture, Vector2.one, Vector2.zero, 0);
		}

		/// <summary>
		/// Colorize specified region of a territory by indexes.
		/// </summary>
		public void TerritoryToggleRegionSurface (int territoryIndex, bool visible, Color color, Texture2D texture, Vector2 textureScale, Vector2 textureOffset, float textureRotation) {
			if (!visible) {
				TerritoryHideRegionSurface (territoryIndex);
				return;
			}
			GameObject surf = null;
			Region region = territories [territoryIndex].region;
			int cacheIndex = GetCacheIndexForTerritoryRegion (territoryIndex);
			// Checks if current cached surface contains a material with a texture, if it exists but it has not texture, destroy it to recreate with uv mappings
			if (surfaces.ContainsKey (cacheIndex) && surfaces [cacheIndex] != null) 
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
			bool isHighlighted = territoryHighlightedIndex == territoryIndex;
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
				surf = GenerateTerritoryRegionSurface (territoryIndex, surfMaterial, textureScale, textureOffset, textureRotation);
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
		}

		/// <summary>
		/// Returns a list of neighbour cells for specificed cell index.
		/// </summary>
		public List<Territory> TerritoryGetNeighbours(int territoryIndex) {
			List<Territory>neighbours = new List<Territory>();
			Region region = territories[territoryIndex].region;
			for (int k=0;k<region.neighbours.Count;k++) {
				neighbours.Add ( (Territory)region.neighbours[k].entity);
			}
			return neighbours;
		}

		/// <summary>
		/// Colors a territory and fades it out during "duration" in seconds.
		/// </summary>
		public void TerritoryFadeOut(int territoryIndex, Color color, float duration) {
			if (_territoryHighlightedIndex==territoryIndex) return;
			TerritoryToggleRegionSurface(territoryIndex, true, color);
			int cacheIndex = GetCacheIndexForTerritoryRegion (territoryIndex);
			if (surfaces.ContainsKey (cacheIndex)) {
				GameObject territorySurface = surfaces[cacheIndex];
				Material mat = Instantiate(territorySurface.GetComponent<Renderer>().sharedMaterial);
				mat.color = color;
				mat.hideFlags = HideFlags.DontSave;
				territorySurface.GetComponent<Renderer>().sharedMaterial = mat;
				territories[territoryIndex].region.customMaterial = mat;
				SurfaceFader.FadeOut(this, territorySurface, territories[territoryIndex].region, color, duration); // fader = territorySurface.GetComponent<SurfaceFader>() ?? territorySurface.AddComponent<SurfaceFader>();
			}
		}

		/// <summary>
		/// Specifies if a given cell is visible.
		/// </summary>
		public void TerritorySetVisible(int territoryIndex, bool visible) {
			if (territoryIndex<0 || territoryIndex>=territories.Count) return;
			territories[territoryIndex].visible = visible;
			if (territoryIndex == _territoryLastOverIndex) {
				ClearLastOver();
			}
		}

		/// <summary>
		/// Specifies if a given cell is visible.
		/// </summary>
		public void TerritorySetFrontierColor(int territoryIndex, Color color) {
			if (territoryIndex<0 || territoryIndex>=territories.Count) return;
			Territory terr = territories[territoryIndex];
			if (terr.frontierColor != color) {
				terr.frontierColor = color;
				DrawTerritoryBorders();
			}
		}


		/// <summary>
		/// Returns true if territory is visible
		/// </summary>
		public bool TerritoryIsVisible(int territoryIndex) {
			if (territoryIndex<0 || territoryIndex>=territories.Count) return false;
			return territories[territoryIndex].visible;
		}

		/// <summary>
		/// Returns the territory object under position in local coordinates
		/// </summary>
		public Territory TerritoryGetAtPosition(Vector2 localPosition) {
			return GetTerritoryAtPoint(localPosition);
		}

		/// <summary>
		/// Automatically generates territories based on the different colors included in the texture.
		/// </summary>
		/// <param name="neutral">This color won't generate any texture.</param>
		public void CreateTerritories(Texture2D texture, Color neutral) {

			if (texture == null || cells == null) return;

			List<Color> dsColors = new List<Color>();
			int cellCount = cells.Count;
			Color[] colors = texture.GetPixels();
			for (int k=0;k<cellCount;k++) {
				if (!cells[k].visible) continue;
				Vector2 uv = cells[k].center;
				uv.x += 0.5f;
				uv.y += 0.5f;

				int x = (int)(uv.x * texture.width);
				int y = (int)(uv.y * texture.height);
				int pos = y * texture.width + x;
				if (pos<0 || pos>colors.Length) continue;
				Color pixelColor = colors[pos];
				int territoryIndex = dsColors.IndexOf(pixelColor);
				if (territoryIndex<0) {
					dsColors.Add (pixelColor);
					territoryIndex = dsColors.Count - 1;
				}
				CellSetTerritory(k, territoryIndex);
				if (territoryIndex>=255) break;
			}
			if (dsColors.Count>0) {
				_numTerritories = dsColors.Count;
				_showTerritories = true;

				if (territories==null) {
					territories = new List<Territory>(_numTerritories);
				} else {
					territories.Clear();
				}
				for (int c = 0; c < _numTerritories; c++) {
					Territory territory = new Territory (c.ToString ());
					Color territoryColor = dsColors[c];
					if (territoryColor.r!=neutral.r || territoryColor.g!=neutral.g || territoryColor.b!=neutral.b) {
						territory.fillColor = territoryColor;
					} else {
						territory.fillColor = new Color(0,0,0,0);
						territory.visible = false;
					}
					territories.Add (territory);
				}
				isDirty = true;
				Redraw ();
			}
		}

		#endregion


	
	}
}


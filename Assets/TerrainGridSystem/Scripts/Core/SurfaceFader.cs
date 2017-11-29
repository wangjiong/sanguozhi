using UnityEngine;
using System.Collections;

namespace TGS {

	public class SurfaceFader : MonoBehaviour {

		Material fadeMaterial;
		float startTime, duration;
		TerrainGridSystem tgs;
		Color color;
		Renderer _renderer;
		Region region;

		public static void FadeOut(TerrainGridSystem tgs, GameObject surface, Region region, Color color, float duration) {
			SurfaceFader fader = surface.GetComponent<SurfaceFader>();
			if (fader!=null) DestroyImmediate(fader);
			fader = surface.AddComponent<SurfaceFader>();
			fader.tgs = tgs;
			fader.startTime = Time.time;
			fader.duration = duration + 0.0001f;
			fader.color = color;
			fader.region = region;
		}

		void Start () {
			_renderer = GetComponent<Renderer>();
		}

		// Update is called once per frame
		void Update () {
			float elapsed = Time.time - startTime;
			if (tgs.highlightedObj != gameObject) {
				float newAlpha = Mathf.Clamp01 (1.0f - elapsed / duration);
				SetAlpha(newAlpha);
			}
			if (elapsed >= duration) {
				SetAlpha (0);
				region.customMaterial = null;
				tgs.HideHighlightedObject(gameObject);	// forces to highlight again this cell refreshing hud material (if it's the cell under cursor)
				DestroyImmediate (this);
			}
		}

		void SetAlpha(float newAlpha) {
			Color newColor = new Color(color.r, color.g, color.b, newAlpha);
			_renderer.sharedMaterial.color = newColor;
		}

		void OnDestroy() {
			region.customMaterial = null;
		}

	}

}
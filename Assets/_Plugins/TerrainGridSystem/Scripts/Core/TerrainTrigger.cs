using UnityEngine;
using System.Collections;

namespace TGS {
	public class TerrainTrigger : MonoBehaviour {

		// Use this for initialization
		TerrainGridSystem[] ths;

		void Start () {
			if (GetComponent<TerrainCollider> () == null) {
				gameObject.AddComponent<TerrainCollider> ();
			}
			ths = transform.GetComponentsInChildren<TerrainGridSystem>();
			if (ths==null || ths.Length == 0) {
				Debug.LogError("Missing Terrain Highlight System reference in Terrain Trigger script.");
				DestroyImmediate (this);
			}
		}

		void OnMouseEnter () {
			for (int k=0;k<ths.Length;k++) {
				if (ths[k]!=null) {
					ths[k].mouseIsOver = true;
				}
			}
		}
		
		void OnMouseExit () {
			// Make sure it's outside of grid
			Vector3 mousePos = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay (mousePos);
			RaycastHit[] hits = Physics.RaycastAll (Camera.main.transform.position, ray.direction, 5000);
			if (hits.Length > 0) {
				for (int k=0; k<hits.Length; k++) {
					if (ths[0]==null || hits [k].collider.gameObject == this.ths[0].terrain.gameObject) 
						return; 
				}
			}
			for (int k=0;k<ths.Length;k++) {
				if (ths[k]!=null) {
					ths[k].mouseIsOver = false;
				}
			}
		}

	}

}
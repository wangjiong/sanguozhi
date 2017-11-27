using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wujiang : MonoBehaviour , IPointerClickHandler{
    HighlightableObject ho;
    bool selected;
    public void OnPointerClick(PointerEventData eventData) {
        print("eventData");
        if (ho == null) {
            ho = gameObject.AddComponent<HighlightableObject>();
        }
        selected = !selected;
        if (selected) {
            ho.ConstantOnImmediate(Color.red);
        } else {
            ho.Off();
        }
    }

    void Start () {
		
	}
}

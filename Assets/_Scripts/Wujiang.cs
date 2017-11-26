using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wujiang : MonoBehaviour , IPointerClickHandler{

    public void OnPointerClick(PointerEventData eventData) {
        print("eventData");
    }

    void Start () {
		
	}

	void Update () {
		if(Input.GetMouseButton(0)){
			//print (tag + "GetMouseButton");
		}
	}
}

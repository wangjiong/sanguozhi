using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateGeneralRow : MonoBehaviour {
    public GameObject mRow;
    public Scrollbar mScrollbar;

    public GameObject[] mRows = new GameObject[19];

	void Start () {
        for (int i=0;i<19;i++) {
            GameObject row = Instantiate(mRow);
            row.transform.SetParent(this.transform);
            row.transform.localScale = new Vector3(1,1,1);
            row.name = "i " + i;
            row.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30 - i * 27.5f);

            General general = GameManager.sGenerals[i];
            row.transform.Find("Button01").Find("Text").GetComponent<Text>().text = general.name;
            row.transform.Find("Button02").Find("Text").GetComponent<Text>().text = general.tongshuai;
            row.transform.Find("Button03").Find("Text").GetComponent<Text>().text = general.wuli;
            row.transform.Find("Button04").Find("Text").GetComponent<Text>().text = general.zhili;
            row.transform.Find("Button05").Find("Text").GetComponent<Text>().text = general.zhengzhi;
            row.transform.Find("Button06").Find("Text").GetComponent<Text>().text = general.meili;

            mRows[i] = row;
        }
        mScrollbar.size = 1f / GameManager.sGenerals.Count;
    }
	
    public void ScrollbarOnValueChanged() {
        int index = (int)Mathf.Floor(mScrollbar.value / mScrollbar.size);
        print(index +" " + GameManager.sGenerals.Count);
        for (int i = 0; i < 19; i++) {
            GameObject row = mRows[i];
            if (i + index < GameManager.sGenerals.Count) {
                General general = GameManager.sGenerals[i + index];
                row.transform.Find("Button01").Find("Text").GetComponent<Text>().text = general.name;
                row.transform.Find("Button02").Find("Text").GetComponent<Text>().text = general.tongshuai;
                row.transform.Find("Button03").Find("Text").GetComponent<Text>().text = general.wuli;
                row.transform.Find("Button04").Find("Text").GetComponent<Text>().text = general.zhili;
                row.transform.Find("Button05").Find("Text").GetComponent<Text>().text = general.zhengzhi;
                row.transform.Find("Button06").Find("Text").GetComponent<Text>().text = general.meili;
            } else {
                row.transform.Find("Button01").Find("Text").GetComponent<Text>().text = "";
                row.transform.Find("Button02").Find("Text").GetComponent<Text>().text = "";
                row.transform.Find("Button03").Find("Text").GetComponent<Text>().text = "";
                row.transform.Find("Button04").Find("Text").GetComponent<Text>().text = "";
                row.transform.Find("Button05").Find("Text").GetComponent<Text>().text = "";
                row.transform.Find("Button06").Find("Text").GetComponent<Text>().text = "";
            }
        }

    }


}

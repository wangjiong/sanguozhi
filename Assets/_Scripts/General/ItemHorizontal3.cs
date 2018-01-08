using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneP.InfinityScrollView;

public class ItemHorizontal3 : InfinityBaseItem {
    static string TAG = "ItemHorizontal3==";
	public override void Reload(InfinityScrollView _infinity,int _index){
		base.Reload(_infinity, _index);
        print(TAG + "_index:" + _index);

        GameObject row = this.gameObject;
        General general = GameManager.sCurrentGenerals[_index];
        row.transform.Find("Button01").Find("Text").GetComponent<Text>().text = general.name;
        row.transform.Find("Button02").Find("Text").GetComponent<Text>().text = general.tongshuai;
        row.transform.Find("Button03").Find("Text").GetComponent<Text>().text = general.wuli;
        row.transform.Find("Button04").Find("Text").GetComponent<Text>().text = general.zhili;
        row.transform.Find("Button05").Find("Text").GetComponent<Text>().text = general.zhengzhi;
        row.transform.Find("Button06").Find("Text").GetComponent<Text>().text = general.meili;

    }
	
	public void OnClick1()
	{
		Sample2.Instance.OnClickItem (Index*2 + 1);
	}
	public void OnClick2()
	{
		Sample2.Instance.OnClickItem (Index*2 + 2);
	}
}

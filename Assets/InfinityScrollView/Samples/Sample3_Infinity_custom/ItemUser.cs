using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OneP.InfinityScrollView;
using OneP.Samples;
public class ItemUser : InfinityBaseItem {
	public Sprite spriteDefault;
	public Text text;
	public Image avatar;
	private string urlAva = "";
	public override void Reload(InfinityScrollView _infinity,int _index){
		base.Reload(_infinity,_index);
		//reset
		StopAllCoroutines();

		text.text = "User " + (Index+1);
		string idStr = Sample3.Instance.GetIDUser (Index);
		if (string.IsNullOrEmpty (idStr)) {
			avatar.sprite = spriteDefault;
		} else {
			string path = "http://graph.facebook.com/" + idStr + "/picture?type=square";
			urlAva = path;
			Sprite sprite = ResourceLoaderManager.Instance.GetMemorySprite (path);
			if (sprite != null) {
				avatar.sprite = sprite;
			} else {
				//Load new avatar
				avatar.sprite = spriteDefault;

				StartCoroutine (LoadAvatar (path));
			}
		}
	}
	
	private IEnumerator LoadAvatar(string path){
		yield return new WaitForSeconds (0.6f);
		if (path.Length> 0) {
			this.urlAva = path;
			ResourceLoaderManager.Instance.DownloadSpriteCustom (path, res => {
				if (res != null && res.url.Equals (this.urlAva)) {
					if (avatar != null) {
						avatar.sprite = res.sprite;
					}
				}
			});
		}
	}
	
	public void OnClickItem(){
		Sample3.Instance.OnClickItem (Index + 1);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

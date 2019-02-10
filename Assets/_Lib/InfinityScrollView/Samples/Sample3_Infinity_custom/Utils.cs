using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OneP.Samples
{
	#region singleton
	/// <summary>
	/// Singleton Class
	/// </summary>
	public abstract class Singleton<T> where T : new()
	{
		private static T singleton;
		public static T Instance
		{
			get
			{
				if (singleton == null)
				{
					singleton = new T();
				}
				return singleton;
			}
		}
		public static T instance
		{
			get
			{
				if (singleton == null)
				{
					singleton = new T();
				}
				return singleton;
			}
		}
	}
	#endregion

	#region SingletonMono
	/// <summary>
	/// Single Ton Monobehavior. unique in a scene
	/// </summary>
	public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T singleton;
		
		public static bool IsInstanceValid() { return singleton != null; }
		
		void Reset()
		{
			gameObject.name = typeof(T).Name;
		}
		
		public static T Instance
		{
			get
			{
				if (SingletonMono<T>.singleton == null)
				{
					SingletonMono<T>.singleton = (T)FindObjectOfType(typeof(T));
					if (SingletonMono<T>.singleton == null)
					{
						GameObject obj = new GameObject();
						obj.name = "[@" + typeof(T).Name + "]";
						SingletonMono<T>.singleton = obj.AddComponent<T>();
					}
				}
				
				return SingletonMono<T>.singleton;
			}
		}
		public static T instance
		{
			get
			{
				if (SingletonMono<T>.singleton == null)
				{
					SingletonMono<T>.singleton = (T)FindObjectOfType(typeof(T));
					if (SingletonMono<T>.singleton == null)
					{
						GameObject obj = new GameObject();
						obj.name = "[@" + typeof(T).Name + "]";
						SingletonMono<T>.singleton = obj.AddComponent<T>();
					}
				}
				
				return SingletonMono<T>.singleton;
			}
		}
	}
	#endregion
	
	#region Control Resource Manager
	public class CustomSpriteData{
		public Sprite sprite;
		public string url;
		public CustomSpriteData(){
		}
	}
	public class ResourceLoaderManager: Singleton<ResourceLoaderManager>
	{
		public Dictionary<string,WWW> GetLoader(){
			return dicResourceLoader;
		}
		private Dictionary<string,WWW> dicResourceLoader = new Dictionary<string, WWW> ();
		private Dictionary<string,Texture2D> dicTextureLoader = new Dictionary<string, Texture2D> ();
		private Dictionary<string,Sprite> dicSpriteLoader = new Dictionary<string, Sprite> ();
		private Dictionary<string,List<Action<ExtraInfoDownload>>> processQueue = new Dictionary<string,List<Action<ExtraInfoDownload>>> ();
		
		public bool PushToQueue(string url,Action<ExtraInfoDownload> actionCallback){
			List<Action<ExtraInfoDownload>> list=null;
			processQueue.TryGetValue (url, out list);
			if (list == null) {
				list = new List<Action<ExtraInfoDownload>> ();
				if (actionCallback != null) {
					list.Add (actionCallback);
				}
				processQueue [url] = list;
				
				return true;
			} else {
				if (actionCallback != null) {
					list.Add (actionCallback);
				}
				return false;
			}
		}
		
		public void DispathQueue(string url,ExtraInfoDownload info){
			List<Action<ExtraInfoDownload>> list=null;
			processQueue.TryGetValue (url, out list);
			if (list != null) {
				for (int i = 0; i < list.Count; i++) {
					if (list [i] != null) {
						list [i] (info);
					}
				}
				list.Clear ();
				processQueue.Remove (url);
			}	
		}
		
		public Sprite GetMemorySprite(string url){
			Sprite sprite = null;
			dicSpriteLoader.TryGetValue (url, out sprite);
			return sprite;
		}
		
		public void DownloadSprite(string url,Action<Sprite> callback=null){
			if (string.IsNullOrEmpty (url)) {
				return;
			}
			if (dicSpriteLoader.ContainsKey (url)) {
				callback (dicSpriteLoader [url]);
			} else {
				DownLoadTexture (url, texture => {
					if (texture != null) {
						Rect rect;
						if (texture.width > texture.height) {
							rect = new Rect(texture.width / 2 - texture.height / 2, 0, texture.height, texture.height);
						} else {
							rect = new Rect(0, texture.height / 2 - texture.width / 2, texture.width, texture.width);
						}
						
						Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
						dicSpriteLoader[url]=sprite;
						if (callback != null) {
							callback (sprite);
						}
					}	
				});
			}
		}
		public void DownloadSpriteCustom(string url,Action<CustomSpriteData> callback=null){
			CustomSpriteData customSpriteData = new CustomSpriteData ();
			if (string.IsNullOrEmpty (url)) {
				return;
			}
			if (dicSpriteLoader.ContainsKey (url)) {
				customSpriteData.sprite = dicSpriteLoader [url];
				customSpriteData.url = url;
				callback (customSpriteData);
			} else {
				DownLoadTexture (url, texture => {
					if (texture != null) {
						Rect rect;
						if (texture.width > texture.height) {
							rect = new Rect (texture.width / 2 - texture.height / 2, 0, texture.height, texture.height);
						} else {
							rect = new Rect (0, texture.height / 2 - texture.width / 2, texture.width, texture.width);
						}
						
						Sprite sprite = Sprite.Create (texture, rect, new Vector2 (0.5f, 0.5f));
						dicSpriteLoader [url] = sprite;
						if (callback != null) {
							customSpriteData.sprite = sprite;
							customSpriteData.url = url;
							callback (customSpriteData);
						}
					}	
				});
			}
		}
		
		public void DownLoadTexture(string url,Action<Texture2D> callback=null){
			if (string.IsNullOrEmpty (url)) {
				return;
			}
			if (dicTextureLoader.ContainsKey (url)) {
				callback (dicTextureLoader [url]);
			} else {
				WWW w = GetWWW (url);
				if (w != null) {
					if (w.texture != null) {
						dicTextureLoader [url] = w.texture;
						if (callback != null) {
							callback (w.texture);
						}
					}
				} else {
					DownloadFileUtil.Instance.OnDownloadFile (url,
					                                          wwwCallBack => {
						if (wwwCallBack != null && wwwCallBack.www!=null&& string.IsNullOrEmpty (wwwCallBack.www.error)) {
							dicTextureLoader [url] = wwwCallBack.www.texture;
							if (callback != null) {
								callback (wwwCallBack.www.texture);
							}
						}
					}, 
					process => {
					}, 5, 3);
				}
			}
		}
		public WWW GetWWW(string url){
			WWW w = null;
			if (string.IsNullOrEmpty (url)) {
				return null;
			}
			if (dicResourceLoader.TryGetValue (url, out w)) {
				if (w!=null&&string.IsNullOrEmpty (w.error)) {
					return w;
				}
			}
			return null;
		}
		public void AddWWW(string key, WWW w)
		{
			if (w != null && string.IsNullOrEmpty (w.error)) {
				dicResourceLoader [key] = w;
			}
		}
		
		public void DisposeWWW(string url,WWW w)// using to free memory
		{
			try
			{
				if(w!=null)
				{
					if (w.assetBundle != null) {
						w.assetBundle.Unload(false);
					}
					else if (w.texture != null) {
						GameObject.Destroy (w.texture);
					}
					w.Dispose();
					w=null;
					dicSpriteLoader.Remove (url);
					dicTextureLoader.Remove (url);
					dicResourceLoader.Remove (url);
				}
			}
			catch(Exception ex) {
				Debug.LogError ("Unload Resource Exception:" +url+ ex.Message);
			}
		}
		
	}
	#endregion
	
	#region Download File Util
	public class ExtraInfoDownload{
		public WWW www;
		public float timeDownload;
		public int retryTime;
		public ExtraInfoDownload(WWW _www,float _timeDownload,int _retryTime){
			this.www = _www;
			this.timeDownload = _timeDownload;
			this.retryTime = _retryTime;
		}
	}
	public class DownloadFileUtil : SingletonMono<DownloadFileUtil> {
		
		public void OnDownloadFile(string url,Action<ExtraInfoDownload> resultCallback,Action<float> processCallback,float secondTimeout,int maxRetry=3)
		{
			if (string.IsNullOrEmpty (url)) {
				return;
			}
			if (secondTimeout < 0)
				secondTimeout = 1;
			if (ResourceLoaderManager.Instance.PushToQueue (url, resultCallback)) {
				StartCoroutine( OnDownloadFileRoutine(url,processCallback,secondTimeout,maxRetry,0));
			}
		}
		
		private IEnumerator OnDownloadFileRoutine(string url,Action<float> processCallback,float secondTimeout,int maxRetry, int retryCount=0)
		{
			WWW w = null;
			float timeDownload = 0;
			w = ResourceLoaderManager.Instance.GetWWW (url);
			if (w!=null) {// kiem tra co cache hay khong
				ResourceLoaderManager.Instance.DispathQueue(url,new ExtraInfoDownload(w,0,retryCount));
				if (processCallback != null) {
					processCallback (1);
				}
				yield break;
			} else {
				w = new WWW (url);
				float cachePercent = 0;
				int unDownLoadStep = 0;
				bool isFailDownload = false;
				while (!w.isDone) {
					if (Mathf.Abs (cachePercent - w.progress) > 0.0f) {
						cachePercent = w.progress;
						unDownLoadStep = 0;
						if (processCallback != null) {
							processCallback (cachePercent);
						}
					} else { // truong hop mang qua cham hoac download file bi stop do rot mang...
						unDownLoadStep++;
						if (unDownLoadStep > 10 * secondTimeout) { //secondTimeout
							isFailDownload = true;
							// Download stop because freeze forever
							break;
						}
						if (processCallback != null) {
							processCallback (cachePercent);
						}
						yield return new WaitForSeconds (0.1f);
						timeDownload += 0.1f;
					}
					if (!string.IsNullOrEmpty (w.error)) {
						isFailDownload = true;
					}
					yield return null;
				}
				if (isFailDownload) {
					RetryAttempDownloadFile (url, processCallback, secondTimeout, maxRetry, retryCount);
				} else { // Download Succesful
					ResourceLoaderManager.Instance.AddWWW (url, w);
					ResourceLoaderManager.Instance.DispathQueue(url,new ExtraInfoDownload(w,timeDownload,retryCount));
				}
			}
		}
		public void RetryAttempDownloadFile(string url,Action<float> processCallback,float secondTimeout,int maxRetry, int retryCount)
		{
			if(retryCount<maxRetry)// retry lai lan nua
			{
				retryCount++;
				StartCoroutine( OnDownloadFileRoutine(url,processCallback,secondTimeout,maxRetry,retryCount));
			}
			else
			{
				// total fail
				ResourceLoaderManager.Instance.DispathQueue(url,null);
			}
		}
	}
	#endregion
}
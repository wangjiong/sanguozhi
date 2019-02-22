using UnityEngine;
using System.Collections;

namespace OneP.Samples
{
	public enum SampleScene{
		Sample1			=0,
		Sample2			=1,
		Sample3			=2,
		Sample1_Invert	=3,
		Sample2_Invert	=4,
		Sample3_Invert	=5,
	}
	public class SampleGlobalValue{
		public static SampleScene sceneNow=SampleScene.Sample1;
		public static void GoToNextSample(){
			int next = (int)sceneNow;
			next++;
			next=next%6;
			sceneNow = (SampleScene)next;
			Application.LoadLevel (sceneNow.ToString ());
		}
	}
}

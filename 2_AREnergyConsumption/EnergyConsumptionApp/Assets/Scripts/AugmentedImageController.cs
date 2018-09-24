using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentedImageController : MonoBehaviour {

	public AugmentedImage Image;
	public Text DebugText;
	public string DebugString
	{
		get { return DebugText.text; }
		set { DebugText.text = value; }
	}
	

	public void Update()
	{
		if (Image == null || Image.TrackingState != TrackingState.Tracking)
		{
			return;
		}

	}
}

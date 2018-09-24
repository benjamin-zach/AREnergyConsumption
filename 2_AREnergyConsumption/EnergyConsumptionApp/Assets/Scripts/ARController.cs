using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

using GoogleARCore;
using GoogleARCore.Examples.AugmentedImage;
using System;

/// <summary>C:\Users\bzach\Documents\AugmentedImages\Assets\Scripts\DataReader.cs
/// Controller for AugmentedImage example.
/// </summary>
public class ARController : MonoBehaviour
{
	//public List<GameObject> panels;
	//public Text DebugText;
	//public Canvas UICanvas;
	public Camera ARCamera;
	public Canvas MainCanvas;	
	//public TextAsset DataFile;
	private string DataFilePath;

	public AugmentedImageController AugmentedImageVisualizerPrefab;

	/// <summary>
	/// The overlay containing the fit to scan user guide.
	/// </summary>
	public GameObject FitToScanOverlay;

	private Dictionary<int, AugmentedImageController> m_Visualizers
		= new Dictionary<int, AugmentedImageController>();

	private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

	/// <summary>
	/// 
	/// 
	public void Start()
	{
		//AugmentedImageVisualizerPrefab.gameObject.SetActive(false);
		//DebugText.gameObject.SetActive(false);

		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			DataFilePath = Application.dataPath + "/StreamingAssets/Data.xml";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			DataFilePath = "jar:file://" + Application.dataPath + "!/assets/Data.xml";
		}
		else
		{
			Debug.Log("Could not set data file path for platform " + Application.platform.ToString());
		}

	}
	/// The Unity Update method.
	/// </summary>
	public void Update()
	{

		// Exit the app when the 'back' button is pressed.
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}

		// Check that motion tracking is tracking.
		if (Session.Status != SessionStatus.Tracking)
		{
			return;
		}

		//UpdateDebug();
		UpdateUICanvas();

	}

	private void UpdateDebug()
	{
		// Get updated augmented images for this frame.
		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);
		//bool CanvasActive = false;

		// Create visualizers and anchors for updated augmented images that are tracking and do not previously
		// have a visualizer. Remove visualizers for stopped images.
		foreach (var image in m_TempAugmentedImages)
		{
			AugmentedImageController visualizer = null;
			m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
			if (image.TrackingState == TrackingState.Tracking && visualizer == null)
			{
				Debug.Log("Found image " + image.DatabaseIndex.ToString() + ". Setting up ui canvas");

				// Create an anchor to ensure that ARCore keeps tracking this augmented image.
				Anchor anchor = image.CreateAnchor(image.CenterPose);
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.AddComponent<Rigidbody>();
				cube.GetComponent<Rigidbody>().useGravity = false;
				cube.transform.position = anchor.transform.position;
			}
		}

		foreach (var visualizer in m_Visualizers.Values)
		{
			if (visualizer.Image.TrackingState == TrackingState.Tracking)
			{
				FitToScanOverlay.SetActive(false);
				return;
			}
		}
		FitToScanOverlay.SetActive(true);

		//UICanvas.gameObject.SetActive(CanvasActive);
	}

	private void UpdateUICanvas()
	{
		// Get updated augmented images for this frame.
		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);
		//bool CanvasActive = false;

		// Create visualizers and anchors for updated augmented images that are tracking and do not previously
		// have a visualizer. Remove visualizers for stopped images.
		foreach (var image in m_TempAugmentedImages)
		{
			AugmentedImageController visualizer = null;
			m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
			if (image.TrackingState == TrackingState.Tracking && visualizer == null)
			{
				Debug.Log("Found image " + image.DatabaseIndex.ToString() + ". Setting up ui canvas");

				// Create an anchor to ensure that ARCore keeps tracking this augmented image.
				Anchor anchor = image.CreateAnchor(image.CenterPose);
				visualizer = (AugmentedImageController)Instantiate(AugmentedImageVisualizerPrefab, anchor.transform);
				if(visualizer == null)
				{
					Debug.Log("Visualizer is still null");
				}
				visualizer.transform.RotateAround(visualizer.transform.position, visualizer.transform.right, 90);
				visualizer.gameObject.SetActive(true);
				visualizer.DebugString = "House " + image.DatabaseIndex.ToString();
				visualizer.Image = image;
				m_Visualizers.Add(image.DatabaseIndex, visualizer);

				//CanvasActive = true;
				//Vector3 testPos = Camera.main.WorldToScreenPoint(anchor.transform.position);
				//Debug.Log("Test pos: " + testPos.ToString());
				//UICanvas.transform.position = testPos;
			}
			else if(image.TrackingState == TrackingState.Stopped && visualizer != null)
			{
				m_Visualizers.Remove(image.DatabaseIndex);
				GameObject.Destroy(visualizer.gameObject);
			}
		}

		foreach (var visualizer in m_Visualizers.Values)
		{
			if (visualizer.Image.TrackingState == TrackingState.Tracking)
			{
				FitToScanOverlay.SetActive(false);
				return;
			}
		}
		FitToScanOverlay.SetActive(true);

		//UICanvas.gameObject.SetActive(CanvasActive);
	}

	/*private void UpdateAugmentedImage()
	{
		// Get updated augmented images for this frame.
		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

		// Create visualizers and anchors for updated augmented images that are tracking and do not previously
		// have a visualizer. Remove visualizers for stopped images.
		foreach (var image in m_TempAugmentedImages)
		{
			AugmentedImageVisualizer visualizer = null;
			m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);

			if (image.TrackingState == TrackingState.Tracking && visualizer == null)
			{
				AugmentedImageVisualizerPrefab.gameObject.SetActive(true);
				// Create an anchor to ensure that ARCore keeps tracking this augmented image.
				Anchor anchor = image.CreateAnchor(image.CenterPose);
				visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizerPrefab, anchor.transform);
				visualizer.Image = image;
				m_Visualizers.Add(image.DatabaseIndex, visualizer);
			}
			else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
			{
				m_Visualizers.Remove(image.DatabaseIndex);
				GameObject.Destroy(visualizer.gameObject);
			}
		}

		// Show the fit-to-scan overlay if there are no images that are Tracking.
		foreach (var visualizer in m_Visualizers.Values)
		{
			if (visualizer.Image.TrackingState == TrackingState.Tracking)
			{
				FitToScanOverlay.SetActive(false);
				return;
			}
		}

		FitToScanOverlay.SetActive(true);

	}*/
}

public struct HouseData_t
{
	public struct TemperatureData_t
	{
		public int Inside;
		public int Outside;
	};
	public struct WaterData_t
	{
		public int ConsumptionToday;
		public int ConsumptionMonth;
		public int ConsumptionYear;
	}

	public TemperatureData_t TemperatureData;
	public WaterData_t WaterData;
}

class XMLReader
{

	private string DataFilePath;

	public XMLReader(string _DataFilePath)
	{
		Debug.Log("Loading data file " + _DataFilePath);
		DataFilePath = _DataFilePath;
	}

	public HouseData_t GetData(string Key = "Default")
	{
		XmlDocument doc = new XmlDocument();
		doc.Load(DataFilePath);

		XmlNodeList nodeList;
		XmlNode root = doc.DocumentElement;

		nodeList = root.SelectNodes("descendant::DataSet[@Key='" + Key + "']");
		if (nodeList.Count == 0 && Key != "Default")
		{
			Debug.Log("Could not find the key " + Key + " in the data, using default data");
			Key = "Default";
			nodeList = root.SelectNodes("descendant::DataSet[@Key='" + Key + "']");
		}

		//Change the price on the books.
		HouseData_t OutData = new HouseData_t();
		if (nodeList.Count == 0)
		{
			Debug.Log("Error: Could not find the default data in the data set");
		}
		else if (nodeList.Count > 1)
		{
			Debug.Log("Found multiple data sets for key " + Key);
		}
		else
		{
			Debug.Log(nodeList[0].ToString());
			Int32.TryParse(nodeList[0].SelectSingleNode("descendant::Temperature::Inside").Value, out OutData.TemperatureData.Inside);
			Int32.TryParse(nodeList[0].SelectSingleNode("descendant::Temperature::Outside").Value, out OutData.TemperatureData.Outside);

			Int32.TryParse(nodeList[0].SelectSingleNode("descendant::Water::Today").Value, out OutData.WaterData.ConsumptionToday);
			Int32.TryParse(nodeList[0].SelectSingleNode("descendant::Water::Month").Value, out OutData.WaterData.ConsumptionMonth);
			Int32.TryParse(nodeList[0].SelectSingleNode("descendant::Water::Year").Value, out OutData.WaterData.ConsumptionYear);
		}


		return OutData;
	}
}
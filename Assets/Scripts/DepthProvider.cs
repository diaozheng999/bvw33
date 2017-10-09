using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PGT.Core;
using PGT.Core.DataStructures;
using Windows.Kinect;

public class DepthProvider : Disposable {

	KinectSensor sensor = null;
	DepthFrameReader reader = null;

	Image image;

	[SerializeField] Texture2D depthTexture;
	[SerializeField] ushort lowPlane;
	[SerializeField] ushort highPlane;

	float diff;

	ushort[] buffer;
	Color[] colourBuffer;

	Material material;

	int width;
	int height;

	uint size;

	[SerializeField] Color Dark;
	[SerializeField] Color Bright;

	// Use this for initialization
	void Start () {
		sensor = KinectSensor.GetDefault();
		reader = sensor?.DepthFrameSource.OpenReader();

		if((!sensor?.IsOpen) ?? false) {
			sensor.Open();
		}

		image = GetComponent<Image>();

		AddDisposable(reader);
		AddDisposable(sensor.Close);

		diff = highPlane - lowPlane;

		if(reader != null) {
			var desc = reader.DepthFrameSource.FrameDescription;
			width = desc.Width;
			height = desc.Height;
			depthTexture = new Texture2D(width, height);
			buffer = new ushort[desc.LengthInPixels];
			colourBuffer = new Color[desc.LengthInPixels];
			material = image.material;
			material.mainTexture = depthTexture;
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		using (var frame = reader?.AcquireLatestFrame()) {
			
			frame?.CopyFrameDataToArray(buffer);
			var len = buffer.Length;


			for(int i=0; i<len; ++i){

				if(buffer[i]==0){
					colourBuffer[i] = Dark;
					continue;
				}

				var p = highPlane - (float)Mathf.Clamp(buffer[i], lowPlane, highPlane);
				colourBuffer[i] = Color.Lerp(Dark, Bright, p/diff);
			}

			depthTexture.SetPixels(colourBuffer);
			depthTexture.Apply();
		}
	}
}

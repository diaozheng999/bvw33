using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;

using Windows.Kinect;

public class PoseEstimator : Singleton<PoseEstimator> {

	const int MAX_BODY_COUNT = 6;

	KinectSensor sensor = null;
	BodyFrameReader reader = null;

	Body[] data;

	[SerializeField] KinectSkeleton skeleton;

	// Use this for initialization
	void Start () {
		sensor = KinectSensor.GetDefault();
		reader = sensor?.BodyFrameSource.OpenReader();

		if((!sensor?.IsOpen) ?? false) {
			sensor.Open();
		}

		data = new Body[MAX_BODY_COUNT];

		AddDisposable(reader);
		AddDisposable(sensor.Close);
	}
	
	void FixedUpdate () {
		using (var frame = reader?.AcquireLatestFrame()) {
			frame?.GetAndRefreshBodyData(data);
		}
	}

	void Update () {
		var body = data[0];
		if (body==null) return;
		foreach(JointType joint in System.Enum.GetValues(typeof(JointType))){
			
			if(joint == JointType.SpineBase) continue;
			skeleton[joint].localPosition = skeleton[joint].localPosition.magnitude * ToVector3(body.Joints[joint]); 

		}
	}

	Vector3 ToVector3(Windows.Kinect.Joint j){
		return new Vector3(j.Position.X, j.Position.Y, j.Position.Z).normalized;
	}
}

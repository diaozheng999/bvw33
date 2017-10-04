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
    float[] skeletonDimensions;

    ulong trackingId = 0;

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

        skeletonDimensions = new float[25];

        for(int i=1; i<25; ++i)
        {
            var parent = KinectSkeleton.GetParent((JointType)i).Value();
            skeletonDimensions[i] = (skeleton[i].transform.position - skeleton[parent].transform.position).magnitude;
        }
	}
	
	void FixedUpdate () {
		using (var frame = reader?.AcquireLatestFrame()) {
			frame?.GetAndRefreshBodyData(data);
		}
	}

	void Update () {
        Body body = null;
        
        foreach(var b in data)
        {
            if(trackingId == 0 && b != null && b.IsTracked == true && b.TrackingId > 0)
            {
                trackingId = b.TrackingId;
                body = b;
                break;
            }
            if(trackingId != 0 && b != null && b.TrackingId == trackingId)
            {
                body = b;
                break;
            }
        }

        
		if (body==null) return;
        Debug.Log(body?.Joints[JointType.HandRight].Position.Y);
		foreach(JointType joint in System.Enum.GetValues(typeof(JointType))){
			if(joint == JointType.SpineBase) continue;
			skeleton[joint].localPosition =  GetDirection(body, joint); 

		}
	}

	Vector3 GetDirection(Body b, JointType j){
		var parent = KinectSkeleton.GetParent(j).Value();
		return (ToVector3(b.Joints[j]) - ToVector3(b.Joints[parent])).normalized;
	}


	Vector3 ToVector3(Windows.Kinect.Joint j){
		return new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
	}
}

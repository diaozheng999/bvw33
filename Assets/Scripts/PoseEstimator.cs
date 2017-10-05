using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;
using PGT.Core.DataStructures;

using Windows.Kinect;

public class PoseEstimator : Singleton<PoseEstimator> {

	const int MAX_BODY_COUNT = 6;

	KinectSensor sensor = null;
	BodyFrameReader reader = null;

	Body[] data;

	[SerializeField] KinectSkeleton skeleton;
	[SerializeField] KinectSkeleton debugSkeleton;
	[SerializeField] SkinnedMeshRenderer _renderer;

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

        for(int i=0; i<25; ++i)
        {
            var _p = KinectSkeleton.GetParent((JointType)i);
            if (_p.IsNone()) continue;
            var parent = _p.Value();
            skeletonDimensions[i] = (skeleton[i].transform.position - skeleton[parent].transform.position).magnitude;
        }


		//SkinnedMeshRenderer r = GetComponent<SkinnedMeshRenderer>();
		skeleton.FillDefaultValues();
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

		if (body == null) return;

		foreach(JointType joint in System.Enum.GetValues(typeof(JointType))){
			//if(joint == JointType.SpineMid) continue;
			//compute rotation
			skeleton[joint].rotation = ToQuaternion(body.JointOrientations[joint]);
		}
	}

	Vector3 GetRelativePos(KinectSkeleton b, JointType j){
		return b[j].transform.position - b[JointType.SpineMid].transform.position;
	}

	Vector3 GetRelativePos(Body b, JointType j){
		var parent = KinectSkeleton.GetParent(j).Value();
        var dir = ToVector3(b.Joints[j]) - ToVector3(b.Joints[parent]);
        
		return dir.normalized;
	}


	Vector3 ToVector3(Windows.Kinect.Joint j){
		return new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
	}

	Quaternion ToQuaternion(JointOrientation j){
		return new Quaternion(j.Orientation.X, j.Orientation.Y, j.Orientation.Z, j.Orientation.W);
	}
}

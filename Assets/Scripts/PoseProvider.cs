using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;
using PGT.Core.Func;
using PGT.Core.DataStructures;

using Windows.Kinect;



public class PoseProvider : Singleton<PoseProvider> {

	const int MAX_BODY_COUNT = 6;

	[SerializeField] bool damp;
	[SerializeField] int dampBufferSize;

	KinectSensor sensor = null;
	BodyFrameReader reader = null;

	Body[] data;

    ulong trackingId = 0;

	Pose loadedPose;

	public Windows.Kinect.Vector4 floorClipPlane;

	JointType[] jointTypes;

	QuaternionDamper[] dampers;

	// Use this for initialization
	void Start () {
		sensor = KinectSensor.GetDefault();
		reader = sensor?.BodyFrameSource.OpenReader();

		if((!sensor?.IsOpen) ?? false) {
			sensor.Open();
		}

		data = new Body[MAX_BODY_COUNT];

		jointTypes = (JointType[])System.Enum.GetValues(typeof(JointType));

		if(damp){
			dampers = new QuaternionDamper[25];
			foreach(var jointType in jointTypes){
				dampers[(int)jointType] = new QuaternionDamper(dampBufferSize);
			}
		}

		AddDisposable(reader);
		AddDisposable(sensor.Close);
	}

	void FixedUpdate () {
		using (var frame = reader?.AcquireLatestFrame()) {
			frame?.GetAndRefreshBodyData(data);
			if (frame != null)
				floorClipPlane = frame.FloorClipPlane;
		}

		if(damp){
			var _body = GetCurrentTrackedBody();
			if(_body.IsNone()) return;
			var body = _body.Value();
			foreach(var joint in jointTypes){
				dampers[(int)joint].Update(ToQuaternion(body.JointOrientations[joint]));
			}
		}
	}

	public Option<Tuple<ulong, Body>> GetFirstTrackedBody(ulong trackingId) {

		// specific tracking ID
		if(trackingId!=0){
			foreach(var body in data){
				if(body != null && body.TrackingId == trackingId){
					return Option.Some(Tuple._(trackingId, body));
				}
			}
		}

		// generic tracking ID
		foreach(var body in data){
			if(body!=null && body.TrackingId > 0){
				return Option.Some(Tuple._(body.TrackingId, body));
			}
		}

		// nothing
		return Option.None<Tuple<ulong, Body>>();
	}

	public Option<Body> GetCurrentTrackedBody() {
		var _body = PoseProvider.instance.GetFirstTrackedBody(trackingId);
        if(_body.IsNone()) return Option.None<Body>();
        var _tup = _body.Value();

        trackingId = _tup.car;
		return Option.Some(_tup.cdr);
	}

	public JointType[] JointTypes() {
		return jointTypes;
	}

	public Sequence<float> GetPoseData(Body body){
		return Sequence.Tabulate(100, (int i) => {
			JointType j = (JointType) (i/4);
			var quaternion = body.JointOrientations[j].Orientation;

			switch (i%4) {
				case 0:
					return quaternion.X;
				case 1:
					return quaternion.Y;
				case 2:
					return quaternion.Z;
				case 3:
					return quaternion.W;
				default:
					return 0;
			}
		});
	}

	public Vector3 GetRelativePos(KinectSkeleton b, JointType j){
		return b[j].transform.position - b[JointType.SpineMid].transform.position;
	}

	public Vector3 GetRelativePos(Body b, JointType j){
		var parent = KinectSkeleton.GetParent(j).Value();
        var dir = ToVector3(b.Joints[j]) - ToVector3(b.Joints[parent]);
        
		return dir.normalized;
	}

	public Vector3 ToVector3(Windows.Kinect.Joint j){
		return new Vector3(j.Position.X, j.Position.Y, j.Position.Z);
	}

	public Quaternion ToQuaternion(JointOrientation j){
		return new Quaternion(j.Orientation.X, j.Orientation.Y, j.Orientation.Z, j.Orientation.W);
	}

	public Quaternion GetDampedQuaternion(JointType joint){
		return dampers[(int)joint].Value();
	}
}

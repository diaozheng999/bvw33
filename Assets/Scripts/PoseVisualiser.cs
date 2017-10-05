using UnityEngine;
using PGT.Core;
using Windows.Kinect;


public class PoseVisualiser : Singleton<PoseVisualiser> {
	[SerializeField] KinectSkeleton skeleton;


    void Start() {
        skeleton.FillDefaultValues();
    }

    void Update() {
        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()) return;
        var body = _body.Value();

        foreach(var joint in PoseProvider.instance.JointTypes()){
            //if(joint == JointType.SpineMid) continue;
            //compute rotation
            var child = KinectSkeleton.PointsAt(joint);
            if (child.IsSome())
            {
                var rot = PoseProvider.instance.ToQuaternion(body.JointOrientations[child.Value()]);
                //var nrot = Quaternion.Euler(transform.TransformDirection(rot.eulerAngles));
                skeleton[joint].rotation =  rot * transform.rotation;
            }
		}
    }
}
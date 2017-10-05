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
        Debug.Log(_body);
        if(_body.IsNone()) return;
        var body = _body.Value();

        Debug.Log("Update!!!");
        foreach(var joint in PoseProvider.instance.JointTypes()){
            //if(joint == JointType.SpineMid) continue;
            //compute rotation
            var child = KinectSkeleton.PointsAt(joint);
            if (child.IsSome())
            {

                var my_pos = PoseProvider.instance.ToVector3(body.Joints[joint]);               
                var child_pos = PoseProvider.instance.ToVector3(body.Joints[child.Value()]);

                var up_dir = transform.TransformVector(child_pos - my_pos);

                skeleton[joint].rotation = Quaternion.LookRotation(transform.forward, up_dir);
            }
		}
    }
}
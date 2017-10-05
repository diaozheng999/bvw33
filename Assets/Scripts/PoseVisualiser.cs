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

                var my_pos = PoseProvider.instance.ToVector3(body.Joints[joint]);
                var child_pos = PoseProvider.instance.ToVector3(body.Joints[child.Value()]);

                var up_dir = transform.TransformVector(child_pos - my_pos);

                var rot = Quaternion.LookRotation(up_dir) * Quaternion.Euler(90 * Vector3.right);

                float angle;
                Vector3 axis;

                rot.ToAngleAxis(out angle, out axis);
                axis.z *= -1;
                rot = Quaternion.AngleAxis(-angle, axis);

                skeleton[joint].rotation = rot;
            }
		}
    }
}
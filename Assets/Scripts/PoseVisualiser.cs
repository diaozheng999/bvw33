using UnityEngine;
using PGT.Core;
using Windows.Kinect;


public class PoseVisualiser : Singleton<PoseVisualiser> {

    [SerializeField] string mixamoPrefix = "mixamorig";
    [SerializeField] bool fillMixamo = true;
	[SerializeField] KinectSkeleton skeleton;


    void Start () {
        if(fillMixamo){
            skeleton.FillMixamo(transform, mixamoPrefix);
        }
    }


    void Update() {
        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()) return;
        var body = _body.Value();

        // rotate to identity
        var baseRot = transform.rotation;
        transform.rotation = Quaternion.identity;

        
        foreach(var joint in PoseProvider.instance.JointTypes()){
            //if(joint == JointType.SpineMid) continue;
            //compute rotation
            if(skeleton[joint] == null) continue;


            skeleton[joint].rotation = Mirror(PoseProvider.instance.GetDampedQuaternion(joint));


            // Mixamo Rig corrections
            // This assumes all joints point upwards in mixamo rigging.

            switch(joint){
                case JointType.KneeLeft:
                case JointType.AnkleLeft:
                    skeleton[joint].rotation *= Quaternion.Euler(0, -90, 180);
                    break;
                case JointType.KneeRight:
                case JointType.AnkleRight:
                    skeleton[joint].rotation *= Quaternion.Euler(0, 90, 180);
                    break;
                case JointType.ShoulderLeft:
                case JointType.ElbowLeft:
                case JointType.WristLeft:
                case JointType.HandLeft:
                    skeleton[joint].rotation *= Quaternion.Euler(0, 0, -90);
                    break;
                case JointType.ShoulderRight:
                case JointType.ElbowRight:
                case JointType.WristRight:
                case JointType.HandRight:
                    skeleton[joint].rotation *= Quaternion.Euler(0, 0, 90);
                    break;
            }

		}

        
        var e = skeleton[JointType.SpineBase].eulerAngles;
        e.y = 0;
        skeleton[JointType.SpineBase].eulerAngles = e;
        

        transform.rotation = baseRot;
    }

    Quaternion Mirror(Quaternion rot) {
        return new Quaternion(rot.x, -rot.y, -rot.z, rot.w);
    }
}
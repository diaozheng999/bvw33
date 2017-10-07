using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Linq;

public class KinectAvatar : MonoBehaviour {

    public bool IsMirror = true;

    public GameObject _UnityChan;

    public GameObject Ref;
    public GameObject Hips;
    public GameObject LeftUpLeg;
    public GameObject LeftLeg;
    public GameObject RightUpLeg;
    public GameObject RightLeg;
    public GameObject Spine1;
    public GameObject Spine2;
    public GameObject LeftShoulder;
    public GameObject LeftArm;
    public GameObject LeftForeArm;
    public GameObject LeftHand;
    public GameObject RightShoulder;
    public GameObject RightArm;
    public GameObject RightForeArm;
    public GameObject RightHand;
    public GameObject Neck;
    public GameObject Head;


	// Use this for initialization
	void Start () {
        Hips = gameObject.transform.Find( "mixamorig:Hips" ).gameObject;
        LeftUpLeg = Hips.transform.Find( "mixamorig:LeftUpLeg" ).gameObject;
        LeftLeg = LeftUpLeg.transform.Find( "mixamorig:LeftLeg" ).gameObject;
        RightUpLeg = Hips.transform.Find( "mixamorig:RightUpLeg" ).gameObject;
        RightLeg = RightUpLeg.transform.Find( "mixamorig:RightLeg" ).gameObject;
        Spine1 = Hips.transform.Find( "mixamorig:Spine" ).
                    gameObject.transform.Find( "mixamorig:Spine1" ).gameObject;
        Spine2 = Spine1.transform.Find( "mixamorig:Spine2" ).gameObject;
        LeftShoulder = Spine2.transform.Find( "mixamorig:LeftShoulder" ).gameObject;
        LeftArm = LeftShoulder.transform.Find( "mixamorig:LeftArm" ).gameObject;
        LeftForeArm = LeftArm.transform.Find( "mixamorig:LeftForeArm" ).gameObject;
        LeftHand = LeftForeArm.transform.Find( "mixamorig:LeftHand" ).gameObject;
        RightShoulder = Spine2.transform.Find( "mixamorig:RightShoulder" ).gameObject;
        RightArm = RightShoulder.transform.Find( "mixamorig:RightArm" ).gameObject;
        RightForeArm = RightArm.transform.Find( "mixamorig:RightForeArm" ).gameObject;
        RightHand = RightForeArm.transform.Find( "mixamorig:RightHand" ).gameObject;
        Neck = Spine2.transform.Find( "mixamorig:Neck" ).gameObject;
        Head = Neck.transform.Find( "mixamorig:Head" ).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        // 最初に追跡している人を取得する
        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if ( _body.IsNone() ) {
            return;
        }

        var body = _body.Value();

        // 床の傾きを取得する
        var floorPlane = PoseProvider.instance.floorClipPlane;
        var comp = Quaternion.FromToRotation(
            new Vector3( floorPlane.X, floorPlane.Y, floorPlane.Z ), Vector3.up );

        // 関節の回転を取得する
        var joints = body.JointOrientations;

		Quaternion SpineBase;
		Quaternion SpineMid;
		Quaternion SpineShoulder;
		Quaternion ShoulderLeft;
		Quaternion ShoulderRight;
		Quaternion ElbowLeft;
		Quaternion WristLeft;
		Quaternion HandLeft;
		Quaternion ElbowRight;
		Quaternion WristRight;
		Quaternion HandRight;
		Quaternion KneeLeft;
		Quaternion AnkleLeft;
		Quaternion KneeRight;
		Quaternion AnkleRight;

        // 鏡
        if ( IsMirror ) {
            SpineBase = joints[JointType.SpineBase].Orientation.ToMirror().ToQuaternion( comp );
            SpineMid = joints[JointType.SpineMid].Orientation.ToMirror().ToQuaternion( comp );
            SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToMirror().ToQuaternion( comp );
            ShoulderLeft = joints[JointType.ShoulderRight].Orientation.ToMirror().ToQuaternion( comp );
            ShoulderRight = joints[JointType.ShoulderLeft].Orientation.ToMirror().ToQuaternion( comp );
            ElbowLeft = joints[JointType.ElbowRight].Orientation.ToMirror().ToQuaternion( comp );
            WristLeft = joints[JointType.WristRight].Orientation.ToMirror().ToQuaternion( comp );
            HandLeft = joints[JointType.HandRight].Orientation.ToMirror().ToQuaternion( comp );
            ElbowRight = joints[JointType.ElbowLeft].Orientation.ToMirror().ToQuaternion( comp );
            WristRight = joints[JointType.WristLeft].Orientation.ToMirror().ToQuaternion( comp );
            HandRight = joints[JointType.HandLeft].Orientation.ToMirror().ToQuaternion( comp );
            KneeLeft = joints[JointType.KneeRight].Orientation.ToMirror().ToQuaternion( comp );
            AnkleLeft = joints[JointType.AnkleRight].Orientation.ToMirror().ToQuaternion( comp );
            KneeRight = joints[JointType.KneeLeft].Orientation.ToMirror().ToQuaternion( comp );
            AnkleRight = joints[JointType.AnkleLeft].Orientation.ToMirror().ToQuaternion( comp );
        }
        // そのまま
        else {
            SpineBase = joints[JointType.SpineBase].Orientation.ToQuaternion( comp );
            SpineMid = joints[JointType.SpineMid].Orientation.ToQuaternion( comp );
            SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToQuaternion( comp );
            ShoulderLeft = joints[JointType.ShoulderLeft].Orientation.ToQuaternion( comp );
            ShoulderRight = joints[JointType.ShoulderRight].Orientation.ToQuaternion( comp );
            ElbowLeft = joints[JointType.ElbowLeft].Orientation.ToQuaternion( comp );
            WristLeft = joints[JointType.WristLeft].Orientation.ToQuaternion( comp );
            HandLeft = joints[JointType.HandLeft].Orientation.ToQuaternion( comp );
            ElbowRight = joints[JointType.ElbowRight].Orientation.ToQuaternion( comp );
            WristRight = joints[JointType.WristRight].Orientation.ToQuaternion( comp );
            HandRight = joints[JointType.HandRight].Orientation.ToQuaternion( comp );
            KneeLeft = joints[JointType.KneeLeft].Orientation.ToQuaternion( comp );
            AnkleLeft = joints[JointType.AnkleLeft].Orientation.ToQuaternion( comp );
            KneeRight = joints[JointType.KneeRight].Orientation.ToQuaternion( comp );
            AnkleRight = joints[JointType.AnkleRight].Orientation.ToQuaternion( comp );
		}

        // 関節の回転を計算する
        var q = transform.rotation;
        transform.rotation = Quaternion.identity;

        var comp2 = Quaternion.AngleAxis( 90, new Vector3( 0, 1, 0 ) ) *
                    Quaternion.AngleAxis( -90, new Vector3( 0, 0, 1 ) );

        comp2 = Quaternion.identity;

        Spine1.transform.rotation = SpineMid * comp2;

        RightArm.transform.rotation = ElbowRight * comp2;
        RightForeArm.transform.rotation = WristRight * comp2;
        RightHand.transform.rotation = HandRight * comp2;

        LeftArm.transform.rotation = ElbowLeft * comp2;
        LeftForeArm.transform.rotation = WristLeft * comp2;
        LeftHand.transform.rotation = HandLeft * comp2;

        RightUpLeg.transform.rotation = KneeRight * comp2;
        RightLeg.transform.rotation = AnkleRight * comp2;

		LeftUpLeg.transform.rotation = KneeLeft *  Quaternion.AngleAxis( -90, new Vector3( 0, 0, 1 ) );
		LeftLeg.transform.rotation = AnkleLeft * Quaternion.AngleAxis( -90, new Vector3( 0, 0, 1 ) );

        // モデルの回転を設定する
        transform.rotation = q;

        // モデルの位置を移動する
        var pos = body.Joints[JointType.SpineMid].Position;
        //Hips.transform.position = new Vector3( -pos.X, pos.Y, -pos.Z );
    }
}

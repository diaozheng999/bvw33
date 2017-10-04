using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using PGT.Core.Func;

[System.Serializable]
public struct KinectSkeleton : ICollection<Transform> {
    public Transform SpineBase;
    public Transform SpineMid;
    public Transform Neck;
    public Transform Head;
    public Transform ShoulderLeft;
    public Transform ElbowLeft;
    public Transform WristLeft;
    public Transform HandLeft;
    public Transform ShoulderRight;
    public Transform ElbowRight;
    public Transform WristRight;
    public Transform HandRight;
    public Transform HipLeft;
    public Transform KneeLeft;
    public Transform AnkleLeft;
    public Transform FootLeft;
    public Transform HipRight;
    public Transform KneeRight;
    public Transform AnkleRight;
    public Transform FootRight;
    public Transform SpineShoulder;
    public Transform HandTipLeft;
    public Transform ThumbLeft;
    public Transform HandTipRight;
    public Transform ThumbRight;

    public IEnumerator<Transform> GetEnumerator(){
        yield return SpineBase;
        yield return SpineMid;
        yield return Neck;
        yield return Head;
        yield return ShoulderLeft;
        yield return ElbowLeft;
        yield return WristLeft;
        yield return HandLeft;
        yield return ShoulderRight;
        yield return ElbowRight;
        yield return WristRight;
        yield return HandRight;
        yield return HipLeft;
        yield return KneeLeft;
        yield return AnkleLeft;
        yield return FootLeft;
        yield return HipRight;
        yield return KneeRight;
        yield return AnkleRight;
        yield return FootRight;
        yield return SpineShoulder;
        yield return HandTipLeft;
        yield return ThumbLeft;
        yield return HandTipRight;
        yield return ThumbRight;
    }

    IEnumerator IEnumerable.GetEnumerator(){
        yield return SpineBase;
        yield return SpineMid;
        yield return Neck;
        yield return Head;
        yield return ShoulderLeft;
        yield return ElbowLeft;
        yield return WristLeft;
        yield return HandLeft;
        yield return ShoulderRight;
        yield return ElbowRight;
        yield return WristRight;
        yield return HandRight;
        yield return HipLeft;
        yield return KneeLeft;
        yield return AnkleLeft;
        yield return FootLeft;
        yield return HipRight;
        yield return KneeRight;
        yield return AnkleRight;
        yield return FootRight;
        yield return SpineShoulder;
        yield return HandTipLeft;
        yield return ThumbLeft;
        yield return HandTipRight;
        yield return ThumbRight;
    }


    public int Count { get { return 24; } }

    public bool IsReadOnly { get { return false; } }

    public bool Contains(Transform t){
        if(t==null) return false;
        for(int i=0; i<24; ++i){
            if(this[i]!=null && t.GetInstanceID() == this[i].GetInstanceID()){
                return true;
            }
        }
        return false;
    }

    public void Clear(){
        for(int i=0; i<24; ++i){
            this[i] = null;
        }
    }

    public void Add(Transform t){
        throw new System.NotImplementedException("Skeletons don't support add/remove.");
    }

    public bool Remove(Transform t){
        throw new System.NotImplementedException("Skeletons don't support add/remove.");
    }

    public void CopyTo(Transform[] array, int id){
        for(int i=0; i<Count; ++i){
            array[id+i] = this[i];
        }
    }


    public static Option<JointType> GetParent(JointType joint){
        switch(joint){
            case JointType.SpineBase: return Option.None<JointType>();
            case JointType.SpineMid: return Option.Some(JointType.SpineBase);
            case JointType.Neck: return Option.Some(JointType.SpineShoulder);
            case JointType.Head: return Option.Some(JointType.Neck);
            case JointType.ShoulderLeft: return Option.Some(JointType.SpineShoulder);
            case JointType.ElbowLeft: return Option.Some(JointType.ShoulderLeft);
            case JointType.WristLeft: return Option.Some(JointType.ElbowLeft);
            case JointType.HandLeft: return Option.Some(JointType.WristLeft);
            case JointType.ShoulderRight: return Option.Some(JointType.SpineShoulder);
            case JointType.ElbowRight: return Option.Some(JointType.ShoulderRight);
            case JointType.WristRight: return Option.Some(JointType.ElbowRight);
            case JointType.HandRight: return Option.Some(JointType.WristRight);
            case JointType.HipLeft: return Option.Some(JointType.SpineBase);
            case JointType.KneeLeft: return Option.Some(JointType.HipLeft);
            case JointType.AnkleLeft: return Option.Some(JointType.KneeLeft);
            case JointType.FootLeft: return Option.Some(JointType.AnkleLeft);
            case JointType.HipRight: return Option.Some(JointType.SpineBase);
            case JointType.KneeRight: return Option.Some(JointType.HipRight);
            case JointType.AnkleRight: return Option.Some(JointType.KneeRight);
            case JointType.FootRight: return Option.Some(JointType.AnkleRight);
            case JointType.SpineShoulder: return Option.Some(JointType.SpineMid);
            case JointType.HandTipLeft: return Option.Some(JointType.HandLeft);
            case JointType.ThumbLeft: return Option.Some(JointType.WristLeft);
            case JointType.HandTipRight: return Option.Some(JointType.HandRight);
            case JointType.ThumbRight: return Option.Some(JointType.WristRight);
            default: return Option.None<JointType>();
        }
    }

    public Transform this[int jointId]{
        get { return this[(JointType)jointId]; }
        set { this[(JointType)jointId] = value; }
    }

    public Transform this[JointType jointId] {
        get {
            switch(jointId){
                case JointType.SpineBase: return SpineBase;
                case JointType.SpineMid: return SpineMid;
                case JointType.Neck: return Neck;
                case JointType.Head: return Head;
                case JointType.ShoulderLeft: return ShoulderLeft;
                case JointType.ElbowLeft: return ElbowLeft;
                case JointType.WristLeft: return WristLeft;
                case JointType.HandLeft: return HandLeft;
                case JointType.ShoulderRight: return ShoulderRight;
                case JointType.ElbowRight: return ElbowRight;
                case JointType.WristRight: return WristRight;
                case JointType.HandRight: return HandRight;
                case JointType.HipLeft: return HipLeft;
                case JointType.KneeLeft: return KneeLeft;
                case JointType.AnkleLeft: return AnkleLeft;
                case JointType.FootLeft: return FootLeft;
                case JointType.HipRight: return HipRight;
                case JointType.KneeRight: return KneeRight;
                case JointType.AnkleRight: return AnkleRight;
                case JointType.FootRight: return FootRight;
                case JointType.SpineShoulder: return SpineShoulder;
                case JointType.HandTipLeft: return HandTipLeft;
                case JointType.ThumbLeft: return ThumbLeft;
                case JointType.HandTipRight: return HandTipRight;
                case JointType.ThumbRight: return ThumbRight;
                default: throw new System.IndexOutOfRangeException();
            }
        }

        set {
            switch(jointId){
                case JointType.SpineBase: SpineBase = value; break;
                case JointType.SpineMid: SpineMid = value; break;
                case JointType.Neck:  Neck = value; break;
                case JointType.Head:  Head = value; break;
                case JointType.ShoulderLeft:  ShoulderLeft = value; break;
                case JointType.ElbowLeft:  ElbowLeft = value; break;
                case JointType.WristLeft:  WristLeft = value; break;
                case JointType.HandLeft:  HandLeft = value; break;
                case JointType.ShoulderRight:  ShoulderRight = value; break;
                case JointType.ElbowRight:  ElbowRight = value; break;
                case JointType.HandRight:  HandRight = value; break;
                case JointType.HipLeft:  HipLeft = value; break;
                case JointType.KneeLeft:  KneeLeft = value; break;
                case JointType.AnkleLeft:  AnkleLeft = value; break;
                case JointType.FootLeft:  FootLeft = value; break;
                case JointType.HipRight:  HipRight = value; break;
                case JointType.KneeRight:  KneeRight = value; break;
                case JointType.AnkleRight:  AnkleRight = value; break;
                case JointType.FootRight:  FootRight = value; break;
                case JointType.SpineShoulder:  SpineShoulder = value; break;
                case JointType.HandTipLeft:  HandTipLeft = value; break;
                case JointType.ThumbLeft:  ThumbLeft = value; break;
                case JointType.HandTipRight:  HandTipRight = value; break;
                case JointType.ThumbRight:  ThumbRight = value; break;
                default: throw new System.IndexOutOfRangeException();
            }
        }
    }

}
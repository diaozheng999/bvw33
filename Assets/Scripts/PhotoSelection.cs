using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;
using PGT.Core.Func;
using PGT.Core.DataStructures;
using Windows.Kinect;

using UnityEngine.UI;

public class PhotoSelection : Singleton<PhotoSelection> {

    
	KinectSensor sensor = null;
	ColorFrameReader reader = null;

    [SerializeField] int numFrames;

    [SerializeField] bool beginCapture;

    [SerializeField] Image[] debug;

    [SerializeField] Texture2D[] frames;

    float[] frameScores;

    Sequence<Quaternion> previousOrientation;

    int width;
    int height;

    byte[] buffer;

    FrameDescription desc;

    void Start() {
        sensor = KinectSensor.GetDefault();
		reader = sensor?.ColorFrameSource.OpenReader();

		if((!sensor?.IsOpen) ?? false) {
			sensor.Open();
		}

		AddDisposable(reader);
		AddDisposable(sensor.Close);

        frames = new Texture2D[numFrames];
        frameScores = new float[numFrames];

        desc = reader.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
        width = desc.Width;
        height = desc.Height;

        buffer = new byte[desc.LengthInPixels * desc.BytesPerPixel];
        StartCoroutine(beginc());
    }

    IEnumerator beginc(){
        yield return new WaitForSeconds(6f);
        BeginCapture();
    }


    bool JointFilter(int i) {
        switch((JointType)i){
            case JointType.SpineBase:
            case JointType.SpineMid:
            case JointType.SpineShoulder:
            case JointType.Neck:
            case JointType.ShoulderLeft:
            case JointType.ShoulderRight:
            case JointType.ElbowLeft:
            case JointType.ElbowRight:
            case JointType.KneeLeft:
            case JointType.KneeRight:
                return true;
            default:
                return false;
        }
    }

    public void BeginCapture(){
        Debug.Log("Hee?");
        // get base value
        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()){
            Debug.LogError("No one is being tracked right now.");
            return;
        }
        previousOrientation = PoseProvider.instance.GetPoseValues(_body.Value()).FilterIndex(JointFilter);

        for(int i=0; i<numFrames; ++i){
            frames[i] = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var tex = debug[i].material;
            tex.mainTexture = frames[i];
        }

        beginCapture = true;
        Debug.Log("Hee!");
    }

    void FixedUpdate () {
        if(!beginCapture) return;

        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()) return;

        var nextOrientation = PoseProvider.instance.GetPoseValues(_body.Value()).FilterIndex(JointFilter);

        var score = previousOrientation.MapWith(nextOrientation, Quaternion.Angle).Reduce(Function.fadd, 0f);


        Debug.Log(score);

        for(int i=0; i<numFrames; ++i){
            if(score > frameScores[i]){
                BubbleTextures(i);
                //set frame
                SetFrame(i);
                frameScores[i] = score;
                break;
            }
        }

        previousOrientation = nextOrientation;
    }

    void SetFrame(int i){
        using (var frame = reader?.AcquireLatestFrame()){
            Debug.Log(frame);
            if(frame!=null){
                Debug.Log(i, frames[i]);
                frame.CopyConvertedFrameDataToArray(buffer, ColorImageFormat.Rgba);
                frames[i].LoadRawTextureData(buffer);
                frames[i].Apply();
            }
        }
    }

    void BubbleTextures(int i){
        for(int j = numFrames - 1; j>i; --j){
            Graphics.CopyTexture(frames[j-1], frames[j]);
            frameScores[j] = frameScores[j-1];
        }
    }

}
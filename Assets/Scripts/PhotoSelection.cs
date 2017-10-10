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

    [SerializeField] Image image;
    [SerializeField] Image image2;

    [SerializeField] Texture2D[] frames;

    [SerializeField] GameObject[] fireworks;
    [SerializeField] Vector3[] fireworkPositions;

    float[] frameScores;
    bool[] pictureTaken;

    int width;
    int height;

    byte[] buffer;

    Sequence<Quaternion> previousPose;

    FrameDescription desc;

    bool timeOut = false;

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
        pictureTaken = new bool[numFrames];

        desc = reader.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
        width = desc.Width;
        height = desc.Height;

        for(int i=0; i<numFrames; ++i){
            pictureTaken[i] = false;
        }

        buffer = new byte[desc.LengthInPixels * desc.BytesPerPixel];
    }

    


    IEnumerator Firework(){
        var n_fw = Random.Range(2, fireworkPositions.Length);
        HashSet<int> seen = new HashSet<int>();
        for(int i=0; i<n_fw; ++i){
            int setPos;
            do{
                setPos = Random.Range(0, fireworkPositions.Length);
            }while(seen.Contains(setPos));
            seen.Add(setPos);

            Instantiate(fireworks[Random.Range(0, fireworks.Length)], fireworkPositions[setPos], Quaternion.Euler(-90, 0, 0));
            yield return new WaitForSeconds(0.1f * Random.value);
        }
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

        previousPose = PoseProvider.instance.GetPoseValues(_body.Value()).FilterIndex(JointFilter);

        for(int i=0; i<numFrames; ++i){
            frames[i] = new Texture2D(width, height, TextureFormat.RGBA32, false);
            frameScores[i] = float.PositiveInfinity;
        }

        beginCapture = true;
        Debug.Log("Hee!");
    }

    void FixedUpdate () {
        if(!beginCapture) return;
        if(timeOut) return;

        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()) return;

        var pose = PoseProvider.instance.GetPoseValues(_body.Value()).FilterIndex(JointFilter);

        var movement = pose.MapWith(previousPose, Quaternion.Angle).Reduce(Function.fadd, 0);

        previousPose = pose;
        if(movement > 30) return;   

        var score = PoseEstimator.instance.Estimate(6);

        for(int i=0; i<numFrames; ++i){
            if(score <= frameScores[i]){
                BubbleTextures(i);
                //set frame
                SetFrame(i);
                frameScores[i] = score;
                SoundManager.instance.PlayFlashOnce();
                StartCoroutine(Firework());
                StartCoroutine(Timeout());
                break;
            }
        }
    }

    IEnumerator Timeout(){
        timeOut = true;
        yield return new WaitForSeconds(1f);
        timeOut = false;
    }

    void SetFrame(int i){
        using (var frame = reader?.AcquireLatestFrame()){
            Debug.Log(frame);
            if(frame!=null){
                Debug.Log(i, frames[i]);
                frame.CopyConvertedFrameDataToArray(buffer, ColorImageFormat.Rgba);
                pictureTaken[i] = true;
                frames[i].LoadRawTextureData(buffer);
                frames[i].Apply();
            }
        }
    }

    void BubbleTextures(int i){
        for(int j = numFrames - 1; j>i; --j){
            Graphics.CopyTexture(frames[j-1], frames[j]);
            frameScores[j] = frameScores[j-1];
            pictureTaken[j] = pictureTaken[j-1];
        }
    }

    public void StopCapture(){
        beginCapture = false;
        var mat = image.material;


        int firstImage;
        do{
            firstImage = Random.Range(0, numFrames);
        }while(!pictureTaken[firstImage]);
        mat.mainTexture = frames[firstImage];

        var mat2 = image2.material;

        int snd;
        do {
            snd = Random.Range(0, numFrames);
        }while(snd == firstImage || !pictureTaken[snd]);
        
        mat2.mainTexture = frames[snd];
        image.gameObject.SetActive(true);
        image2.gameObject.SetActive(true);
    }

}
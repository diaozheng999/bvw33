using UnityEngine;
using PGT.Core;
using PGT.Core.DataStructures;

public class PoseRecorder : Singleton<PoseRecorder> {

    [SerializeField] string filename;

    CSVWriter writer;

    void Start() {
        writer = new CSVWriter(filename);
        AddDisposable(writer);
    }

    void Update() {
        var _body = PoseProvider.instance.GetCurrentTrackedBody();
        if(_body.IsNone()) return;
        var body = _body.Value();


        float response = Input.GetKey(KeyCode.Space) ? 1 : 0;
        
        writer.Write(PoseProvider.instance.GetPoseData(body).Append(Sequence.Singleton(response)), 
            (float f) => f.ToString());
    }

}
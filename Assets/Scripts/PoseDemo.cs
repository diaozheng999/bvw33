using UnityEngine;

public class PoseDemo : MonoBehaviour {

    [SerializeField] int poseId;
    [SerializeField] Renderer poseRenderer;

    void Update() {
        var p = PoseEstimator.instance.Estimate(poseId);
        Debug.Log(p);
        poseRenderer.enabled = p > 0.4f;
    }

}
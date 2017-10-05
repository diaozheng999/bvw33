using UnityEngine;

public class PoseDemo : MonoBehaviour {

    [SerializeField] int poseId;
    [SerializeField] Renderer poseRenderer;

    void Update() {
        poseRenderer.enabled = PoseEstimator.instance.Estimate(poseId) > 0.4f;
    }

}
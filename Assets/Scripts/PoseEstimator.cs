using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;
using PGT.Core.Func;
using PGT.Core.DataStructures;

using Windows.Kinect;



public class PoseEstimator : Singleton<PoseEstimator> {

	[SerializeField] string[] poses;
    ulong trackingId = 0;

	JsonLoader<Pose>[] loaders;

	volatile bool isComplete = false;

	int n_loaded = 0;
	volatile int n_poses;

	Pose[] poseModels;


	// Use this for initialization
	void Start () {
		
		n_poses = poses.Length;
		loaders = new JsonLoader<Pose>[n_poses];
		poseModels = new Pose[n_poses];

		for(int i=0; i<n_poses; ++i){
			loaders[i] = new JsonLoader<Pose>(poses[i]);
			loaders[i].LoadJson(OnPoseWeightLoaded(i));
		}
	}

	void IncrementLoaded(){
		n_loaded += 1;
		if(n_loaded == n_poses){
			isComplete = true;
		}
	}

	System.Action<Pose> OnPoseWeightLoaded(int i) => (Pose pose) => {
		poseModels[i] = pose;
		UnityExecutionThread.instance.ExecuteInMainThread(() => IncrementLoaded());
	};

	public float Estimate(int poseId){
        return 0f;
		if(!isComplete && !loaders[poseId].IsComplete) return 0f;

		var _body = PoseProvider.instance.GetCurrentTrackedBody();
		if(_body.IsNone()) return 0f;
		
		var features = PoseProvider.instance.GetPoseData(_body.Value());

		return poseModels[poseId].Compute(features);
	}

}

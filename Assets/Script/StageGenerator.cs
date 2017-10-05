using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject stagePrefab;
    [SerializeField]
    private GameObject roundStagePrefab;

    public void GenerateStageAt(float offSet)
    {
        GameObject newStage = Instantiate(stagePrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
    }

    public void GenerateRoundStageAt(float offSet)
    {
        GameObject newStage = Instantiate(roundStagePrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
    }
}

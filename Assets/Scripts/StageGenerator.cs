using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject stagePrefab;
    [SerializeField]
    private GameObject leftLightBarPrefab;
    [SerializeField]
    private GameObject rightLightBarPrefab;
    [SerializeField]
    private GameObject roundStagePrefab;

    public void GenerateStageAt(float offSet)
    {
        GameObject newStage = Instantiate(stagePrefab, transform);
        GameObject newLeftLight = Instantiate(leftLightBarPrefab, transform);
        GameObject newRightLight = Instantiate(rightLightBarPrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
        newLeftLight.transform.position = new Vector3(-1f, 3.56f, offSet);
        newRightLight.transform.position = new Vector3(1f, 3.56f, offSet);
    }

    public void GenerateRoundStageAt(float offSet)
    {
        GameObject newStage = Instantiate(roundStagePrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
    }
}

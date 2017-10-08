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
    [SerializeField]
    private GameObject bigLightPrefab;
    [SerializeField]
    private GameObject smallLightPrefab;

    private float stageOffSet = 5f;

    public void GenerateStageAt(float offSet)
    {
        GameObject newStage = Instantiate(stagePrefab, transform);
        GameObject newLeftLight = Instantiate(leftLightBarPrefab, transform);
        GameObject newRightLight = Instantiate(rightLightBarPrefab, transform);
        GameObject leftTargetLight = Instantiate(bigLightPrefab, transform);
        GameObject rightTargetLight = Instantiate(bigLightPrefab, transform);
        GameObject leadingLight1 = Instantiate(smallLightPrefab, transform);
        GameObject leadingLight2 = Instantiate(smallLightPrefab, transform);
        GameObject leadingLight3 = Instantiate(smallLightPrefab, transform);
        GameObject leadingLight4 = Instantiate(smallLightPrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
        newLeftLight.transform.position = new Vector3(-1.5f, 3.56f, offSet);
        newRightLight.transform.position = new Vector3(1.5f, 3.56f, offSet);
        leftTargetLight.transform.position = new Vector3(-2.3f, 0.15f, offSet);
        rightTargetLight.transform.position = new Vector3(2.3f, 0.15f, offSet);
        leadingLight1.transform.position = new Vector3(-2.3f, 0.11f, offSet - stageOffSet / 3f);
        leadingLight2.transform.position = new Vector3(2.3f, 0.11f, offSet - stageOffSet / 3f);
        leadingLight3.transform.position = new Vector3(-2.3f, 0.11f, offSet - stageOffSet / 3f * 2);
        leadingLight4.transform.position = new Vector3(2.3f, 0.11f, offSet - stageOffSet / 3f * 2);
    }

    public void GenerateRoundStageAt(float offSet)
    {
        GameObject newStage = Instantiate(roundStagePrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
    }
}

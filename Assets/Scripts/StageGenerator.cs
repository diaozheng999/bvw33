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
    private GameObject bigWhiteLightPrefab;
    [SerializeField]
    private GameObject smallWhiteLightPrefab;

    private float stageOffSet = 5f;

    public void GenerateStageAt(float offSet)
    {
        GameObject newStage = Instantiate(stagePrefab, transform);
        GameObject newLeftLight = Instantiate(leftLightBarPrefab, transform);
        GameObject newRightLight = Instantiate(rightLightBarPrefab, transform);
        GameObject targetLight = Instantiate(bigWhiteLightPrefab, transform);
        GameObject leadingLight1 = Instantiate(smallWhiteLightPrefab, transform);
        GameObject leadingLight2 = Instantiate(smallWhiteLightPrefab, transform);
        GameObject leadingLight3 = Instantiate(smallWhiteLightPrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
        newLeftLight.transform.position = new Vector3(-1.5f, 3.56f, offSet);
        newRightLight.transform.position = new Vector3(1.5f, 3.56f, offSet);
        targetLight.transform.position = new Vector3(-2.3f, 0.15f, offSet);

    }

    public void GenerateRoundStageAt(float offSet)
    {
        GameObject newStage = Instantiate(roundStagePrefab, transform);
        newStage.transform.position = new Vector3(0f, 0f, offSet);
    }
}

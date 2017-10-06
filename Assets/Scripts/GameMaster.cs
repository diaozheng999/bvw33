using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    private static float BPM = 120.0f;
    private static float secondPerBeat = 60.0f / BPM;
    
    private static float stageOffSet = 5f;
    private float currentStagePosition = 0;

    private Vector3 nextPlayerPosition;

    private float startTime;
    private float currentTime;

    private float startJudgeTime;
    private float stopJudgeTime;

    private float nextGenerateTime;
    private float generateWaitTime = secondPerBeat * 8f;

    private float nextRoundTime;
    private float waitTime = secondPerBeat * 4f;

    private float perfectPeriod = 0.1f;
    private float gracePeriod = 0.3f;

    private float moveSpeed = stageOffSet / (secondPerBeat * 3f);

    private int currentBlock = 0;
    private int numOfGeneratedBlock = 0;
    private int numOfBlockStage = 10;
    private int numOfPoseOnRoundStage = 5;

    [SerializeField]
    private GameObject stageGenerator;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject camera;

    private float threshold = 0.4f;

    private bool isEarly = false;
    private bool isPerfect = false;
    private bool isLate = false;
    private bool isPassCheckPoint = true;

    [SerializeField]
    private Text feedbackText;

    void Start () {
        startTime = Time.time;
        nextGenerateTime = startTime + generateWaitTime;
        SetTheScene(7);
        SetThePlayer();
	}

    private void SetTheScene(int numOfStage)
    {
        stageGenerator.GetComponent<StageGenerator>().GenerateStageAt(currentStagePosition);
        for (int i = 1; i < numOfStage; i++)
        {
            currentStagePosition += stageOffSet;
            stageGenerator.GetComponent<StageGenerator>().GenerateStageAt(currentStagePosition);
        }
        numOfGeneratedBlock = numOfStage;
    }

    private void SetThePlayer()
    {
        // update judge time
        startJudgeTime = startTime + secondPerBeat * 4f;
        stopJudgeTime = startTime + secondPerBeat * 7f;
        // update player position
        nextPlayerPosition = player.transform.position;
        nextPlayerPosition.z += stageOffSet;
        currentBlock++;
        nextRoundTime = startTime + waitTime * 2f;
    }

    void Update () {
        currentTime = Time.time;
        // Generate new stage
        if (currentTime >= nextGenerateTime)
        {
            if (numOfGeneratedBlock < numOfBlockStage)
            {
                // generate new block stage
                numOfGeneratedBlock++;
                currentStagePosition += stageOffSet;
                stageGenerator.GetComponent<StageGenerator>().GenerateStageAt(currentStagePosition);
                
                nextGenerateTime += generateWaitTime;
            }
            if (numOfGeneratedBlock == numOfBlockStage)
            {
                // generate round stage
                numOfGeneratedBlock++;
                currentStagePosition += (stageOffSet / 2f);
                stageGenerator.GetComponent<StageGenerator>().GenerateRoundStageAt(currentStagePosition);

                nextGenerateTime = float.MaxValue;
            }
        }

        // update judge time
        if (currentTime > (stopJudgeTime + gracePeriod))
        {
            feedbackText.text = "";
            isPerfect = false;
            isEarly = false;
            isLate = false;
            isPassCheckPoint = true;
            Debug.Log("reset bool");
            if (currentBlock < (numOfBlockStage - 1))
            {
                // update judge time (judge every 8 beats)
                startJudgeTime += (waitTime * 2f);
                stopJudgeTime += (waitTime * 2f);
            }
            else if (currentBlock == (numOfBlockStage - 1))
            {
                // update judge time (judge in 12 beats)
                startJudgeTime += (waitTime * 3f);
                stopJudgeTime += (waitTime * 3f);
            }
            else if (currentBlock < (numOfBlockStage + numOfPoseOnRoundStage - 1))
            {
                // update judge time (judge every 4 beats)
                startJudgeTime += waitTime;
                stopJudgeTime += waitTime;
            }
        }

        // update player position
        if (currentTime >= nextRoundTime)
        {
            if (currentBlock < (numOfBlockStage - 1))
            {
                // update player position
                nextPlayerPosition.z += stageOffSet;
                currentBlock++;
                nextRoundTime += waitTime * 2f;
            }
            else if (currentBlock == (numOfBlockStage - 1))
            {
                // update player position
                nextPlayerPosition.z += (stageOffSet * 2f);
                currentBlock++;
                nextRoundTime += waitTime * 3f;
            }
            else if (currentBlock < (numOfBlockStage + numOfPoseOnRoundStage - 1))
            {
                currentBlock++;
                nextRoundTime += waitTime;
            }
        }

        // move the player
        float step = moveSpeed * Time.deltaTime;
        player.transform.position = Vector3.MoveTowards(player.transform.position, nextPlayerPosition, step);
        camera.transform.position = player.transform.position;
        // float p = 0;
        float p = PoseEstimator.instance.Estimate((currentBlock-1) % 3 /* NOTE TO SELF: DON'T HARD CODE THIS! */);
        // check gesture at start point
        if (currentTime <= (startJudgeTime + perfectPeriod) && currentTime >= (startJudgeTime - perfectPeriod))
        {
            Debug.Log("perfect time block: "+p+", pose="+((currentBlock - 1) % 3));
            if (p > threshold)
            {
                isPerfect = true;
            }
        } else if (currentTime > (startJudgeTime + perfectPeriod) && currentTime <= (startJudgeTime + gracePeriod))
        {
            Debug.Log("late time block: " + p + ", pose=" + ((currentBlock - 1) % 3 ));
            // late
            if (p > threshold)
            {
                isLate = true;
            }
        } else if (currentTime >= (startJudgeTime - gracePeriod) && currentTime < (startJudgeTime - perfectPeriod))
        {
            
            Debug.Log("early time block: " + p + ", pose=" + ((currentBlock - 1) % 3));
            // early
            if (p > threshold)
            {
                isEarly = true;
            }
        }

        // Display feedback
        if (currentTime > startJudgeTime + gracePeriod)
        {
            if (!isPerfect && !isEarly && !isLate)
            {
                feedbackText.text = "Miss";
            } else if (!isPassCheckPoint) 
            {
                feedbackText.text = "Hold the gesture";
            } else if (isEarly)
            {
                feedbackText.text = "Too early";
            } else if (isPerfect) {
                feedbackText.text = "Perfect";
            }  else if (isLate)
            {
                feedbackText.text = "Too late";
            }
        }

        // check gesture within judge period
        if (currentTime <= stopJudgeTime && currentTime >= startJudgeTime)
        {
            player.GetComponent<Renderer>().material.color = Color.red;
        } else
        {
            player.GetComponent<Renderer>().material.color = Color.white;
        }

    }

    
}

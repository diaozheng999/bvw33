﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float startJudgeWaitTime = secondPerBeat * 4f;
    private float stopJudgeWaitTime = secondPerBeat * 7f;
    private float waitTime = secondPerBeat * 4f;
    private float generateWaitTime = secondPerBeat * 8f; 

    private float moveSpeed = stageOffSet / (secondPerBeat * 3f);

    private int currentBlock = 0;
    private int numOfGeneratedBlock = 0;
    private int numOfBlockStage = 3;
    private int numOfPoseOnRoundStage = 5;

    [SerializeField]
    private GameObject stageGenerator;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject camera;

    void Start () {
        startTime = Time.time;
        nextGenerateTime = startTime + generateWaitTime;
        SetTheScene(2);
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
        startJudgeTime = startTime + startJudgeWaitTime;
        stopJudgeTime = startTime + stopJudgeWaitTime;
        // update player position
        nextPlayerPosition = player.transform.position;
        nextPlayerPosition.z += stageOffSet;
        currentBlock++;
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

        if (currentTime >= (stopJudgeTime + secondPerBeat))
        {
            if (currentBlock < (numOfBlockStage - 1))
            {
                // update player position
                nextPlayerPosition.z += stageOffSet;
                currentBlock++;
                // update judge time (judge every 8 beats)
                startJudgeTime += (waitTime * 2f);
                stopJudgeTime += (waitTime * 2f);
            }
            else if (currentBlock == (numOfBlockStage - 1))
            {
                // update player position
                nextPlayerPosition.z += (stageOffSet * 2f);
                currentBlock++;
                // update judge time (judge in 12 beats)
                startJudgeTime += (waitTime * 3f);
                stopJudgeTime += (waitTime * 3f);
            }
            else if (currentBlock < (numOfBlockStage + numOfPoseOnRoundStage - 1))
            {
                currentBlock++;
                // update judge time (judge every 4 beats)
                startJudgeTime += waitTime;
                stopJudgeTime += waitTime;
            }
        }
        // move the player
        float step = moveSpeed * Time.deltaTime;
        player.transform.position = Vector3.MoveTowards(player.transform.position, nextPlayerPosition, step);
        camera.transform.position = player.transform.position;

        // check position at start point

        // check position within judge period
        if (currentTime <= stopJudgeTime && currentTime >= startJudgeTime)
        {
            player.GetComponent<Renderer>().material.color = Color.red;
        } else
        {
            player.GetComponent<Renderer>().material.color = Color.white;
        }

    }

    
}

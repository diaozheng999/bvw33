using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    private static float BPM = 120.0f;
    private static float secondPerBeat = 60.0f / BPM;

    private float gameDelay = 0.0f;
    
    private static float stageOffSet = 5f;
    private float currentStagePosition = 0;

    private Vector3 nextCameraPosition;
    private Vector3 nextModel2Position;

    private float startTime;
    private float currentTime;

    private float startJudgeTime;
    private float stopJudgeTime;

    private float nextGenerateTime;
    private float generateWaitTime = secondPerBeat * 8f;

    private float nextRoundTime;
    private float waitTime = secondPerBeat * 4f;

    private float perfectPeriod = 0.3f;
    private float gracePeriod = 0.5f;

    private float moveSpeed = stageOffSet / (secondPerBeat * 3f);

    private int currentBlock = 0;
    private int numOfGeneratedBlock = 0;
    private int numOfBlockStage = 10;
    private int numOfPoseOnRoundStage = 5;

    [SerializeField]
    private GameObject stageGenerator;
    [SerializeField]
    private GameObject model1;
    [SerializeField]
    private GameObject model2;
    [SerializeField] GameObject visualiser;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField]
    private AudioClip drumSound;

    private float threshold = 0.4f;

    private bool isStart = false;
    private bool isOver = false;

    private bool isEarly = false;
    private bool isPerfect = false;
    private bool isLate = false;
    private bool isPassCheckPoint = true;

    [SerializeField]
    private Text feedbackText;
    [SerializeField]
    private Image instructionImage;
    public Sprite[] instructions;

    private string model1StartAnimationTriggerName;
    private string model1EndAnimationTriggerName;
    private string model2StartAnimationTriggerName;
    private string model2EndAnimationTriggerName;

    void Start () {
        SetTheScene(7); 
    }

    public void StartGame()
    {
        StartCoroutine(DelayedStartTime());
        StartCoroutine(PlayDrumSound());
    }

    private IEnumerator DelayedStartTime()
    {
        yield return new WaitForSeconds(gameDelay);
        isStart = true;
        startTime = Time.time;
        nextGenerateTime = startTime + generateWaitTime;
        
        SetThePlayer();
        SetTheInstruction();
        SetPoseAnimation();
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
        // update model1 position
        nextCameraPosition = model1.transform.position;
        nextCameraPosition.z += stageOffSet;
        nextModel2Position = model2.transform.position;
        nextModel2Position.z += stageOffSet;
        currentBlock++;
        nextRoundTime = startTime + waitTime * 2f;
        // play the walking animation
        model1.GetComponent<Animator>().SetTrigger("StartGame");
        model2.GetComponent<Animator>().SetTrigger("StartGame");
    }

    private void SetTheInstruction()
    {
        instructionImage.sprite = instructions[(currentBlock - 1) % 3]; /* NOTE TO SELF: DON'T HARD CODE THIS! */
    }

    private void SetPoseAnimation()
    {
        model1StartAnimationTriggerName = "StartPose" + ((currentBlock - 1) % 3 + 1);
        model1EndAnimationTriggerName = "EndPose" + ((currentBlock - 1) % 3 + 1);
        model2StartAnimationTriggerName = "StartPose" + ((currentBlock) % 3 + 1);
        model2EndAnimationTriggerName = "EndPose" + ((currentBlock) % 3 + 1);
    }

    private IEnumerator PlayDrumSound()
    {
        soundSource.Play();
        // soundSource.PlayOneShot(drumSound);
        yield return new WaitForSeconds(secondPerBeat);
        soundSource.Stop();
        StartCoroutine(PlayDrumSound());
    }

    void Update () {
        if (isStart)
        {
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

            // update model position
            if (currentTime >= nextRoundTime)
            {
                if (currentBlock < (numOfBlockStage - 1))
                {
                    // update model position
                    nextCameraPosition.z += stageOffSet;
                    nextModel2Position.z += stageOffSet;
                    currentBlock++;
                    nextRoundTime += waitTime * 2f;
                }
                else if (currentBlock == (numOfBlockStage - 1))
                {
                    // update model1 position
                    nextCameraPosition.z += (stageOffSet * 2f);
                    nextModel2Position.z += (stageOffSet * 2f);
                    currentBlock++;
                    nextRoundTime += waitTime * 3f;
                }
                else if (currentBlock < (numOfBlockStage + numOfPoseOnRoundStage - 1))
                {
                    currentBlock++;
                    nextRoundTime += waitTime;
                }
                // update the instruction image
                SetTheInstruction();
                model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, false);
                model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, false);
                SetPoseAnimation();
            }

            // move the camera and model
            float step = moveSpeed * Time.deltaTime;
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, nextCameraPosition, step);
            if (currentBlock <= (numOfBlockStage - 1))
            {
                model1.transform.position = Vector3.MoveTowards(model1.transform.position, nextCameraPosition, step);
            }
            else
            {
                model1.GetComponent<Animator>().SetTrigger("TurnLeft");
            }
            if (currentBlock <= (numOfBlockStage - 2))
            {
                model2.transform.position = Vector3.MoveTowards(model2.transform.position, nextModel2Position, step);
            }
            else
            {
                model2.GetComponent<Animator>().SetTrigger("TurnRight");
            }


            //float p = 0;
            float p = PoseEstimator.instance.Estimate((currentBlock-1) % 3 /* NOTE TO SELF: DON'T HARD CODE THIS! */);
            // check gesture at start point
            if (currentTime <= (startJudgeTime + perfectPeriod) && currentTime >= (startJudgeTime - perfectPeriod))
            {
                Debug.Log("perfect time block: " + p + ", pose=" + ((currentBlock - 1) % 3));
                if (p > threshold)
                {
                    isPerfect = true;
                }
            }
            else if (currentTime > (startJudgeTime + perfectPeriod) && currentTime <= (startJudgeTime + gracePeriod))
            {
                Debug.Log("late time block: " + p + ", pose=" + ((currentBlock - 1) % 3));
                // late
                if (p > threshold)
                {
                    isLate = true;
                }
            }
            else if (currentTime >= (startJudgeTime - gracePeriod) && currentTime < (startJudgeTime - perfectPeriod))
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
                }
                else if (!isPassCheckPoint)
                {
                    feedbackText.text = "Hold the gesture";
                }
                else if (isEarly)
                {
                    feedbackText.text = "Too early";
                }
                else if (isPerfect)
                {
                    feedbackText.text = "Perfect";
                }
                else if (isLate)
                {
                    feedbackText.text = "Too late";
                }
            }

            // change animation
            if (currentTime <= stopJudgeTime && currentTime >= startJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, true);
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, true);
            }
            else if (currentTime > stopJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, false);
                model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, true);
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, false);
                model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, true);
            }

            // check gesture within judge period
            /*
            if (currentTime <= stopJudgeTime && currentTime >= startJudgeTime)
            {
                model1.GetComponent<Renderer>().material.color = Color.red;
            } else
            {
                model1.GetComponent<Renderer>().material.color = Color.white;
            }*/
        }
    }
        
}

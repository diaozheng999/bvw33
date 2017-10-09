using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    private static float BPM = 120.0f;
    private static float secondPerBeat = 60.0f / BPM;

    private float gameDelay = 0.05f;
    
    private static float stageOffSet = 5f;
    private float currentStagePosition = 0;

    private Vector3 nextCameraPosition;
    private Vector3 nextModel1Position;
    private Vector3 nextModel2Position;

    private float startTime;
    private float currentTime;

    private float startJudgeTime;
    private float stopJudgeTime;

    private float endTutorialTime = secondPerBeat * 16f;
    private float prepareTime = secondPerBeat * 24f;
    private float tutorialTime = secondPerBeat * 24f;
    private float countdownTime = secondPerBeat * 8f;

    private float nextGenerateTime;
    private float generateWaitTime = secondPerBeat * 8f;

    private float nextRoundTime;
    private float waitTime = secondPerBeat * 4f;

    private float pictureTime = float.MaxValue;

    private float perfectPeriod = 0.5f; // 1s in total
    private float earlyGracePeriod = 0.6f;
    private float lateGracePeriod = 0.8f;

    private float moveSpeed = stageOffSet / (secondPerBeat * 3f);

    private int currentBlock = 0;
    private int numOfGeneratedBlock = 0;
    private int numOfTutorialPose = 3;
    private static int numOfBlockStage = 16; /* total number of poses is numOfBlockStage - 1 */
    private int numOfPoseOnRoundStage = 4;

    private int numOfPose = 3;
    private int currentPose = -1;
    private int[] poseSequence = new int[] { 1, 2, 3, /* <-tutorial*/0, 1, 2, 3, 4, 1, 2, 3, 2, 1, 4, 5, 6, 2, 5, 6 };

    private float score = 0;
    float averageScore = 0f;
    float countableValues = 0f;
    private float scoreForEachPose = 100f / (numOfBlockStage - 1);
    private float successScore = 60f;

    [SerializeField]
    private GameObject stageGenerator;
    [SerializeField]
    private GameObject model1;
    [SerializeField]
    private GameObject model2;
    [SerializeField]
    private GameObject visualiser;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private ParticleSystem flashParticleSystem;
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField]
    private AudioClip drumSound;

    [SerializeField]
    GameObject bananaEmitter;

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
    [SerializeField]
    private GameObject readyImgae;
    [SerializeField]
    private GameObject[] countdownImages;
    [SerializeField]
    private Texture countdown2Texture;
    [SerializeField]
    private Texture countdown1Texture;
    [SerializeField]
    private GameObject hereWeGoImgae;
    [SerializeField]
    private Button startButton;
    public Sprite[] instructions;
    [SerializeField]
    private GameObject freeSytleImage;
    [SerializeField]
    private GameObject winImage;
    [SerializeField]
    private GameObject loseImage;
    [SerializeField]
    public Animator fadeImageAnimator;
    [SerializeField]
    public AnimationClip fadeImageAnimationClip;

    private string model1StartAnimationTriggerName;
    private string model1EndAnimationTriggerName;
    private string model2StartAnimationTriggerName;
    private string model2EndAnimationTriggerName;

    private bool isWalk = false;
    private bool isFreeStyleImage = false;
    private bool freestyle = false;

    bool beginScoreCapture = false;

    void Start () {
        SetTheScene(7); 
    }

    public void StartGame()
    {
        soundSource.Play();
		StartCoroutine(DelayedTutorial());
    }

    private IEnumerator DelayedTutorial()
    {
        Destroy(startButton.gameObject);
        yield return new WaitForSeconds(gameDelay);
        isStart = true;
        startTime = Time.time;
        nextGenerateTime = startTime + prepareTime + tutorialTime + countdownTime + generateWaitTime;

        GamePreparation();
    }


    private IEnumerator DelayedStartTime()
    {

        // readyText.text = "Ready?";
        readyImgae.SetActive(true);
        var readyanim = readyImgae.GetComponent<Animator>();
        readyanim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(waitTime);
        readyanim.SetTrigger("FadeOut");

        foreach (var countdown in countdownImages){
            var anim = countdown.GetComponent<Animator>();
            countdown.SetActive(true);
            anim.SetTrigger("FadeIn");
            yield return new WaitForSeconds(secondPerBeat);
            anim.SetTrigger("FadeOut");
        }


        hereWeGoImgae.SetActive(true);
        var hwganim = hereWeGoImgae.GetComponent<Animator>();
        hwganim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(secondPerBeat);
        hwganim.SetTrigger("FadeOut");
        flashParticleSystem.Play();

        beginScoreCapture = true;
        StarController.instance.Begin();

        yield return new WaitForSeconds(1);
        readyImgae.SetActive(false);
        foreach(var countdown in countdownImages){
            countdown.SetActive(false);
        }
        hereWeGoImgae.SetActive(false);
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

    private void GamePreparation()
    {
        // update start time
        nextRoundTime = startTime + prepareTime;
        startJudgeTime = startTime + prepareTime - secondPerBeat * 4f;
        stopJudgeTime = startTime + prepareTime - secondPerBeat;
        // update camera & model1 & model2 position
        nextCameraPosition = camera.transform.position;
        nextModel1Position = model1.transform.position;
        nextModel2Position = model2.transform.position;
    }

    

    private void SetTheInstruction()
    {
        instructionImage.sprite = instructions[poseSequence[currentPose] - 1]; 
    }

    private void SetPoseAnimation()
    {
        model1StartAnimationTriggerName = "StartPose" + (poseSequence[currentPose]);
        model1EndAnimationTriggerName = "EndPose" + (poseSequence[currentPose]);
        model2StartAnimationTriggerName = "StartPose" + (poseSequence[currentPose + 1]);
        model2EndAnimationTriggerName = "EndPose" + (poseSequence[currentPose + 1]);
    }

    /* 
    private IEnumerator PlayDrumSound()
    {
        soundSource.Play();
        yield return new WaitForSeconds(secondPerBeat);
        soundSource.Stop();
        StartCoroutine(PlayDrumSound());
    }*/


    IEnumerator EndingDelayed(){
        fadeImageAnimator.SetBool("isFade", true);
        StarController.instance.End();

        yield return new WaitForSeconds(2f);

        PhotoSelection.instance.StopCapture();
        if (score >= successScore)
        {
            // TODO: win!!!
            winImage.SetActive(true);
        }
        else
        {
            // TODO: lose!!!
            loseImage.SetActive(true);
        }

    }

    void DropBananas () {
        Instantiate(bananaEmitter, camera.transform);
    }

    void Update () {
        // game over
        if (isOver)
        {
            StartCoroutine(EndingDelayed());
            isStart = false;
            isOver = false;
        }

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

            if (currentTime >= endTutorialTime)
            {
                model1.GetComponent<Animator>().SetTrigger("EndTutorial");
                model2.GetComponent<Animator>().SetTrigger("EndTutorial");
            }

            // update model position && judge timing && calculate score
            if (currentTime >= nextRoundTime)
            {
                if (!isWalk)
                {
                    isWalk = true;
                    model1.transform.Rotate(0, 180, 0);
                    model2.transform.Rotate(0, 180, 0);
                    // play the walking animation
                    model1.GetComponent<Animator>().SetTrigger("StartGame");
                    model2.GetComponent<Animator>().SetTrigger("StartGame");
                }
                

                var sc = 0f;

                if (currentBlock > 0)
                {
                    if (isPerfect)
                    {
                        score += scoreForEachPose;
                        sc = 1f;
                    }
                    else if (isEarly || isLate)
                    {
                        score += scoreForEachPose * 0.6f;
                        sc = 0.6f;
                    }
                }

                if (beginScoreCapture) {

                    Debug.Log(currentPose);
                    averageScore = ((averageScore * countableValues) + sc ) / (countableValues+1);
                    Debug.Log(averageScore);
                    countableValues += 1;
                }
                StarController.instance.SetScore(averageScore);

                feedbackText.text = "";
                isPerfect = false;
                isEarly = false;
                isLate = false;
                isPassCheckPoint = true;
                Debug.Log("reset bool");

                // tutorial period
                if (currentPose < (numOfTutorialPose - 1))
                {
                    // update judge time (judge every 8 beats)
                    startJudgeTime += (waitTime * 2f);
                    stopJudgeTime += (waitTime * 2f);
                    nextRoundTime += waitTime * 2f;
                    currentPose++;
                } else if (currentPose == (numOfTutorialPose - 1))
                {
                    // update judge time (judge every 8 beats)
                    startJudgeTime += (waitTime * 2f);
                    stopJudgeTime += (waitTime * 2f);
                    nextRoundTime += waitTime * 2f;
                    currentPose++;
                    StartCoroutine(DelayedStartTime());
                } else if (currentBlock < (numOfBlockStage - 1))
                {
                    // update judge time (judge every 8 beats)
                    startJudgeTime += (waitTime * 2f);
                    stopJudgeTime += (waitTime * 2f);
                    // update model position
                    nextCameraPosition.z += stageOffSet;
                    nextModel1Position.z += stageOffSet;
                    nextModel2Position.z += stageOffSet;
                    currentPose++;
                    currentBlock++;
                    nextRoundTime += waitTime * 2f;
                }
                else if (currentBlock == (numOfBlockStage - 1))
                {
                    // update judge time (judge in 12 beats)
                    // startJudgeTime += (waitTime * 3f);
                    // stopJudgeTime += (waitTime * 3f);
                    // update model1 position
                    nextCameraPosition.z += (stageOffSet * 2f);
                    nextModel1Position.z += (stageOffSet * 2f);
                    nextModel2Position.z += (stageOffSet * 2f);
                    currentPose++;
                    currentBlock++;
                    nextRoundTime += waitTime * 2f;
                    pictureTime = nextRoundTime + secondPerBeat * 2f;
                }
                else if (currentBlock < (numOfBlockStage + numOfPoseOnRoundStage))
                {
                    // update judge time (judge every 4 beats)
                    // startJudgeTime += waitTime;
                    // stopJudgeTime += waitTime;
                    currentBlock++;
                    nextRoundTime += waitTime;
                } else if (currentBlock == (numOfBlockStage + numOfPoseOnRoundStage))
                {
                    isOver = true;
                }

                // update the instruction image
                if (currentBlock < numOfBlockStage && poseSequence[currentPose] > 0)
                {
                    SetTheInstruction();
                    model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, false);
                    model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, false);
                    SetPoseAnimation();
                }
                else
                {
                    instructionImage.sprite = instructions[6];
                    if (currentBlock > numOfBlockStage && !isFreeStyleImage)
                    {
                        isFreeStyleImage = true;
                        freeSytleImage.SetActive(true);
                        var freeanim = freeSytleImage.GetComponent<Animator>();
                        freeanim.SetTrigger("FadeIn");
                    }
                }

            }

            // move the camera and model
            float step = moveSpeed * Time.deltaTime;
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, nextCameraPosition, step);
            if (currentBlock <= (numOfBlockStage - 1))
            {
                if (model1.transform.position != nextModel1Position)
                {
                    model1.transform.position = Vector3.MoveTowards(model1.transform.position, nextModel1Position, step);
                }
            }
            else
            {
                model1.GetComponent<Animator>().SetTrigger("TurnLeft");
            }
            if (currentBlock <= (numOfBlockStage - 2))
            {
                if (model2.transform.position != nextModel2Position)
                {
                    model2.transform.position = Vector3.MoveTowards(model2.transform.position, nextModel2Position, step);
                }
            }
            else
            {
                model2.GetComponent<Animator>().SetTrigger("TurnRight");
            }

            if (currentPose >=0 && currentPose < poseSequence.Length && poseSequence[currentPose] > 0)
            {
                //float p = 0;
                float p = PoseEstimator.instance.Estimate(poseSequence[currentPose] - 1);
                // check gesture at start point
                if (!isEarly && currentTime <= (startJudgeTime + perfectPeriod) && currentTime >= (startJudgeTime - perfectPeriod))

                {
                    Debug.Log("perfect time block: " + p + ", pose=" + (poseSequence[currentPose] - 1));
                    if (p > threshold)
                    {
                        if (!isPerfect && poseSequence[currentPose] == 2) DropBananas();
                        isPerfect = true;
                        feedbackText.text = "Perfect";

                    }
                }
                else if (!isEarly && !isPerfect && currentTime > (startJudgeTime + perfectPeriod) && currentTime <= (startJudgeTime + lateGracePeriod))

                {
                    Debug.Log("late time block: " + p + ", pose=" + (poseSequence[currentPose] - 1));
                    // late
                    if (p > threshold)
                    {
                        if (!isLate && poseSequence[currentPose] == 2) DropBananas();
                        isLate = true;
                        feedbackText.text = "A little bit late";

                    }
                }
                else if (currentTime >= (startJudgeTime - earlyGracePeriod) && currentTime < (startJudgeTime - perfectPeriod))     
                {

                    Debug.Log("early time block: " + p + ", pose=" + (poseSequence[currentPose] - 1));
                    // early
                    if (p > threshold)
                    {
                        if (!isEarly && poseSequence[currentPose] == 2) DropBananas();
                        isEarly = true;
                        feedbackText.text = "A little bit early";

                    }
                }

                // Display feedback
                if (currentTime > startJudgeTime + lateGracePeriod)
                {
                    if (!isPerfect && !isEarly && !isLate)
                    {
                        feedbackText.text = "Miss";
                    }
                    else if (!isPassCheckPoint)
                    {
                        feedbackText.text = "Hold the gesture"; // TODO: NOT IMPLEMENTED YET!!!
                    }
                }
            }
            

            // change animation
            if (currentBlock == 0)
            {
                if (currentPose >= 0 && poseSequence[currentPose] > 0 && currentTime >= (startJudgeTime - secondPerBeat) && currentTime < stopJudgeTime)
                {
                    model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, true);
                    model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, true);
                }
            }
            else
            {
                if (model1.transform.position == nextModel1Position && currentTime < startJudgeTime)
                {
                    model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, true);
                }
                if (model2.transform.position == nextModel2Position && currentTime < startJudgeTime)
                {
                    model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, true);
                }
            }
            
            if (currentPose >= 0 && currentPose < poseSequence.Length && poseSequence[currentPose] > 0 && currentTime >= stopJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, false);
                model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, true);
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, false);
                model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, true);
            }

            // taking picture for player
            if (currentTime > pictureTime && !freestyle)
            {
                freestyle = true;
                PhotoSelection.instance.BeginCapture();
                // TODO: Take picture here!!!
                pictureTime += waitTime;
            }
        }
    }
        
}

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
    private Vector3 nextModel2Position;

    private float startTime;
    private float currentTime;

    private float startJudgeTime;
    private float stopJudgeTime;

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
    private static int numOfBlockStage = 9; /* total number of poses is numOfBlockStage - 1 */
    private int numOfPoseOnRoundStage = 4;

    private int numOfPose = 3;

    private float score = 0;
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

    private float threshold = 0.4f;

    private bool isMoveCamera = false;
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

    bool freestyle = false;

    void Start () {
        SetTheScene(7); 
    }

    public void StartGame()
    {
        //StartCoroutine(PlayDrumSound());
        soundSource.Play();
        StartCoroutine(DelayedStartTime());
    }


    private IEnumerator DelayedStartTime()
    {
        yield return new WaitForSeconds(16 * secondPerBeat + gameDelay);
        Destroy(startButton.gameObject);
        // readyText.text = "Ready?";
        isMoveCamera = true;
        yield return new WaitForSeconds(waitTime);

        foreach(var countdown in countdownImages){
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
        isStart = true;
        startTime = Time.time;
        nextGenerateTime = startTime + generateWaitTime;

        
        SetThePlayer();
        SetTheInstruction();
        SetPoseAnimation();
        yield return new WaitForSeconds(1);
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
        instructionImage.sprite = instructions[(currentBlock - 1) % numOfPose]; 
    }

    private void SetPoseAnimation()
    {
        model1StartAnimationTriggerName = "StartPose" + ((currentBlock - 1) % numOfPose + 1);
        model1EndAnimationTriggerName = "EndPose" + ((currentBlock - 1) % numOfPose + 1);
        model2StartAnimationTriggerName = "StartPose" + ((currentBlock) % numOfPose + 1);
        model2EndAnimationTriggerName = "EndPose" + ((currentBlock) % numOfPose + 1);
    }

    /* 
    private IEnumerator PlayDrumSound()
    {
        soundSource.Play();
        // soundSource.PlayOneShot(drumSound);
        yield return new WaitForSeconds(secondPerBeat);
        soundSource.Stop();
        StartCoroutine(PlayDrumSound());
    }*/

    public void EndingDelayed()
    {
        fadeImageAnimator.SetBool("isFade", false);
        PhotoSelection.instance.StopCapture();

        if (score >= successScore)
        {
            // TODO: win!!!
        }
        else
        {
            // TODO: lose!!!
            loseImage.SetActive(true);
        }
    }

    void Update () {
        // move the camera in the beginning
        if (!isStart && isMoveCamera)
        {
            float step = 0.5f / (secondPerBeat * 3) * Time.deltaTime;
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, model1.transform.position, step);
        }  
        // game over
        if (isOver)
        {
            Invoke("EndingDelayed", fadeImageAnimationClip.length);
            fadeImageAnimator.SetBool("isFade", true);
            isMoveCamera = false;
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

            /*
            // update judge time
            if (currentTime > nextRoundTime  (stopJudgeTime + secondPerBeat/2 ))
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
            */

            // update model position && judge timing && calculate score
            if (currentTime >= nextRoundTime)
            {
                if (isPerfect)
                {
                    score += scoreForEachPose;
                }
                else if (isEarly || isLate)
                {
                    score += scoreForEachPose * 0.6f;
                }
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
                    // update model position
                    nextCameraPosition.z += stageOffSet;
                    nextModel2Position.z += stageOffSet;
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
                    nextModel2Position.z += (stageOffSet * 2f);
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
                if (currentBlock < numOfBlockStage)
                {
                    SetTheInstruction();
                    model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, false);
                    model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, false);
                    SetPoseAnimation();
                } else
                {
                    instructionImage.sprite = null;
                    if (currentBlock > numOfBlockStage)
                    {
                        freeSytleImage.SetActive(true);
                    }
                }
                
            }

            // move the camera and model
            float step = moveSpeed * Time.deltaTime;
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, nextCameraPosition, step);
            if (currentBlock <= (numOfBlockStage - 1))
            {
                if (model1.transform.position != nextCameraPosition)
                {
                    // model1.GetComponent<Animator>().speed = 1.2f;
                    model1.transform.position = Vector3.MoveTowards(model1.transform.position, nextCameraPosition, step);
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
                    // model2.GetComponent<Animator>().speed = 1.2f;
                    model2.transform.position = Vector3.MoveTowards(model2.transform.position, nextModel2Position, step);
                }
            }
            else
            {
                model2.GetComponent<Animator>().SetTrigger("TurnRight");
            }

            //float p = 0;
            float p = PoseEstimator.instance.Estimate((currentBlock-1) % numOfPose);
            // check gesture at start point
            if (!isEarly && currentTime <= (startJudgeTime + perfectPeriod) && currentTime >= (startJudgeTime - perfectPeriod))
            {
                //Debug.Log("perfect time block: " + p + ", pose=" + ((currentBlock - 1) % numOfPose));
                if (p > threshold)
                {
                    isPerfect = true;
                    feedbackText.text = "Perfect";
                }
            }
            else if (!isEarly && !isPerfect && currentTime > (startJudgeTime + perfectPeriod) && currentTime <= (startJudgeTime + lateGracePeriod))
            {
                //Debug.Log("late time block: " + p + ", pose=" + ((currentBlock - 1) % numOfPose));
                // late
                if (p > threshold)
                {
                    isLate = true;
                    feedbackText.text = "A little bit late";
                }
            }
            else if (currentTime >= (startJudgeTime - earlyGracePeriod) && currentTime < (startJudgeTime - perfectPeriod))
            {

                //Debug.Log("early time block: " + p + ", pose=" + ((currentBlock - 1) % numOfPose));
                // early
                if (p > threshold)
                {
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

            // change animation
            if (model1.transform.position == nextCameraPosition && currentTime < startJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, true);
            }
            if (model2.transform.position == nextModel2Position && currentTime < startJudgeTime)
            {
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, true);
            }
           
            /*
            if (currentTime < stopJudgeTime && currentTime >= startJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, true);
                model1.GetComponent<Animator>().speed = 1f;
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, true);
                model2.GetComponent<Animator>().speed = 1f;
            }
            else 
            */
            
            if (currentTime >= stopJudgeTime)
            {
                model1.GetComponent<Animator>().SetBool(model1StartAnimationTriggerName, false);
                model1.GetComponent<Animator>().SetBool(model1EndAnimationTriggerName, true);
                model2.GetComponent<Animator>().SetBool(model2StartAnimationTriggerName, false);
                model2.GetComponent<Animator>().SetBool(model2EndAnimationTriggerName, true);
            }

            // TEST: check gesture within judge period
            /*
            if (currentTime <= stopJudgeTime && currentTime >= startJudgeTime)
            {
                model1.GetComponent<Renderer>().material.color = Color.red;
            } else
            {
                model1.GetComponent<Renderer>().material.color = Color.white;
            }*/

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

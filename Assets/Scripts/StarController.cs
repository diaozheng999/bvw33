using System.Collections.Generic;
using UnityEngine;
using PGT.Core;

public class StarController : Singleton<StarController> {

    [SerializeField] Animator[] background;
    [SerializeField] Animator[] foreground;

    [SerializeField] float animateInDelay;

    [SerializeField] float animateOutDelay;

    bool begun = false;

    int target = 0;
    int currentValue = 0;

    float targetf = 0f;

    public void Begin(){
        this.StartCoroutine1(BeginCoroutine());
    }

    public void SetScore(float value){
        targetf = value;
        target = Mathf.FloorToInt(value * (background.Length+1));
        if(begun) this.StartCoroutine1(Adjust());
    }

    public void SetScore(int value){
        target = value;
        targetf = (float) value / (background.Length+1);
        if(begun) this.StartCoroutine1(Adjust());
    }

    public void Add(float delta){
        targetf += delta;
        target = Mathf.FloorToInt(targetf * (background.Length+1));
        if(begun) this.StartCoroutine1(Adjust());
    }

    IEnumerator<object> Adjust(){
        var delay = new WaitForSeconds(0.1f);
        while(currentValue != target){
            if(currentValue < target){
                foreground[currentValue++].SetTrigger("FadeIn");
            }else if(currentValue > target){
                foreground[--currentValue].SetTrigger("FadeOut");
            }
            yield return delay;
        }
    }

    IEnumerator<object> BeginCoroutine(){
        var delay = new WaitForSeconds(0.1f);
        foreach(var anim in background){
            anim.SetTrigger("FadeIn");
            yield return delay;
        }
        yield return this.StartCoroutine1(Adjust()).GetAwaiter();
        begun = true;
    }

    void Update(){
        if (Input.GetKeyUp(KeyCode.Space) && !begun){
            Begin();
        }

        if(Input.GetKeyUp(KeyCode.Alpha0)){
            SetScore(0);
        }
        if(Input.GetKeyUp(KeyCode.Alpha1)){
            SetScore(1);
        }
        
        if(Input.GetKeyUp(KeyCode.Alpha2)){
            SetScore(2);
        }
        if(Input.GetKeyUp(KeyCode.Alpha3)){
            SetScore(3);
        }
        if(Input.GetKeyUp(KeyCode.Alpha4)){
            SetScore(4);
        }
        if(Input.GetKeyUp(KeyCode.Alpha5)){
            SetScore(5);
        }
    }

}
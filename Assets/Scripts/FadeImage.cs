using UnityEngine;


public class FadeImage : MonoBehaviour {
    [SerializeField] GameMaster master;

    public void FadeIn(){
        master.DelayedEnding();
    }
}
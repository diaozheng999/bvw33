using System.Collections.Generic;
using UnityEngine;
using PGT.Core;

public class Bananas : MonoBehaviour {
    ParticleSystem particles;


    void Start () {
        particles = GetComponent<ParticleSystem>();
        Simulate();
    }

    public void Simulate(){
        this.StartCoroutine1(BananaCoroutine());
    }

    IEnumerator<object> BananaCoroutine(){
        particles.Play();
        var emission = particles.emission;
        emission.rateOverTime = 20f;
        yield return new WaitForSeconds(1f);
        emission.rateOverTime = 0f;
        yield return new WaitForSeconds(1);
        particles.Stop();
    }
}
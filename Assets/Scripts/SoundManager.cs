using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;

public class SoundManager : Singleton<SoundManager> {

	[SerializeField] AudioClip[] flashes;
	[SerializeField] AudioClip[] excellent;
	[SerializeField] AudioClip applause;
	[SerializeField] AudioClip boo;

	[SerializeField] AudioClip monkey;

	[SerializeField] AudioSource asrc;
	[SerializeField] AudioSource asrcSec;



	IEnumerator PlayFlashesCr() {
		var n_flashes = 5;
		for(int i=0; i<n_flashes; ++i){
			asrcSec.PlayOneShot(flashes[Random.Range(0, flashes.Length)]);
			yield return new WaitForSeconds(Random.value/2);
		}
	}

	public void PlayFlashes(){
		StartCoroutine(PlayFlashesCr());
	}

	public void PlayFlashOnce(){
		asrc.PlayOneShot(flashes[Random.Range(0, flashes.Length)]);
	}

	public void PlayApplause(){
		asrc.PlayOneShot(applause);
	}

	public void PlayBoo(){
		asrc.PlayOneShot(boo);
	}
	public void PlayExcellentSoundscape(){
		asrc.PlayOneShot(excellent[Random.Range(0, excellent.Length)]);
		PlayFlashes();
		PlayApplause();
	}
	

	public void PlayGoodSoundscape(){
		PlayFlashOnce();
		PlayApplause();
	}

	public void PlayMonkeySound(){
		asrc.PlayOneShot(monkey);
	}

	/* 
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.F)){
			PlayFlashOnce();
		}
		if(Input.GetKeyUp(KeyCode.G)){
			PlayGoodSoundscape();
		}
		if(Input.GetKeyUp(KeyCode.E)){
			PlayExcellentSoundscape();
		}
	}*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PGT.Core.Behaviours
{
    public class Ephemeral : SyncMonoBehaviour {


        [SerializeField]
        float timeout;

	    // Use this for initialization
	    IEnumerator Start () {
            yield return new WaitForSeconds(timeout);
            Destroy(gameObject);
	    }
	
    }

}

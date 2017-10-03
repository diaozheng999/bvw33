using UnityEngine;
using UnityEngine.SceneManagement;


namespace PGT.Core.Behaviours
{

    public class GoToScene : SyncMonoBehaviour {

        public string scene;


        public void gotoScene()
        {
            SceneManager.LoadScene(scene);
        }
        
    }
}

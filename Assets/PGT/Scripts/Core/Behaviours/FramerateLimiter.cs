namespace PGT.Core.Behaviours
{
    using UnityEngine;

    public class FramerateLimiter : SyncMonoBehaviour
    {
        public bool limitFramerate = false;
        public int targetFramerate = 15;

        void Awake()
        {
            if (limitFramerate)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFramerate;
            }
        }
    }
}

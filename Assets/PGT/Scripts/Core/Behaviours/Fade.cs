using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGT.Core.Behaviours
{
    public class Fade : MonoBehaviour
    {

        [SerializeField]
        Shader blendShader;

        [SerializeField]
        float blend;


        Material blendMat;

        void Start()
        {
            blendMat = new Material(blendShader);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // if blend is 1, then simply do nothing
            if (Mathf.Abs(blend - 1) < 0.000001f)
            {
                Graphics.Blit(src, dest);
                return;
            }

            blendMat.SetFloat("_Blend", blend);
            Graphics.Blit(src, dest, blendMat);
        }

        public void SetBlend(float _blend)
        {
            blend = _blend;
        }
    }
}
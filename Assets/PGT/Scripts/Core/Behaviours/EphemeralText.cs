using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PGT.Core.Behaviours
{
    public class EphemeralText : SyncMonoBehaviour
    {

        public void BeginDecay()
        {
            StartCoroutine(Decay());
        }

        IEnumerator Decay()
        {
            var txt = GetComponent<Text>();
            var pos = transform.position;
            for (float v = 0; v < 0.9; v = Mathf.Lerp(v, 1, 3f * Time.deltaTime))
            {
                transform.position = pos + (20 * v) * Vector3.up;
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, (1 - v));
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }
}

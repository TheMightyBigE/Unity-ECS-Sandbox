using System.Collections;
using UnityEngine;

namespace ECS.Sandbox.Runtime
{
    public class FrameRateUI : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI fpsText;
        int fps = 0;

        private void Start()
        {
            StartCoroutine(FrameRoutine());
        }

        private void Update()
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
        }

        private IEnumerator FrameRoutine()
        {
            WaitForSeconds wfs = new WaitForSeconds(1 / 30f);
            while (this.enabled)
            {
                fpsText.text = fps.ToString();
                yield return wfs;
            }
        }
    }
}
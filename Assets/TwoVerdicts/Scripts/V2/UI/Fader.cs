using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 화면 암전/밝아짐 + 가운데 자막. 페이드 중에는 입력을 막는다.
    /// </summary>
    public class Fader : MonoBehaviour
    {
        private CanvasGroup group;
        private TextMeshProUGUI caption;

        public static Fader Create(Transform parent)
        {
            RectTransform root = Ui.Stretch("Fader", parent);
            var fader = root.gameObject.AddComponent<Fader>();
            fader.Build();
            return fader;
        }

        private void Build()
        {
            group = gameObject.AddComponent<CanvasGroup>();
            Ui.Image(Ui.Stretch("Black", transform), null, Ui.Ink, true);
            caption = Ui.Label(transform, "Caption", "", 640f, 360f, 1100f, 120f, 28f, Ui.Paper, TextAlignmentOptions.Center);
            group.alpha = 1f;
            group.blocksRaycasts = true;
        }

        public void SetBlack(bool black, string text = "")
        {
            group.alpha = black ? 1f : 0f;
            group.blocksRaycasts = black;
            caption.text = text;
        }

        /// <summary>암전 -> (자막 표시) -> middle 실행 -> 밝아짐 -> done.</summary>
        public void Transition(Action middle, Action done = null, string text = "", float hold = 0.2f)
        {
            StartCoroutine(TransitionRoutine(middle, done, text, hold));
        }

        private IEnumerator TransitionRoutine(Action middle, Action done, string text, float hold)
        {
            Core.InputLock.Push();
            caption.text = "";
            yield return Fade(0f, 1f, 0.35f);
            caption.text = text;
            Ui.SafeInvoke(middle);
            yield return new WaitForSecondsRealtime(hold);
            caption.text = "";
            yield return Fade(1f, 0f, 0.35f);
            Core.InputLock.Pop();
            Ui.SafeInvoke(done);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            group.blocksRaycasts = true;
            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            group.alpha = to;
            group.blocksRaycasts = to > 0.5f;
        }
    }
}

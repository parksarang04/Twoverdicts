using System;
using System.Collections;
using TMPro;
using TwoVerdicts.V2.Core;
using UnityEngine;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 시작 화면: 키 아트 + 제목 + 시작하기 / 설정 / 종료.
    /// </summary>
    public class TitleView : View
    {
        private CanvasGroup group;
        private Action onStart;
        private Action onSettings;

        public static TitleView Create(Transform parent) => CreateView<TitleView>(parent, "Title");

        protected override void Build()
        {
            group = gameObject.AddComponent<CanvasGroup>();
            Ui.Image(Ui.Stretch("Bg", transform), null, Ui.Ink, true);
            UnityEngine.UI.Image art = Ui.Image(Ui.Stretch("Art", transform), Art.Ui("title"), Color.white);
            art.preserveAspect = true;
            Ui.Label(transform, "Title", "회색 증언", 330f, 150f, 600f, 90f, 72f, Ui.Paper, TextAlignmentOptions.Center);
            Ui.Label(transform, "Sub", "두 개의 판결", 330f, 222f, 600f, 40f, 26f, Ui.Red, TextAlignmentOptions.Center);
            Ui.Button(transform, "시작하기", 330f, 360f, 280f, 60f, () => onStart?.Invoke(), 24f);
            Ui.Button(transform, "설정", 330f, 436f, 280f, 60f, () => onSettings?.Invoke(), 24f);
            Ui.Button(transform, "종료", 330f, 512f, 280f, 60f, Quit, 24f);
            Ui.Label(transform, "Notice", "이 게임은 실제 사건에서 영감을 받은 가상의 이야기입니다. 등장하는 인물과 지명은 모두 허구입니다.",
                640f, 690f, 1200f, 30f, 15f, Ui.Dim, TextAlignmentOptions.Center);
        }

        public void Show(Action start, Action settings)
        {
            onStart = start;
            onSettings = settings;
            group.alpha = 0f;
            OpenView();
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            for (float t = 0f; t < 1.5f; t += Time.unscaledDeltaTime)
            {
                group.alpha = t / 1.5f;
                yield return null;
            }
            group.alpha = 1f;
        }

        public void Close() => CloseView();

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

using System;
using TMPro;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// F1 개발자 메뉴: 공략 + 치트 (단서 전부, 행동력, 반전 바로 보기, 재판 바로 가기).
    /// 발표 시연용.
    /// </summary>
    public class DevCheatView : View
    {
        private Action onAllClues;
        private Action onActions;
        private Action onReveal;
        private Action onTrial;

        public static DevCheatView Create(Transform parent) => CreateView<DevCheatView>(parent, "DevCheat");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.9f), true);
            Ui.Box(transform, "Panel", 640f, 360f, 1220f, 660f, 0.98f);
            Ui.Label(transform, "Title", "개발자 메뉴", 640f, 64f, 900f, 40f, 28f, Ui.Red, TextAlignmentOptions.Center);
            Ui.Button(transform, "단서 전부 얻기", 190f, 160f, 280f, 56f, () => Run(onAllClues), 18f);
            Ui.Button(transform, "행동력 +10", 190f, 230f, 280f, 56f, () => Run(onActions), 18f);
            Ui.Button(transform, "반전 바로 보기", 190f, 300f, 280f, 56f, () => Run(onReveal), 18f);
            Ui.Button(transform, "바로 재판으로", 190f, 370f, 280f, 56f, () => Run(onTrial), 18f);
            Ui.Button(transform, "닫기 (Esc)", 190f, 600f, 280f, 56f, Close, 18f);
            Ui.Label(transform, "Note", "단서 전부 얻기: 모든 인물 선명도 100,\n반전 직전 추리까지 완료", 190f, 470f, 280f, 80f,
                14f, Ui.Gray, TextAlignmentOptions.Center);
            TextMeshProUGUI guide = Ui.Label(transform, "Guide", StoryDb.Walkthrough, 790f, 360f, 860f, 580f, 17f, Ui.Paper);
            guide.lineSpacing = 12f;
        }

        public void Show(Action allClues, Action actions, Action reveal, Action trial)
        {
            onAllClues = allClues;
            onActions = actions;
            onReveal = reveal;
            onTrial = trial;
            OpenView();
        }

        private void Run(Action action)
        {
            CloseView();
            Ui.SafeInvoke(action);
        }

        private void Update()
        {
            if (!KeyReady) return;
            if (InputLock.Pressed(UnityEngine.InputSystem.Key.F1) || InputLock.Pressed(UnityEngine.InputSystem.Key.Backquote)
                || InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Close();
        }

        private void Close() => CloseView();
    }
}

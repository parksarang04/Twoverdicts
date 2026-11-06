using System;
using TMPro;
using TwoVerdicts.V2.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 게임 중 Esc: 계속하기 / 단서 트리 / 설정 / 처음으로.
    /// </summary>
    public class PauseView : View
    {
        private Action onSettings;
        private Action onTree;
        private Action onDev;

        public static PauseView Create(Transform parent) => CreateView<PauseView>(parent, "Pause");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.8f), true);
            Ui.Box(transform, "Panel", 640f, 380f, 440f, 520f, 0.98f);
            Ui.Label(transform, "Title", "일시정지", 640f, 190f, 400f, 50f, 32f, Ui.Paper, TextAlignmentOptions.Center);
            Ui.Button(transform, "계속하기", 640f, 270f, 300f, 54f, Close);
            Ui.Button(transform, "단서 트리 (T)", 640f, 336f, 300f, 54f, () => Go(onTree));
            Ui.Button(transform, "설정", 640f, 402f, 300f, 54f, () => Go(onSettings));
            Ui.Button(transform, "처음으로", 640f, 468f, 300f, 54f, BackToTitle);
            Ui.Button(transform, "개발자 메뉴 (공략/치트)", 640f, 548f, 300f, 54f, () => Go(onDev), 17f);
        }

        public void Show(Action settings, Action tree, Action dev)
        {
            onSettings = settings;
            onTree = tree;
            onDev = dev;
            OpenView();
        }

        private void Go(Action next)
        {
            CloseView();
            Ui.SafeInvoke(next);
        }

        private void Update()
        {
            if (KeyReady && InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Close();
        }

        private void Close() => CloseView();

        private static void BackToTitle()
        {
            InputLock.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

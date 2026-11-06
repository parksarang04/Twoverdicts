using System;
using TMPro;
using TwoVerdicts.V2.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 설정창: 배경음/효과음 볼륨, 텍스트 속도, 난이도, 전체화면.
    /// </summary>
    public class SettingsView : View
    {
        private Button speedButton;
        private Button difficultyButton;
        private Button fullscreenButton;
        private TextMeshProUGUI note;
        private Action onClosed;

        /// <summary>효과음 미리듣기 (슬라이더 놓을 때).</summary>
        public Action OnPreviewSfx { get; set; }

        public static SettingsView Create(Transform parent) => CreateView<SettingsView>(parent, "Settings");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.85f), true);
            Ui.Box(transform, "Panel", 640f, 360f, 760f, 560f, 0.98f);
            Ui.Label(transform, "Title", "설정", 640f, 120f, 600f, 50f, 34f, Ui.Paper, TextAlignmentOptions.Center);
            Row("배경음", 210f);
            Ui.Slider(transform, 760f, 210f, 340f, GameSettings.BgmVolume, v => GameSettings.BgmVolume = v);
            Row("효과음", 280f);
            Ui.Slider(transform, 760f, 280f, 340f, GameSettings.SfxVolume, v =>
            {
                GameSettings.SfxVolume = v;
                OnPreviewSfx?.Invoke();
            });
            Row("글자 속도", 350f);
            speedButton = Ui.Button(transform, "", 760f, 350f, 340f, 50f, CycleSpeed);
            Row("난이도", 420f);
            difficultyButton = Ui.Button(transform, "", 760f, 420f, 340f, 50f, ToggleDifficulty);
            Row("전체화면", 490f);
            fullscreenButton = Ui.Button(transform, "", 760f, 490f, 340f, 50f, ToggleFullscreen);
            note = Ui.Label(transform, "Note", "", 640f, 545f, 700f, 30f, 15f, Ui.Gray, TextAlignmentOptions.Center);
            Ui.Button(transform, "닫기", 640f, 600f, 220f, 54f, Close);
        }

        private void Row(string label, float y)
        {
            Ui.Label(transform, "Row_" + label, label, 400f, y, 220f, 40f, 22f, Ui.Paper, TextAlignmentOptions.Left);
        }

        public void Show(Action closed = null)
        {
            onClosed = closed;
            RefreshLabels();
            OpenView();
        }

        private void RefreshLabels()
        {
            Ui.SetLabel(speedButton, GameSettings.SpeedNames[GameSettings.TextSpeed]);
            Ui.SetLabel(difficultyButton, GameSettings.EasyMode ? "쉬움 (추천)" : "보통");
            Ui.SetLabel(fullscreenButton, Screen.fullScreen ? "켜짐" : "꺼짐");
            note.text = GameSettings.EasyMode
                ? "쉬움: 행동력 14, 틀린 추리도 행동력 안 깎임, 단서 위치 힌트"
                : "보통: 행동력 10, 틀린 추리마다 행동력 -1 (난이도는 새 게임부터 적용)";
        }

        private void CycleSpeed()
        {
            GameSettings.TextSpeed = (GameSettings.TextSpeed + 1) % 3;
            RefreshLabels();
        }

        private void ToggleDifficulty()
        {
            GameSettings.EasyMode = !GameSettings.EasyMode;
            RefreshLabels();
        }

        private void ToggleFullscreen()
        {
            GameSettings.ToggleFullscreen();
            RefreshLabels();
        }

        private void Update()
        {
            if (KeyReady && InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Close();
        }

        private void Close()
        {
            CloseView();
            Action callback = onClosed;
            onClosed = null;
            Ui.SafeInvoke(callback);
        }
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 선택지 창. 보기마다 활성/비활성 지정 가능. 취소 허용 시 "돌아가기" 버튼 추가.
    /// </summary>
    public class ChoiceView : View
    {
        private const float ButtonGap = 64f;

        private TextMeshProUGUI promptText;
        private RectTransform buttonRoot;
        private Action<int> onPick;
        private Action onCancel;

        public static ChoiceView Create(Transform parent) => CreateView<ChoiceView>(parent, "Choice");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.7f), true);
            promptText = Ui.Label(transform, "Prompt", "", 640f, 92f, 1100f, 110f, 24f, Ui.Paper,
                TextAlignmentOptions.Center);
            buttonRoot = Ui.Stretch("Buttons", transform);
        }

        public void Show(string prompt, IList<string> options, Action<int> pick, IList<bool> enabled = null,
            Action cancel = null)
        {
            promptText.text = prompt;
            onPick = pick;
            onCancel = cancel;
            Ui.ClearChildren(buttonRoot);
            int count = options.Count + (cancel != null ? 1 : 0);
            float startY = 360f - (count - 1) * ButtonGap / 2f + 40f;
            for (int i = 0; i < options.Count; i++)
            {
                CreateOption(i, options[i], startY + i * ButtonGap, enabled == null || enabled[i]);
            }
            if (cancel != null)
            {
                Ui.Button(buttonRoot, "돌아가기", 640f, startY + options.Count * ButtonGap, 360f, 50f, Cancel, 18f);
            }
            OpenView();
        }

        private void CreateOption(int index, string label, float y, bool isEnabled)
        {
            Button button = Ui.Button(buttonRoot, label, 640f, y, 760f, 54f, () => Pick(index));
            button.interactable = isEnabled;
        }

        private void Pick(int index)
        {
            CloseView();
            Action<int> callback = onPick;
            onPick = null;
            Ui.SafeInvoke(() => callback?.Invoke(index));
        }

        private void Cancel()
        {
            CloseView();
            Action callback = onCancel;
            onCancel = null;
            Ui.SafeInvoke(callback);
        }
    }
}

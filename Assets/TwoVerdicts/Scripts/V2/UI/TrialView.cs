using System;
using TMPro;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 최종 재판. 세 칸을 채워 문장을 완성하고 판결을 요청한다.
    /// </summary>
    public class TrialView : View
    {
        public const int SlotCount = 3;

        private readonly Button[] slots = new Button[SlotCount];
        private readonly bool[] filled = new bool[SlotCount];
        private Button submitButton;

        public Action<int> OnSlotClicked { get; set; }
        public Action OnSubmit { get; set; }
        public Action OnRefuse { get; set; }

        public static TrialView Create(Transform parent) => CreateView<TrialView>(parent, "Trial");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.55f), true);
            Ui.Box(transform, "Panel", 640f, 380f, 1060f, 520f, 0.97f);
            Ui.Label(transform, "Title", "최종 진술", 640f, 160f, 900f, 44f, 30f, Ui.Paper, TextAlignmentOptions.Center);
            string[] labels = { StoryDb.SlotKiller, StoryDb.SlotEvidence, StoryDb.SlotPlanter };
            for (int i = 0; i < SlotCount; i++) BuildSlot(i, labels[i], 240f + i * 100f);
            submitButton = Ui.Button(transform, "판결을 요청한다", 520f, 580f, 300f, 60f, () => OnSubmit?.Invoke(), 22f);
            Ui.Button(transform, StoryDb.NoJudgement, 830f, 580f, 260f, 60f, () => OnRefuse?.Invoke(), 20f);
        }

        private void BuildSlot(int index, string label, float y)
        {
            Ui.Label(transform, "Slot" + index, label, 340f, y, 420f, 40f, 22f, Ui.Gray, TextAlignmentOptions.Right);
            slots[index] = Ui.Button(transform, "( 고르기 )", 830f, y, 520f, 60f, () => OnSlotClicked?.Invoke(index), 20f);
        }

        public void Show()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                filled[i] = false;
                Ui.SetLabel(slots[i], "( 고르기 )");
            }
            submitButton.interactable = false;
            OpenView();
        }

        public void SetSlot(int index, string label)
        {
            filled[index] = true;
            Ui.SetLabel(slots[index], label);
            submitButton.interactable = filled[0] && filled[1] && filled[2];
        }

        public void Close() => CloseView();
    }
}

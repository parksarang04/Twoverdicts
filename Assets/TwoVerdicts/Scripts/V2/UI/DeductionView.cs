using System;
using TMPro;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 추리 보드. 위: 카드 두 장을 직접 골라 연결. 아래: 조합 목록(단서 트리와 같은 내용).
    /// 조합 목록에서 재료가 다 모인 줄은 [조합하기] 버튼으로 바로 연결할 수 있다.
    /// </summary>
    public class DeductionView : View
    {
        private const float ListTop = 372f;
        private const float RowGap = 50f;

        private Button slotA;
        private Button slotB;
        private Button connectButton;
        private TextMeshProUGUI noteText;
        private RectTransform list;
        private string cardA;
        private string cardB;
        private GameSession session;

        public Action<int> OnSlotClicked { get; set; }
        public Action<string, string> OnConnect { get; set; }
        public Action OnTree { get; set; }

        public static DeductionView Create(Transform parent) => CreateView<DeductionView>(parent, "Deduction");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.88f), true);
            Ui.Box(transform, "Board", 640f, 368f, 1180f, 680f, 0.98f);
            Ui.Label(transform, "Title", "추리 보드", 640f, 62f, 900f, 44f, 30f, Ui.Paper, TextAlignmentOptions.Center);
            slotA = Ui.Button(transform, "단서 A 고르기", 380f, 140f, 340f, 70f, () => OnSlotClicked?.Invoke(0), 19f);
            slotB = Ui.Button(transform, "단서 B 고르기", 900f, 140f, 340f, 70f, () => OnSlotClicked?.Invoke(1), 19f);
            Ui.Label(transform, "Plus", "+", 640f, 140f, 60f, 60f, 40f, Ui.Red, TextAlignmentOptions.Center);
            connectButton = Ui.Button(transform, "연결한다", 520f, 222f, 200f, 50f, Connect, 20f);
            Ui.Button(transform, "닫기 (Tab)", 760f, 222f, 200f, 50f, Close, 18f);
            noteText = Ui.Label(transform, "Note", "", 640f, 274f, 1000f, 30f, 18f, Ui.Gold, TextAlignmentOptions.Center);
            Ui.Label(transform, "ListTitle", "조합 목록 - 재료가 다 모이면 [조합하기]를 누르자", 640f, 320f, 1000f, 30f, 19f,
                Ui.Gray, TextAlignmentOptions.Center);
            list = Ui.Stretch("List", transform);
        }

        public void Show(GameSession gameSession)
        {
            session = gameSession;
            cardA = null;
            cardB = null;
            Ui.SetLabel(slotA, "단서 A 고르기");
            Ui.SetLabel(slotB, "단서 B 고르기");
            noteText.text = "";
            connectButton.interactable = false;
            OpenView();
            RefreshList();
        }

        public void RefreshList()
        {
            if (session == null) return;
            Ui.ClearChildren(list);
            for (int i = 0; i < StoryDb.Deductions.Length; i++) BuildRow(i, StoryDb.Deductions[i], ListTop + i * RowGap);
        }

        private void BuildRow(int index, Deduction d, float y)
        {
            bool hasA = session.HasCard(d.CardA);
            bool hasB = session.HasCard(d.CardB);
            bool done = session.HasCard(d.Result);
            string a = hasA ? StoryDb.GetCard(d.CardA).Name : "???";
            string b = hasB ? StoryDb.GetCard(d.CardB).Name : "???";
            string result = done ? StoryDb.GetCard(d.Result).Name : (d.IsReveal ? "그는 누구인가?" : "???");
            string text = $"{index + 1}. {StoryDb.TreeRowNames[index]}:  [{a}] + [{b}]  =>  {result}";
            Color color = done ? Ui.Dim : (hasA && hasB ? Ui.Paper : Ui.Gray);
            Ui.Label(list, "Row" + index, text, 560f, y, 960f, 40f, 17f, color, TextAlignmentOptions.Left);
            if (done)
            {
                Ui.Label(list, "Done" + index, "완료", 1130f, y, 120f, 40f, 17f, Ui.Dim, TextAlignmentOptions.Center);
                return;
            }
            Button go = Ui.Button(list, "조합하기", 1130f, y, 130f, 42f, () => OnConnect?.Invoke(d.CardA, d.CardB), 16f);
            go.interactable = hasA && hasB;
        }

        public void SetSlot(int index, string cardId)
        {
            Card card = StoryDb.GetCard(cardId);
            if (card == null) return;
            if (index == 0) cardA = cardId;
            else cardB = cardId;
            Ui.SetLabel(index == 0 ? slotA : slotB, card.Name);
            connectButton.interactable = cardA != null && cardB != null && cardA != cardB;
        }

        public void SetNote(string text) => noteText.text = text;

        private void Connect()
        {
            if (cardA == null || cardB == null) return;
            OnConnect?.Invoke(cardA, cardB);
        }

        private void Update()
        {
            if (!KeyReady) return;
            if (InputLock.Pressed(UnityEngine.InputSystem.Key.Escape) || InputLock.Pressed(UnityEngine.InputSystem.Key.Tab)) Close();
        }

        public void Close() => CloseView();
    }
}

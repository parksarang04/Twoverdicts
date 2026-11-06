using System;
using System.Collections.Generic;
using TMPro;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 수첩 카드 목록. 고르기 모드(onPick 있음)와 구경 모드(수첩, Q키) 겸용.
    /// </summary>
    public class CardPickerView : View
    {
        private const int Columns = 3;
        private const float ColumnW = 212f;
        private const float RowH = 46f;

        private TextMeshProUGUI titleText;
        private TextMeshProUGUI descName;
        private TextMeshProUGUI descBody;
        private RectTransform grid;
        private Button confirmButton;
        private string selected;
        private Action<string> onPick;
        private Action onCancel;

        public static CardPickerView Create(Transform parent) => CreateView<CardPickerView>(parent, "CardPicker");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.75f), true);
            Ui.Box(transform, "Panel", 640f, 370f, 1220f, 640f, 0.98f);
            titleText = Ui.Label(transform, "Title", "", 360f, 82f, 640f, 40f, 26f, Ui.Paper);
            grid = Ui.Stretch("Grid", transform);
            Ui.Box(transform, "DescBox", 1000f, 330f, 400f, 440f, 1f);
            descName = Ui.Label(transform, "DescName", "", 1000f, 150f, 360f, 60f, 22f, Ui.Gold);
            descBody = Ui.Label(transform, "DescBody", "", 1000f, 340f, 360f, 300f, 19f, Ui.Paper);
            confirmButton = Ui.Button(transform, "이걸로 한다", 920f, 620f, 220f, 54f, Confirm);
            Ui.Button(transform, "닫기", 1110f, 620f, 140f, 54f, Cancel);
            Ui.Label(transform, "Hint", "Esc 닫기", 1150f, 82f, 140f, 30f, 15f, Ui.Dim, TextAlignmentOptions.Right);
        }

        public void Show(string title, IReadOnlyList<string> cardIds, Action<string> pick, Action cancel = null)
        {
            titleText.text = title;
            onPick = pick;
            onCancel = cancel;
            selected = null;
            confirmButton.gameObject.SetActive(pick != null);
            confirmButton.interactable = false;
            descName.text = "";
            descBody.text = cardIds.Count == 0 ? "아직 아무것도 없다." : "카드를 골라 내용을 확인하자.";
            BuildGrid(cardIds);
            OpenView();
        }

        private void BuildGrid(IReadOnlyList<string> cardIds)
        {
            Ui.ClearChildren(grid);
            for (int i = 0; i < cardIds.Count; i++)
            {
                Card card = StoryDb.GetCard(cardIds[i]);
                if (card == null) continue;
                float x = 160f + (i % Columns) * ColumnW;
                float y = 140f + (i / Columns) * RowH;
                Button button = Ui.Button(grid, card.Name, x, y, ColumnW - 8f, RowH - 6f, () => Select(card), 16f);
                if (card.Kind == CardKind.Deduction) button.GetComponentInChildren<TextMeshProUGUI>().color = Ui.Gold;
            }
        }

        private void Select(Card card)
        {
            selected = card.Id;
            descName.text = card.Name;
            string hint = StoryDb.HintFor(card.Id);
            descBody.text = string.IsNullOrEmpty(hint) ? card.Desc : $"{card.Desc}\n\n<color=#888888>얻은 곳: {hint}</color>";
            confirmButton.interactable = true;
        }

        private void Update()
        {
            if (KeyReady && Core.InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Cancel();
        }

        private void Confirm()
        {
            if (selected == null) return;
            CloseView();
            Action<string> callback = onPick;
            onPick = null;
            string id = selected;
            Ui.SafeInvoke(() => callback?.Invoke(id));
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

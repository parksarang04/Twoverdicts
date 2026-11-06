using System;
using TMPro;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 인터뷰 창. 진술을 고르고 [추궁] 하거나 [증거 제시] 로 깬다.
    /// 판정 로직은 V2Game 이 하고, 이 창은 보여주기와 버튼 전달만 담당.
    /// </summary>
    public class InterviewView : View
    {
        private const float RowGap = 74f;

        private Image portrait;
        private TextMeshProUGUI nameText;
        private TextMeshProUGUI infoText;
        private TextMeshProUGUI selectedText;
        private RectTransform list;
        private Button pressButton;
        private Button presentButton;
        private Npc npc;
        private Statement selected;

        public Action<Npc, Statement> OnPress { get; set; }
        public Action<Npc, Statement> OnPresent { get; set; }
        public Action OnClosed { get; set; }

        public static InterviewView Create(Transform parent) => CreateView<InterviewView>(parent, "Interview");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0f, 0f, 0f, 0.82f), true);
            Ui.Box(transform, "PortraitBox", 210f, 270f, 320f, 320f, 1f);
            portrait = Ui.Image(Ui.Place("Portrait", transform, 210f, 270f, 288f, 288f), null, Color.white);
            portrait.preserveAspect = true;
            nameText = Ui.Label(transform, "Name", "", 210f, 460f, 320f, 40f, 28f, Ui.Paper, TextAlignmentOptions.Center);
            infoText = Ui.Label(transform, "Info", "", 210f, 505f, 320f, 60f, 18f, Ui.Gray, TextAlignmentOptions.Center);
            Ui.Label(transform, "ListTitle", "진술", 820f, 70f, 760f, 34f, 22f, Ui.Gray);
            list = Ui.Stretch("Statements", transform);
            selectedText = Ui.Label(transform, "Selected", "", 820f, 560f, 760f, 40f, 18f, Ui.Gold);
            pressButton = Ui.Button(transform, "추궁한다", 600f, 630f, 200f, 56f, () => Fire(OnPress));
            presentButton = Ui.Button(transform, "증거 제시", 820f, 630f, 200f, 56f, () => Fire(OnPresent));
            Ui.Button(transform, "나가기", 1060f, 630f, 200f, 56f, Close);
        }

        public void Show(Npc target, GameSession session, Sprite face)
        {
            npc = target;
            selected = null;
            portrait.sprite = face;
            nameText.text = target.Name;
            Refresh(session, face);
            OpenView();
        }

        public void Refresh(GameSession session, Sprite face)
        {
            portrait.sprite = face;
            int trust = session.GetTrust(npc.Id);
            infoText.text = $"선명도 {session.GetClarity(npc.Id)}%\n신뢰 {new string('#', trust)}{new string('-', GameSession.MaxTrust - trust)}";
            BuildList(session);
            UpdateButtons(session);
        }

        private void BuildList(GameSession session)
        {
            Ui.ClearChildren(list);
            int row = 0;
            foreach (Statement s in npc.Statements)
            {
                if (!session.IsUnlocked(s)) continue;
                bool done = session.IsDone(s.Id);
                string label = (done ? "[밝혀짐] " : "") + s.Text;
                Button b = Ui.Button(list, label, 820f, 130f + row * RowGap, 760f, 64f, () => Select(s, session), 17f);
                if (done) b.GetComponentInChildren<TextMeshProUGUI>().color = Ui.Dim;
                row++;
            }
        }

        private void Select(Statement statement, GameSession session)
        {
            selected = statement;
            selectedText.text = "> " + statement.Text;
            UpdateButtons(session);
        }

        private void UpdateButtons(GameSession session)
        {
            bool canAct = selected != null && !session.IsDone(selected.Id);
            pressButton.interactable = canAct;
            presentButton.interactable = canAct;
            if (selected == null) selectedText.text = "진술을 하나 고르자.";
        }

        private void Fire(Action<Npc, Statement> handler)
        {
            if (selected == null) return;
            handler?.Invoke(npc, selected);
        }

        private void Update()
        {
            if (KeyReady && InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Close();
        }

        public void Close()
        {
            if (!IsOpen) return;
            CloseView();
            Ui.SafeInvoke(OnClosed);
        }

        /// <summary>대사창/선택창을 띄우는 동안 잠깐 숨김.</summary>
        public void SetHidden(bool hidden)
        {
            if (IsOpen) gameObject.SetActive(!hidden);
        }
    }
}

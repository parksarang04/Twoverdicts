using TMPro;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 단서 트리. 1쪽: 어떤 단서끼리 조합하는지 (A + B = 결론).
    /// 2쪽: 누구의 어떤 진술을 무엇으로 깨는지.
    /// 가진 카드는 이름, 없는 카드는 ??? + 얻는 곳(쉬움 모드).
    /// </summary>
    public class HintTreeView : View
    {
        private const float BoxW = 270f;
        private const float BoxH = 62f;

        private RectTransform page;
        private GameSession session;
        private bool showingInterviews;

        public static HintTreeView Create(Transform parent) => CreateView<HintTreeView>(parent, "HintTree");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Dim", transform), null, new Color(0.02f, 0.02f, 0.03f, 0.95f), true);
            Ui.Label(transform, "Title", "단서 트리", 640f, 40f, 600f, 44f, 30f, Ui.Paper, TextAlignmentOptions.Center);
            Ui.Button(transform, "조합 트리", 520f, 92f, 220f, 46f, () => ShowPage(false), 18f);
            Ui.Button(transform, "진술 깨기 힌트", 760f, 92f, 220f, 46f, () => ShowPage(true), 18f);
            Ui.Button(transform, "닫기 (T / Esc)", 1140f, 40f, 200f, 46f, Close, 17f);
            page = Ui.Stretch("Page", transform);
        }

        public void Show(GameSession gameSession)
        {
            session = gameSession;
            OpenView();
            ShowPage(showingInterviews);
        }

        private void ShowPage(bool interviews)
        {
            showingInterviews = interviews;
            Ui.ClearChildren(page);
            if (interviews) BuildInterviewPage();
            else BuildTreePage();
        }

        private void BuildTreePage()
        {
            for (int i = 0; i < StoryDb.Deductions.Length; i++)
            {
                Deduction d = StoryDb.Deductions[i];
                float y = 170f + i * 84f;
                Ui.Label(page, "Row" + i, $"{i + 1}. {StoryDb.TreeRowNames[i]}", 110f, y, 190f, 60f, 17f, Ui.Gray,
                    TextAlignmentOptions.Left);
                CardBox(d.CardA, 350f, y, false);
                Ui.Label(page, "Plus", "+", 505f, y, 40f, 40f, 30f, Ui.Red, TextAlignmentOptions.Center);
                CardBox(d.CardB, 660f, y, false);
                Ui.Label(page, "Arrow", "=>", 815f, y, 50f, 40f, 26f, Ui.Red, TextAlignmentOptions.Center);
                CardBox(d.Result, 1000f, y, true, d.IsReveal);
            }
            Ui.Label(page, "Foot", "Tab 추리 보드에서 왼쪽 두 카드를 이으면 오른쪽 결론을 얻는다. 결론 카드는 다음 줄의 재료가 되기도 한다.",
                640f, 690f, 1200f, 30f, 16f, Ui.Dim, TextAlignmentOptions.Center);
        }

        private void CardBox(string cardId, float x, float y, bool isResult, bool secret = false)
        {
            bool owned = session.HasCard(cardId);
            Card card = StoryDb.GetCard(cardId);
            Image box = Ui.Box(page, "Card_" + cardId, x, y, BoxW, BoxH, 1f);
            box.color = owned ? (isResult ? Ui.Gold : Color.white) : new Color(0.5f, 0.5f, 0.55f, 1f);
            string title = owned ? card.Name : (secret ? "???  (그는 누구인가)" : "???");
            string hint = owned ? "" : HintLine(cardId, isResult);
            Ui.Label(page, "Name", title, x, y - 10f, BoxW - 20f, 28f, 18f, owned ? Ui.Paper : Ui.Gray,
                TextAlignmentOptions.Center);
            Ui.Label(page, "Hint", hint, x, y + 16f, BoxW - 16f, 24f, 13f, Ui.Dim, TextAlignmentOptions.Center);
        }

        private static string HintLine(string cardId, bool isResult)
        {
            if (isResult) return "왼쪽 두 카드를 조합";
            return GameSettings.EasyMode ? StoryDb.HintFor(cardId) : "(보통 모드: 힌트 없음)";
        }

        private void BuildInterviewPage()
        {
            int row = 0;
            foreach (Npc npc in StoryDb.Npcs)
            {
                if (npc.Statements == null) continue;
                foreach (Statement s in npc.Statements)
                {
                    float x = row < 9 ? 330f : 950f;
                    float y = 160f + (row % 9) * 58f;
                    StatementLine(npc, s, x, y);
                    row++;
                }
            }
        }

        private void StatementLine(Npc npc, Statement s, float x, float y)
        {
            bool done = session.IsDone(s.Id);
            string how;
            if (done) how = "밝혀짐";
            else if (s.BreakCard == null) how = "추궁해 보자";
            else if (session.HasCard(s.BreakCard)) how = "제시: " + StoryDb.GetCard(s.BreakCard).Name;
            else how = GameSettings.EasyMode ? "필요: ??? (" + StoryDb.HintFor(s.BreakCard) + ")" : "필요: ???";
            string quote = s.Text.Length > 26 ? s.Text.Substring(0, 26) + "..." : s.Text;
            Ui.Label(page, "S", $"{npc.Name}: \"{quote}\"", x, y - 10f, 600f, 26f, 16f, done ? Ui.Dim : Ui.Paper);
            Ui.Label(page, "H", "   " + how, x, y + 13f, 600f, 22f, 14f, done ? Ui.Dim : Ui.Gold);
        }

        private void Update()
        {
            if (!KeyReady) return;
            if (InputLock.Pressed(UnityEngine.InputSystem.Key.Escape) || InputLock.Pressed(UnityEngine.InputSystem.Key.T)) Close();
        }

        public void Close() => CloseView();
    }
}

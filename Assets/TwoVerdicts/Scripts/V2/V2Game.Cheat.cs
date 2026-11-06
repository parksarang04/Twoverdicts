using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;

namespace TwoVerdicts.V2
{
    /// <summary>F1 개발자 치트 + 인물 머리 위 "!" 조건.</summary>
    public partial class V2Game
    {
        private void OpenCheat()
        {
            cheat.Show(CheatAllClues, CheatActions, CheatReveal, CheatTrial);
        }

        private void CheatAllClues()
        {
            foreach (string id in StoryDb.Cards.Keys)
            {
                if (id != "d_me") session.AddCard(id);
            }
            foreach (Npc npc in StoryDb.Npcs) session.SetClarity(npc.Id, 100);
            hud.Toast("[개발자] 단서 전부 획득 - Tab 추리 보드 6번으로 반전");
        }

        private void CheatActions()
        {
            session.ResetActions(session.Actions + 10);
            hud.Toast("[개발자] 행동력 +10");
        }

        private void CheatReveal()
        {
            if (session.Revealed)
            {
                hud.Toast("이미 반전을 봤다");
                return;
            }
            CheatAllClues();
            PlayReveal();
        }

        private void CheatTrial()
        {
            if (!session.Revealed) CheatAllClues();
            StartTrial();
        }

        /// <summary>쉬움 모드에서 지금 깰 수 있는 진술(또는 해야 할 대화)이 있는 인물.</summary>
        private bool CanProgress(string npcId)
        {
            if (!GameSettings.EasyMode) return false;
            Npc npc = StoryDb.GetNpc(npcId);
            if (npc == null) return false;
            if (npc.IsTalkOnly) return !session.HasFlag("sua_done");
            if (session.GetTrust(npcId) <= 0) return false;
            foreach (Statement s in npc.Statements)
            {
                if (!session.IsUnlocked(s) || session.IsDone(s.Id)) continue;
                if (s.BreakCard != null && session.HasCard(s.BreakCard)) return true;
                if (s.PressReward != null && !session.HasFlag("press_" + s.Id)) return true;
            }
            return false;
        }
    }
}

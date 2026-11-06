using System.Collections.Generic;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;

namespace TwoVerdicts.V2
{
    /// <summary>인터뷰: 추궁, 증거 제시, 진술 깨기, 신뢰도.</summary>
    public partial class V2Game
    {
        private Npc interviewNpc;

        private void OpenInterview(Npc npc)
        {
            interviewNpc = npc;
            interview.OnPress = Press;
            interview.OnPresent = Present;
            interview.OnClosed = () => interviewNpc = null;
            interview.Show(npc, session, FaceOf(npc.Id));
        }

        private Sprite FaceOf(string npcId) => Art.Portrait(npcId, Clarity.StageOf(session.GetClarity(npcId)));

        private void RefreshInterview()
        {
            if (interviewNpc != null) interview.Refresh(session, FaceOf(interviewNpc.Id));
        }

        private void Press(Npc npc, Statement s)
        {
            if (s.PressLines == null)
            {
                dialogue.Show(Say(npc.Name, "...더 할 말 없습니다."), RefreshInterview);
                return;
            }
            dialogue.Show(s.PressLines, () =>
            {
                RewardPress(npc, s);
                RefreshInterview();
            });
        }

        private void RewardPress(Npc npc, Statement s)
        {
            string flag = "press_" + s.Id;
            if (session.HasFlag(flag)) return;
            session.SetFlag(flag);
            if (s.PressReward != null)
            {
                session.AddCard(s.PressReward);
                Complete(npc, s);
            }
            else if (s.BreakCard == null)
            {
                session.AddClarity(npc.Id, s.ClarityGain);
            }
        }

        private void Present(Npc npc, Statement s)
        {
            picker.Show($"{npc.Name}에게 무엇을 내밀까?", session.CardIds, id => Judge(npc, s, id));
        }

        private void Judge(Npc npc, Statement s, string cardId)
        {
            if (s.BreakCard != null && s.BreakCard == cardId)
            {
                dialogue.Show(s.BreakLines, () =>
                {
                    if (s.BreakReward != null) session.AddCard(s.BreakReward);
                    Complete(npc, s);
                    AfterBreak(s);
                });
                return;
            }
            WrongPresent(npc);
        }

        private void WrongPresent(Npc npc)
        {
            session.LoseTrust(npc.Id);
            sound.Sfx("sfx_wrong");
            var lines = new List<Line> { Say(npc.Name, "그게 지금 무슨 상관이죠?") };
            bool closed = session.GetTrust(npc.Id) <= 0;
            if (closed) lines.Add(Think($"({npc.Name}은(는) 입을 닫아 버렸다. 더는 들을 수 없다.)"));
            dialogue.Show(lines, () =>
            {
                if (closed) interview.Close();
                else RefreshInterview();
            });
        }

        private void Complete(Npc npc, Statement s)
        {
            session.MarkDone(s.Id);
            if (s.Flag != null) session.SetFlag(s.Flag);
            session.AddClarity(npc.Id, s.ClarityGain);
            world.Flash(0.9f);
            sound.Sfx("sfx_break");
        }

        private void AfterBreak(Statement s)
        {
            if (s.Flag != "deal")
            {
                RefreshInterview();
                return;
            }
            choice.Show("박진표가 거래를 제안한다.", StoryDb.DealOptions, index =>
            {
                session.SoldStory = index == 0;
                Line reply = index == 0
                    ? Say("박진표", "좋아, 거래 성립. 내일 1면은 '살인자의 딸'이다.")
                    : Say("박진표", "쳇. 원칙주의자 납셨네. 가져가.");
                dialogue.Show(reply, RefreshInterview);
            });
        }
    }
}

using System.Collections.Generic;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;

namespace TwoVerdicts.V2
{
    /// <summary>최종 재판, 판결, 2018년 재심 장면, 엔딩.</summary>
    public partial class V2Game
    {
        private static readonly string[] Suspects = { "mujin", "hangyeol", "jihye", "chorong", "reporter", "chief" };
        private static readonly string[] PlanterCandidates = { "chief", "reporter", "jihye", "mujin", "hangyeol" };

        private string chosenKiller;
        private string chosenEvidence;
        private string chosenPlanter;

        private void StartTrial()
        {
            fader.transform.SetAsLastSibling();
            sound.PlayBgm(SoundManager.BgmTrial);
            fader.Transition(() =>
            {
                sound.Sfx("sfx_gavel");
                blackout.SetActive(false);
                world.ShowRoom(StoryDb.CourtRoom, -6f);
                hud.SetRoom("해운지방법원");
                hud.SetPrompt(null);
            }, () => dialogue.Show(new[]
            {
                Think(StoryDb.TrialIntro),
                Say("재판장", "증인 최윤석. 수사 결과를 진술하세요.")
            }, OpenTrialView));
        }

        private void OpenTrialView()
        {
            chosenKiller = null;
            chosenEvidence = null;
            chosenPlanter = null;
            trial.OnSlotClicked = PickSlot;
            trial.OnSubmit = SubmitVerdict;
            trial.OnRefuse = ConfirmRefuse;
            trial.Show();
        }

        private void PickSlot(int slot)
        {
            if (slot == 0) PickKiller();
            else if (slot == 1) picker.Show(StoryDb.SlotEvidence, session.CardIds, id =>
            {
                chosenEvidence = id;
                trial.SetSlot(1, StoryDb.GetCard(id).Name);
            });
            else PickPlanter();
        }

        private void PickKiller()
        {
            var labels = new List<string>();
            var enabled = new List<bool>();
            foreach (string id in Suspects)
            {
                Npc npc = StoryDb.GetNpc(id);
                bool known = id == "mujin" || session.GetClarity(id) >= StoryDb.ColorThreshold;
                labels.Add(id == "mujin" ? "강무진 (모두가 지목한 사람)" : known ? npc.Name : "??? (아직 잘 모르는 사람)");
                enabled.Add(known);
            }
            choice.Show(StoryDb.SlotKiller, labels, index =>
            {
                chosenKiller = Suspects[index];
                trial.SetSlot(0, StoryDb.GetNpc(chosenKiller).Name);
            }, enabled, () => { });
        }

        private void PickPlanter()
        {
            var ids = new List<string>(PlanterCandidates);
            var labels = new List<string>();
            foreach (string id in ids) labels.Add(StoryDb.GetNpc(id).Name);
            if (session.Revealed && session.PageKept)
            {
                ids.Add(VerdictResolver.PlanterMe);
                labels.Add("최윤석 (나)");
            }
            ids.Add(VerdictResolver.PlanterNone);
            labels.Add(StoryDb.NoOne);
            choice.Show(StoryDb.SlotPlanter, labels, index =>
            {
                chosenPlanter = ids[index];
                trial.SetSlot(2, labels[index]);
            }, null, () => { });
        }

        private void SubmitVerdict()
        {
            trial.Close();
            EndingId id = VerdictResolver.Resolve(chosenKiller, chosenEvidence, chosenPlanter, false);
            string reason = id == EndingId.Fragment ? VerdictResolver.FailReason(chosenKiller, chosenEvidence) : null;
            PlayEnding(id, reason);
        }

        private void ConfirmRefuse()
        {
            choice.Show("정말 아무것도 판단하지 않겠는가?", new[] { StoryDb.NoJudgement }, _ =>
            {
                trial.Close();
                PlayEnding(EndingId.Silence, null);
            }, null, () => { });
        }

        private void PlayEnding(EndingId id, string reason)
        {
            gameOver = true;
            EndingDef def = StoryDb.GetEnding(id);
            var verdict = new List<Line>();
            if (reason != null) verdict.Add(Think(reason));
            verdict.AddRange(def.Verdict);
            sound.Sfx("sfx_gavel");
            dialogue.Show(verdict, () =>
            {
                fader.transform.SetAsLastSibling();
                fader.Transition(EnterFrame, () => PlayFrame(id, def), StoryDb.FrameCaption, 2.8f);
            });
        }

        private void EnterFrame()
        {
            inFrame = true;
            sound.PlayBgm(SoundManager.BgmEnding);
            world.SetColor(true);
            world.ShowRoom(StoryDb.FrameRoom, 0f, false);
            UnityEngine.Transform room = world.RoomTransform(StoryDb.FrameRoom);
            world.CreateActor("yunseok_old", room, -2.4f, () => 100, 5).Face(true);
            world.CreateActor("sua_adult", room, 2.6f, () => 100, 5).Face(false);
            hud.SetRoom("2018년 - 재심 증인 대기실");
            hud.SetActions(0);
        }

        private void PlayFrame(EndingId id, EndingDef def)
        {
            var lines = new List<Line>(def.Frame);
            lines.AddRange(StoryDb.ChoiceEcho(session.SuaAnswer, session.SoldStory));
            dialogue.Show(lines, () =>
            {
                hud.SetVisible(false);
                ending.Show(def.Title, def.Question, QuoteFor(id));
            });
        }

        private static string QuoteFor(EndingId id)
        {
            switch (id)
            {
                case EndingId.True: return "\"괴물과 싸우는 자는 스스로 괴물이 되지 않도록 조심하라.\" - 니체";
                case EndingId.Hidden: return "\"사실은 없다. 오직 해석만 있을 뿐이다.\" - 니체";
                case EndingId.Silence: return "\"검토되지 않은 삶은 살 가치가 없다.\" - 소크라테스";
                default: return "\"우리는 동굴 벽에 비친 그림자를 실재라고 믿는다.\" - 플라톤";
            }
        }
    }
}

using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;

namespace TwoVerdicts.V2
{
    /// <summary>추리 보드와 반전 장면.</summary>
    public partial class V2Game
    {
        private void OpenDeduction()
        {
            deduction.OnSlotClicked = index =>
                picker.Show("어떤 카드를 올릴까?", session.CardIds, id => deduction.SetSlot(index, id));
            deduction.OnConnect = TryConnect;
            deduction.OnTree = OpenTree;
            deduction.Show(session);
        }

        private void TryConnect(string a, string b)
        {
            Deduction d = StoryDb.FindDeduction(a, b);
            if (d == null)
            {
                deduction.SetNote("이 둘은 이어지지 않는다.");
                return;
            }
            if (session.HasCard(d.Result))
            {
                deduction.SetNote("이미 내린 결론이다.");
                return;
            }
            choice.Show(d.Question, d.Options, index => Conclude(d, index), null, () => { });
        }

        private void Conclude(Deduction d, int index)
        {
            if (index != d.Correct)
            {
                sound.Sfx("sfx_wrong");
                if (GameSettings.EasyMode)
                {
                    deduction.SetNote("말이 안 된다. 다른 결론을 골라 보자.");
                }
                else
                {
                    session.SpendAction();
                    deduction.SetNote($"말이 안 된다. (행동력 -1, 남은 행동 {session.Actions})");
                }
                if (d.IsReveal) dialogue.Show(Think("(아니다. 그 사람은 그 시간에 편의점에 없었다. ...그럼 누구지.)"));
                return;
            }
            deduction.Close();
            if (d.IsReveal)
            {
                PlayReveal();
                return;
            }
            session.AddCard(d.Result);
            world.Flash(1.4f);
            sound.Sfx("sfx_break");
            dialogue.Show(Think($"({StoryDb.GetCard(d.Result).Name}.)"));
        }

        private void PlayReveal()
        {
            world.SetColor(true);
            sound.StopBgm();
            sound.Sfx("stinger_reveal");
            dialogue.Show(StoryDb.RevealLines, () =>
            {
                session.MarkRevealed();
                session.AddCard("d_me");
                choice.Show("찢긴 수첩 한 장. 이걸 어떻게 할까?", StoryDb.PageOptions, ChoosePage);
            });
        }

        private void ChoosePage(int index)
        {
            bool keep = index == 0;
            session.ChoosePage(keep);
            if (!keep) sound.Sfx("sfx_burn");
            Line[] lines = keep
                ? new[] { Think("(종이를 수첩 사이에 끼웠다. 손이 떨렸다.)"), Think("(처음으로 거울 속 내 얼굴이 또렷했다.)") }
                : StoryDb.BurnLines;
            dialogue.Show(lines, () =>
            {
                world.SetColor(false);
                sound.PlayBgm(SoundManager.BgmExplore);
            });
        }
    }
}

using TwoVerdicts.V2.Core;
using System.Collections.Generic;
using TwoVerdicts.V2.Data;
using TwoVerdicts.V2.World;
using UnityEngine;

namespace TwoVerdicts.V2
{
    /// <summary>프롤로그, 지도 이동, 방 안 조사, 수아와의 대화.</summary>
    public partial class V2Game
    {
        private const float EntryX = -8f;

        private void BeginPrologue()
        {
            hud.SetVisible(false);
            blackout.SetActive(true);
            sound.PlayBgm(SoundManager.BgmExplore);
            dialogue.Show(StoryDb.Prologue, () =>
            {
                dialogue.Show(Think("(A/D 이동, E 조사와 대화. 반짝이는 곳이 단서, 머리 위 ! 는 지금 이야기할 사람. Tab 추리 보드, Esc 메뉴에 공략)"), OpenMap);
            });
        }

        private void OpenMap()
        {
            hud.SetPrompt(null);
            if (session.Actions <= 0)
            {
                dialogue.Show(Think("(시간이 다 됐다. 송치 서류를 들고 법정으로 간다.)"), StartTrial);
                return;
            }
            map.Show(session.Actions, GoTo, ConfirmCourt);
        }

        private void GoTo(string roomId)
        {
            if (roomId == world.CurrentRoom && !blackout.activeSelf) return;
            session.SpendAction();
            sound.Sfx("sfx_door");
            fader.transform.SetAsLastSibling();
            fader.Transition(() => EnterRoom(roomId));
        }

        private void EnterRoom(string roomId)
        {
            blackout.SetActive(false);
            world.ShowRoom(roomId, EntryX);
            hud.SetVisible(true);
            hud.SetRoom(StoryDb.GetRoom(roomId)?.Name ?? roomId);
        }

        private void ConfirmCourt()
        {
            choice.Show("재판을 시작할까? 한 번 들어가면 돌아올 수 없다.", new[] { "법정으로 간다" },
                _ => StartTrial(), null, OpenMap);
        }

        private void HandleUse(Interactable item)
        {
            switch (item.Kind)
            {
                case InteractKind.Spot: UseSpot(item); break;
                case InteractKind.Npc: TalkTo(item.Key); break;
                case InteractKind.Door: LeaveRoom(); break;
            }
        }

        private void UseSpot(Interactable item)
        {
            if (!int.TryParse(item.Key, out int index) || index < 0 || index >= StoryDb.Spots.Length) return;
            Spot spot = StoryDb.Spots[index];
            dialogue.Show(spot.Lines, () =>
            {
                if (spot.CardId != null && session.AddCard(spot.CardId)) world.MarkSpent(item);
            });
        }

        private void LeaveRoom()
        {
            string[] lines = StoryDb.VoiceOnLeave;
            string voice = lines[Random.Range(0, lines.Length)];
            dialogue.Show(Say(StoryDb.Voice, voice), OpenMap);
        }

        private void TalkTo(string npcId)
        {
            Npc npc = StoryDb.GetNpc(npcId);
            if (npc == null) return;
            if (npc.IsTalkOnly)
            {
                TalkToSua(npc);
                return;
            }
            if (session.GetTrust(npc.Id) <= 0)
            {
                dialogue.Show(Think($"({npc.Name}은(는) 더 이상 입을 열지 않는다.)"));
                return;
            }
            string metFlag = "met_" + npc.Id;
            if (session.HasFlag(metFlag))
            {
                OpenInterview(npc);
                return;
            }
            session.SetFlag(metFlag);
            dialogue.Show(npc.Greeting, () => OpenInterview(npc));
        }

        private void TalkToSua(Npc sua)
        {
            if (session.HasFlag("sua_done"))
            {
                dialogue.Show(Say(sua.Name, "약속했어요. 나중에 꼭 다시 물어볼 거예요."));
                return;
            }
            dialogue.Show(sua.Greeting, () =>
                choice.Show("수아에게 뭐라고 대답할까?", StoryDb.SuaAnswers, AnswerSua));
        }

        private void AnswerSua(int answer)
        {
            session.SuaAnswer = answer;
            session.SetFlag("sua_done");
            var lines = new List<Line> { Say(StoryDb.Me, StoryDb.SuaAnswers[answer]) };
            lines.AddRange(StoryDb.SuaLetterLines);
            dialogue.Show(lines, () =>
            {
                session.AddCard("c_sua_letter");
                session.AddClarity("sua", 40);
                session.AddClarity("mujin", 15);
                world.Flash(2.5f);
            });
        }
    }
}

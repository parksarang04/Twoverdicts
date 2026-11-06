using System;
using TwoVerdicts.V2.Core;

namespace TwoVerdicts.V2
{
    /// <summary>시작 화면, 인트로 영상, 설정, 일시정지, 단서 트리.</summary>
    public partial class V2Game
    {
        private bool started;

        private void ShowTitle()
        {
            hud.SetVisible(false);
            blackout.SetActive(true);
            sound.PlayBgm(SoundManager.BgmTitle);
            titleView.Show(StartGame, () => OpenSettings(null));
        }

        private void StartGame()
        {
            if (started) return;
            started = true;
            titleView.Close();
            session.ResetActions(GameSettings.StartActions);
            sound.StopBgm();
            introVideo.Play(BeginPrologue);
        }

        private void OpenSettings(Action back)
        {
            settings.Show(back);
        }

        private void OpenPause()
        {
            if (!started) return;
            pause.Show(() => OpenSettings(null), OpenTree, OpenCheat);
        }

        private void OpenTree()
        {
            sound.Sfx("sfx_paper");
            tree.Show(session);
        }
    }
}

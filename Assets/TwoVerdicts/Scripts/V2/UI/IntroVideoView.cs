using System;
using TMPro;
using TwoVerdicts.V2.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 뉴스 인트로 영상 (Resources/V2/video/intro_v2). 클릭, E, Space, Esc 로 건너뛰기.
    /// 영상이 없거나 오류가 나면 바로 다음으로 넘어감.
    /// </summary>
    public class IntroVideoView : View
    {
        private const string ClipPath = "V2/video/intro_v2";

        private RawImage screen;
        private VideoPlayer player;
        private RenderTexture target;
        private Action onDone;

        public static IntroVideoView Create(Transform parent) => CreateView<IntroVideoView>(parent, "IntroVideo");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Bg", transform), null, Color.black, true);
            screen = Ui.Stretch("Screen", transform).gameObject.AddComponent<RawImage>();
            screen.raycastTarget = false;
            Ui.FullScreenButton(transform, Finish);
            Ui.Label(transform, "Skip", "클릭하면 건너뛰기", 1150f, 700f, 240f, 26f, 15f, Ui.Dim, TextAlignmentOptions.Right);
            player = gameObject.AddComponent<VideoPlayer>();
            player.playOnAwake = false;
            player.isLooping = false;
            player.renderMode = VideoRenderMode.RenderTexture;
            player.audioOutputMode = VideoAudioOutputMode.Direct;
            player.prepareCompleted += p => p.Play();
            player.loopPointReached += _ => Finish();
            player.errorReceived += (_, msg) =>
            {
                Debug.LogWarning("[IntroVideoView] 영상 오류: " + msg);
                Finish();
            };
        }

        public void Play(Action done)
        {
            onDone = done;
            var clip = Resources.Load<VideoClip>(ClipPath);
            if (clip == null)
            {
                Debug.LogWarning("[IntroVideoView] 영상 없음: " + ClipPath);
                Ui.SafeInvoke(done);
                return;
            }
            target = new RenderTexture(1280, 720, 0);
            player.clip = clip;
            player.targetTexture = target;
            screen.texture = target;
            OpenView();
            player.Prepare();
            player.SetDirectAudioVolume(0, GameSettings.BgmVolume);
        }

        private void Update()
        {
            if (!KeyReady) return;
            if (InputLock.AdvancePressed() || InputLock.Pressed(UnityEngine.InputSystem.Key.Escape)) Finish();
        }

        private void Finish()
        {
            if (!IsOpen) return;
            player.Stop();
            CloseView();
            ReleaseTarget();
            Action callback = onDone;
            onDone = null;
            Ui.SafeInvoke(callback);
        }

        private void ReleaseTarget()
        {
            if (target == null) return;
            player.targetTexture = null;
            screen.texture = null;
            target.Release();
            Destroy(target);
            target = null;
        }

        private void OnDestroy() => ReleaseTarget();
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TwoVerdicts.Intro
{
    /// <summary>
    /// 뉴스 인트로 영상(intro_news.mp4) 재생. 끝나거나 "건너뛰기"를 누르면 콜백 호출.
    /// 이 스크립트는 항상 켜진 오브젝트에, root 는 자식 패널(RawImage + 건너뛰기 버튼)로 연결.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class IntroVideoController : MonoBehaviour
    {
        private const int VideoWidth = 1280;
        private const int VideoHeight = 720;

        [SerializeField] private VideoClip introClip;
        [SerializeField] private GameObject root;
        [SerializeField] private RawImage screen;
        [SerializeField] private Button skipButton;

        private VideoPlayer player;
        private RenderTexture renderTexture;
        private Action onFinished;
        private bool isPlaying;

        private void Awake()
        {
            player = GetComponent<VideoPlayer>();
            if (introClip == null || root == null || screen == null || skipButton == null)
            {
                Debug.LogError("[IntroVideoController] 인스펙터 참조 누락", this);
                enabled = false;
                return;
            }
            ConfigurePlayer();
            skipButton.onClick.AddListener(Finish);
            root.SetActive(false);
        }

        private void ConfigurePlayer()
        {
            player.playOnAwake = false;
            player.isLooping = false;
            player.source = VideoSource.VideoClip;
            player.clip = introClip;
            player.renderMode = VideoRenderMode.RenderTexture;
            player.audioOutputMode = VideoAudioOutputMode.Direct;
            player.prepareCompleted += HandlePrepared;
            player.loopPointReached += HandleEnded;
            player.errorReceived += HandleError;
        }

        public void Play(Action finished)
        {
            if (!enabled)
            {
                finished?.Invoke();
                return;
            }
            onFinished = finished;
            isPlaying = true;
            renderTexture = new RenderTexture(VideoWidth, VideoHeight, 0);
            player.targetTexture = renderTexture;
            screen.texture = renderTexture;
            root.SetActive(true);
            player.Prepare();
        }

        private void HandlePrepared(VideoPlayer source)
        {
            if (isPlaying) source.Play();
        }

        private void HandleEnded(VideoPlayer source)
        {
            Finish();
        }

        private void HandleError(VideoPlayer source, string message)
        {
            Debug.LogError($"[IntroVideoController] 영상 재생 오류: {message}", this);
            Finish();
        }

        private void Finish()
        {
            if (!isPlaying) return;
            isPlaying = false;
            player.Stop();
            root.SetActive(false);
            ReleaseTexture();
            Action callback = onFinished;
            onFinished = null;
            try
            {
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private void ReleaseTexture()
        {
            if (renderTexture == null) return;
            player.targetTexture = null;
            screen.texture = null;
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }

        private void OnDestroy()
        {
            if (skipButton != null) skipButton.onClick.RemoveListener(Finish);
            if (player == null) return;
            player.prepareCompleted -= HandlePrepared;
            player.loopPointReached -= HandleEnded;
            player.errorReceived -= HandleError;
            ReleaseTexture();
        }
    }
}

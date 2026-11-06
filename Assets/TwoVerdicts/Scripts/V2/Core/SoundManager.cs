using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 배경음(크로스페이드)과 효과음. 클립은 Resources/V2/audio 에서 불러옴.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public const string BgmTitle = "bgm_title";
        public const string BgmExplore = "bgm_explore";
        public const string BgmTrial = "bgm_trial";
        public const string BgmEnding = "bgm_ending";

        private const float FadeTime = 1.2f;

        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private AudioSource bgm;
        private AudioSource sfx;
        private Coroutine fade;
        private float bgmScale = 1f;

        private void Awake()
        {
            bgm = gameObject.AddComponent<AudioSource>();
            bgm.loop = true;
            bgm.playOnAwake = false;
            sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            GameSettings.OnChanged += ApplyVolume;
        }

        private void OnDestroy()
        {
            GameSettings.OnChanged -= ApplyVolume;
        }

        private AudioClip Clip(string name)
        {
            if (clips.TryGetValue(name, out AudioClip clip)) return clip;
            clip = Resources.Load<AudioClip>("V2/audio/" + name);
            if (clip == null) Debug.LogWarning("[SoundManager] 사운드 없음: " + name);
            clips[name] = clip;
            return clip;
        }

        public void PlayBgm(string name)
        {
            AudioClip clip = Clip(name);
            if (clip == null || (bgm.clip == clip && bgm.isPlaying)) return;
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(SwapBgm(clip));
        }

        public void StopBgm()
        {
            if (fade != null) StopCoroutine(fade);
            fade = StartCoroutine(SwapBgm(null));
        }

        private IEnumerator SwapBgm(AudioClip next)
        {
            for (float t = 0f; bgm.isPlaying && t < FadeTime; t += Time.unscaledDeltaTime)
            {
                bgmScale = 1f - t / FadeTime;
                ApplyVolume();
                yield return null;
            }
            bgm.Stop();
            bgm.clip = next;
            if (next == null) yield break;
            bgm.Play();
            for (float t = 0f; t < FadeTime; t += Time.unscaledDeltaTime)
            {
                bgmScale = t / FadeTime;
                ApplyVolume();
                yield return null;
            }
            bgmScale = 1f;
            ApplyVolume();
        }

        public void Sfx(string name, float volume = 1f)
        {
            AudioClip clip = Clip(name);
            if (clip != null) sfx.PlayOneShot(clip, volume * GameSettings.SfxVolume);
        }

        private void ApplyVolume()
        {
            if (bgm != null) bgm.volume = GameSettings.BgmVolume * bgmScale;
        }
    }
}

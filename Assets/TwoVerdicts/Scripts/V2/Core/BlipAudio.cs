using TwoVerdicts.V2.Data;
using UnityEngine;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 언더테일식 대사 기계음 + 진술 깨기 효과음 + 반전 효과음. 전부 코드로 합성 (오디오 파일 없음).
    /// </summary>
    public class BlipAudio : MonoBehaviour
    {
        private const int SampleRate = 44100;

        private AudioSource source;
        private AudioClip blipClip;
        private AudioClip chimeClip;
        private AudioClip revealClip;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            blipClip = Square("blip", 0.045f, 520f, 0.18f);
            chimeClip = Chord("chime", 0.7f, new[] { 523f, 659f, 784f }, 0.15f);
            revealClip = Chord("reveal", 2.2f, new[] { 55f, 82f, 110f }, 0.35f);
        }

        public void Play(string speaker)
        {
            if (source == null) return;
            source.pitch = PitchFor(speaker);
            source.PlayOneShot(blipClip, (speaker == null ? 0.35f : 0.6f) * GameSettings.SfxVolume);
        }

        public void PlayChime()
        {
            if (source == null) return;
            source.pitch = 1f;
            source.PlayOneShot(chimeClip, 0.8f * GameSettings.SfxVolume);
        }

        public void PlayReveal()
        {
            if (source == null) return;
            source.pitch = 1f;
            source.PlayOneShot(revealClip, GameSettings.SfxVolume);
        }

        private static float PitchFor(string speaker)
        {
            if (speaker == null) return 0.85f;
            if (speaker == StoryDb.Voice) return 0.7f;
            if (speaker == "강수아") return 1.6f;
            int hash = 0;
            foreach (char c in speaker) hash = hash * 31 + c;
            return 0.9f + ((hash & 0x7fffffff) % 50) / 100f;
        }

        private static AudioClip Square(string name, float seconds, float freq, float volume)
        {
            int n = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SampleRate;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t) > 0f ? 1f : -1f;
                data[i] = wave * volume * Mathf.Exp(-t / (seconds * 0.6f));
            }
            return Build(name, data);
        }

        private static AudioClip Chord(string name, float seconds, float[] freqs, float volume)
        {
            int n = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = (float)i / SampleRate;
                float sum = 0f;
                foreach (float f in freqs) sum += Mathf.Sin(2f * Mathf.PI * f * t);
                data[i] = sum / freqs.Length * volume * Mathf.Exp(-t / (seconds * 0.45f));
            }
            return Build(name, data);
        }

        private static AudioClip Build(string name, float[] data)
        {
            AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}

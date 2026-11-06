using System;
using UnityEngine;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 설정값 (PlayerPrefs 저장). 볼륨, 텍스트 속도, 난이도, 전체화면.
    /// </summary>
    public static class GameSettings
    {
        public const int EasyActions = 14;
        public const int NormalActions = 10;

        private const string KeyBgm = "tv2_bgm";
        private const string KeySfx = "tv2_sfx";
        private const string KeySpeed = "tv2_speed";
        private const string KeyEasy = "tv2_easy";

        public static readonly string[] SpeedNames = { "느림", "보통", "빠름" };

        public static event Action OnChanged;

        public static float BgmVolume
        {
            get => PlayerPrefs.GetFloat(KeyBgm, 0.7f);
            set => SetFloat(KeyBgm, Mathf.Clamp01(value));
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(KeySfx, 0.8f);
            set => SetFloat(KeySfx, Mathf.Clamp01(value));
        }

        public static int TextSpeed
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KeySpeed, 1), 0, 2);
            set => SetInt(KeySpeed, Mathf.Clamp(value, 0, 2));
        }

        /// <summary>쉬움: 행동력 14, 틀린 추리에 행동력 소모 없음, 단서 위치 힌트 표시.</summary>
        public static bool EasyMode
        {
            get => PlayerPrefs.GetInt(KeyEasy, 1) == 1;
            set => SetInt(KeyEasy, value ? 1 : 0);
        }

        public static int StartActions => EasyMode ? EasyActions : NormalActions;

        public static float CharInterval
        {
            get
            {
                if (TextSpeed == 0) return 0.045f;
                if (TextSpeed == 2) return 0.012f;
                return 0.028f;
            }
        }

        public static void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
            Notify();
        }

        private static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            Save();
        }

        private static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            Save();
        }

        private static void Save()
        {
            try
            {
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogWarning("[GameSettings] 저장 실패: " + e.Message);
            }
            Notify();
        }

        private static void Notify()
        {
            try
            {
                OnChanged?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

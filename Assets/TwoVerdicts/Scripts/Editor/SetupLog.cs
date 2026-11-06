using System;
using System.IO;
using UnityEngine;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 자동 세팅 진행 상황을 Logs/twoverdicts_setup.txt 에 남김 (콘솔에도 같이 출력).
    /// </summary>
    public static class SetupLog
    {
        private const string LogPath = "Logs/twoverdicts_setup.txt";

        public static void Info(string message)
        {
            Debug.Log("[TwoVerdicts] " + message);
            Append("INFO  " + message);
        }

        public static void Error(string message)
        {
            Debug.LogError("[TwoVerdicts] " + message);
            Append("ERROR " + message);
        }

        public static void Exception(Exception e)
        {
            Debug.LogException(e);
            Append("ERROR " + e);
        }

        private static void Append(string line)
        {
            try
            {
                Directory.CreateDirectory("Logs");
                File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss} {line}\n");
            }
            catch (Exception e)
            {
                Debug.LogWarning("[TwoVerdicts] 로그 파일 기록 실패: " + e.Message);
            }
        }
    }
}

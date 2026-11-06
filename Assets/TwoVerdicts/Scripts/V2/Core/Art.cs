using System.Collections.Generic;
using UnityEngine;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// Resources/V2 아래 스프라이트 로더 (캐시 포함).
    /// </summary>
    public static class Art
    {
        private const string Root = "V2/";
        private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        private static readonly HashSet<string> missing = new HashSet<string>();

        public static Sprite Load(string path)
        {
            if (cache.TryGetValue(path, out Sprite sprite)) return sprite;
            sprite = Resources.Load<Sprite>(Root + path);
            if (sprite == null && missing.Add(path)) Debug.LogWarning("[Art] 스프라이트 없음: " + Root + path);
            cache[path] = sprite;
            return sprite;
        }

        public static Sprite Room(string roomId, bool color) => Load($"rooms/{roomId}_{(color ? "color" : "gray")}");

        public static Sprite Char(string charId, int stage, int frame) => Load($"chars/{charId}_s{stage}_f{frame}");

        public static Sprite Portrait(string charId, int stage) => Load($"portraits/{charId}_s{stage}");

        public static Sprite Ui(string name) => Load("ui/" + name);
    }
}

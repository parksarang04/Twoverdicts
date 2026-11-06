using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 갈무리11(OFL) 로 한글 TMP 폰트 에셋을 만들고 TMP 기본 폰트로 지정.
    /// </summary>
    public static class FontSetup
    {
        private const string TtfPath = "Assets/TwoVerdicts/Fonts/Galmuri11.ttf";
        private const string FontAssetPath = "Assets/TwoVerdicts/Fonts/Galmuri11 SDF.asset";
        private const string SettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        private const string AsciiChars =
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        public static TMP_FontAsset Load()
        {
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        }

        public static TMP_FontAsset CreateOrLoad(IEnumerable<string> extraTexts)
        {
            TMP_FontAsset existing = Load();
            if (existing != null) return existing;

            Font source = AssetDatabase.LoadAssetAtPath<Font>(TtfPath);
            if (source == null) throw new InvalidOperationException("폰트 파일 없음: " + TtfPath);

            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(source, 44, 5, GlyphRenderMode.SDFAA,
                2048, 2048, AtlasPopulationMode.Dynamic, true);
            if (font == null) throw new InvalidOperationException("TMP 폰트 에셋 생성 실패");

            font.name = "Galmuri11 SDF";
            AssetDatabase.CreateAsset(font, FontAssetPath);
            font.TryAddCharacters(BuildCharset(extraTexts), out string missing);
            SaveSubAssets(font);
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
            SetupLog.Info($"한글 폰트 에셋 생성 완료 (빠진 글자 {missing?.Length ?? 0}개)");
            SetAsDefault(font);
            return font;
        }

        private static string BuildCharset(IEnumerable<string> texts)
        {
            var unique = new HashSet<char>(AsciiChars);
            foreach (string text in texts)
            {
                if (text == null) continue;
                foreach (char c in text) unique.Add(c);
            }
            var sb = new StringBuilder();
            foreach (char c in unique)
            {
                if (!char.IsControl(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        private static void SaveSubAssets(TMP_FontAsset font)
        {
            for (int i = 0; i < font.atlasTextures.Length; i++)
            {
                Texture2D atlas = font.atlasTextures[i];
                if (atlas == null || AssetDatabase.Contains(atlas)) continue;
                atlas.name = $"{font.name} Atlas {i}";
                AssetDatabase.AddObjectToAsset(atlas, font);
            }
            if (font.material != null && !AssetDatabase.Contains(font.material))
            {
                font.material.name = font.name + " Material";
                AssetDatabase.AddObjectToAsset(font.material, font);
            }
        }

        private static void SetAsDefault(TMP_FontAsset font)
        {
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(SettingsPath);
            if (settings == null)
            {
                SetupLog.Error("TMP Settings 를 찾지 못해 기본 폰트 지정은 건너뜀");
                return;
            }
            var so = new SerializedObject(settings);
            SerializedProperty prop = so.FindProperty("m_defaultFontAsset");
            if (prop == null)
            {
                SetupLog.Error("TMP Settings 에 m_defaultFontAsset 필드 없음, 기본 폰트 지정 건너뜀");
                return;
            }
            prop.objectReferenceValue = font;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
        }
    }
}

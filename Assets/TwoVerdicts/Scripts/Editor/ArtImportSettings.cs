using UnityEditor;
using UnityEngine;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// TwoVerdicts 의 PNG 를 픽셀아트용 스프라이트로 임포트.
    /// v1: Assets/TwoVerdicts/Art, v2: Assets/TwoVerdicts/V2/Resources/V2 (16 PPU, 인물은 발밑 기준).
    /// 새로 들어오는 파일은 자동, 이미 들어온 파일은 ApplyAll 로 다시 적용.
    /// </summary>
    public class ArtImportSettings : AssetPostprocessor
    {
        public const string ArtFolder = "Assets/TwoVerdicts/Art";
        public const string V2Folder = "Assets/TwoVerdicts/V2/Resources/V2";
        private const float V2PixelsPerUnit = 16f;

        private void OnPreprocessTexture()
        {
            if (!IsOurs(assetPath)) return;
            Configure((TextureImporter)assetImporter, assetPath);
        }

        private static bool IsOurs(string path)
        {
            return path.StartsWith(ArtFolder) || path.StartsWith(V2Folder);
        }

        public static void Configure(TextureImporter importer, string path)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.spriteBorder = BorderFor(path);
            if (path.StartsWith(V2Folder)) ConfigureV2(importer, path);
        }

        private static void ConfigureV2(TextureImporter importer, string path)
        {
            // UI 스프라이트는 캔버스 기준(100)이어야 9-slice 테두리가 1:1 로 나옴
            importer.spritePixelsPerUnit = path.Contains("/ui/") ? 100f : V2PixelsPerUnit;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            bool isCharacter = path.Contains("/chars/");
            settings.spriteAlignment = (int)(isCharacter ? SpriteAlignment.BottomCenter : SpriteAlignment.Center);
            importer.SetTextureSettings(settings);
        }

        private static Vector4 BorderFor(string path)
        {
            if (path.Contains("ui_dialogue_frame") || path.EndsWith("/ui/frame.png")) return new Vector4(12, 12, 12, 12);
            if (path.Contains("ui_button") || path.EndsWith("/ui/button.png")) return new Vector4(8, 8, 8, 8);
            return Vector4.zero;
        }

        public static void ApplyAll()
        {
            ApplyFolder(ArtFolder);
            ApplyFolder(V2Folder);
        }

        private static void ApplyFolder(string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder)) return;
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) continue;
                Configure(importer, path);
                importer.SaveAndReimport();
            }
            SetupLog.Info($"아트 임포트 설정 적용 ({folder}): {guids.Length}개");
        }
    }
}

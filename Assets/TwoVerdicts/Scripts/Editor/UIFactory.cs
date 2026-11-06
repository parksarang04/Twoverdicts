using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 씬 빌더용 uGUI 생성 도우미. 좌표는 1280x720 화면 기준, 왼쪽 위가 (0,0).
    /// </summary>
    public static class UIFactory
    {
        public const float ScreenW = 1280f;
        public const float ScreenH = 720f;
        private const int UILayer = 5;

        public static readonly Color Ink = new Color32(12, 12, 14, 255);
        public static readonly Color Paper = new Color32(236, 234, 226, 255);
        public static readonly Color Red = new Color32(204, 32, 36, 255);
        public static readonly Color Gray = new Color32(150, 150, 150, 255);
        public static readonly Color Clear = new Color(0f, 0f, 0f, 0f);

        public static TMP_FontAsset Font { get; set; }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform)) { layer = UILayer };
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static RectTransform Stretch(string name, Transform parent)
        {
            RectTransform rt = Rect(name, parent);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static RectTransform Place(string name, Transform parent, Vector2 center, Vector2 size)
        {
            RectTransform rt = Rect(name, parent);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = new Vector2(center.x - ScreenW / 2f, ScreenH / 2f - center.y);
            return rt;
        }

        public static Image AddImage(RectTransform rt, Sprite sprite, Color color, bool sliced = false)
        {
            var image = rt.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = sliced && sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            return image;
        }

        public static TextMeshProUGUI AddText(RectTransform rt, string text, float size, Color color,
            TextAlignmentOptions alignment)
        {
            var label = rt.gameObject.AddComponent<TextMeshProUGUI>();
            if (Font != null) label.font = Font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.raycastTarget = false;
            return label;
        }

        public static TextMeshProUGUI Label(Transform parent, string name, string text, Vector2 center,
            Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            return AddText(Place(name, parent, center, size), text, fontSize, color, alignment);
        }

        public static Button Button(Transform parent, string name, string label, Vector2 center, Vector2 size,
            Color fill)
        {
            RectTransform rt = Place(name, parent, center, size);
            Image image = AddImage(rt, LoadSprite("UI/ui_button"), fill, true);
            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            RectTransform textRt = Stretch("Label", rt);
            AddText(textRt, label, 22, Paper, TextAlignmentOptions.Center);
            return button;
        }

        public static Button FullScreenButton(Transform parent, string name)
        {
            RectTransform rt = Stretch(name, parent);
            Image image = AddImage(rt, null, Clear);
            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            return button;
        }

        public static Image Panel(Transform parent, string name, Vector2 center, Vector2 size, float alpha = 0.9f)
        {
            RectTransform rt = Place(name, parent, center, size);
            // 프레임 스프라이트 자체가 검은 바탕 + 흰 테두리라서 흰색으로 두고 알파만 조절
            Color fill = new Color(1f, 1f, 1f, alpha);
            return AddImage(rt, LoadSprite("UI/ui_dialogue_frame"), fill, true);
        }

        public static Sprite LoadSprite(string relativePath)
        {
            string path = $"{ArtImportSettings.ArtFolder}/{relativePath}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) SetupLog.Error("스프라이트 없음: " + path);
            return sprite;
        }

        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) throw new InvalidOperationException("에셋 없음: " + path);
            return asset;
        }
    }
}

using System;
using TMPro;
using TwoVerdicts.V2.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 런타임 uGUI 생성 도우미. 좌표는 1280x720 기준, 왼쪽 위 (0,0).
    /// </summary>
    public static class Ui
    {
        public const float W = 1280f;
        public const float H = 720f;

        public static readonly Color Ink = new Color32(12, 12, 14, 255);
        public static readonly Color Paper = new Color32(236, 234, 226, 255);
        public static readonly Color Red = new Color32(214, 40, 44, 255);
        public static readonly Color Gray = new Color32(150, 150, 150, 255);
        public static readonly Color Dim = new Color32(90, 90, 92, 255);
        public static readonly Color Gold = new Color32(232, 196, 92, 255);

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform)) { layer = 5 };
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

        public static RectTransform Place(string name, Transform parent, float cx, float cy, float w, float h)
        {
            RectTransform rt = Rect(name, parent);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(w, h);
            rt.anchoredPosition = new Vector2(cx - W / 2f, H / 2f - cy);
            return rt;
        }

        public static Image Image(RectTransform rt, Sprite sprite, Color color, bool raycast = false)
        {
            var image = rt.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = raycast;
            if (sprite != null && sprite.border != Vector4.zero) image.type = UnityEngine.UI.Image.Type.Sliced;
            return image;
        }

        public static Image Box(Transform parent, string name, float cx, float cy, float w, float h, float alpha = 0.92f)
        {
            RectTransform rt = Place(name, parent, cx, cy, w, h);
            return Image(rt, Art.Ui("frame"), new Color(1f, 1f, 1f, alpha), true);
        }

        public static TextMeshProUGUI Text(RectTransform rt, string text, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var label = rt.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = align;
            label.raycastTarget = false;
            return label;
        }

        public static TextMeshProUGUI Label(Transform parent, string name, string text, float cx, float cy,
            float w, float h, float size, Color color, TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            return Text(Place(name, parent, cx, cy, w, h), text, size, color, align);
        }

        /// <summary>모든 버튼 클릭 때 호출 (클릭음).</summary>
        public static Action OnAnyClick { get; set; }

        public static Button Button(Transform parent, string label, float cx, float cy, float w, float h,
            Action onClick, float fontSize = 20f)
        {
            RectTransform rt = Place("Btn_" + label, parent, cx, cy, w, h);
            Image bg = Image(rt, Art.Ui("button"), Color.white, true);
            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.75f, 0.75f, 1f);
            colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.colors = colors;
            Text(Stretch("Label", rt), label, fontSize, Paper, TextAlignmentOptions.Center);
            button.onClick.AddListener(() => SafeInvoke(OnAnyClick));
            if (onClick != null) button.onClick.AddListener(() => SafeInvoke(onClick));
            return button;
        }

        public static Button FullScreenButton(Transform parent, Action onClick)
        {
            RectTransform rt = Stretch("ClickCatcher", parent);
            Image bg = Image(rt, null, new Color(0f, 0f, 0f, 0f), true);
            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => SafeInvoke(onClick));
            return button;
        }

        public static Slider Slider(Transform parent, float cx, float cy, float w, float value, Action<float> onChange)
        {
            RectTransform rt = Place("Slider", parent, cx, cy, w, 20f);
            Image(rt, null, new Color(0.25f, 0.25f, 0.28f, 1f), true);
            RectTransform fill = Stretch("Fill", Stretch("Fill Area", rt));
            Image(fill, null, Red);
            RectTransform area = Stretch("Handle Area", rt);
            RectTransform handle = Rect("Handle", area);
            handle.sizeDelta = new Vector2(18f, 34f);
            Image knob = Image(handle, null, Paper, true);
            var slider = rt.gameObject.AddComponent<Slider>();
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = knob;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = value;
            slider.onValueChanged.AddListener(v => SafeInvoke(() => onChange?.Invoke(v)));
            return slider;
        }

        public static void SetLabel(Button button, string text)
        {
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = text;
        }

        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        public static void SafeInvoke(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

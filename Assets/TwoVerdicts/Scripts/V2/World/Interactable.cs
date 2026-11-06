using System;
using UnityEngine;

namespace TwoVerdicts.V2.World
{
    public enum InteractKind { Spot, Npc, Door }

    /// <summary>
    /// 방 안에서 E 로 쓸 수 있는 것.
    /// 단서 지점: 돋보기(Shift)를 누르면 반짝임. 쉬움 모드에서는 항상 반짝임.
    /// 인물: 지금 깰 수 있는 진술이 있으면 머리 위에 "!" (쉬움 모드).
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public const float UseRange = 1.3f;
        public const float GlintRange = 4.5f;

        public InteractKind Kind { get; private set; }
        public string Key { get; private set; }
        public string Label { get; private set; }
        public float X => transform.position.x;
        public bool Spent { get; set; }

        /// <summary>인물용: 지금 진행할 게 있는가.</summary>
        public Func<bool> Hot { get; set; }

        private SpriteRenderer glint;

        public void Setup(InteractKind kind, string key, string label, SpriteRenderer glintRenderer)
        {
            Kind = kind;
            Key = key;
            Label = label;
            glint = glintRenderer;
            if (glint != null) glint.enabled = false;
        }

        public void ShowGlint(bool show)
        {
            if (glint == null) return;
            bool on = Kind == InteractKind.Npc ? SafeHot() : show && !Spent;
            glint.enabled = on;
            if (!on) return;
            float a = 0.55f + 0.45f * Mathf.Sin(Time.time * 7f);
            glint.color = new Color(1f, 1f, 1f, a);
        }

        private bool SafeHot()
        {
            try
            {
                return Hot != null && Hot();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Hot = null;
                return false;
            }
        }
    }
}

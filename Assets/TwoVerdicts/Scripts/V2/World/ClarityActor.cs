using System;
using TwoVerdicts.V2.Core;
using UnityEngine;

namespace TwoVerdicts.V2.World
{
    /// <summary>
    /// 선명도에 따라 실루엣 -> 모자이크 흑백 -> 흑백 -> 컬러 스프라이트로 바뀌는 인물.
    /// 프레임 0/1 = 숨쉬기, 2/3 = 걷기 (플레이어만).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ClarityActor : MonoBehaviour
    {
        private const float IdleFrameTime = 0.55f;
        private const float WalkFrameTime = 0.14f;

        private SpriteRenderer spriteRenderer;
        private Func<int> clarity;
        private string charId;
        private float timer;
        private int frame;

        public bool Walking { get; set; }

        public void Setup(string id, Func<int> clarityGetter, int sortingOrder)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = sortingOrder;
            charId = id;
            clarity = clarityGetter;
            Refresh();
        }

        public void SetCharacter(string id)
        {
            charId = id;
            Refresh();
        }

        public void Face(bool right)
        {
            if (spriteRenderer != null) spriteRenderer.flipX = !right;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float limit = Walking ? WalkFrameTime : IdleFrameTime;
            if (timer < limit) return;
            timer = 0f;
            frame = Walking ? (frame == 2 ? 3 : 2) : (frame == 0 ? 1 : 0);
            Refresh();
        }

        public void Refresh()
        {
            if (spriteRenderer == null || clarity == null) return;
            int stage = Clarity.StageOf(clarity());
            Sprite sprite = Art.Char(charId, stage, frame);
            if (sprite == null) sprite = Art.Char(charId, stage, 0);
            spriteRenderer.sprite = sprite;
        }
    }
}

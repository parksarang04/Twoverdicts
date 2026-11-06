using System;
using System.Collections.Generic;
using TwoVerdicts.V2.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoVerdicts.V2.World
{
    /// <summary>
    /// 횡스크롤 이동(A/D, 방향키) + 가장 가까운 상호작용 찾기 + E 로 사용.
    /// Shift(돋보기): 느리게 걷는 대신 근처 단서가 반짝임.
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        private const float Speed = 4.2f;
        private const float MinX = -9.3f;
        private const float MaxX = 9.3f;

        private ClarityActor actor;
        private Func<IReadOnlyList<Interactable>> itemsProvider;
        private Interactable focused;

        public event Action<Interactable> OnFocusChanged;
        public event Action<Interactable> OnUse;
        public event Action OnStep;

        private const float StepDistance = 0.9f;
        private float stepAccum;

        public void Setup(ClarityActor body, Func<IReadOnlyList<Interactable>> items)
        {
            actor = body;
            itemsProvider = items;
        }

        public void Place(float x)
        {
            Vector3 p = transform.position;
            transform.position = new Vector3(Mathf.Clamp(x, MinX, MaxX), p.y, p.z);
            actor.Face(true);
        }

        private void Update()
        {
            bool free = InputLock.IsFree;
            bool magnify = free && (InputLock.Held(Key.LeftShift) || InputLock.Held(Key.RightShift));
            float dir = free ? ReadDirection() : 0f;
            Move(dir, magnify);
            UpdateFocus(magnify);
            if (free && focused != null && InputLock.Pressed(Key.E)) OnUse?.Invoke(focused);
        }

        private static float ReadDirection()
        {
            float dir = 0f;
            if (InputLock.Held(Key.A) || InputLock.Held(Key.LeftArrow)) dir -= 1f;
            if (InputLock.Held(Key.D) || InputLock.Held(Key.RightArrow)) dir += 1f;
            return dir;
        }

        private void Move(float dir, bool magnify)
        {
            actor.Walking = Mathf.Abs(dir) > 0.01f;
            if (!actor.Walking) return;
            float speed = magnify ? Speed * 0.45f : Speed;
            Vector3 p = transform.position;
            float before = p.x;
            p.x = Mathf.Clamp(p.x + dir * speed * Time.deltaTime, MinX, MaxX);
            transform.position = p;
            stepAccum += Mathf.Abs(p.x - before);
            if (stepAccum >= StepDistance)
            {
                stepAccum = 0f;
                OnStep?.Invoke();
            }
            actor.Face(dir > 0f);
        }

        private void UpdateFocus(bool magnify)
        {
            Interactable best = null;
            float bestDist = Interactable.UseRange;
            IReadOnlyList<Interactable> items = itemsProvider?.Invoke();
            if (items != null)
            {
                foreach (Interactable item in items)
                {
                    float dist = Mathf.Abs(item.X - transform.position.x);
                    item.ShowGlint(GameSettings.EasyMode || (magnify && dist < Interactable.GlintRange));
                    if (dist < bestDist)
                    {
                        best = item;
                        bestDist = dist;
                    }
                }
            }
            if (best == focused) return;
            focused = best;
            OnFocusChanged?.Invoke(focused);
        }
    }
}

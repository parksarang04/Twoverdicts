using System;
using System.Collections.Generic;
using TwoVerdicts.Core;
using TwoVerdicts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.Investigation
{
    /// <summary>
    /// 조사 단계 진행. 단서 클릭 -> 표면 / 다시 클릭 -> 내막.
    /// 횟수를 다 쓰거나 "법정으로" 버튼을 누르면 종료.
    /// </summary>
    public class InvestigationController : MonoBehaviour
    {
        private const string DeepHint = "(더 파고들 수 있을 것 같다.)";

        [SerializeField] private DialogueUI dialogue;
        [SerializeField] private LocationNavigator navigator;
        [SerializeField] private List<EvidenceHotspot> hotspots = new List<EvidenceHotspot>();
        [SerializeField] private Button goToCourtButton;

        private GameState state;
        private bool isRunning;

        public event Action OnInvestigationFinished;

        public void Begin(GameState gameState)
        {
            if (gameState == null || dialogue == null || navigator == null || goToCourtButton == null)
            {
                Debug.LogError("[InvestigationController] 상태 또는 참조 누락", this);
                return;
            }
            state = gameState;
            isRunning = true;
            SubscribeHotspots(true);
            goToCourtButton.onClick.AddListener(Finish);
            RefreshAll();
            navigator.ShowFirst();
        }

        private void SubscribeHotspots(bool subscribe)
        {
            foreach (EvidenceHotspot hotspot in hotspots)
            {
                if (hotspot == null) continue;
                if (subscribe) hotspot.OnClicked += HandleHotspot;
                else hotspot.OnClicked -= HandleHotspot;
            }
        }

        private void RefreshAll()
        {
            foreach (EvidenceHotspot hotspot in hotspots)
            {
                if (hotspot != null) hotspot.Refresh(state.GetLevel(hotspot.Evidence));
            }
        }

        private void HandleHotspot(EvidenceHotspot hotspot)
        {
            if (!isRunning || dialogue.IsOpen) return;
            try
            {
                if (!state.CanInspect(hotspot.Evidence)) return;
                InspectLevel level = state.Inspect(hotspot.Evidence);
                hotspot.Refresh(level);
                dialogue.Show(BuildLines(hotspot, level), CheckActionsLeft);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private List<string> BuildLines(EvidenceHotspot hotspot, InspectLevel level)
        {
            var result = new List<string> { $"[{hotspot.Evidence.Title}]" };
            if (level == InspectLevel.Deep)
            {
                result.Add(hotspot.Evidence.DeepText);
                return result;
            }
            result.Add(hotspot.Evidence.SurfaceText);
            if (hotspot.Evidence.HasDeepLayer) result.Add(DeepHint);
            return result;
        }

        private void CheckActionsLeft()
        {
            if (state != null && state.ActionsLeft <= 0) Finish();
        }

        private void Finish()
        {
            if (!isRunning || dialogue.IsOpen) return;
            isRunning = false;
            SubscribeHotspots(false);
            goToCourtButton.onClick.RemoveListener(Finish);
            OnInvestigationFinished?.Invoke();
        }

        private void OnDestroy()
        {
            SubscribeHotspots(false);
            if (goToCourtButton != null) goToCourtButton.onClick.RemoveListener(Finish);
        }
    }
}

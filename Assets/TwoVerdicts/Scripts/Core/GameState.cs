using System;
using System.Collections.Generic;
using TwoVerdicts.Data;
using UnityEngine;

namespace TwoVerdicts.Core
{
    /// <summary>
    /// 한 판의 진행 상태. MonoBehaviour가 아니라서 테스트/재시작이 쉬움.
    /// </summary>
    public class GameState
    {
        private const int SurfaceCrowdFactor = 5;
        private const int TaintCrowdPenalty = 8;
        private const int MinCrowdGuilt = 10;
        private const int MaxCrowdGuilt = 100;

        private readonly Dictionary<string, InspectLevel> inspectLevels = new Dictionary<string, InspectLevel>();
        private readonly int deepCapableCount;

        public int ActionsLeft { get; private set; }
        public int CrowdGuilt { get; private set; }
        public VerdictChoice CriminalVerdict { get; private set; } = VerdictChoice.None;
        public VerdictChoice CivilVerdict { get; private set; } = VerdictChoice.None;

        public event Action<int> OnActionsChanged;
        public event Action<int> OnCrowdChanged;

        public GameState(CaseData caseData)
        {
            if (caseData == null) throw new ArgumentNullException(nameof(caseData));
            ActionsLeft = caseData.MaxActions;
            CrowdGuilt = caseData.StartCrowdGuilt;
            deepCapableCount = Mathf.Max(1, caseData.DeepCapableCount());
        }

        public InspectLevel GetLevel(EvidenceData evidence)
        {
            if (evidence == null) return InspectLevel.None;
            return inspectLevels.TryGetValue(evidence.EvidenceId, out InspectLevel level)
                ? level
                : InspectLevel.None;
        }

        public bool CanInspect(EvidenceData evidence)
        {
            if (evidence == null || ActionsLeft <= 0) return false;
            InspectLevel level = GetLevel(evidence);
            if (level == InspectLevel.None) return true;
            return level == InspectLevel.Surface && evidence.HasDeepLayer;
        }

        public InspectLevel Inspect(EvidenceData evidence)
        {
            if (!CanInspect(evidence)) return GetLevel(evidence);
            InspectLevel next = GetLevel(evidence) == InspectLevel.None
                ? InspectLevel.Surface
                : InspectLevel.Deep;
            inspectLevels[evidence.EvidenceId] = next;
            ActionsLeft--;
            ApplyCrowdEffect(evidence, next);
            OnActionsChanged?.Invoke(ActionsLeft);
            return next;
        }

        private void ApplyCrowdEffect(EvidenceData evidence, InspectLevel level)
        {
            int delta = level == InspectLevel.Surface
                ? evidence.GuiltWeight * SurfaceCrowdFactor
                : (evidence.IsTainted ? -TaintCrowdPenalty : 0);
            CrowdGuilt = Mathf.Clamp(CrowdGuilt + delta, MinCrowdGuilt, MaxCrowdGuilt);
            OnCrowdChanged?.Invoke(CrowdGuilt);
        }

        public void SetVerdict(TrialType trial, VerdictChoice choice)
        {
            if (choice == VerdictChoice.None)
            {
                Debug.LogWarning("[GameState] VerdictChoice.None 은 판결로 쓸 수 없음");
                return;
            }
            if (trial == TrialType.Criminal) CriminalVerdict = choice;
            else CivilVerdict = choice;
        }

        public int CountByLevel(InspectLevel target)
        {
            int count = 0;
            foreach (InspectLevel level in inspectLevels.Values)
            {
                if (level == target) count++;
            }
            return count;
        }

        public float DeepRatio()
        {
            return (float)CountByLevel(InspectLevel.Deep) / deepCapableCount;
        }

        public int DeepCapableCount => deepCapableCount;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace TwoVerdicts.Data
{
    [CreateAssetMenu(fileName = "Case_", menuName = "TwoVerdicts/Case")]
    public class CaseData : ScriptableObject
    {
        [SerializeField] private string caseTitle;
        [SerializeField, TextArea(2, 4)] private List<string> introLines = new List<string>();
        [SerializeField, TextArea(2, 4)] private List<string> betweenTrialsLines = new List<string>();
        [SerializeField] private List<EvidenceData> allEvidence = new List<EvidenceData>();
        [SerializeField, Min(1)] private int maxActions = 8;
        [SerializeField, Range(0, 100)] private int startCrowdGuilt = 50;
        [SerializeField, TextArea(2, 4)] private string criminalStandardText;
        [SerializeField, TextArea(2, 4)] private string civilStandardText;

        public string CaseTitle => caseTitle;
        public IReadOnlyList<string> IntroLines => introLines;
        public IReadOnlyList<string> BetweenTrialsLines => betweenTrialsLines;
        public IReadOnlyList<EvidenceData> AllEvidence => allEvidence;
        public int MaxActions => maxActions;
        public int StartCrowdGuilt => startCrowdGuilt;
        public string CriminalStandardText => criminalStandardText;
        public string CivilStandardText => civilStandardText;

        public int DeepCapableCount()
        {
            int count = 0;
            foreach (EvidenceData evidence in allEvidence)
            {
                if (evidence != null && evidence.HasDeepLayer) count++;
            }
            return count;
        }
    }
}

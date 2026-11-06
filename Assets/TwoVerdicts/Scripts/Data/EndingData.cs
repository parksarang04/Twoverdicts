using UnityEngine;

namespace TwoVerdicts.Data
{
    public enum EndingType { SnapJudgment, TwoVerdicts, Contradiction, CertainGuilt, CertainDoubt }

    [CreateAssetMenu(fileName = "Ending_", menuName = "TwoVerdicts/Ending")]
    public class EndingData : ScriptableObject
    {
        [SerializeField] private EndingType endingType;
        [SerializeField] private string title;
        [SerializeField, TextArea(3, 8)] private string body;
        [SerializeField, TextArea(2, 4)] private string finalQuestion;

        public EndingType EndingType => endingType;
        public string Title => title;
        public string Body => body;
        public string FinalQuestion => finalQuestion;
    }
}

using UnityEngine;

namespace TwoVerdicts.Data
{
    public enum EvidenceCategory { Physical, Testimony, Record }

    [CreateAssetMenu(fileName = "Evidence_", menuName = "TwoVerdicts/Evidence")]
    public class EvidenceData : ScriptableObject
    {
        [SerializeField] private string evidenceId;
        [SerializeField] private string title;
        [SerializeField, TextArea(2, 6)] private string surfaceText;
        [SerializeField, TextArea(2, 6)] private string deepText;
        [SerializeField] private EvidenceCategory category;
        [Tooltip("+ 는 유죄 쪽, - 는 무죄 쪽으로 여론을 움직임")]
        [SerializeField, Range(-3, 3)] private int guiltWeight;
        [Tooltip("내막을 조사하면 증거의 신뢰도가 흔들리는가")]
        [SerializeField] private bool isTainted;
        [SerializeField] private Sprite icon;

        public string EvidenceId => evidenceId;
        public string Title => title;
        public string SurfaceText => surfaceText;
        public string DeepText => deepText;
        public EvidenceCategory Category => category;
        public int GuiltWeight => guiltWeight;
        public bool IsTainted => isTainted;
        public Sprite Icon => icon;
        public bool HasDeepLayer => !string.IsNullOrWhiteSpace(deepText);

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(evidenceId))
            {
                Debug.LogWarning($"[EvidenceData] {name}: evidenceId가 비어 있음", this);
            }
        }
    }
}

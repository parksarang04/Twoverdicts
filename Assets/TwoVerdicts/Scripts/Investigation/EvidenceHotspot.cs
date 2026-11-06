using System;
using TwoVerdicts.Core;
using TwoVerdicts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.Investigation
{
    /// <summary>
    /// 배경 위 클릭 가능한 단서 지점. 빨강 = 아직 안 본 것, 연빨강 = 내막 남음, 회색 = 끝.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class EvidenceHotspot : MonoBehaviour
    {
        [SerializeField] private EvidenceData evidence;
        [SerializeField] private Image markerImage;
        [SerializeField] private Color unseenColor = new Color(0.85f, 0.1f, 0.1f, 1f);
        [SerializeField] private Color surfaceColor = new Color(1f, 0.55f, 0.55f, 1f);
        [SerializeField] private Color doneColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private Button button;

        public EvidenceData Evidence => evidence;
        public event Action<EvidenceHotspot> OnClicked;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (evidence == null)
            {
                Debug.LogError($"[EvidenceHotspot] {name}: evidence 미할당", this);
                button.interactable = false;
                return;
            }
            button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (button != null) button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnClicked?.Invoke(this);
        }

        public void Refresh(InspectLevel level)
        {
            // 패널이 꺼져 있어 Awake 전에 호출될 수 있으므로 직접 가져옴
            if (button == null) button = GetComponent<Button>();
            if (evidence == null || button == null) return;
            bool finished = level == InspectLevel.Deep
                || (level == InspectLevel.Surface && !evidence.HasDeepLayer);
            button.interactable = !finished;
            if (markerImage != null) markerImage.color = PickColor(level, finished);
        }

        private Color PickColor(InspectLevel level, bool finished)
        {
            if (finished) return doneColor;
            return level == InspectLevel.None ? unseenColor : surfaceColor;
        }
    }
}

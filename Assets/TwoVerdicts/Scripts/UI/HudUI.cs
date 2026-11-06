using TMPro;
using TwoVerdicts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.UI
{
    /// <summary>
    /// 남은 조사 횟수 + 군중 여론 게이지.
    /// </summary>
    public class HudUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text actionsText;
        [SerializeField] private TMP_Text crowdText;
        [SerializeField] private Slider crowdSlider;

        private GameState boundState;

        private void Awake()
        {
            if (root == null || actionsText == null || crowdText == null || crowdSlider == null)
            {
                Debug.LogError("[HudUI] 인스펙터 참조 누락", this);
                enabled = false;
                return;
            }
            crowdSlider.minValue = 0;
            crowdSlider.maxValue = 100;
            crowdSlider.interactable = false;
        }

        public void Bind(GameState state)
        {
            if (!enabled || state == null) return;
            Unbind();
            boundState = state;
            boundState.OnActionsChanged += UpdateActions;
            boundState.OnCrowdChanged += UpdateCrowd;
            UpdateActions(state.ActionsLeft);
            UpdateCrowd(state.CrowdGuilt);
        }

        public void SetVisible(bool visible)
        {
            if (root != null) root.SetActive(visible);
        }

        private void Unbind()
        {
            if (boundState == null) return;
            boundState.OnActionsChanged -= UpdateActions;
            boundState.OnCrowdChanged -= UpdateCrowd;
            boundState = null;
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void UpdateActions(int actionsLeft)
        {
            actionsText.text = $"남은 조사 {actionsLeft}";
        }

        private void UpdateCrowd(int crowdGuilt)
        {
            crowdSlider.value = crowdGuilt;
            crowdText.text = $"여론: 유죄 {crowdGuilt}%";
        }
    }
}

using System;
using TMPro;
using TwoVerdicts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.Trial
{
    /// <summary>
    /// 재판 화면. 판결 기준 + 플레이어가 알아낸 사실 요약을 보여주고 유죄/무죄를 고르게 함.
    /// </summary>
    public class TrialController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text standardText;
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private Button guiltyButton;
        [SerializeField] private Button notGuiltyButton;

        private TrialType currentTrial;
        private Action<TrialType, VerdictChoice> onVerdict;

        private void Awake()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[TrialController] 인스펙터 참조 누락", this);
                enabled = false;
                return;
            }
            guiltyButton.onClick.AddListener(ChooseGuilty);
            notGuiltyButton.onClick.AddListener(ChooseNotGuilty);
            root.SetActive(false);
        }

        private bool HasAllReferences()
        {
            return root != null && headerText != null && standardText != null
                && summaryText != null && guiltyButton != null && notGuiltyButton != null;
        }

        private void OnDestroy()
        {
            if (guiltyButton != null) guiltyButton.onClick.RemoveListener(ChooseGuilty);
            if (notGuiltyButton != null) notGuiltyButton.onClick.RemoveListener(ChooseNotGuilty);
        }

        public void Open(TrialType trial, string header, string standard, string summary,
            Action<TrialType, VerdictChoice> callback)
        {
            if (!enabled)
            {
                Debug.LogError("[TrialController] 비활성 상태에서 Open 호출됨", this);
                return;
            }
            currentTrial = trial;
            onVerdict = callback;
            headerText.text = header;
            standardText.text = standard;
            summaryText.text = summary;
            root.SetActive(true);
        }

        private void ChooseGuilty() => Choose(VerdictChoice.Guilty);

        private void ChooseNotGuilty() => Choose(VerdictChoice.NotGuilty);

        private void Choose(VerdictChoice choice)
        {
            root.SetActive(false);
            Action<TrialType, VerdictChoice> callback = onVerdict;
            onVerdict = null;
            try
            {
                callback?.Invoke(currentTrial, choice);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}

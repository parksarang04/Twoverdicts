using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.Investigation
{
    [Serializable]
    public class LocationEntry
    {
        public string displayName;
        public GameObject panel;
    }

    /// <summary>
    /// 장소 패널 전환. 장소 이동은 조사 횟수를 소모하지 않음.
    /// </summary>
    public class LocationNavigator : MonoBehaviour
    {
        [SerializeField] private List<LocationEntry> locations = new List<LocationEntry>();
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text locationNameText;

        private int currentIndex;

        private void Awake()
        {
            if (locations.Count == 0 || prevButton == null || nextButton == null || locationNameText == null)
            {
                Debug.LogError("[LocationNavigator] 장소 목록 또는 버튼 참조 누락", this);
                enabled = false;
                return;
            }
            prevButton.onClick.AddListener(GoPrev);
            nextButton.onClick.AddListener(GoNext);
        }

        private void OnDestroy()
        {
            if (prevButton != null) prevButton.onClick.RemoveListener(GoPrev);
            if (nextButton != null) nextButton.onClick.RemoveListener(GoNext);
        }

        public void ShowFirst()
        {
            if (enabled) Show(0);
        }

        private void GoPrev()
        {
            Show((currentIndex - 1 + locations.Count) % locations.Count);
        }

        private void GoNext()
        {
            Show((currentIndex + 1) % locations.Count);
        }

        private void Show(int index)
        {
            currentIndex = Mathf.Clamp(index, 0, locations.Count - 1);
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].panel == null)
                {
                    Debug.LogWarning($"[LocationNavigator] {i}번 장소 패널 미할당", this);
                    continue;
                }
                locations[i].panel.SetActive(i == currentIndex);
            }
            locationNameText.text = locations[currentIndex].displayName;
        }
    }
}

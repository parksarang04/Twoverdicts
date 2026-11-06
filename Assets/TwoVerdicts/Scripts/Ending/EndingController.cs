using System.Collections;
using TMPro;
using TwoVerdicts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoVerdicts.Ending
{
    /// <summary>
    /// 엔딩 화면. 본문 -> 잠시 후 마지막 질문이 페이드인.
    /// 이 스크립트는 항상 켜진 오브젝트에, root 는 자식 패널로 연결.
    /// </summary>
    public class EndingController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private CanvasGroup questionGroup;
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private Button restartButton;
        [SerializeField, Min(0f)] private float questionDelay = 2.5f;
        [SerializeField, Min(0.1f)] private float fadeDuration = 2f;

        private void Awake()
        {
            if (!HasAllReferences())
            {
                Debug.LogError("[EndingController] 인스펙터 참조 누락", this);
                enabled = false;
                return;
            }
            restartButton.onClick.AddListener(Restart);
            root.SetActive(false);
        }

        private bool HasAllReferences()
        {
            return root != null && titleText != null && bodyText != null && statsText != null
                && questionGroup != null && questionText != null && restartButton != null;
        }

        private void OnDestroy()
        {
            if (restartButton != null) restartButton.onClick.RemoveListener(Restart);
        }

        public void Show(EndingData ending, string stats)
        {
            if (!enabled || ending == null)
            {
                Debug.LogError("[EndingController] 엔딩 데이터 없음 또는 비활성", this);
                return;
            }
            titleText.text = ending.Title;
            bodyText.text = ending.Body;
            statsText.text = stats;
            questionText.text = ending.FinalQuestion;
            questionGroup.alpha = 0f;
            restartButton.gameObject.SetActive(false);
            root.SetActive(true);
            StartCoroutine(RevealQuestion());
        }

        private IEnumerator RevealQuestion()
        {
            yield return new WaitForSecondsRealtime(questionDelay);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                questionGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            questionGroup.alpha = 1f;
            restartButton.gameObject.SetActive(true);
        }

        private void Restart()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index < 0)
            {
                Debug.LogError("[EndingController] 씬이 Build Profiles 씬 목록에 없음", this);
                return;
            }
            SceneManager.LoadScene(index);
        }
    }
}

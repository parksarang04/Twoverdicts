using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 엔딩 화면. 제목 -> 잠시 뒤 마지막 질문이 천천히 떠오름 -> 다시 하기.
    /// </summary>
    public class EndingView : View
    {
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI questionText;
        private TextMeshProUGUI quoteText;
        private CanvasGroup questionGroup;
        private GameObject restart;

        public static EndingView Create(Transform parent) => CreateView<EndingView>(parent, "Ending");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Bg", transform), null, Ui.Ink, true);
            Ui.Label(transform, "Tag", "ENDING", 640f, 150f, 600f, 30f, 18f, Ui.Dim, TextAlignmentOptions.Center);
            titleText = Ui.Label(transform, "Title", "", 640f, 210f, 1000f, 70f, 48f, Ui.Red, TextAlignmentOptions.Center);
            RectTransform group = Ui.Place("QuestionGroup", transform, 640f, 380f, 1100f, 160f);
            questionGroup = group.gameObject.AddComponent<CanvasGroup>();
            questionText = Ui.Text(Ui.Stretch("Question", group), "", 30f, Ui.Paper, TextAlignmentOptions.Center);
            quoteText = Ui.Label(transform, "Quote", "", 640f, 540f, 1000f, 40f, 18f, Ui.Gray, TextAlignmentOptions.Center);
            restart = Ui.Button(transform, "처음부터 다시", 640f, 640f, 260f, 56f, Restart).gameObject;
        }

        public void Show(string title, string question, string quote)
        {
            titleText.text = title;
            questionText.text = question;
            quoteText.text = quote;
            questionGroup.alpha = 0f;
            restart.SetActive(false);
            OpenView();
            StartCoroutine(Reveal());
        }

        private IEnumerator Reveal()
        {
            yield return new WaitForSecondsRealtime(2f);
            for (float t = 0f; t < 2.5f; t += Time.unscaledDeltaTime)
            {
                questionGroup.alpha = t / 2.5f;
                yield return null;
            }
            questionGroup.alpha = 1f;
            restart.SetActive(true);
        }

        private void Restart()
        {
            Core.InputLock.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

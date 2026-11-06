using System.Collections;
using TMPro;
using UnityEngine;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 장소 이름, 남은 행동, 조사 안내([E] ...), 획득 알림.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        private TextMeshProUGUI roomText;
        private TextMeshProUGUI actionsText;
        private TextMeshProUGUI promptText;
        private TextMeshProUGUI toastText;
        private Coroutine toastRoutine;

        public static HudView Create(Transform parent)
        {
            RectTransform root = Ui.Stretch("Hud", parent);
            var hud = root.gameObject.AddComponent<HudView>();
            hud.Build();
            return hud;
        }

        private void Build()
        {
            Ui.Box(transform, "RoomBox", 220f, 36f, 400f, 52f, 0.85f).raycastTarget = false;
            roomText = Ui.Label(transform, "Room", "", 220f, 36f, 370f, 40f, 22f, Ui.Paper, TextAlignmentOptions.Center);
            Ui.Box(transform, "ActBox", 1130f, 36f, 260f, 52f, 0.85f).raycastTarget = false;
            actionsText = Ui.Label(transform, "Actions", "", 1130f, 36f, 240f, 40f, 22f, Ui.Red, TextAlignmentOptions.Center);
            promptText = Ui.Label(transform, "Prompt", "", 640f, 470f, 600f, 40f, 22f, Ui.Paper, TextAlignmentOptions.Center);
            toastText = Ui.Label(transform, "Toast", "", 640f, 110f, 900f, 40f, 22f, Ui.Gold, TextAlignmentOptions.Center);
            Ui.Label(transform, "Help", "A/D 이동   E 조사·대화   반짝이 = 단서   ! = 대화할 사람   Q 수첩   Tab 추리 보드   Esc 메뉴   ` 개발자", 640f, 700f,
                1000f, 30f, 16f, Ui.Dim, TextAlignmentOptions.Center);
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);

        public void SetRoom(string name) => roomText.text = name;

        public void SetActions(int actions) => actionsText.text = $"남은 행동 {actions}";

        public void SetPrompt(string label) => promptText.text = string.IsNullOrEmpty(label) ? "" : $"[E] {label}";

        public void Toast(string message)
        {
            if (toastRoutine != null) StopCoroutine(toastRoutine);
            if (gameObject.activeInHierarchy) toastRoutine = StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            toastText.text = message;
            toastText.alpha = 1f;
            yield return new WaitForSecondsRealtime(2.2f);
            for (float t = 0f; t < 0.8f; t += Time.unscaledDeltaTime)
            {
                toastText.alpha = 1f - t / 0.8f;
                yield return null;
            }
            toastText.text = "";
        }
    }
}

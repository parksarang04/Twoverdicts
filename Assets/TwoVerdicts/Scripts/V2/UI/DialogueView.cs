using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 타자기 대사창. 화자 없음 = 윤석의 속마음(회색), "???" = 붉은 목소리.
    /// </summary>
    public class DialogueView : View
    {
        private TextMeshProUGUI nameText;
        private TextMeshProUGUI bodyText;
        private Image portrait;
        private GameObject portraitBox;
        private readonly Queue<Line> queue = new Queue<Line>();
        private Action onDone;
        private Coroutine typing;
        private bool isTyping;

        /// <summary>화자 이름 -> 초상화 스프라이트 (없으면 null).</summary>
        public Func<string, Sprite> PortraitFor { get; set; }

        /// <summary>글자가 찍힐 때마다 (기계음 재생용).</summary>
        public Action<string> OnBlip { get; set; }

        public static DialogueView Create(Transform parent) => CreateView<DialogueView>(parent, "Dialogue");

        protected override void Build()
        {
            Ui.FullScreenButton(transform, Advance);
            Ui.Box(transform, "Box", 640f, 598f, 1220f, 210f, 0.96f);
            portraitBox = Ui.Box(transform, "PortraitBox", 120f, 598f, 170f, 170f, 1f).gameObject;
            portrait = Ui.Image(Ui.Place("Portrait", portraitBox.transform, 640f, 360f, 150f, 150f), null, Color.white);
            portrait.preserveAspect = true;
            nameText = Ui.Label(transform, "Name", "", 420f, 514f, 400f, 30f, 22f, Ui.Paper);
            bodyText = Ui.Label(transform, "Body", "", 735f, 608f, 1000f, 140f, 24f, Ui.Paper);
            Ui.Label(transform, "Hint", "E / 클릭 >", 1180f, 686f, 140f, 24f, 15f, Ui.Dim, TextAlignmentOptions.Right);
        }

        public void Show(IEnumerable<Line> lines, Action done = null)
        {
            queue.Clear();
            if (lines != null)
            {
                foreach (Line line in lines) queue.Enqueue(line);
            }
            onDone = done;
            OpenView();
            Next();
        }

        public void Show(Line line, Action done = null) => Show(new[] { line }, done);

        private void Update()
        {
            if (KeyReady && Core.InputLock.AdvancePressed()) Advance();
        }

        private void Advance()
        {
            if (!IsOpen) return;
            if (isTyping)
            {
                FinishTyping();
                return;
            }
            Next();
        }

        private void Next()
        {
            if (queue.Count == 0)
            {
                Finish();
                return;
            }
            Line line = queue.Dequeue();
            ApplySpeaker(line);
            if (typing != null) StopCoroutine(typing);
            typing = StartCoroutine(Type(line));
        }

        private void ApplySpeaker(Line line)
        {
            bool thought = line.Speaker == null;
            bool voice = line.Speaker == StoryDb.Voice;
            nameText.text = thought ? "" : line.Speaker;
            nameText.color = voice ? Ui.Red : Ui.Paper;
            bodyText.color = voice ? Ui.Red : (thought ? Ui.Gray : Ui.Paper);
            Sprite face = thought || voice ? null : PortraitFor?.Invoke(line.Speaker);
            portraitBox.SetActive(face != null);
            portrait.sprite = face;
        }

        private IEnumerator Type(Line line)
        {
            isTyping = true;
            bodyText.text = line.Text;
            bodyText.maxVisibleCharacters = 0;
            bodyText.ForceMeshUpdate();
            int total = bodyText.textInfo.characterCount;
            for (int i = 1; i <= total; i++)
            {
                bodyText.maxVisibleCharacters = i;
                if (i % 2 == 0) OnBlip?.Invoke(line.Speaker);
                yield return new WaitForSecondsRealtime(Core.GameSettings.CharInterval);
            }
            isTyping = false;
        }

        private void FinishTyping()
        {
            if (typing != null) StopCoroutine(typing);
            bodyText.maxVisibleCharacters = 99999;
            isTyping = false;
        }

        private void Finish()
        {
            CloseView();
            Action callback = onDone;
            onDone = null;
            Ui.SafeInvoke(callback);
        }
    }
}

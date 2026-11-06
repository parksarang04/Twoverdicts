using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.UI
{
    /// <summary>
    /// 타자기 효과 대사창. 이 스크립트는 항상 켜져 있는 오브젝트에 붙이고,
    /// root 에는 실제로 켜고 끌 자식 패널을 연결할 것 (꺼지면 코루틴이 멈춤).
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        private const int ShowAllCharacters = 99999;

        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private Button nextButton;
        [SerializeField, Min(0.005f)] private float charInterval = 0.03f;

        private readonly Queue<string> lines = new Queue<string>();
        private Coroutine typingRoutine;
        private bool isTyping;
        private Action onComplete;

        public bool IsOpen => root != null && root.activeSelf;

        private void Awake()
        {
            if (root == null || bodyText == null || nextButton == null)
            {
                Debug.LogError("[DialogueUI] 인스펙터 참조 누락", this);
                enabled = false;
                return;
            }
            nextButton.onClick.AddListener(HandleNext);
            root.SetActive(false);
        }

        private void OnDestroy()
        {
            if (nextButton != null) nextButton.onClick.RemoveListener(HandleNext);
        }

        public void Show(IEnumerable<string> newLines, Action completed = null)
        {
            if (!enabled || newLines == null)
            {
                completed?.Invoke();
                return;
            }
            lines.Clear();
            foreach (string line in newLines) lines.Enqueue(line);
            onComplete = completed;
            root.SetActive(true);
            ShowNextLine();
        }

        public void Show(string line, Action completed = null)
        {
            Show(new[] { line }, completed);
        }

        private void HandleNext()
        {
            if (isTyping)
            {
                FinishTyping();
                return;
            }
            ShowNextLine();
        }

        private void ShowNextLine()
        {
            if (lines.Count == 0)
            {
                Close();
                return;
            }
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(TypeLine(lines.Dequeue()));
        }

        private IEnumerator TypeLine(string line)
        {
            isTyping = true;
            bodyText.text = line;
            bodyText.maxVisibleCharacters = 0;
            bodyText.ForceMeshUpdate();
            int total = bodyText.textInfo.characterCount;
            for (int i = 1; i <= total; i++)
            {
                bodyText.maxVisibleCharacters = i;
                yield return new WaitForSecondsRealtime(charInterval);
            }
            isTyping = false;
        }

        private void FinishTyping()
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            bodyText.maxVisibleCharacters = ShowAllCharacters;
            isTyping = false;
        }

        private void Close()
        {
            root.SetActive(false);
            Action callback = onComplete;
            onComplete = null;
            try
            {
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}

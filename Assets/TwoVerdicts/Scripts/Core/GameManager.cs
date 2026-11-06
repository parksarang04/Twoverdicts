using System;
using System.Collections.Generic;
using System.Text;
using TwoVerdicts.Data;
using TwoVerdicts.Ending;
using TwoVerdicts.Intro;
using TwoVerdicts.Investigation;
using TwoVerdicts.Trial;
using TwoVerdicts.UI;
using UnityEngine;

namespace TwoVerdicts.Core
{
    /// <summary>
    /// 전체 흐름: 인트로 -> 조사 -> 형사재판 -> 민사재판 -> 엔딩.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private CaseData caseData;
        [SerializeField] private List<EndingData> endings = new List<EndingData>();

        [Header("Systems")]
        [Tooltip("비워두면 뉴스 인트로 영상 없이 바로 인트로 대사로 시작")]
        [SerializeField] private IntroVideoController introVideo;
        [SerializeField] private DialogueUI dialogue;
        [SerializeField] private HudUI hud;
        [SerializeField] private GameObject investigationRoot;
        [SerializeField] private InvestigationController investigation;
        [SerializeField] private TrialController trial;
        [SerializeField] private EndingController ending;

        private GameState state;

        public GamePhase Phase { get; private set; } = GamePhase.Intro;

        private void Start()
        {
            if (!ValidateReferences()) return;
            try
            {
                state = new GameState(caseData);
                hud.Bind(state);
                EnterIntro();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private bool ValidateReferences()
        {
            bool ok = caseData != null && dialogue != null && hud != null && investigationRoot != null
                && investigation != null && trial != null && ending != null && endings.Count > 0;
            if (!ok) Debug.LogError("[GameManager] 인스펙터 참조 누락", this);
            return ok;
        }

        private void EnterIntro()
        {
            Phase = GamePhase.Intro;
            investigationRoot.SetActive(false);
            hud.SetVisible(false);
            if (introVideo != null)
            {
                introVideo.Play(ShowIntroLines);
                return;
            }
            ShowIntroLines();
        }

        private void ShowIntroLines()
        {
            dialogue.Show(caseData.IntroLines, EnterInvestigation);
        }

        private void EnterInvestigation()
        {
            Phase = GamePhase.Investigation;
            investigationRoot.SetActive(true);
            hud.SetVisible(true);
            investigation.OnInvestigationFinished += HandleInvestigationFinished;
            investigation.Begin(state);
        }

        private void HandleInvestigationFinished()
        {
            investigation.OnInvestigationFinished -= HandleInvestigationFinished;
            investigationRoot.SetActive(false);
            dialogue.Show("조사는 여기까지다. 이제 법정으로.", EnterCriminalTrial);
        }

        private void EnterCriminalTrial()
        {
            Phase = GamePhase.CriminalTrial;
            trial.Open(TrialType.Criminal, "1부 - 형사재판", caseData.CriminalStandardText,
                BuildSummary(), HandleVerdict);
        }

        private void EnterCivilTrial()
        {
            Phase = GamePhase.CivilTrial;
            trial.Open(TrialType.Civil, "2부 - 민사재판", caseData.CivilStandardText,
                BuildSummary(), HandleVerdict);
        }

        private void HandleVerdict(TrialType trialType, VerdictChoice choice)
        {
            state.SetVerdict(trialType, choice);
            if (trialType == TrialType.Criminal)
            {
                dialogue.Show(caseData.BetweenTrialsLines, EnterCivilTrial);
                return;
            }
            EnterEnding();
        }

        private void EnterEnding()
        {
            Phase = GamePhase.Ending;
            hud.SetVisible(false);
            try
            {
                EndingType type = EndingResolver.Resolve(state);
                EndingData data = endings.Find(x => x != null && x.EndingType == type);
                if (data == null) throw new InvalidOperationException($"엔딩 데이터 없음: {type}");
                ending.Show(data, BuildStats());
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private string BuildSummary()
        {
            var sb = new StringBuilder("당신이 알고 있는 것\n");
            foreach (EvidenceData evidence in caseData.AllEvidence)
            {
                if (evidence == null) continue;
                InspectLevel level = state.GetLevel(evidence);
                if (level == InspectLevel.None) continue;
                string tag = level == InspectLevel.Deep ? "내막" : "표면";
                sb.AppendLine($"- {evidence.Title} ({tag})");
            }
            if (state.CountByLevel(InspectLevel.Surface) + state.CountByLevel(InspectLevel.Deep) == 0)
            {
                sb.AppendLine("- 아무것도 조사하지 않았다.");
            }
            return sb.ToString();
        }

        private string BuildStats()
        {
            string criminal = VerdictToText(state.CriminalVerdict);
            string civil = VerdictToText(state.CivilVerdict);
            int deep = state.CountByLevel(InspectLevel.Deep);
            return $"형사: {criminal}   민사: {civil}\n"
                + $"내막까지 본 증거 {deep} / {state.DeepCapableCount}\n"
                + $"여론: 유죄 {state.CrowdGuilt}%";
        }

        private static string VerdictToText(VerdictChoice choice)
        {
            switch (choice)
            {
                case VerdictChoice.Guilty: return "유죄";
                case VerdictChoice.NotGuilty: return "무죄";
                default: return "-";
            }
        }
    }
}

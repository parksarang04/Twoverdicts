using System;
using TwoVerdicts.Core;
using TwoVerdicts.Data;

namespace TwoVerdicts.Ending
{
    /// <summary>
    /// 판결 + 조사 깊이로 엔딩을 결정하는 순수 로직.
    /// </summary>
    public static class EndingResolver
    {
        // 내막까지 본 증거 비율이 이 값 미만이면 "단편" 엔딩
        public const float SnapJudgmentThreshold = 0.25f;

        public static EndingType Resolve(GameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (state.DeepRatio() < SnapJudgmentThreshold) return EndingType.SnapJudgment;

            bool criminalGuilty = state.CriminalVerdict == VerdictChoice.Guilty;
            bool civilGuilty = state.CivilVerdict == VerdictChoice.Guilty;

            if (criminalGuilty && !civilGuilty) return EndingType.Contradiction;
            if (!criminalGuilty && civilGuilty) return EndingType.TwoVerdicts;
            return criminalGuilty ? EndingType.CertainGuilt : EndingType.CertainDoubt;
        }
    }
}

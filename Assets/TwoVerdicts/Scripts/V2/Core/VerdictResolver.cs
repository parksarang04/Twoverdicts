using TwoVerdicts.V2.Data;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 최종 재판 선택 -> 엔딩. 순수 로직이라 테스트하기 쉬움.
    /// </summary>
    public static class VerdictResolver
    {
        public const string Killer = "hangyeol";
        public const string PlanterMe = "me";
        public const string PlanterNone = "none";

        public static EndingId Resolve(string killerId, string evidenceId, string planterId, bool refused)
        {
            if (refused) return EndingId.Silence;
            if (killerId != Killer) return EndingId.Fragment;
            if (evidenceId == null || !StoryDb.ValidEvidence.Contains(evidenceId)) return EndingId.Fragment;
            return planterId == PlanterMe ? EndingId.True : EndingId.Hidden;
        }

        public static string FailReason(string killerId, string evidenceId)
        {
            if (killerId != Killer) return "당신이 지목한 사람은 그날 밤 세라를 죽이지 않았다.";
            if (evidenceId == null || !StoryDb.ValidEvidence.Contains(evidenceId))
            {
                return "증거가 그를 가리키지 못했다. 서한결은 증거 불충분으로 풀려났다.";
            }
            return null;
        }
    }

    public static class Clarity
    {
        public const int Stages = 4;

        public static int StageOf(int value)
        {
            if (value < 25) return 0;
            if (value < 50) return 1;
            if (value < 80) return 2;
            return 3;
        }
    }
}

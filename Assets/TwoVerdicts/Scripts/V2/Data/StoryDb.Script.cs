using System.Collections.Generic;

namespace TwoVerdicts.V2.Data
{
    public enum EndingId { Fragment, Hidden, True, Silence }

    public static partial class StoryDb
    {
        public static readonly Line[] Prologue =
        {
            L(Voice, "그날 밤 이야기를 처음부터 해 주세요. 천천히요."),
            Think("1998년 11월 14일. 해운시에는 사흘째 비가 내리고 있었다."),
            Think("심야 라디오 '세라의 새벽'의 DJ 윤세라가 자기 집에서 숨진 채 발견됐다."),
            Think("체포된 건 아파트 야간 경비원 강무진. 절도 전과 1범. 피 묻은 소매로 달아나다 잡혔다."),
            Think("도시는 이미 판결을 내렸다. 사형."),
            L("오태식", "최 형사. 내일 아침 송치다. 오늘 안에 빈틈 없이 마무리해."),
            Think("\"우리는 동굴 벽에 비친 그림자를 실재라고 믿는다.\" - 플라톤"),
            L(Voice, "그래서... 어디부터 가셨죠?")
        };

        public static readonly string[] VoiceOnLeave =
        {
            "그다음엔요?", "거기서 뭘 보셨어요?", "그 사람 표정, 기억나세요?", "천천히요. 기억나는 대로요.", "그때 무슨 생각을 하셨어요?"
        };

        public static readonly Line[] SuaLetterLines =
        {
            L("강수아", "이거... 아빠한테 줄 편지예요. 아저씨가 대신 전해 줘요."),
            Think("\"아빠 수술비 걱정 하지 마. 나 안 아파도 돼. 아빠만 집에 와.\""),
            L("강수아", "약속해요. 나중에 꼭 다시 물어볼 거예요. 아빠가 진짜 나쁜 사람이었는지.")
        };

        public static readonly string[] SuaAnswers = { "그래. 아빠는 나쁜 짓을 했어.", "아직 모르겠어.", "아저씨가 꼭 알아볼게." };

        public static readonly string[] DealOptions = { "수아 이야기를 넘긴다", "거절하고 사진만 압수한다" };

        public static readonly Line[] RevealLines =
        {
            Think("(23:20, 편의점에서 소주 두 병.)"),
            Think("(23:30, 신고를 받고 제일 먼저 도착한 형사.)"),
            Think("(술 냄새. 창문을 연 사람.)"),
            Think("(23:50, 경비실. 코트 소매로 번진 서명.)"),
            Think("(그리고 찢겨 나간 수첩 한 장.)"),
            Think("코트 안주머니에서 구겨진 종이가 나왔다."),
            L(Me, "\"11/14 23:52 - 무진 놈 서랍에서 라이터. 이번엔 못 빠져나간다.\""),
            Think("내 글씨였다."),
            Think("\"너 자신을 알라.\" - 소크라테스")
        };

        public static readonly string[] PageOptions = { "이 페이지를 증거로 남긴다", "이 페이지를 태운다" };

        public static readonly Line[] BurnLines =
        {
            Think("라이터를 켰다. 종이가 오그라들며 탔다."),
            Think("거울을 보지 않아도 알 수 있었다. 나는 다시 흐려지고 있었다.")
        };

        public const string TrialIntro = "\"괴물과 싸우는 자는 스스로 괴물이 되지 않도록 조심하라.\" - 니체";
        public const string SlotKiller = "윤세라를 죽인 사람은";
        public const string SlotEvidence = "그것을 증명하는 증거는";
        public const string SlotPlanter = "무진의 라이터를 현장에 놓은 사람은";
        public const string NoOne = "아무도 놓지 않았다";
        public const string NoJudgement = "판단하지 않는다";

        public static readonly HashSet<string> ValidEvidence = new HashSet<string>
        {
            "d_not_live", "d_outside", "d_motive", "c_memo_270", "c_shoes", "c_stairs_man"
        };
    }
}

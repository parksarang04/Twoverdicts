using System.Collections.Generic;

namespace TwoVerdicts.V2.Data
{
    public class EndingDef
    {
        public string Title;
        public Line[] Verdict;
        public Line[] Frame;
        public string Question;
    }

    public static partial class StoryDb
    {
        public const string FrameCaption = "2018년 11월 14일. 해운지방법원, 재심 증인 대기실.";

        public static EndingDef GetEnding(EndingId id)
        {
            switch (id)
            {
                case EndingId.True: return TrueEnding;
                case EndingId.Hidden: return HiddenEnding;
                case EndingId.Silence: return SilenceEnding;
                default: return FragmentEnding;
            }
        }

        /// <summary>엔딩 공통 대사 뒤에 붙는, 중간 선택에 따른 수아의 한마디.</summary>
        public static List<Line> ChoiceEcho(int suaAnswer, bool soldStory)
        {
            var lines = new List<Line>();
            if (soldStory) lines.Add(L("강수아", "그때 그 기사, 기억하세요? '살인자의 딸'. 전 학교를 세 번 옮겼어요."));
            if (suaAnswer == 0) lines.Add(L("강수아", "그날 아저씨가 '아빠는 나쁜 짓을 했어'라고 했죠. 반만 맞는 말이었어요."));
            if (suaAnswer == 1) lines.Add(L("강수아", "그날 아저씨는 '모르겠다'고 했어요. 그 대답이 20년 동안 제일 오래 남았어요."));
            if (suaAnswer == 2) lines.Add(L("강수아", "그날 아저씨가 '꼭 알아볼게'라고 했어요. 그래서 제가 왔어요."));
            return lines;
        }

        private static readonly EndingDef TrueEnding = new EndingDef
        {
            Title = "두 개의 판결",
            Verdict = new[]
            {
                L("재판장", "피고인 강무진. 살인 혐의는 무죄. 주거침입과 절도미수는 유죄. 징역 1년, 집행유예 2년."),
                L("재판장", "서한결은 살인 혐의로 구속 기소한다."),
                L("재판장", "그리고 증인 최윤석. 증거 조작 혐의로 재판에 회부한다."),
                Think("판결이 두 번 내려졌다. 한 번은 그에게, 한 번은 나에게.")
            },
            Frame = new[]
            {
                L(Me, "...그게 그날 밤의 전부입니다."),
                L(Voice, "20년 걸렸네요."),
                L("강수아", "저예요, 아저씨. 강수아. 오늘 재심, 제가 아빠 변호인이에요."),
                L("강수아", "아빠는 밖에서 기다리고 계세요. 아저씨한테 고맙다고 전해 달래요."),
                L(Me, "...고마워할 일이 아니야. 그 라이터는 내가 놓았어."),
                L("강수아", "알아요. 그래서 오늘 판결은 두 개예요.")
            },
            Question = "인간이 정한 법의 테두리로 한 사람의 인생을 판단할 수 있는가?\n그 판단이 나를 향할 때에도."
        };

        private static readonly EndingDef HiddenEnding = new EndingDef
        {
            Title = "골라낸 진실",
            Verdict = new[]
            {
                L("재판장", "서한결, 살인 혐의 유죄."),
                L("재판장", "강무진. 현장에서 발견된 라이터로 보아 범행 은폐를 도운 공범으로 판단한다. 징역 7년."),
                Think("라이터는 끝까지 증거로 남았다. 아무도 그 출처를 묻지 않았다.")
            },
            Frame = new[]
            {
                L(Me, "...그게 그날 밤의 전부입니다."),
                L(Voice, "전부요?"),
                L("강수아", "저예요. 강수아. 오늘 재심, 제가 아빠 변호인이에요."),
                L("강수아", "이 사진, 기억하세요? 23시 35분. 탁자 위에 라이터가 없어요."),
                L("강수아", "구석에 코트 입은 남자가 있어요. 20년 동안 얼굴이 흐릿했죠. 이제는 보여요."),
                L("강수아", "아저씨. 20년 전에도 이 부분만 빼고 말씀하셨어요.")
            },
            Question = "당신은 진실을 말했는가, 진실을 골라서 말했는가?"
        };

        private static readonly EndingDef FragmentEnding = new EndingDef
        {
            Title = "단편",
            Verdict = new[]
            {
                L("재판장", "피고인 강무진에게 사형을 선고한다."),
                Think("법정 밖에서 환호가 터졌다. 비는 그쳤고, 사람들은 정의가 이루어졌다고 말했다.")
            },
            Frame = new[]
            {
                L(Me, "...그게 그날 밤의 전부입니다."),
                L(Voice, "20년 전에도 똑같이 말씀하셨어요. 한 글자도 안 틀리고."),
                L("강수아", "저예요. 강수아. 오늘 재심, 제가 아빠 변호인이에요."),
                L("강수아", "아빠는 재작년 겨울에 교도소에서 돌아가셨어요. 재심은 아빠 이름을 위해서 하는 거예요."),
                L("강수아", "아저씨는 그날 무엇을 보셨어요? 아니면, 무엇만 보셨어요?")
            },
            Question = "당신은 단편만 보고 한 사람의 인생을 판단할 수 있는가?"
        };

        private static readonly EndingDef SilenceEnding = new EndingDef
        {
            Title = "침묵",
            Verdict = new[]
            {
                L("재판장", "증인이 판단을 거부했으므로, 제출된 증거만으로 판단한다."),
                L("재판장", "라이터, 혈흔, 목격 진술. 피고인 강무진, 무기징역."),
                Think("내가 판단하지 않자, 그림자들이 대신 판단했다.")
            },
            Frame = new[]
            {
                L(Me, "...그게 그날 밤의 전부입니다."),
                L(Voice, "판단하지 않으셨네요. 그날도요."),
                L("강수아", "저예요. 강수아. 오늘 재심, 제가 아빠 변호인이에요."),
                L("강수아", "아저씨가 아무 말도 안 해서, 다른 사람들이 대신 말했어요. 기자도, 반장님도, 사람들도요.")
            },
            Question = "판단하지 않는 것도 판단인가?"
        };
    }
}

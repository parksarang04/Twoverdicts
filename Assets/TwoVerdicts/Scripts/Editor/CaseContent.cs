using TwoVerdicts.Data;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 코너 사건 시나리오 원본 텍스트. 실제 사건(1994 미국)에서 영감을 받은 가상 사건.
    /// 인물/지명은 모두 가상.
    /// </summary>
    public static class CaseContent
    {
        public struct EvidenceDef
        {
            public string Id, Title, Surface, Deep;
            public EvidenceCategory Category;
            public int Weight;
            public bool Tainted;
        }

        public struct EndingDef
        {
            public EndingType Type;
            public string Title, Body, Question;
        }

        public const string CaseTitle = "코너 사건";
        public const int MaxActions = 8;
        public const int StartCrowdGuilt = 50;

        public static readonly string[] Intro =
        {
            "1995년, 웨스트포트.",
            "은퇴한 국민 야구 스타 레이 코너의 전처 엘렌, 그리고 그녀의 지인 대니가 엘렌의 집 앞에서 숨진 채 발견됐다.",
            "닷새 뒤, 레이는 흰 차를 타고 고속도로를 달렸다. 수천만 명이 그 추격을 생중계로 지켜봤다.",
            "도시는 둘로 갈라졌다. \"당연히 그가 범인이다.\" \"경찰이 또 누군가를 만들어낸다.\"",
            "당신은 배심원이다. 판결 전까지 주어진 조사 기회는 8번.",
            "모든 것을 볼 수는 없다. 무엇을 볼지는 당신이 정한다."
        };

        public static readonly string[] BetweenTrials =
        {
            "형사재판이 끝나고 1년 뒤.",
            "유족은 레이를 상대로 민사소송을 걸었다.",
            "증거는 그대로다. 바뀐 것은 판단의 기준뿐이다."
        };

        public const string CriminalStandard =
            "형사재판의 기준\n합리적 의심이 없을 정도로 증명되었는가?\n조금이라도 합리적인 의심이 남는다면, 무죄다.";

        public const string CivilStandard =
            "민사재판의 기준\n어느 쪽이 더 그럴듯한가?\n50%를 조금이라도 넘는다면, 책임이 있다.";

        public static readonly EvidenceDef[] Evidence =
        {
            new EvidenceDef { Id = "glove_left", Title = "현장의 왼쪽 장갑", Category = EvidenceCategory.Physical, Weight = 2, Tainted = true,
                Surface = "현장에서 피 묻은 가죽 장갑 한 짝이 나왔다. 레이가 즐겨 끼던 것과 같은 제품이다.",
                Deep = "법정에서 레이가 장갑을 끼워 보았다. 손이 잘 들어가지 않았다. 검사 측은 피에 젖었다 마른 가죽이 줄어든 것이라고 반박했다." },
            new EvidenceDef { Id = "glove_right", Title = "저택 뒤뜰의 오른쪽 장갑", Category = EvidenceCategory.Physical, Weight = 3, Tainted = true,
                Surface = "레이의 저택 뒤편 통로에서 나머지 한 짝이 나왔다. 두 피해자의 혈흔이 묻어 있다.",
                Deep = "장갑을 발견한 사람은 혼자 담을 넘어 들어간 하먼 형사다. 영장은 없었고, 목격자도 없었다." },
            new EvidenceDef { Id = "harman_tape", Title = "하먼 형사의 녹음테이프", Category = EvidenceCategory.Record, Weight = -2, Tainted = true,
                Surface = "한 작가가 몇 년 전 하먼 형사를 인터뷰한 테이프를 갖고 있다고 한다.",
                Deep = "테이프 속 하먼은 흑인을 향한 멸칭을 수십 번 쓰고, 증거를 만들어낸 적이 있다고 자랑한다. 그는 법정에서 그런 말을 한 적 없다고 선서했다." },
            new EvidenceDef { Id = "blood_sample", Title = "혈액 샘플 보관 기록", Category = EvidenceCategory.Record, Weight = 3, Tainted = true,
                Surface = "레이에게서 채취한 혈액의 DNA가 현장 혈흔과 일치한다. 우연일 확률은 수십억 분의 일.",
                Deep = "채취한 혈액 중 일부의 양이 기록과 맞지 않는다. 담당 수사관은 그 샘플을 하루 동안 들고 다녔다." },
            new EvidenceDef { Id = "car_blood", Title = "흰 차 안의 혈흔", Category = EvidenceCategory.Physical, Weight = 2, Tainted = true,
                Surface = "레이의 흰 차 운전석과 콘솔에서 피해자들의 혈흔이 나왔다.",
                Deep = "차는 견인된 뒤 여러 사람이 드나드는 보관소에 며칠 동안 있었다. 출입 기록은 비어 있다." },
            new EvidenceDef { Id = "dv_record", Title = "가정폭력 신고 기록", Category = EvidenceCategory.Record, Weight = 2, Tainted = false,
                Surface = "엘렌은 결혼 생활 동안 여러 번 경찰을 불렀다. 한 녹음에서 그녀는 \"그가 날 죽일 거예요\"라고 말한다.",
                Deep = "레이는 매번 가벼운 처분만 받았다. 출동한 경찰들은 그에게 사인을 받아 갔다." },
            new EvidenceDef { Id = "driver_testimony", Title = "리무진 기사의 증언", Category = EvidenceCategory.Testimony, Weight = 2, Tainted = false,
                Surface = "공항으로 가기로 한 밤, 기사는 레이의 집 앞에서 한참 초인종을 눌렀지만 아무도 나오지 않았다고 한다.",
                Deep = "잠시 뒤 검은 옷을 입은 큰 체격의 사람이 현관으로 들어가는 것을 봤다. 그 직후 집에 불이 켜졌고, 레이가 나왔다." },
            new EvidenceDef { Id = "live_chase", Title = "추격 생중계 영상", Category = EvidenceCategory.Record, Weight = 2, Tainted = false,
                Surface = "체포 직전 레이는 고속도로를 달렸다. 차 안에는 여권, 가짜 수염, 현금이 있었다.",
                Deep = "그는 출발 전 \"나 때문에 슬퍼하지 마라\"로 끝나는 편지를 남겼다. 도주였을까, 무너진 걸까." }
        };

        public static readonly EndingDef[] Endings =
        {
            new EndingDef { Type = EndingType.SnapJudgment, Title = "단편",
                Body = "당신은 사건의 표면만 보고 판결했다.\n장갑, 혈흔, 생중계 화면.\n법정 밖에서 돌을 던지던 사람들도 정확히 그것만 보았다.",
                Question = "당신은 단편만 보고 한 사람의 인생을 판단할 수 있는가?" },
            new EndingDef { Type = EndingType.TwoVerdicts, Title = "두 개의 판결",
                Body = "형사재판: 무죄.\n민사재판: 책임 있음.\n같은 사람, 같은 증거. 바뀐 것은 인간이 정한 기준뿐이었다.",
                Question = "진실은 하나인데, 판결은 둘이다.\n인간이 정한 법의 테두리로 한 사람의 인생을 판단할 수 있는가?" },
            new EndingDef { Type = EndingType.Contradiction, Title = "뒤집힌 저울",
                Body = "당신은 더 엄격한 기준에서 유죄를, 더 느슨한 기준에서 무죄를 골랐다.\n법의 논리로는 성립하지 않는 판결이다.",
                Question = "당신의 판결을 만든 것은 법이었는가, 당신이었는가?" },
            new EndingDef { Type = EndingType.CertainGuilt, Title = "확신",
                Body = "당신은 두 번 모두 유죄를 선택했다.\n오염된 증거와 편견 섞인 수사를 보고도.\n어쩌면 당신이 옳을지도 모른다.",
                Question = "확신은 증명이 될 수 있는가?" },
            new EndingDef { Type = EndingType.CertainDoubt, Title = "의심",
                Body = "당신은 두 번 모두 무죄를 선택했다.\n더 그럴듯한 쪽을 고르면 되는 기준에서도.\n남겨진 신고 기록과 두 사람의 죽음은 누구의 몫으로 남는가.",
                Question = "의심은 누구를 지키고, 누구를 남겨두는가?" }
        };
    }
}

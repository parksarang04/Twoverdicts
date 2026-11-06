namespace TwoVerdicts.V2.Data
{
    public static partial class StoryDb
    {
        public const string CourtRoom = "court";
        public const string FrameRoom = "visit2018";

        public static readonly RoomDef[] Rooms =
        {
            new RoomDef("apartment", "세라의 집 (사건 현장)", true),
            new RoomDef("guard", "아파트 경비실", true),
            new RoomDef("store", "24시 편의점", true),
            new RoomDef("station", "해운방송국", true),
            new RoomDef("police", "해운경찰서 강력반", true),
            new RoomDef(CourtRoom, "해운지방법원", false),
            new RoomDef(FrameRoom, "재심 증인 대기실", false)
        };

        public static RoomDef GetRoom(string id)
        {
            foreach (RoomDef room in Rooms)
            {
                if (room.Id == id) return room;
            }
            return null;
        }

        public static readonly Spot[] Spots =
        {
            new Spot("apartment", -6.5f, "현관 바닥", "c_footprints", Think("(발자국이 엉망이다. 누가 이렇게 밟고 다녔지.)")),
            new Spot("apartment", -0.5f, "라디오", "c_radio", Think("(세라는 매일 자기 방송을 녹음했다. 22:40 부분을 들어본다.)"), L("윤세라", "...오늘 밤은 창밖에 별이 참 많네요. 다들 하늘 한번 보세요.")),
            new Spot("apartment", 2.5f, "탁자", "c_lighter", Think("(라이터. K.M.J. 무진의 것. 이걸로 끝난 사건이다.)")),
            new Spot("apartment", 5f, "거울", null, Think("(거울에 비친 내 얼굴이 흐릿하다.)"), Think("(사흘째 못 잤으니까.)")),
            new Spot("apartment", 8f, "창문", "c_window", Think("(창문이 활짝 열려 있다. 빗물이 바닥까지 들이쳤다.)")),

            new Spot("guard", -4f, "열쇠 보관함", "c_masterkey", Think("(마스터키 사용 기록. 23:05, 1102호.)")),
            new Spot("guard", 1f, "무진의 서랍", "c_drawer", Think("(딸 사진. 담배. 라이터 자리는 비어 있다.)")),
            new Spot("guard", 5f, "방문록", "c_visitor_log", Think("(23:50. 서명이 번졌다. 코트 소매로 문댄 것 같은 자국.)")),
            new Spot("guard", 8f, "작은 TV", null, L("TV", "...세라의 새벽, 매일 밤 10시. 당신의 밤을 지켜드립니다.")),

            new Spot("store", -5f, "유리문", null, Think("(비. 사흘째 그칠 줄을 모른다.)")),
            new Spot("store", 3.2f, "계산대", "c_rain_receipt", Think("(그날 밤 영수증. 22:20 부터 우산이 줄줄이 팔렸다.)")),

            new Spot("station", -5f, "우산꽂이", "c_umbrella", Think("(검은 장우산. 서한결. 흙탕물 자국.)")),
            new Spot("station", -2.2f, "신발장", "c_shoes", Think("(서한결 칸. 270mm 구두.)")),
            new Spot("station", 4.5f, "PD 책상", "c_ledger", Think("(협찬금 장부. 숫자가 맞지 않는다.)")),
            new Spot("station", 7.5f, "대본 보드", "c_script", Think("(세라의 다음 날 대본이 붙어 있다.)")),

            new Spot("police", -6.5f, "증거 목록", "c_evidence_list", Think("(라이터 발견 00:20. 발견자 칸이 비어 있다.)")),
            new Spot("police", 3.6f, "내 책상", null, Think("(서랍에 빈 소주병이 굴러다닌다.)"), Think("(...언제 마셨더라.)"))
        };

        public static readonly Deduction[] Deductions =
        {
            new Deduction
            {
                CardA = "c_radio", CardB = "c_rain_receipt", Correct = 1, Result = "d_not_live",
                Question = "22:40 의 \"별이 많네요\" 와 22:20 에 시작된 비.",
                Options = new[] { "세라는 창밖을 보지 않고 말했다", "그 방송은 비가 오기 전에 녹음됐다", "라디오 시계가 틀렸다" }
            },
            new Deduction
            {
                CardA = "c_stairs_man", CardB = "c_umbrella", Correct = 2, Result = "d_outside",
                Question = "계단의 젖은 장우산과 한결의 흙탕물 묻은 장우산.",
                Options = new[] { "무진이 우산을 훔쳐 갔다", "우산은 누구나 쓴다", "한결은 그날 밤 세라의 아파트 계단에 있었다" }
            },
            new Deduction
            {
                CardA = "c_ledger", CardB = "c_script", Correct = 0, Result = "d_motive",
                Question = "맞지 않는 장부와 \"내일은 중요한 이야기를 할게요\".",
                Options = new[] { "세라는 방송에서 한결의 횡령을 폭로하려 했다", "세라는 방송국을 그만두려 했다", "장부는 세라가 조작했다" }
            },
            new Deduction
            {
                CardA = "c_photo", CardB = "c_evidence_list", Correct = 1, Result = "d_planted",
                Question = "23:35 사진엔 라이터가 없다. 00:20 에 발견됐다.",
                Options = new[] { "감식반이 처음에 라이터를 놓쳤다", "그 사이 누군가 라이터를 탁자에 놓았다", "기자가 라이터를 가져갔다가 돌려놨다" }
            },
            new Deduction
            {
                CardA = "d_planted", CardB = "c_visitor_log", Correct = 0, Result = "d_gray_man",
                Question = "나중에 놓인 라이터와 23:50 경비실의 번진 서명.",
                Options = new[] { "23:50 경비실에서 라이터를 가져간 사람이 현장에 놓았다", "무진이 몰래 경비실로 돌아왔다", "방문록은 누군가 꾸며낸 것이다" }
            },
            new Deduction
            {
                CardA = "d_gray_man", CardB = "c_soju", Correct = 2, Result = "d_me", IsReveal = true,
                Question = "번진 서명의 코트 입은 남자. 23:20 소주 두 병을 사 간 코트 입은 남자. 그는 누구인가.",
                Options = new[] { "반장 오태식", "기자 박진표", "최윤석" }
            }
        };

        public static Deduction FindDeduction(string a, string b)
        {
            foreach (Deduction d in Deductions)
            {
                bool match = (d.CardA == a && d.CardB == b) || (d.CardA == b && d.CardB == a);
                if (match) return d;
            }
            return null;
        }
    }
}

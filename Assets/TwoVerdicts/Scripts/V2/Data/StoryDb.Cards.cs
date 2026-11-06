using System.Collections.Generic;

namespace TwoVerdicts.V2.Data
{
    /// <summary>
    /// 회색 증언 - 전체 시나리오 데이터. (실제 사건에서 영감을 받은 가상의 이야기)
    /// </summary>
    public static partial class StoryDb
    {
        public const string Me = "최윤석";
        public const string Voice = "???";
        public const string PlayerId = "yunseok";
        public const string StartCard = "c_notebook";
        public const int StartActions = 10;
        public const int ColorThreshold = 80;

        private static Dictionary<string, Card> cards;

        public static IReadOnlyDictionary<string, Card> Cards => cards ?? (cards = BuildCards());

        public static Card GetCard(string id)
        {
            return id != null && Cards.TryGetValue(id, out Card card) ? card : null;
        }

        private static Dictionary<string, Card> BuildCards()
        {
            var list = new List<Card>();
            list.AddRange(SceneCards);
            list.AddRange(TestimonyCards);
            list.AddRange(DeductionCards);
            var map = new Dictionary<string, Card>();
            foreach (Card card in list) map[card.Id] = card;
            return map;
        }

        private static readonly Card[] SceneCards =
        {
            new Card("c_notebook", "내 수첩", "1998년 수사 수첩. 11월 14일 밤 페이지가 찢겨 나가 있다. 누가 찢었을까."),
            new Card("c_radio", "세라의 방송 테이프", "세라는 매일 자기 방송을 녹음해 뒀다. 22:40 부분: \"오늘 밤은 창밖에 별이 참 많네요.\""),
            new Card("c_window", "열린 창문", "11월 밤인데 현장 창문이 활짝 열려 있었다. 감식 전에 누군가 열었다. 체온으로 사망 시각을 재기 어렵게 됐다."),
            new Card("c_lighter", "각인 라이터", "탁자 위에서 나온 라이터. K.M.J 각인. 강무진의 것. 이 사건의 결정적 증거."),
            new Card("c_footprints", "엉킨 발자국", "현관 바닥에 여러 사람의 발자국이 엉켜 있다. 일부는 닦아낸 흔적이 있다."),
            new Card("c_masterkey", "마스터키 기록", "23:05 - 1102호(윤세라) 마스터키 사용. 서명: 강무진."),
            new Card("c_drawer", "무진의 빈 서랍", "경비실 무진의 서랍. 담배와 딸 사진. 라이터를 두던 자리만 비어 있다."),
            new Card("c_visitor_log", "번진 방문록", "경비실 방문록 23:50. 서명이 빗물에 번져 읽을 수 없다. 코트 소매로 문지른 자국."),
            new Card("c_rain_receipt", "우산 영수증", "편의점 영수증 묶음. 22:20 부터 우산이 연달아 팔렸다. 비는 그때 시작됐다."),
            new Card("c_umbrella", "한결의 장우산", "방송국 우산꽂이의 검은 장우산. 이름표: 서한결. 흙탕물 자국이 말라붙어 있다."),
            new Card("c_shoes", "270mm 구두", "방송국 신발장 '서한결' 칸의 구두. 270mm. 밑창에 마른 흙."),
            new Card("c_ledger", "협찬금 장부", "숫자가 맞지 않는 협찬금 장부. 3년간 결재자는 전부 서한결."),
            new Card("c_script", "세라의 다음 대본", "다음 날 방송 대본 첫 줄: \"내일은 우리 방송국의 아주 중요한 이야기를 할게요.\""),
            new Card("c_evidence_list", "증거 목록", "라이터 - 00:20 현장 탁자 위에서 발견. 발견자 기재란이 비어 있다.")
        };

        private static readonly Card[] TestimonyCards =
        {
            new Card("c_mujin_entered", "무진의 자백", "무진은 23:05 에 돈을 훔치러 1102호에 들어갔다. 딸 수술비 때문이었다."),
            new Card("c_stairs_man", "계단의 우산 든 남자", "무진이 들어갈 때 계단으로 누가 내려갔다. 젖은 장우산을 들고 있었다."),
            new Card("c_memo_270", "지워진 감식 메모", "감식반 문지혜의 메모. 현장에 270mm 족적이 하나 더 있었다. 무진의 발은 255mm. 반장이 보고서에서 빼라고 했다."),
            new Card("c_photo", "기자의 현장 사진", "박진표 기자가 23:35 에 몰래 찍은 사진. 탁자 위에 라이터가 없다. 구석에 코트 입은 남자가 흐릿하게 찍혔다."),
            new Card("c_coat_man", "코트 입은 사람", "지혜: 감식반이 오기 전, 기자 말고 한 사람이 더 현장에 있었다. 코트를 입고 탁자 쪽에 서 있었다."),
            new Card("c_soju", "소주 손님", "초롱: 23:20 에 코트 입은 손님이 소주 두 병을 사 갔다. 술 냄새가 났다."),
            new Card("c_sua_letter", "수아의 편지", "\"아빠 수술비 걱정 하지 마. 나 안 아파도 돼. 아빠만 집에 와.\""),
            new Card("c_cover_up", "반장의 은폐", "반장은 족적 메모를 뺐다. \"빨리 끝내야 했어. 자네를 위해서이기도 했고.\"")
        };

        private static readonly Card[] DeductionCards =
        {
            new Card("d_not_live", "생방송이 아니었다", "비는 22:20 에 시작됐는데 22:40 방송은 별이 많다고 했다. 그 방송은 미리 녹음된 것이다.", CardKind.Deduction),
            new Card("d_outside", "한결은 그날 밤 밖에 있었다", "계단의 남자가 들고 있던 젖은 장우산은 한결의 것이다.", CardKind.Deduction),
            new Card("d_motive", "세라는 폭로하려 했다", "세라는 다음 방송에서 한결의 협찬금 횡령을 밝히려 했다.", CardKind.Deduction),
            new Card("d_planted", "라이터는 나중에 놓였다", "23:35 사진엔 없고 00:20 에 발견됐다. 그 사이 누군가 라이터를 탁자에 놓았다.", CardKind.Deduction),
            new Card("d_gray_man", "번진 서명의 남자", "23:50 경비실에서 라이터를 가져간 사람이 현장에 놓았다. 코트 입은 남자.", CardKind.Deduction),
            new Card("d_me", "그건 나였다", "소주 두 병. 제일 먼저 도착한 형사. 창문을 연 사람. 번진 서명. 찢긴 수첩. 라이터를 놓은 사람은 최윤석, 나다.", CardKind.Deduction)
        };
    }
}

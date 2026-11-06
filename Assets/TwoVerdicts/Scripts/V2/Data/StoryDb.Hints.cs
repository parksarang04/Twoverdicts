using System.Collections.Generic;

namespace TwoVerdicts.V2.Data
{
    /// <summary>
    /// 단서를 어디서 얻는지 (단서 트리 + 수첩 힌트용).
    /// </summary>
    public static partial class StoryDb
    {
        public static readonly Dictionary<string, string> CardHints = new Dictionary<string, string>
        {
            { "c_notebook", "처음부터 가지고 있다" },
            { "c_radio", "사건 현장 - 라디오" },
            { "c_window", "사건 현장 - 창문" },
            { "c_lighter", "사건 현장 - 탁자" },
            { "c_footprints", "사건 현장 - 현관 바닥" },
            { "c_masterkey", "경비실 - 열쇠 보관함" },
            { "c_drawer", "경비실 - 무진의 서랍" },
            { "c_visitor_log", "경비실 - 방문록" },
            { "c_rain_receipt", "편의점 - 계산대" },
            { "c_umbrella", "방송국 - 우산꽂이" },
            { "c_shoes", "방송국 - 신발장" },
            { "c_ledger", "방송국 - PD 책상" },
            { "c_script", "방송국 - 대본 보드" },
            { "c_evidence_list", "경찰서 - 벽의 증거 목록" },
            { "c_mujin_entered", "경찰서 - 무진의 첫 진술에 [마스터키 기록]" },
            { "c_stairs_man", "경찰서 - 무진의 두 번째 진술을 추궁" },
            { "c_memo_270", "사건 현장 - 지혜의 첫 진술에 [엉킨 발자국]" },
            { "c_photo", "편의점 - 기자의 첫 진술에 [엉킨 발자국]" },
            { "c_coat_man", "사건 현장 - 지혜의 세 번째 진술에 [기자의 현장 사진]" },
            { "c_soju", "편의점 - 초롱의 두 번째 진술을 추궁" },
            { "c_sua_letter", "경찰서 - 벤치의 아이와 이야기" },
            { "c_cover_up", "경찰서 - 반장의 세 번째 진술에 [지워진 감식 메모]" },
            { "d_not_live", "단서 트리 1번 줄을 조합" },
            { "d_outside", "단서 트리 2번 줄을 조합" },
            { "d_motive", "단서 트리 3번 줄을 조합" },
            { "d_planted", "단서 트리 4번 줄을 조합" },
            { "d_gray_man", "단서 트리 5번 줄을 조합" },
            { "d_me", "단서 트리 마지막 줄" }
        };

        /// <summary>단서 트리 각 줄의 이름 (Deductions 배열 순서와 같음).</summary>
        public static readonly string[] TreeRowNames =
        {
            "PD의 알리바이", "계단의 남자", "세라의 동기", "라이터의 시간", "라이터를 옮긴 손", "코트의 남자"
        };

        public static string HintFor(string cardId)
        {
            return cardId != null && CardHints.TryGetValue(cardId, out string hint) ? hint : "";
        }
    }
}

using UnityEngine;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 장소 4곳과 단서 위치 (1280x720 기준, 배경 그림의 빨간 지점 근처).
    /// </summary>
    public static class SceneLayout
    {
        public struct HotspotDef
        {
            public string EvidenceId;
            public Vector2 Position;

            public HotspotDef(string evidenceId, float x, float y)
            {
                EvidenceId = evidenceId;
                Position = new Vector2(x, y);
            }
        }

        public struct LocationDef
        {
            public string ObjectName;
            public string DisplayName;
            public string Background;
            public HotspotDef[] Hotspots;
        }

        public static readonly LocationDef[] Locations =
        {
            new LocationDef
            {
                ObjectName = "Loc_CrimeScene", DisplayName = "사건 현장", Background = "bg_crime_scene",
                Hotspots = new[] { new HotspotDef("glove_left", 656, 636) }
            },
            new LocationDef
            {
                ObjectName = "Loc_Mansion", DisplayName = "레이의 저택", Background = "bg_mansion",
                Hotspots = new[]
                {
                    new HotspotDef("glove_right", 908, 600),
                    new HotspotDef("car_blood", 320, 520),
                    new HotspotDef("driver_testimony", 668, 300)
                }
            },
            new LocationDef
            {
                ObjectName = "Loc_Police", DisplayName = "경찰서 증거보관실", Background = "bg_police",
                Hotspots = new[]
                {
                    new HotspotDef("harman_tape", 984, 336),
                    new HotspotDef("blood_sample", 1096, 290),
                    new HotspotDef("dv_record", 400, 220)
                }
            },
            new LocationDef
            {
                ObjectName = "Loc_Studio", DisplayName = "WNN 방송국", Background = "bg_studio",
                Hotspots = new[] { new HotspotDef("live_chase", 640, 280) }
            }
        };

        /// <summary>폰트 아틀라스에 미리 구워둘 UI 문구.</summary>
        public static readonly string[] UiTexts =
        {
            "법정으로", "남은 조사", "여론: 유죄", "유죄", "무죄", "다시 하기", "클릭하면 건너뛰기", "클릭 >",
            "빨간 표시를 누르면 조사, 같은 표시를 다시 누르면 내막. < > 로 장소 이동",
            "조사는 여기까지다. 이제 법정으로.", "1부 - 형사재판", "2부 - 민사재판", "당신이 알고 있는 것",
            "아무것도 조사하지 않았다.", "내막", "표면", "형사: 민사: 내막까지 본 증거", "(더 파고들 수 있을 것 같다.)"
        };
    }
}

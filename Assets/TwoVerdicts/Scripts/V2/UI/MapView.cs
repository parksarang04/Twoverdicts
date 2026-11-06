using System;
using System.Collections.Generic;
using TMPro;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.V2.UI
{
    /// <summary>
    /// 도시 지도. 장소로 이동할 때마다 행동력 1. 행동력이 0이면 법정만 열림.
    /// </summary>
    public class MapView : View
    {
        private static readonly Dictionary<string, Vector2> Pins = new Dictionary<string, Vector2>
        {
            { "apartment", new Vector2(330f, 250f) },
            { "guard", new Vector2(250f, 400f) },
            { "store", new Vector2(520f, 440f) },
            { "station", new Vector2(900f, 230f) },
            { "police", new Vector2(960f, 470f) }
        };

        private readonly List<Button> pinButtons = new List<Button>();
        private TextMeshProUGUI actionsText;
        private Action<string> onPick;
        private Action onCourt;

        public static MapView Create(Transform parent) => CreateView<MapView>(parent, "Map");

        protected override void Build()
        {
            Ui.Image(Ui.Stretch("Bg", transform), null, Ui.Ink, true);
            Ui.Image(Ui.Place("Map", transform, 640f, 350f, 1120f, 630f), Art.Ui("map"), Color.white);
            Ui.Label(transform, "Title", "해운시 - 1998년 11월 14일 밤", 640f, 36f, 900f, 40f, 26f, Ui.Paper,
                TextAlignmentOptions.Center);
            actionsText = Ui.Label(transform, "Actions", "", 1110f, 36f, 300f, 40f, 22f, Ui.Red, TextAlignmentOptions.Right);
            foreach (RoomDef room in StoryDb.Rooms)
            {
                if (!room.OnMap || !Pins.TryGetValue(room.Id, out Vector2 pin)) continue;
                string id = room.Id;
                pinButtons.Add(Ui.Button(transform, room.Name, pin.x, pin.y, 250f, 52f, () => Pick(id), 18f));
            }
            Ui.Button(transform, "법정으로 (최종 재판)", 640f, 680f, 340f, 52f, Court, 20f);
        }

        public void Show(int actions, Action<string> pick, Action court)
        {
            onPick = pick;
            onCourt = court;
            actionsText.text = $"남은 행동 {actions}";
            foreach (Button b in pinButtons) b.interactable = actions > 0;
            OpenView();
        }

        private void Pick(string roomId)
        {
            CloseView();
            Ui.SafeInvoke(() => onPick?.Invoke(roomId));
        }

        private void Court()
        {
            CloseView();
            Ui.SafeInvoke(onCourt);
        }
    }
}

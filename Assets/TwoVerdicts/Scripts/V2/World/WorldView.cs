using System;
using System.Collections;
using System.Collections.Generic;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using UnityEngine;

namespace TwoVerdicts.V2.World
{
    /// <summary>
    /// 방(배경 + 단서 지점 + 인물 + 문)과 플레이어를 만들고 방을 전환한다.
    /// 배경은 흑백/컬러 두 장이고, 결정적 장면에서 컬러로 바뀐다.
    /// </summary>
    public class WorldView : MonoBehaviour
    {
        public const float FloorY = -3.75f;
        private const float DoorX = -9.2f;

        private class Room
        {
            public GameObject Root;
            public SpriteRenderer Background;
            public string Id;
            public readonly List<Interactable> Items = new List<Interactable>();
            public readonly List<ClarityActor> Actors = new List<ClarityActor>();
        }

        private readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();
        private Room current;
        private GameSession session;
        private bool colorMode;
        private bool pinnedColor;

        public PlayerMover Player { get; private set; }
        public ClarityActor PlayerBody { get; private set; }
        public string CurrentRoom => current?.Id;

        public void Build(GameSession gameSession)
        {
            session = gameSession;
            foreach (RoomDef def in StoryDb.Rooms) rooms[def.Id] = BuildRoom(def);
            BuildPlayer();
            foreach (Room room in rooms.Values) room.Root.SetActive(false);
        }

        private Room BuildRoom(RoomDef def)
        {
            var room = new Room { Id = def.Id, Root = new GameObject("Room_" + def.Id) };
            room.Root.transform.SetParent(transform, false);
            room.Background = CreateSprite("Background", room.Root.transform, Vector3.zero, 0);
            room.Background.sprite = Art.Room(def.Id, false);
            AddSpots(room);
            AddNpcs(room);
            if (def.OnMap) AddItem(room, InteractKind.Door, "door", "나가기 (지도)", DoorX, null);
            return room;
        }

        private void AddSpots(Room room)
        {
            for (int i = 0; i < StoryDb.Spots.Length; i++)
            {
                Spot spot = StoryDb.Spots[i];
                if (spot.Room != room.Id) continue;
                SpriteRenderer glint = CreateSprite("Glint", room.Root.transform, new Vector3(spot.X, -0.6f, 0f), 20);
                glint.sprite = Art.Load("fx/glint");
                glint.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
                AddItem(room, InteractKind.Spot, i.ToString(), spot.Label, spot.X, glint);
            }
        }

        private void AddNpcs(Room room)
        {
            foreach (Npc npc in StoryDb.Npcs)
            {
                if (npc.Room != room.Id) continue;
                string id = npc.Id;
                room.Actors.Add(CreateActor(npc.Id, room.Root.transform, npc.X, () => session.GetClarity(id), 5));
                SpriteRenderer alert = CreateSprite("Alert", room.Root.transform, new Vector3(npc.X, FloorY + 3.4f, 0f), 20);
                alert.sprite = Art.Load("fx/alert");
                AddItem(room, InteractKind.Npc, npc.Id, npc.Name, npc.X, alert);
            }
        }

        private void AddItem(Room room, InteractKind kind, string key, string label, float x, SpriteRenderer glint)
        {
            var go = new GameObject($"Item_{kind}_{key}");
            go.transform.SetParent(room.Root.transform, false);
            go.transform.localPosition = new Vector3(x, FloorY, 0f);
            var item = go.AddComponent<Interactable>();
            item.Setup(kind, key, label, glint);
            room.Items.Add(item);
        }

        private void BuildPlayer()
        {
            PlayerBody = CreateActor(StoryDb.PlayerId, transform, -8f, () => session.GetClarity(StoryDb.PlayerId), 10);
            Player = PlayerBody.gameObject.AddComponent<PlayerMover>();
            Player.Setup(PlayerBody, () => current != null ? current.Items : null);
        }

        public ClarityActor CreateActor(string charId, Transform parent, float x, Func<int> clarity, int order)
        {
            SpriteRenderer sr = CreateSprite("Actor_" + charId, parent, new Vector3(x, FloorY, 0f), order);
            var actor = sr.gameObject.AddComponent<ClarityActor>();
            actor.Setup(charId, clarity, order);
            return actor;
        }

        private static SpriteRenderer CreateSprite(string name, Transform parent, Vector3 pos, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = order;
            return sr;
        }

        public void ShowRoom(string roomId, float playerX, bool showPlayer = true)
        {
            if (!rooms.TryGetValue(roomId, out Room next))
            {
                Debug.LogError("[WorldView] 없는 방: " + roomId);
                return;
            }
            if (current != null) current.Root.SetActive(false);
            current = next;
            current.Root.SetActive(true);
            ApplyColor();
            PlayerBody.gameObject.SetActive(showPlayer);
            Player.Place(playerX);
        }

        /// <summary>인물 머리 위 "!" 조건 연결 (npcId -> 지금 진행할 게 있나).</summary>
        public void SetNpcHot(Func<string, bool> hot)
        {
            foreach (Room room in rooms.Values)
            {
                foreach (Interactable item in room.Items)
                {
                    if (item.Kind != InteractKind.Npc) continue;
                    string id = item.Key;
                    item.Hot = () => hot(id);
                }
            }
        }

        public Transform RoomTransform(string roomId) => rooms.TryGetValue(roomId, out Room r) ? r.Root.transform : null;

        public void MarkSpent(Interactable item) => item.Spent = true;

        public void SetColor(bool on)
        {
            pinnedColor = on;
            colorMode = on;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (current == null) return;
            Sprite sprite = Art.Room(current.Id, colorMode);
            if (sprite != null) current.Background.sprite = sprite;
        }

        /// <summary>잠깐 컬러로 번쩍 (진술 깨기, 추리 성공).</summary>
        public void Flash(float seconds)
        {
            if (pinnedColor) return;
            StartCoroutine(FlashRoutine(seconds));
        }

        private IEnumerator FlashRoutine(float seconds)
        {
            colorMode = true;
            ApplyColor();
            yield return new WaitForSecondsRealtime(seconds);
            colorMode = pinnedColor;
            ApplyColor();
        }

        public void RefreshActors()
        {
            PlayerBody.Refresh();
            if (current == null) return;
            foreach (ClarityActor actor in current.Actors) actor.Refresh();
        }
    }
}

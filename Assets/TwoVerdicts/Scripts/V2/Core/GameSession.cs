using System;
using System.Collections.Generic;
using TwoVerdicts.V2.Data;
using UnityEngine;

namespace TwoVerdicts.V2.Core
{
    /// <summary>
    /// 한 판의 모든 상태. 단서, 행동력, 인물별 선명도/신뢰도, 깨진 진술, 선택.
    /// </summary>
    public class GameSession
    {
        public const int MaxTrust = 3;

        private readonly List<string> cards = new List<string>();
        private readonly Dictionary<string, int> clarity = new Dictionary<string, int>();
        private readonly Dictionary<string, int> trust = new Dictionary<string, int>();
        private readonly HashSet<string> doneStatements = new HashSet<string>();
        private readonly HashSet<string> usedSpots = new HashSet<string>();
        private readonly HashSet<string> flags = new HashSet<string>();

        public int Actions { get; private set; } = StoryDb.StartActions;
        public int SuaAnswer { get; set; } = -1;
        public bool SoldStory { get; set; }
        public bool Revealed { get; private set; }
        public bool PageKept { get; private set; }

        public IReadOnlyList<string> CardIds => cards;

        public event Action<Card> OnCardAdded;
        public event Action<string, int> OnClarityChanged;
        public event Action<int> OnActionsChanged;

        public GameSession()
        {
            foreach (Npc npc in StoryDb.Npcs)
            {
                clarity[npc.Id] = npc.StartClarity;
                trust[npc.Id] = MaxTrust;
            }
            clarity[StoryDb.PlayerId] = 0;
            cards.Add(StoryDb.StartCard);
        }

        public void ResetActions(int value)
        {
            Actions = Mathf.Max(1, value);
            OnActionsChanged?.Invoke(Actions);
        }

        public bool HasCard(string id) => id != null && cards.Contains(id);

        public bool AddCard(string id)
        {
            Card card = StoryDb.GetCard(id);
            if (card == null)
            {
                Debug.LogWarning("[GameSession] 없는 카드: " + id);
                return false;
            }
            if (cards.Contains(id)) return false;
            cards.Add(id);
            OnCardAdded?.Invoke(card);
            return true;
        }

        public int GetClarity(string npcId) => clarity.TryGetValue(npcId, out int v) ? v : 0;

        public void AddClarity(string npcId, int amount)
        {
            if (amount == 0) return;
            int next = Mathf.Clamp(GetClarity(npcId) + amount, 0, 100);
            clarity[npcId] = next;
            OnClarityChanged?.Invoke(npcId, next);
        }

        public void SetClarity(string npcId, int value)
        {
            clarity[npcId] = Mathf.Clamp(value, 0, 100);
            OnClarityChanged?.Invoke(npcId, clarity[npcId]);
        }

        public int GetTrust(string npcId) => trust.TryGetValue(npcId, out int v) ? v : 0;

        public void LoseTrust(string npcId)
        {
            trust[npcId] = Mathf.Max(0, GetTrust(npcId) - 1);
        }

        public bool SpendAction()
        {
            if (Actions <= 0) return false;
            Actions--;
            OnActionsChanged?.Invoke(Actions);
            return true;
        }

        public bool IsDone(string statementId) => doneStatements.Contains(statementId);

        public void MarkDone(string statementId) => doneStatements.Add(statementId);

        public bool IsUnlocked(Statement statement)
        {
            return string.IsNullOrEmpty(statement.UnlockAfter) || IsDone(statement.UnlockAfter);
        }

        public bool UseSpot(string spotKey) => usedSpots.Add(spotKey);

        public bool HasFlag(string flag) => flags.Contains(flag);

        public void SetFlag(string flag) => flags.Add(flag);

        public void MarkRevealed()
        {
            Revealed = true;
            SetClarity(StoryDb.PlayerId, 100);
        }

        public void ChoosePage(bool keep)
        {
            PageKept = keep;
            if (!keep) SetClarity(StoryDb.PlayerId, 0);
        }
    }
}

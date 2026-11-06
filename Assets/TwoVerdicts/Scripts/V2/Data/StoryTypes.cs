namespace TwoVerdicts.V2.Data
{
    public enum CardKind { Clue, Deduction }

    /// <summary>수첩에 모이는 단서/추론 카드.</summary>
    public class Card
    {
        public readonly string Id;
        public readonly string Name;
        public readonly string Desc;
        public readonly CardKind Kind;

        public Card(string id, string name, string desc, CardKind kind = CardKind.Clue)
        {
            Id = id;
            Name = name;
            Desc = desc;
            Kind = kind;
        }
    }

    /// <summary>대사 한 줄. Speaker 가 null 이면 윤석의 속마음(회색), "???" 면 붉은 목소리.</summary>
    public class Line
    {
        public readonly string Speaker;
        public readonly string Text;

        public Line(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }
    }

    /// <summary>
    /// 인터뷰 진술. BreakCard 를 제시하면 깨지고, 깨지면 BreakLines 출력 + 보상.
    /// BreakCard 가 null 이면 추궁(Press)만 가능한 진술.
    /// </summary>
    public class Statement
    {
        public string Id;
        public string Text;
        public Line[] PressLines;
        public string PressReward;
        public string BreakCard;
        public Line[] BreakLines;
        public string BreakReward;
        public int ClarityGain;
        public string UnlockAfter;
        public string Flag;
    }

    public class Npc
    {
        public string Id;
        public string Name;
        public string Room;
        public float X;
        public int StartClarity;
        public Line[] Greeting;
        public Statement[] Statements;
        public bool IsTalkOnly;
    }

    /// <summary>방 안의 조사 지점. CardId 가 null 이면 분위기용(대사만).</summary>
    public class Spot
    {
        public string Room;
        public float X;
        public string Label;
        public string CardId;
        public Line[] Lines;

        public Spot(string room, float x, string label, string cardId, params Line[] lines)
        {
            Room = room;
            X = x;
            Label = label;
            CardId = cardId;
            Lines = lines;
        }
    }

    public class Deduction
    {
        public string CardA;
        public string CardB;
        public string Question;
        public string[] Options;
        public int Correct;
        public string Result;
        public bool IsReveal;
    }

    public class RoomDef
    {
        public string Id;
        public string Name;
        public bool OnMap;

        public RoomDef(string id, string name, bool onMap)
        {
            Id = id;
            Name = name;
            OnMap = onMap;
        }
    }
}

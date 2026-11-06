namespace TwoVerdicts.V2.Data
{
    public static partial class StoryDb
    {
        private static Line L(string speaker, string text) => new Line(speaker, text);

        private static Line Think(string text) => new Line(null, text);

        public static readonly Npc[] Npcs =
        {
            new Npc
            {
                Id = "mujin", Name = "강무진", Room = "police", X = 6.2f, StartClarity = 10,
                Greeting = new[] { L("강무진", "형사님... 전 안 죽였어요. 정말이에요.") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "m1", Text = "저는 그날 밤 경비실 밖으로 한 발짝도 안 나갔어요.",
                        BreakCard = "c_masterkey", BreakReward = "c_mujin_entered", ClarityGain = 25,
                        BreakLines = new[]
                        {
                            L("강무진", "...들어갔어요. 1102호. 돈을... 훔치려고요."),
                            L("강무진", "수아 수술비 보증금이 모자랐어요. 세라 씨는 그 시간에 방송국에 있을 줄 알았어요.")
                        }
                    },
                    new Statement
                    {
                        Id = "m2", Text = "들어가 보니 이미 쓰러져 있었어요. 맥을 짚다가 피가 묻었고, 무서워서 도망쳤어요.",
                        UnlockAfter = "m1", PressReward = "c_stairs_man", ClarityGain = 15,
                        PressLines = new[]
                        {
                            L("강무진", "아, 그러고 보니... 들어갈 때 계단으로 누가 내려갔어요."),
                            L("강무진", "얼굴은 못 봤어요. 근데 긴 우산을 들고 있었어요. 흠뻑 젖은 우산이요.")
                        }
                    },
                    new Statement
                    {
                        Id = "m3", Text = "라이터요? 그건 늘 경비실 서랍에 둬요. 집에 들고 다닌 적 없어요.",
                        BreakCard = "c_drawer", ClarityGain = 20,
                        BreakLines = new[]
                        {
                            L("강무진", "서랍이 비었다고요? 제가 도망칠 때도 분명 거기 있었어요."),
                            L("강무진", "그날 밤 경비실에 누가 왔다 갔대요. 형사님들 중 한 분이라던데."),
                            Think("(우리 쪽 사람? ...반장인가.)")
                        }
                    }
                }
            },
            new Npc
            {
                Id = "hangyeol", Name = "서한결", Room = "station", X = 1.5f, StartClarity = 10,
                Greeting = new[] { L("서한결", "형사님. 범인 잡으셨다면서요. 세라 장례식이 내일입니다. 빨리 끝내 주세요.") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "h1", Text = "그 시간엔 생방송 중이었습니다. 수천 명이 들었어요.",
                        BreakCard = "d_not_live", ClarityGain = 25,
                        BreakLines = new[]
                        {
                            L("서한결", "...녹음이었습니다. 세라가 몸이 안 좋다고 해서요."),
                            L("서한결", "그래도 전 그 시간에 방송국에 있었어요. 그건 사실입니다.")
                        }
                    },
                    new Statement
                    {
                        Id = "h2", Text = "전 그날 밤 방송국 밖으로 나간 적 없습니다.",
                        UnlockAfter = "h1", BreakCard = "d_outside", ClarityGain = 25,
                        BreakLines = new[]
                        {
                            L("서한결", "......세라를 만나러 갔었습니다."),
                            L("서한결", "하지만 제가 나올 땐 살아 있었어요! 정말입니다!")
                        }
                    },
                    new Statement
                    {
                        Id = "h3", Text = "세라와는 아무 문제 없었습니다. 결혼 이야기까지 했어요.",
                        UnlockAfter = "h2", BreakCard = "d_motive", ClarityGain = 40, Flag = "hangyeol_confessed",
                        BreakLines = new[]
                        {
                            L("서한결", "그 장부가 방송에 나가면 방송국도, 저도 끝이에요."),
                            L("서한결", "말렸어요. 세라는 듣지 않았어요. 밀쳤을 뿐이에요... 탁자 모서리에 부딪칠 줄은..."),
                            L("서한결", "녹음기를 챙겨서 계단으로 내려왔어요. 그때 경비원이랑 스쳤고요.")
                        }
                    },
                    new Statement
                    {
                        Id = "h4", Text = "구두요? 270 은 흔한 사이즈죠.",
                        BreakCard = "c_memo_270", ClarityGain = 10,
                        BreakLines = new[] { L("서한결", "......"), Think("(대답하지 못한다.)") }
                    }
                }
            },
            new Npc
            {
                Id = "jihye", Name = "문지혜", Room = "apartment", X = -3.5f, StartClarity = 20,
                Greeting = new[] { L("문지혜", "아, 형사님... 오셨어요.") , Think("(눈을 마주치지 않는다.)") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "j1", Text = "현장 기록은 전부 보고서에 있어요.",
                        BreakCard = "c_footprints", BreakReward = "c_memo_270", ClarityGain = 20,
                        BreakLines = new[]
                        {
                            L("문지혜", "...270mm 족적이 하나 더 있었어요. 피를 밟은 구두 자국이요."),
                            L("문지혜", "반장님이 보고서에서 빼라고 하셨어요. 경비원 발은 255 라서 헷갈린다고요.")
                        }
                    },
                    new Statement
                    {
                        Id = "j2", Text = "창문은 제가 도착했을 때부터 열려 있었어요.",
                        BreakCard = "c_window", ClarityGain = 15,
                        PressLines = new[] { L("문지혜", "누가 열었는지는... 말 못 해요.") },
                        BreakLines = new[]
                        {
                            L("문지혜", "먼저 와 계시던 분이 여셨어요. 환기시킨다고... 술 냄새가 많이 나서."),
                            L("문지혜", "아니에요. 잊어 주세요. 제가 말했다고 하지 마세요.")
                        }
                    },
                    new Statement
                    {
                        Id = "j3", Text = "감식반이 오기 전에 현장에 들어온 사람은 없어요.",
                        BreakCard = "c_photo", BreakReward = "c_coat_man", ClarityGain = 20,
                        BreakLines = new[]
                        {
                            L("문지혜", "...기자 말고, 한 분 더 계셨어요. 코트 입은 분이요."),
                            L("문지혜", "그분이 탁자 쪽에 서 계셨어요. 형사님도... 그 코트 아시잖아요."),
                            Think("(코트 입은 형사는 강력반에 수두룩하다.)")
                        }
                    }
                }
            },
            new Npc
            {
                Id = "chorong", Name = "이초롱", Room = "store", X = 6f, StartClarity = 30,
                Greeting = new[] { L("이초롱", "어서 오세요~ 아, 형사님이시구나. 오늘은 그거 안 사가세요?"), Think("(무슨 소리지.)") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "o1", Text = "경비원 아저씨가 피 묻은 채로 뛰어나가는 걸 봤어요. 확실해요.",
                        BreakCard = "c_rain_receipt", ClarityGain = 20,
                        BreakLines = new[]
                        {
                            L("이초롱", "...사실 비 때문에 앞이 잘 안 보였어요."),
                            L("이초롱", "경비원 옷이라서 그 아저씨라고 생각한 거예요. 얼굴은 못 봤어요.")
                        }
                    },
                    new Statement
                    {
                        Id = "o2", Text = "그날 밤엔 손님이 거의 없었어요.",
                        PressReward = "c_soju", ClarityGain = 15,
                        PressLines = new[]
                        {
                            L("이초롱", "아, 코트 입은 손님이 한 분 계셨어요. 11시 20분쯤 소주 두 병."),
                            L("이초롱", "형사님이랑 체격이 비슷했는데... 에이, 설마요. 하하."),
                            Think("(그 시간에 나는 집에 있었다. ...아마도.)")
                        }
                    }
                }
            },
            new Npc
            {
                Id = "reporter", Name = "박진표", Room = "store", X = -1.5f, StartClarity = 20,
                Greeting = new[] { L("박진표", "오, 강력반 최 형사. 경비원 놈 언제 송치해? 내일 1면 비워놨는데.") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "r1", Text = "난 현장 근처에도 안 갔어. 경찰 발표 받아 쓴 거야.",
                        BreakCard = "c_footprints", BreakReward = "c_photo", ClarityGain = 25, Flag = "deal",
                        BreakLines = new[]
                        {
                            L("박진표", "...그래, 몇 장 찍었어. 특종이잖아."),
                            L("박진표", "원본 줄게. 대신 하나만 줘. 그 경비원 딸 이야기. 독자들이 울 만한 걸로.")
                        }
                    },
                    new Statement
                    {
                        Id = "r2", Text = "경비원이 범인이야. 독자들이 그걸 원하거든.",
                        ClarityGain = 5,
                        PressLines = new[] { L("박진표", "진실? 진실은 잘 안 팔려, 형사 양반. 사람들은 이야기를 사.") }
                    }
                }
            },
            new Npc
            {
                Id = "chief", Name = "오태식", Room = "police", X = -3f, StartClarity = 30,
                Greeting = new[] { L("오태식", "아직도 거기 서 있나? 송치 서류나 마무리해.") },
                Statements = new[]
                {
                    new Statement
                    {
                        Id = "b1", Text = "증거는 충분해. 라이터, 피, 전과. 뭘 더 원해?",
                        ClarityGain = 5,
                        PressLines = new[]
                        {
                            L("오태식", "라이터 하나면 끝이야. 그거 누가 챙겨왔는지 자네가 제일 잘 알 텐데."),
                            Think("(감식반이 찾았겠지. 무슨 뜻이지.)")
                        }
                    },
                    new Statement
                    {
                        Id = "b2", Text = "현장 보존은 아무 문제 없었어.",
                        BreakCard = "c_window", ClarityGain = 15,
                        BreakLines = new[] { L("오태식", "...창문? 그걸 왜 나한테 묻나. 제일 먼저 도착한 사람한테 물어봐.") }
                    },
                    new Statement
                    {
                        Id = "b3", Text = "지혜 그 친구 메모? 그런 건 없어.",
                        BreakCard = "c_memo_270", BreakReward = "c_cover_up", ClarityGain = 30,
                        BreakLines = new[]
                        {
                            L("오태식", "빨리 끝내야 했어. 위에서 압박이 얼마나 심한지 알아?"),
                            L("오태식", "그리고... 자네를 위해서이기도 했고."),
                            Think("(나를 위해서? 무슨 소리지.)")
                        }
                    }
                }
            },
            new Npc
            {
                Id = "sua", Name = "강수아", Room = "police", X = 1.2f, StartClarity = 60, IsTalkOnly = true,
                Greeting = new[] { L("강수아", "아저씨 형사죠? 우리 아빠... 진짜 나쁜 사람이에요?") }
            }
        };

        public static Npc GetNpc(string id)
        {
            foreach (Npc npc in Npcs)
            {
                if (npc.Id == id) return npc;
            }
            return null;
        }
    }
}

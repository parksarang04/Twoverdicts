using System;
using System.Collections.Generic;
using TwoVerdicts.V2.Core;
using TwoVerdicts.V2.Data;
using TwoVerdicts.V2.UI;
using TwoVerdicts.V2.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TwoVerdicts.V2
{
    /// <summary>
    /// 회색 증언(두 개의 판결 v2) 진입점. 씬에 이 컴포넌트 하나만 있으면
    /// 월드, UI, 상태를 전부 코드로 만들어 게임을 진행한다.
    /// </summary>
    public partial class V2Game : MonoBehaviour
    {
        private const float CameraSize = 5.625f;

        private GameSession session;
        private WorldView world;
        private HudView hud;
        private GameObject blackout;
        private DialogueView dialogue;
        private ChoiceView choice;
        private CardPickerView picker;
        private InterviewView interview;
        private DeductionView deduction;
        private MapView map;
        private TrialView trial;
        private EndingView ending;
        private Fader fader;
        private BlipAudio blip;
        private SoundManager sound;
        private TitleView titleView;
        private SettingsView settings;
        private PauseView pause;
        private HintTreeView tree;
        private IntroVideoView introVideo;
        private DevCheatView cheat;
        private bool gameOver;
        private bool inFrame;

        private void Start()
        {
            try
            {
                InputLock.Reset();
                session = new GameSession();
                SetupCamera();
                BuildWorld();
                BuildUi();
                Subscribe();
                ShowTitle();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        private static void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null) cam = new GameObject("Main Camera") { tag = "MainCamera" }.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = CameraSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.transform.position = new Vector3(0f, 0f, -10f);
        }

        private void BuildWorld()
        {
            world = new GameObject("World").AddComponent<WorldView>();
            world.Build(session);
            blip = gameObject.AddComponent<BlipAudio>();
            sound = gameObject.AddComponent<SoundManager>();
        }

        private void BuildUi()
        {
            EnsureEventSystem();
            Transform canvas = CreateCanvas();
            hud = HudView.Create(canvas);
            blackout = Ui.Stretch("Blackout", canvas).gameObject;
            Ui.Image((RectTransform)blackout.transform, null, Ui.Ink, true);
            interview = InterviewView.Create(canvas);
            deduction = DeductionView.Create(canvas);
            map = MapView.Create(canvas);
            trial = TrialView.Create(canvas);
            picker = CardPickerView.Create(canvas);
            choice = ChoiceView.Create(canvas);
            dialogue = DialogueView.Create(canvas);
            ending = EndingView.Create(canvas);
            tree = HintTreeView.Create(canvas);
            pause = PauseView.Create(canvas);
            titleView = TitleView.Create(canvas);
            settings = SettingsView.Create(canvas);
            introVideo = IntroVideoView.Create(canvas);
            cheat = DevCheatView.Create(canvas);
            fader = Fader.Create(canvas);
            settings.OnPreviewSfx = () => sound.Sfx("sfx_card", 0.6f);
            Ui.OnAnyClick = () => sound.Sfx("sfx_click", 0.4f);
            fader.SetBlack(false);
            dialogue.PortraitFor = PortraitForSpeaker;
            dialogue.OnBlip = blip.Play;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        private static Transform CreateCanvas()
        {
            var go = new GameObject("Canvas", typeof(RectTransform)) { layer = 5 };
            go.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Ui.W, Ui.H);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return go.transform;
        }

        private void Subscribe()
        {
            session.OnCardAdded += card =>
            {
                hud.Toast($"[{(card.Kind == CardKind.Deduction ? "추리" : "단서")}] {card.Name}");
                sound.Sfx("sfx_card");
            };
            world.Player.OnStep += () => sound.Sfx("sfx_step", 0.45f);
            world.SetNpcHot(CanProgress);
            session.OnClarityChanged += HandleClarity;
            session.OnActionsChanged += hud.SetActions;
            world.Player.OnFocusChanged += item => hud.SetPrompt(item != null ? item.Label : null);
            world.Player.OnUse += HandleUse;
            hud.SetActions(session.Actions);
        }

        private void HandleClarity(string id, int value)
        {
            world.RefreshActors();
            if (id == StoryDb.PlayerId) return;
            Npc npc = StoryDb.GetNpc(id);
            if (npc != null) hud.Toast($"{npc.Name}의 모습이 선명해졌다 ({value}%)");
        }

        private void Update()
        {
            if (gameOver || !InputLock.IsFree) return;
            if (InputLock.Pressed(Key.Q)) OpenNotebook();
            else if (InputLock.Pressed(Key.Tab)) OpenDeduction();
            else if (InputLock.Pressed(Key.T)) OpenTree();
            else if (InputLock.Pressed(Key.Escape)) OpenPause();
            else if (started && IsCheatKey()) OpenCheat();
        }

        /// <summary>노트북은 F1 이 밝기/음소거라서 F1, F12, ` (숫자 1 왼쪽 키) 모두 받음.</summary>
        private static bool IsCheatKey()
        {
            return InputLock.Pressed(Key.F1) || InputLock.Pressed(Key.F12) || InputLock.Pressed(Key.Backquote);
        }

        private void OpenNotebook()
        {
            sound.Sfx("sfx_paper");
            picker.Show("수첩", session.CardIds, null);
        }

        private Sprite PortraitForSpeaker(string speaker)
        {
            if (speaker == StoryDb.Me) return Art.Portrait(inFrame ? "yunseok_old" : StoryDb.PlayerId, PlayerStage());
            if (speaker == "윤세라") return Art.Portrait("seira", 2);
            if (inFrame && speaker == "강수아") return Art.Portrait("sua_adult", 3);
            foreach (Npc npc in StoryDb.Npcs)
            {
                if (npc.Name == speaker) return Art.Portrait(npc.Id, Clarity.StageOf(session.GetClarity(npc.Id)));
            }
            return null;
        }

        private int PlayerStage() => inFrame ? 3 : Clarity.StageOf(session.GetClarity(StoryDb.PlayerId));

        private static Line Think(string text) => new Line(null, text);

        private static Line Say(string speaker, string text) => new Line(speaker, text);
    }
}

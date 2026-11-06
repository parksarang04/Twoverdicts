using System;
using System.Collections.Generic;
using TwoVerdicts.Core;
using TwoVerdicts.Data;
using TwoVerdicts.Ending;
using TwoVerdicts.Intro;
using TwoVerdicts.Investigation;
using TwoVerdicts.Trial;
using TwoVerdicts.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// Main 씬 전체(카메라, 캔버스, 모든 UI, 연결)를 코드로 생성.
    /// 메뉴: Tools/TwoVerdicts/Build Scene
    /// </summary>
    public static partial class SceneBuilder
    {
        public const string ScenePath = "Assets/TwoVerdicts/Scenes/Main.unity";
        private const string DataFolder = "Assets/TwoVerdicts/Data";

        private class SceneRefs
        {
            public GameObject InvestigationRoot;
            public InvestigationController Investigation;
            public LocationNavigator Navigator;
            public Button GoToCourt;
            public readonly List<EvidenceHotspot> Hotspots = new List<EvidenceHotspot>();
            public HudUI Hud;
            public TrialController Trial;
            public EndingController Ending;
            public IntroVideoController Intro;
            public DialogueUI Dialogue;
        }

        [MenuItem("Tools/TwoVerdicts/Build Scene")]
        public static void BuildFromMenu()
        {
            try
            {
                Build();
            }
            catch (Exception e)
            {
                SetupLog.Exception(e);
            }
        }

        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                SetupLog.Error("현재 씬 저장이 취소되어 씬 생성을 중단함");
                return;
            }
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            UIFactory.Font = FontSetup.Load();
            CreateCamera();
            CreateEventSystem();
            SceneRefs refs = BuildCanvas(CreateCanvas());
            CreateGameManager(refs);
            SaveScene(scene);
        }

        private static SceneRefs BuildCanvas(Transform canvas)
        {
            var refs = new SceneRefs();
            CreateBackdrop(canvas);
            BuildInvestigation(canvas, refs);
            BuildHud(canvas, refs);
            BuildTrial(canvas, refs);
            BuildEnding(canvas, refs);
            BuildIntro(canvas, refs);
            BuildDialogue(canvas, refs);
            WireInvestigation(refs);
            return refs;
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.transform.position = new Vector3(0f, 0f, -10f);
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.orthographic = true;
            go.AddComponent<AudioListener>();
        }

        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            var module = go.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }

        private static Transform CreateCanvas()
        {
            var go = new GameObject("Canvas", typeof(RectTransform)) { layer = 5 };
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(UIFactory.ScreenW, UIFactory.ScreenH);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return go.transform;
        }

        private static void CreateBackdrop(Transform canvas)
        {
            RectTransform rt = UIFactory.Stretch("Backdrop", canvas);
            Image image = UIFactory.AddImage(rt, UIFactory.LoadSprite("Backgrounds/key_art_scales"),
                new Color(0.35f, 0.35f, 0.35f, 1f));
            image.raycastTarget = false;
        }

        private static void CreateGameManager(SceneRefs refs)
        {
            var gm = new GameObject("GameManager").AddComponent<GameManager>();
            Wire.Ref(gm, "caseData", UIFactory.LoadAsset<CaseData>($"{DataFolder}/Case_Conner.asset"));
            Wire.RefList(gm, "endings", LoadEndings());
            Wire.Ref(gm, "introVideo", refs.Intro);
            Wire.Ref(gm, "dialogue", refs.Dialogue);
            Wire.Ref(gm, "hud", refs.Hud);
            Wire.Ref(gm, "investigationRoot", refs.InvestigationRoot);
            Wire.Ref(gm, "investigation", refs.Investigation);
            Wire.Ref(gm, "trial", refs.Trial);
            Wire.Ref(gm, "ending", refs.Ending);
        }

        private static List<EndingData> LoadEndings()
        {
            var endings = new List<EndingData>();
            foreach (EndingType type in Enum.GetValues(typeof(EndingType)))
            {
                endings.Add(UIFactory.LoadAsset<EndingData>($"{DataFolder}/Endings/Ending_{type}.asset"));
            }
            return endings;
        }

        private static void SaveScene(Scene scene)
        {
            if (!AssetDatabase.IsValidFolder("Assets/TwoVerdicts/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets/TwoVerdicts", "Scenes");
            }
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new InvalidOperationException("씬 저장 실패: " + ScenePath);
            }
            var scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path == ScenePath) continue;
                existing.enabled = false;
                scenes.Add(existing);
            }
            EditorBuildSettings.scenes = scenes.ToArray();
            SetupLog.Info("씬 저장 + 빌드 목록 등록: " + ScenePath);
        }
    }
}

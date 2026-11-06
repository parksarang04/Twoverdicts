using System;
using System.Collections.Generic;
using System.IO;
using TwoVerdicts.V2;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// v2(회색 증언) 씬 생성. 카메라 + EventSystem + V2Game 하나만 두고 나머지는 런타임에 코드로 만든다.
    /// .autosetup_v2 파일이 있으면 컴파일 직후 한 번 자동 실행.
    /// </summary>
    [InitializeOnLoad]
    public static class V2SceneBuilder
    {
        public const string ScenePath = "Assets/TwoVerdicts/Scenes/V2_GrayTestimony.unity";
        private const string FlagPath = "Assets/TwoVerdicts/Scripts/Editor/.autosetup_v2";

        static V2SceneBuilder()
        {
            if (File.Exists(FlagPath)) EditorApplication.delayCall += RunFromFlag;
        }

        private static void RunFromFlag()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RunFromFlag;
                return;
            }
            if (!File.Exists(FlagPath) || EditorApplication.isPlayingOrWillChangePlaymode) return;
            try
            {
                ArtImportSettings.ApplyAll();
                Build();
                File.Delete(FlagPath);
                SetupLog.Info("v2 자동 세팅 완료 - " + ScenePath);
            }
            catch (Exception e)
            {
                SetupLog.Exception(e);
            }
        }

        [MenuItem("Tools/TwoVerdicts/Build V2 Scene (회색 증언)")]
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
                SetupLog.Error("현재 씬 저장이 취소되어 v2 씬 생성을 중단함");
                return;
            }
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            CreateEventSystem();
            new GameObject("V2Game").AddComponent<V2Game>();
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("씬 저장 실패: " + ScenePath);
            PutFirstInBuild();
            SetupLog.Info("v2 씬 저장 + 빌드 목록 1번 등록: " + ScenePath);
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            go.transform.position = new Vector3(0f, 0f, -10f);
            var cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.625f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            go.AddComponent<AudioListener>();
        }

        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        private static void PutFirstInBuild()
        {
            var scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path == ScenePath) continue;
                scenes.Add(existing);
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}

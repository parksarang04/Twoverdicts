using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 처음 한 번 자동으로 전체 세팅을 실행.
    /// (1) TMP 기본 리소스 임포트 (2) 아트 임포트 설정 (3) 데이터 에셋 (4) 한글 폰트 (5) Main 씬 생성.
    /// .autosetup 파일이 있을 때만 자동 실행되고, 끝나면 파일을 지움.
    /// 다시 돌리고 싶으면 메뉴 Tools/TwoVerdicts/Setup Everything.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoSetup
    {
        private const string FlagPath = "Assets/TwoVerdicts/Scripts/Editor/.autosetup";
        private static bool isRunning;

        static AutoSetup()
        {
            if (File.Exists(FlagPath)) EditorApplication.delayCall += RunFromFlag;
        }

        private static void RunFromFlag()
        {
            if (File.Exists(FlagPath)) Run();
        }

        [MenuItem("Tools/TwoVerdicts/Setup Everything")]
        public static void Run()
        {
            if (isRunning) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Run;
                return;
            }
            try
            {
                isRunning = true;
                if (!TmpEssentials.IsImported())
                {
                    TmpEssentials.Import(Run);
                    return;
                }
                BuildEverything();
            }
            catch (Exception e)
            {
                SetupLog.Exception(e);
            }
            finally
            {
                isRunning = false;
            }
        }

        private static void BuildEverything()
        {
            SetupLog.Info("자동 세팅 시작");
            ArtImportSettings.ApplyAll();
            CaseAssetBuilder.BuildAll();
            FontSetup.CreateOrLoad(CollectTexts());
            SceneBuilder.Build();
            if (File.Exists(FlagPath)) File.Delete(FlagPath);
            SetupLog.Info("자동 세팅 완료 - " + SceneBuilder.ScenePath);
        }

        private static IEnumerable<string> CollectTexts()
        {
            var texts = new List<string>(CaseContent.Intro);
            texts.AddRange(CaseContent.BetweenTrials);
            texts.Add(CaseContent.CaseTitle);
            texts.Add(CaseContent.CriminalStandard);
            texts.Add(CaseContent.CivilStandard);
            foreach (CaseContent.EvidenceDef e in CaseContent.Evidence)
            {
                texts.Add(e.Title + e.Surface + e.Deep);
            }
            foreach (CaseContent.EndingDef e in CaseContent.Endings)
            {
                texts.Add(e.Title + e.Body + e.Question);
            }
            foreach (SceneLayout.LocationDef l in SceneLayout.Locations) texts.Add(l.DisplayName);
            texts.AddRange(SceneLayout.UiTexts);
            return texts;
        }
    }
}

using System;
using System.Collections.Generic;
using TwoVerdicts.Data;
using UnityEditor;
using UnityEngine;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 메뉴 Tools/TwoVerdicts/Build Case Assets 한 번 누르면
    /// 증거 8개 + 엔딩 5개 + 사건 데이터 에셋을 자동 생성.
    /// </summary>
    public static class CaseAssetBuilder
    {
        private const string RootFolder = "Assets/TwoVerdicts";
        private const string DataFolder = RootFolder + "/Data";
        private const string EvidenceFolder = DataFolder + "/Evidence";
        private const string EndingFolder = DataFolder + "/Endings";

        [MenuItem("Tools/TwoVerdicts/Build Case Assets")]
        public static void BuildAll()
        {
            try
            {
                EnsureFolders();
                List<EvidenceData> evidence = BuildEvidence();
                BuildEndings();
                BuildCase(evidence);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("[CaseAssetBuilder] 에셋 생성 완료: " + DataFolder);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void EnsureFolders()
        {
            CreateFolderIfMissing("Assets", "TwoVerdicts");
            CreateFolderIfMissing(RootFolder, "Data");
            CreateFolderIfMissing(DataFolder, "Evidence");
            CreateFolderIfMissing(DataFolder, "Endings");
        }

        private static void CreateFolderIfMissing(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static List<EvidenceData> BuildEvidence()
        {
            var result = new List<EvidenceData>();
            foreach (CaseContent.EvidenceDef def in CaseContent.Evidence)
            {
                var asset = LoadOrCreate<EvidenceData>($"{EvidenceFolder}/Evidence_{def.Id}.asset");
                var so = new SerializedObject(asset);
                SetString(so, "evidenceId", def.Id);
                SetString(so, "title", def.Title);
                SetString(so, "surfaceText", def.Surface);
                SetString(so, "deepText", def.Deep);
                FindOrThrow(so, "category").enumValueIndex = (int)def.Category;
                FindOrThrow(so, "guiltWeight").intValue = def.Weight;
                FindOrThrow(so, "isTainted").boolValue = def.Tainted;
                AssignIconIfExists(so, def.Id);
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
                result.Add(asset);
            }
            return result;
        }

        private static void AssignIconIfExists(SerializedObject so, string evidenceId)
        {
            // 아트 zip 의 Icons 폴더를 Assets/TwoVerdicts/Art/Icons 에 넣고 Sprite 로 임포트했다면 자동 연결
            string path = $"{RootFolder}/Art/Icons/icon_{evidenceId}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogWarning($"[CaseAssetBuilder] 아이콘 없음(나중에 수동 연결): {path}");
                return;
            }
            FindOrThrow(so, "icon").objectReferenceValue = sprite;
        }

        private static void BuildEndings()
        {
            foreach (CaseContent.EndingDef def in CaseContent.Endings)
            {
                var asset = LoadOrCreate<EndingData>($"{EndingFolder}/Ending_{def.Type}.asset");
                var so = new SerializedObject(asset);
                FindOrThrow(so, "endingType").enumValueIndex = (int)def.Type;
                SetString(so, "title", def.Title);
                SetString(so, "body", def.Body);
                SetString(so, "finalQuestion", def.Question);
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
            }
        }

        private static void BuildCase(List<EvidenceData> evidence)
        {
            var asset = LoadOrCreate<CaseData>($"{DataFolder}/Case_Conner.asset");
            var so = new SerializedObject(asset);
            SetString(so, "caseTitle", CaseContent.CaseTitle);
            SetStringList(so, "introLines", CaseContent.Intro);
            SetStringList(so, "betweenTrialsLines", CaseContent.BetweenTrials);
            SetObjectList(so, "allEvidence", evidence);
            FindOrThrow(so, "maxActions").intValue = CaseContent.MaxActions;
            FindOrThrow(so, "startCrowdGuilt").intValue = CaseContent.StartCrowdGuilt;
            SetString(so, "criminalStandardText", CaseContent.CriminalStandard);
            SetString(so, "civilStandardText", CaseContent.CivilStandard);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static SerializedProperty FindOrThrow(SerializedObject so, string field)
        {
            SerializedProperty prop = so.FindProperty(field);
            if (prop == null)
            {
                throw new InvalidOperationException($"필드를 찾을 수 없음: {so.targetObject.GetType().Name}.{field}");
            }
            return prop;
        }

        private static void SetString(SerializedObject so, string field, string value)
        {
            FindOrThrow(so, field).stringValue = value;
        }

        private static void SetStringList(SerializedObject so, string field, IReadOnlyList<string> values)
        {
            SerializedProperty list = FindOrThrow(so, field);
            list.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                list.GetArrayElementAtIndex(i).stringValue = values[i];
            }
        }

        private static void SetObjectList(SerializedObject so, string field, IReadOnlyList<EvidenceData> values)
        {
            SerializedProperty list = FindOrThrow(so, field);
            list.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }
    }
}

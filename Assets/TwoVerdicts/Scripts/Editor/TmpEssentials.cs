using System;
using System.IO;
using UnityEditor;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// TextMeshPro 기본 리소스(셰이더, 설정)를 창 없이 임포트.
    /// </summary>
    public static class TmpEssentials
    {
        private const string SettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        private const string UguiPackageJson = "Packages/com.unity.ugui/package.json";

        private static Action onDone;

        public static bool IsImported()
        {
            return File.Exists(SettingsPath);
        }

        public static void Import(Action done)
        {
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(UguiPackageJson);
            if (info == null) throw new InvalidOperationException("com.unity.ugui 패키지를 찾을 수 없음");
            string package = Path.Combine(info.resolvedPath, "Package Resources", "TMP Essential Resources.unitypackage");
            if (!File.Exists(package)) throw new FileNotFoundException("TMP Essential Resources 없음", package);

            onDone = done;
            AssetDatabase.importPackageCompleted += HandleCompleted;
            AssetDatabase.importPackageFailed += HandleFailed;
            SetupLog.Info("TMP Essential Resources 임포트 시작");
            AssetDatabase.ImportPackage(package, false);
        }

        private static void HandleCompleted(string packageName)
        {
            Unsubscribe();
            SetupLog.Info("TMP Essential Resources 임포트 완료: " + packageName);
            Action callback = onDone;
            onDone = null;
            EditorApplication.delayCall += () => callback?.Invoke();
        }

        private static void HandleFailed(string packageName, string error)
        {
            Unsubscribe();
            onDone = null;
            SetupLog.Error($"TMP 임포트 실패 ({packageName}): {error}");
        }

        private static void Unsubscribe()
        {
            AssetDatabase.importPackageCompleted -= HandleCompleted;
            AssetDatabase.importPackageFailed -= HandleFailed;
        }
    }
}

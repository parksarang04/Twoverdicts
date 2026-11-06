using System.Collections.Generic;
using TMPro;
using TwoVerdicts.Data;
using TwoVerdicts.Investigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// 조사 화면: 장소 4곳, 단서 핫스팟 8개, 장소 이동 바, 힌트, 법정으로 버튼.
    /// </summary>
    public static partial class SceneBuilder
    {
        private const string HintText = "빨간 표시를 누르면 조사, 같은 표시를 다시 누르면 내막. < > 로 장소 이동";

        private static void BuildInvestigation(Transform canvas, SceneRefs refs)
        {
            RectTransform root = UIFactory.Stretch("Investigation", canvas);
            refs.InvestigationRoot = root.gameObject;
            refs.Navigator = root.gameObject.AddComponent<LocationNavigator>();
            refs.Investigation = root.gameObject.AddComponent<InvestigationController>();

            var panels = new List<GameObject>();
            foreach (SceneLayout.LocationDef def in SceneLayout.Locations)
            {
                panels.Add(BuildLocation(root, def, refs.Hotspots));
            }
            for (int i = 1; i < panels.Count; i++) panels[i].SetActive(false);

            BuildNavBar(root, refs.Navigator, panels);
            BuildInvestigationFooter(root, refs);
        }

        private static GameObject BuildLocation(Transform root, SceneLayout.LocationDef def,
            List<EvidenceHotspot> hotspots)
        {
            RectTransform panel = UIFactory.Stretch(def.ObjectName, root);
            Image bg = UIFactory.AddImage(panel, UIFactory.LoadSprite("Backgrounds/" + def.Background), Color.white);
            bg.raycastTarget = false;
            foreach (SceneLayout.HotspotDef hotspot in def.Hotspots)
            {
                hotspots.Add(BuildHotspot(panel, hotspot));
            }
            return panel.gameObject;
        }

        private static EvidenceHotspot BuildHotspot(Transform panel, SceneLayout.HotspotDef def)
        {
            RectTransform rt = UIFactory.Place("Hotspot_" + def.EvidenceId, panel, def.Position, new Vector2(72f, 72f));
            Image marker = UIFactory.AddImage(rt, UIFactory.LoadSprite("UI/ui_hotspot_marker"), UIFactory.Red);
            var hotspot = rt.gameObject.AddComponent<EvidenceHotspot>();
            var button = rt.GetComponent<Button>();
            button.targetGraphic = marker;
            button.transition = Selectable.Transition.None;

            string path = $"{DataFolder}/Evidence/Evidence_{def.EvidenceId}.asset";
            Wire.Ref(hotspot, "evidence", UIFactory.LoadAsset<EvidenceData>(path));
            Wire.Ref(hotspot, "markerImage", marker);
            return hotspot;
        }

        private static void BuildNavBar(Transform root, LocationNavigator nav, List<GameObject> panels)
        {
            UIFactory.Panel(root, "NavBar", new Vector2(250f, 52f), new Vector2(460f, 76f));
            Button prev = UIFactory.Button(root, "PrevButton", "<", new Vector2(64f, 52f), new Vector2(64f, 56f), UIFactory.Paper);
            Button next = UIFactory.Button(root, "NextButton", ">", new Vector2(436f, 52f), new Vector2(64f, 56f), UIFactory.Paper);
            TextMeshProUGUI label = UIFactory.Label(root, "LocationName", SceneLayout.Locations[0].DisplayName,
                new Vector2(250f, 52f), new Vector2(290f, 56f), 26f, UIFactory.Paper, TextAlignmentOptions.Center);

            Wire.Ref(nav, "prevButton", prev);
            Wire.Ref(nav, "nextButton", next);
            Wire.Ref(nav, "locationNameText", label);
            WireLocations(nav, panels);
        }

        private static void WireLocations(LocationNavigator nav, List<GameObject> panels)
        {
            var so = new SerializedObject(nav);
            SerializedProperty list = Wire.Find(so, "locations");
            list.arraySize = panels.Count;
            for (int i = 0; i < panels.Count; i++)
            {
                SerializedProperty entry = list.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("displayName").stringValue = SceneLayout.Locations[i].DisplayName;
                entry.FindPropertyRelative("panel").objectReferenceValue = panels[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildInvestigationFooter(Transform root, SceneRefs refs)
        {
            UIFactory.Panel(root, "HintBar", new Vector2(470f, 684f), new Vector2(900f, 52f));
            UIFactory.Label(root, "Hint", HintText, new Vector2(470f, 684f), new Vector2(870f, 40f),
                18f, UIFactory.Gray, TextAlignmentOptions.Center);
            refs.GoToCourt = UIFactory.Button(root, "GoToCourtButton", "법정으로",
                new Vector2(1136f, 676f), new Vector2(240f, 64f), UIFactory.Red);
        }

        private static void WireInvestigation(SceneRefs refs)
        {
            Wire.Ref(refs.Investigation, "dialogue", refs.Dialogue);
            Wire.Ref(refs.Investigation, "navigator", refs.Navigator);
            Wire.RefList(refs.Investigation, "hotspots", refs.Hotspots);
            Wire.Ref(refs.Investigation, "goToCourtButton", refs.GoToCourt);
        }
    }
}

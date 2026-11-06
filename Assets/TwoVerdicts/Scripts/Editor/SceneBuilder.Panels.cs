using TMPro;
using TwoVerdicts.Ending;
using TwoVerdicts.Intro;
using TwoVerdicts.Trial;
using TwoVerdicts.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TwoVerdicts.EditorTools
{
    /// <summary>
    /// HUD, 재판, 엔딩, 인트로 영상, 대사창.
    /// 각 컨트롤러는 항상 켜진 홀더에 붙이고, 켜고 끌 패널(root)은 자식으로 둠.
    /// </summary>
    public static partial class SceneBuilder
    {
        private const string VideoPath = "Assets/TwoVerdicts/Video/intro_news.mp4";
        private const TextAlignmentOptions Center = TextAlignmentOptions.Center;

        private static void BuildHud(Transform canvas, SceneRefs refs)
        {
            RectTransform holder = UIFactory.Stretch("Hud", canvas);
            refs.Hud = holder.gameObject.AddComponent<HudUI>();
            RectTransform root = UIFactory.Stretch("HudPanel", holder);
            UIFactory.Panel(root, "Box", new Vector2(1060f, 70f), new Vector2(380f, 112f));
            TextMeshProUGUI actions = UIFactory.Label(root, "ActionsText", "남은 조사 8",
                new Vector2(1060f, 40f), new Vector2(340f, 32f), 24f, UIFactory.Paper, Center);
            TextMeshProUGUI crowd = UIFactory.Label(root, "CrowdText", "여론: 유죄 50%",
                new Vector2(1060f, 74f), new Vector2(340f, 28f), 18f, UIFactory.Gray, Center);
            Slider slider = BuildSlider(root, new Vector2(1060f, 102f), new Vector2(330f, 12f));

            Wire.Ref(refs.Hud, "root", root.gameObject);
            Wire.Ref(refs.Hud, "actionsText", actions);
            Wire.Ref(refs.Hud, "crowdText", crowd);
            Wire.Ref(refs.Hud, "crowdSlider", slider);
        }

        private static Slider BuildSlider(Transform parent, Vector2 center, Vector2 size)
        {
            RectTransform rt = UIFactory.Place("CrowdSlider", parent, center, size);
            UIFactory.AddImage(rt, null, new Color(0.3f, 0.3f, 0.3f, 1f)).raycastTarget = false;
            RectTransform area = UIFactory.Stretch("Fill Area", rt);
            RectTransform fill = UIFactory.Stretch("Fill", area);
            UIFactory.AddImage(fill, null, UIFactory.Red).raycastTarget = false;

            var slider = rt.gameObject.AddComponent<Slider>();
            slider.fillRect = fill;
            slider.direction = Slider.Direction.LeftToRight;
            slider.transition = Selectable.Transition.None;
            slider.interactable = false;
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 50f;
            return slider;
        }

        private static void BuildTrial(Transform canvas, SceneRefs refs)
        {
            RectTransform holder = UIFactory.Stretch("Trial", canvas);
            refs.Trial = holder.gameObject.AddComponent<TrialController>();
            RectTransform root = UIFactory.Stretch("TrialPanel", holder);
            UIFactory.AddImage(root, UIFactory.LoadSprite("Backgrounds/bg_court"), new Color(0.35f, 0.35f, 0.35f, 1f));

            var header = UIFactory.Label(root, "Header", "1부 - 형사재판", new Vector2(640f, 64f),
                new Vector2(800f, 60f), 40f, UIFactory.Paper, Center);
            UIFactory.Panel(root, "StandardBox", new Vector2(340f, 330f), new Vector2(560f, 300f));
            var standard = UIFactory.Label(root, "StandardText", "", new Vector2(340f, 330f),
                new Vector2(500f, 250f), 24f, UIFactory.Paper, Center);
            UIFactory.Panel(root, "SummaryBox", new Vector2(940f, 330f), new Vector2(560f, 300f));
            var summary = UIFactory.Label(root, "SummaryText", "", new Vector2(940f, 330f),
                new Vector2(500f, 250f), 20f, UIFactory.Paper, TextAlignmentOptions.TopLeft);
            Button guilty = UIFactory.Button(root, "GuiltyButton", "유죄", new Vector2(500f, 600f), new Vector2(240f, 72f), UIFactory.Red);
            Button notGuilty = UIFactory.Button(root, "NotGuiltyButton", "무죄", new Vector2(780f, 600f), new Vector2(240f, 72f), UIFactory.Paper);

            Wire.Ref(refs.Trial, "root", root.gameObject);
            Wire.Ref(refs.Trial, "headerText", header);
            Wire.Ref(refs.Trial, "standardText", standard);
            Wire.Ref(refs.Trial, "summaryText", summary);
            Wire.Ref(refs.Trial, "guiltyButton", guilty);
            Wire.Ref(refs.Trial, "notGuiltyButton", notGuilty);
            root.gameObject.SetActive(false);
        }

        private static void BuildEnding(Transform canvas, SceneRefs refs)
        {
            RectTransform holder = UIFactory.Stretch("Ending", canvas);
            refs.Ending = holder.gameObject.AddComponent<EndingController>();
            RectTransform root = UIFactory.Stretch("EndingPanel", holder);
            UIFactory.AddImage(root, null, UIFactory.Ink);

            var title = UIFactory.Label(root, "TitleText", "", new Vector2(640f, 130f), new Vector2(1000f, 70f), 44f, UIFactory.Red, Center);
            var body = UIFactory.Label(root, "BodyText", "", new Vector2(640f, 285f), new Vector2(1000f, 200f), 24f, UIFactory.Paper, Center);
            var stats = UIFactory.Label(root, "StatsText", "", new Vector2(640f, 430f), new Vector2(1000f, 90f), 18f, UIFactory.Gray, Center);
            RectTransform group = UIFactory.Place("QuestionGroup", root, new Vector2(640f, 545f), new Vector2(1100f, 110f));
            var canvasGroup = group.gameObject.AddComponent<CanvasGroup>();
            var question = UIFactory.AddText(UIFactory.Stretch("QuestionText", group), "", 30f, UIFactory.Paper, Center);
            Button restart = UIFactory.Button(root, "RestartButton", "다시 하기", new Vector2(640f, 660f), new Vector2(240f, 60f), UIFactory.Paper);

            Wire.Ref(refs.Ending, "root", root.gameObject);
            Wire.Ref(refs.Ending, "titleText", title);
            Wire.Ref(refs.Ending, "bodyText", body);
            Wire.Ref(refs.Ending, "statsText", stats);
            Wire.Ref(refs.Ending, "questionGroup", canvasGroup);
            Wire.Ref(refs.Ending, "questionText", question);
            Wire.Ref(refs.Ending, "restartButton", restart);
            root.gameObject.SetActive(false);
        }

        private static void BuildIntro(Transform canvas, SceneRefs refs)
        {
            RectTransform holder = UIFactory.Stretch("Intro", canvas);
            refs.Intro = holder.gameObject.AddComponent<IntroVideoController>();
            RectTransform root = UIFactory.Stretch("IntroPanel", holder);
            UIFactory.AddImage(root, null, UIFactory.Ink).raycastTarget = false;
            var screen = UIFactory.Stretch("Screen", root).gameObject.AddComponent<RawImage>();
            screen.raycastTarget = false;
            Button skip = UIFactory.FullScreenButton(root, "SkipButton");
            UIFactory.Label(root, "SkipHint", "클릭하면 건너뛰기", new Vector2(1140f, 700f), new Vector2(260f, 30f),
                16f, UIFactory.Gray, TextAlignmentOptions.Right);

            Wire.Ref(refs.Intro, "introClip", UIFactory.LoadAsset<VideoClip>(VideoPath));
            Wire.Ref(refs.Intro, "root", root.gameObject);
            Wire.Ref(refs.Intro, "screen", screen);
            Wire.Ref(refs.Intro, "skipButton", skip);
            root.gameObject.SetActive(false);
        }

        private static void BuildDialogue(Transform canvas, SceneRefs refs)
        {
            RectTransform holder = UIFactory.Stretch("Dialogue", canvas);
            refs.Dialogue = holder.gameObject.AddComponent<DialogueUI>();
            RectTransform root = UIFactory.Stretch("DialoguePanel", holder);
            Button next = UIFactory.FullScreenButton(root, "NextButton");
            UIFactory.Panel(root, "Box", new Vector2(640f, 594f), new Vector2(1180f, 196f), 0.95f);
            var body = UIFactory.Label(root, "BodyText", "", new Vector2(640f, 590f), new Vector2(1110f, 150f),
                26f, UIFactory.Paper, TextAlignmentOptions.TopLeft);
            UIFactory.Label(root, "NextHint", "클릭 >", new Vector2(1170f, 672f), new Vector2(120f, 28f),
                16f, UIFactory.Gray, TextAlignmentOptions.Right);

            Wire.Ref(refs.Dialogue, "root", root.gameObject);
            Wire.Ref(refs.Dialogue, "bodyText", body);
            Wire.Ref(refs.Dialogue, "nextButton", next);
            root.gameObject.SetActive(false);
        }
    }
}

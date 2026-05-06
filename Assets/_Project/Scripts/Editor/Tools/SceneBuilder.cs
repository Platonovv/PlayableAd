using System.Collections.Generic;
using System.IO;
using Project.Audio;
using Project.Configs;
using Project.Core;
using Project.Domain;
using Project.Gameplay;
using Project.Gameplay.CameraFx;
using Project.Gameplay.Flow;
using Project.Gameplay.Targeting;
using Project.Gameplay.Vfx;
using Project.Integration;
using Project.UI.Common;
using Project.UI.EndCard;
using Project.UI.Hud;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Сборка <c>Main.unity</c> в один клик: иерархия, компоненты, ссылки и дефолтные SO-конфиги.
    /// </summary>
    public static class SceneBuilder
    {
        private const string ScenePath  = "Assets/_Project/Scenes/Main.unity";
        private const string ConfigsDir = "Assets/_Project/Configs";
        private const string AudioDir   = "Assets/_Project/Audio";

        [MenuItem("Playable/Tools/Build Main Scene")]
        public static void BuildScene()
        {
            EnsureFolders();
            var balance = LoadOrCreate<BalanceConfig>($"{ConfigsDir}/BalanceConfig.asset");
            var level   = CreateDefaultLevel($"{ConfigsDir}/LevelConfig.asset");
            var vfx     = LoadOrCreate<VfxBank>($"{ConfigsDir}/VfxBank.asset");
            var audio   = CreateAudioBank($"{ConfigsDir}/AudioBank.asset");
            var units   = LoadOrCreate<UnitsBank>($"{ConfigsDir}/UnitsBank.asset");

            var scene = OpenOrCreateScene();

            foreach (var go in scene.GetRootGameObjects())
                Object.DestroyImmediate(go);

            var cameraGO = new GameObject("Main Camera",
                typeof(Camera), typeof(AudioListener), typeof(ScreenShake));
            cameraGO.tag = "MainCamera";
            cameraGO.transform.position = new Vector3(0f, 30f, -24f);
            cameraGO.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            var cam = cameraGO.GetComponent<Camera>();
            cam.orthographic = false;
            cam.fieldOfView = 35f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 120f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.allowHDR = false;
            cam.allowMSAA = false;
            var screenShake = cameraGO.GetComponent<ScreenShake>();

            var lightGO = new GameObject("Directional Light", typeof(Light));
            var light = lightGO.GetComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.None;
            light.intensity = 0.55f;
            light.color = new Color(1f, 0.96f, 0.88f, 1f);
            lightGO.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.32f, 0.30f, 0.28f, 1f);
            RenderSettings.fog = false;

            var levelGO = new GameObject("Level");
            CreateBackground(levelGO.transform, cam);
            var spawnerGO = new GameObject("UnitSpawner", typeof(UnitSpawner));
            spawnerGO.transform.SetParent(levelGO.transform);
            var spawner = spawnerGO.GetComponent<UnitSpawner>();
            SetField(spawner, "_bank", units);
            SetField(spawner, "_root", levelGO.transform);

            var flowGO = new GameObject("BattleFlow",
                typeof(BattleFlow), typeof(TapInput), typeof(VfxService));
            var battleFlow = flowGO.GetComponent<BattleFlow>();
            var tapInput   = flowGO.GetComponent<TapInput>();
            var vfxService = flowGO.GetComponent<VfxService>();

            var indicatorGO = new GameObject("TargetIndicator",
                typeof(LineRenderer), typeof(TargetIndicator));
            indicatorGO.transform.SetParent(flowGO.transform);
            var indicatorLine = indicatorGO.GetComponent<LineRenderer>();
            ConfigureLineRenderer(indicatorLine);
            var indicator = indicatorGO.GetComponent<TargetIndicator>();
            SetField(indicator, "_line", indicatorLine);

            var floatingRoot = new GameObject("FloatingNumbersRoot");
            floatingRoot.transform.SetParent(flowGO.transform);

            const string floatingPrefabPath = "Assets/_Project/Art/Prefabs/FloatingNumber.prefab";
            var floatingPrefab = AssetDatabase.LoadAssetAtPath<FloatingNumber>(floatingPrefabPath);
            if (floatingPrefab == null) floatingPrefab = CreateFloatingNumberTemplate(flowGO.transform);

            SetField(tapInput,   "_camera",      cam);
            SetField(vfxService, "_bank",        vfx);
            SetField(battleFlow, "_input",       tapInput);
            SetField(battleFlow, "_indicator",   indicator);
            SetField(battleFlow, "_vfx",         vfxService);
            SetField(battleFlow, "_shake",       screenShake);
            SetField(battleFlow, "_floatingNumberPrefab", floatingPrefab);
            SetField(battleFlow, "_floatingNumbersRoot",  floatingRoot.transform);

            var audioGO = new GameObject("Audio", typeof(AudioService));
            var audioService = audioGO.GetComponent<AudioService>();
            var musicGO = new GameObject("Music_Source", typeof(AudioSource));
            musicGO.transform.SetParent(audioGO.transform);
            var music = musicGO.GetComponent<AudioSource>();
            music.loop = true;
            music.playOnAwake = false;
            var sfxPool = new AudioSource[4];
            for (var i = 0; i < sfxPool.Length; i++)
            {
                var sfxGO = new GameObject($"Sfx_{i}", typeof(AudioSource));
                sfxGO.transform.SetParent(audioGO.transform);
                sfxPool[i] = sfxGO.GetComponent<AudioSource>();
                sfxPool[i].playOnAwake = false;
            }
            SetField(audioService, "_bank",    audio);
            SetField(audioService, "_music",   music);
            SetField(audioService, "_sfxPool", sfxPool);

            var integrationGO = new GameObject("Integration");
            var mraidGO = new GameObject("MraidBridge", typeof(MraidBridge));
            mraidGO.transform.SetParent(integrationGO.transform);
            SetField(mraidGO.GetComponent<MraidBridge>(), "_audio", audioService);
            SetField(mraidGO.GetComponent<MraidBridge>(), "_ctaUrl", "https://example.com");

            var analyticsGO = new GameObject("AnalyticsService", typeof(AnalyticsService));
            analyticsGO.transform.SetParent(integrationGO.transform);

            var canvasGO = new GameObject("Canvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 1f;
            canvas.sortingOrder = 100;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            // Match по высоте: на landscape UI не растягивается, остаётся в 9:16 SafeArea ниже.
            scaler.matchWidthOrHeight = 1f;

            // SafeArea — контейнер 9:16 по центру Canvas. Все UI-элементы дети SafeArea,
            // так что на landscape (desktop preview) они не уезжают на чёрные полосы.
            var safeAreaGO = new GameObject("SafeArea",
                typeof(RectTransform), typeof(AspectRatioFitter));
            safeAreaGO.transform.SetParent(canvasGO.transform, false);
            var safeRect = (RectTransform)safeAreaGO.transform;
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            var safeFitter = safeAreaGO.GetComponent<AspectRatioFitter>();
            safeFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            safeFitter.aspectRatio = 1080f / 1920f;

            var hudGO = new GameObject("HUD", typeof(HudView), typeof(HudPresenter));
            hudGO.transform.SetParent(safeAreaGO.transform, false);
            var hudView = hudGO.GetComponent<HudView>();
            var hudPresenter = hudGO.GetComponent<HudPresenter>();
            SetField(hudPresenter, "_view", hudView);

            var endCardGO = new GameObject("EndCard",
                typeof(CanvasGroup), typeof(Image), typeof(EndCardView), typeof(EndCardPresenter));
            endCardGO.transform.SetParent(canvasGO.transform, false);
            var endCardRect = (RectTransform)endCardGO.transform;
            endCardRect.anchorMin = Vector2.zero;
            endCardRect.anchorMax = Vector2.one;
            endCardRect.offsetMin = Vector2.zero;
            endCardRect.offsetMax = Vector2.zero;
            var endCardImg = endCardGO.GetComponent<Image>();
            endCardImg.color = new Color(0f, 0f, 0f, 0.65f);
            var endCardGroup = endCardGO.GetComponent<CanvasGroup>();
            endCardGroup.alpha = 0f;
            endCardGroup.blocksRaycasts = false;

            var titleText = CreateUiText(endCardGO.transform, "Title", "YOU WIN!", 140,
                anchor: new Vector2(0.5f, 0.7f), pivot: new Vector2(0.5f, 0.5f),
                anchored: Vector2.zero, sizeDelta: new Vector2(900f, 200f), color: Color.white);

            var subtitleText = CreateUiText(endCardGO.transform, "Subtitle", "DEFEATED 0 · POWER 0", 50,
                anchor: new Vector2(0.5f, 0.7f), pivot: new Vector2(0.5f, 0.5f),
                anchored: new Vector2(0f, -130f), sizeDelta: new Vector2(900f, 80f),
                color: new Color(1f, 1f, 1f, 0.85f));

            var stars = new Image[3];
            const float starSpacing = 140f;
            var starFilledSprite = LoadIconSprite("star_white_24dp.png");
            var starOutlineSprite = LoadIconSprite("star_outline_white_24dp.png");
            for (int i = 0; i < 3; i++)
            {
                var starGO = new GameObject("Star_" + i,
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                starGO.transform.SetParent(endCardGO.transform, false);
                var sRect = (RectTransform)starGO.transform;
                sRect.anchorMin = sRect.anchorMax = new Vector2(0.5f, 0.7f);
                sRect.pivot = new Vector2(0.5f, 0.5f);
                sRect.anchoredPosition = new Vector2((i - 1) * starSpacing, -240f);
                sRect.sizeDelta = new Vector2(110f, 110f);
                var img = starGO.GetComponent<Image>();
                img.sprite = starOutlineSprite;
                img.color = new Color(1f, 1f, 1f, 0.45f);
                img.raycastTarget = false;
                stars[i] = img;
            }

            var ctaButton    = CreateButton(endCardGO.transform, "CTA",   "PLAY!",        new Vector2(0f, -120f));
            var retryButton  = CreateButton(endCardGO.transform, "Retry", "RETRY",        new Vector2(0f, -320f), small: true);
            var skipButton   = CreateButton(endCardGO.transform, "Skip",  "GET THE GAME", new Vector2(0f, -120f));

            var endCardView      = endCardGO.GetComponent<EndCardView>();
            var endCardPresenter = endCardGO.GetComponent<EndCardPresenter>();
            SetField(endCardView, "_group",       endCardGroup);
            SetField(endCardView, "_ctaButton",   ctaButton);
            SetField(endCardView, "_retryButton", retryButton);
            SetField(endCardView, "_skipButton",  skipButton);
            SetField(endCardView, "_title",       titleText);
            SetField(endCardView, "_subtitle",    subtitleText);
            SetField(endCardView, "_stars",             stars);
            SetField(endCardView, "_starFilledSprite",  starFilledSprite);
            SetField(endCardView, "_starOutlineSprite", starOutlineSprite);
            SetField(endCardPresenter, "_view",     endCardView);
            SetField(endCardPresenter, "_winTitle", "YOU WIN!");
            SetField(endCardPresenter, "_loseTitle", "GAME OVER");

            BuildMuteToggle(safeAreaGO.transform);

            var eventSystemGO = new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

            var rootGO = new GameObject("GameRoot", typeof(GameRoot));
            var gameRoot = rootGO.GetComponent<GameRoot>();
            SetField(gameRoot, "_levelConfig",   level);
            SetField(gameRoot, "_balanceConfig", balance);
            SetField(gameRoot, "_spawner",       spawner);
            SetField(gameRoot, "_battleFlow",    battleFlow);
            SetField(gameRoot, "_camera",        cam);

            SetField(hudPresenter,    "_root", gameRoot);
            SetField(endCardPresenter, "_root", gameRoot);
            SetField(audioService,    "_root", gameRoot);
            SetField(analyticsGO.GetComponent<AnalyticsService>(), "_root", gameRoot);
            SetField(mraidGO.GetComponent<MraidBridge>(),          "_root", gameRoot);

            BuildTutorialNudge(safeAreaGO.transform, gameRoot);
            BuildMobCounter(safeAreaGO.transform, gameRoot);
            BuildTouchRipple(safeAreaGO, cam);
            BuildTapToStartSplash(canvasGO.transform);

            WireUiClickSound(ctaButton, audioService);
            WireUiClickSound(retryButton, audioService);
            WireUiClickSound(skipButton, audioService);

            DisableShadowsInScene(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            var buildScenes = new List<EditorBuildSettingsScene>();
            foreach (var s in EditorBuildSettings.scenes)
                if (File.Exists(s.path)) buildScenes.Add(s);
            if (!buildScenes.Exists(s => s.path == ScenePath))
                buildScenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = buildScenes.ToArray();

            Debug.Log("[SceneBuilder] Сцена собрана. Осталось привязать префабы юнитов и VFX в инспекторе.");
        }

        private static void EnsureFolders()
        {
            foreach (var path in new[] { "Assets/_Project/Scenes", ConfigsDir })
            {
                if (AssetDatabase.IsValidFolder(path)) continue;
                var parent = Path.GetDirectoryName(path).Replace('\\', '/');
                var leaf = Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent)) EnsureFolders();
                AssetDatabase.CreateFolder(parent, leaf);
            }
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        private static LevelConfig CreateDefaultLevel(string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
            if (existing != null) return existing;

            var level = ScriptableObject.CreateInstance<LevelConfig>();
            level.Player = new LevelConfig.PlayerSpawn { Position = new Vector2(-4f, 2f), Power = 2 };
            level.Targets = new List<LevelConfig.TargetSpawn>
            {
                new() { Kind = UnitKind.Chest, Position = new Vector2(  1f,    0f), Power = 3, PrefabKey = ""       },
                new() { Kind = UnitKind.Enemy, Position = new Vector2(  1.5f,  -10f), Power = 3, PrefabKey = "goblin" },
                new() { Kind = UnitKind.Enemy, Position = new Vector2( -3.5f, -3f),  Power = 6, PrefabKey = "ogre"   },
                new() { Kind = UnitKind.Enemy, Position = new Vector2(  3f,    5f),  Power = 10, PrefabKey = "ogre"   },
            };
            AssetDatabase.CreateAsset(level, path);
            AssetDatabase.SaveAssets();
            return level;
        }

        private static AudioBank CreateAudioBank(string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<AudioBank>(path);
            if (existing != null) return existing;

            var bank = ScriptableObject.CreateInstance<AudioBank>();
            bank.Music = LoadAudio("music_loop");
            bank.MusicVolume = 0.5f;
            bank.Sfxs = new[]
            {
                Sfx("tap",     "sfx_tap",     0.6f),
                Sfx("hit",     "sfx_hit",     0.8f),
                Sfx("fail",    "sfx_fail",    0.6f),
                Sfx("chest",   "sfx_chest",   0.8f),
                Sfx("upgrade", "sfx_upgrade", 0.7f),
                Sfx("win",     "sfx_win",     0.8f),
            };
            AssetDatabase.CreateAsset(bank, path);
            AssetDatabase.SaveAssets();
            return bank;
        }

        private static AudioBank.Sfx Sfx(string key, string fileName, float volume)
            => new() { Key = key, Clip = LoadAudio(fileName), Volume = volume };

        private static AudioClip LoadAudio(string fileName)
        {
            foreach (var ext in new[] { ".ogg", ".wav", ".mp3" })
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudioDir}/{fileName}{ext}");
                if (clip != null) return clip;
            }
            return null;
        }

        private static void ConfigureLineRenderer(LineRenderer line)
        {
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.startWidth = 0.08f;
            line.endWidth = 0.08f;
            line.numCornerVertices = 2;
            line.numCapVertices = 4;
            var mat = new Material(Shader.Find("Sprites/Default")) { color = new Color(1f, 0.85f, 0.2f, 1f) };
            line.material = mat;
            line.startColor = mat.color;
            line.endColor   = mat.color;
            line.enabled = false;
        }

        private static Text CreateUiText(Transform parent, string name, string content, float fontSize,
            Vector2 anchor, Vector2 pivot, Vector2 anchored, Vector2 sizeDelta, Color? color = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = anchored;
            rect.sizeDelta = sizeDelta;
            var ui = go.AddComponent<Text>();
            ui.font = LegacyFont();
            ui.text = content;
            ui.fontSize = Mathf.Max(1, Mathf.RoundToInt(fontSize));
            ui.alignment = TextAnchor.MiddleCenter;
            ui.color = color ?? Color.white;
            ui.fontStyle = FontStyle.Bold;
            ui.horizontalOverflow = HorizontalWrapMode.Overflow;
            ui.verticalOverflow = VerticalWrapMode.Overflow;
            return ui;
        }

        private static Font LegacyFont()
        {
            try { return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); }
            catch { }
            try { return Resources.GetBuiltinResource<Font>("Arial.ttf"); }
            catch { }
            return null;
        }

        private static Sprite LoadIconSprite(string fileName)
        {
            var path = "Assets/_Project/Art/Icons/" + fileName;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void BuildMobCounter(Transform canvasParent, GameRoot root)
        {
            var go = new GameObject("MobCounter",
                typeof(RectTransform), typeof(CanvasGroup), typeof(MobCounter));
            go.transform.SetParent(canvasParent, false);

            var rect = (RectTransform)go.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(32f, -15f);
            rect.sizeDelta = new Vector2(500f, 90f);

            var label = CreateUiText(go.transform, "Label", "MOBS 0/0", 56,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20f, 0f),
                new Vector2(500f, 90f), color: Color.white);
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleLeft;

            var group = go.GetComponent<CanvasGroup>();
            group.alpha = 1f;
            group.blocksRaycasts = false;
            group.interactable = false;

            var counter = go.GetComponent<MobCounter>();
            SetField(counter, "_root", root);
            SetField(counter, "_label", label);
            SetField(counter, "_group", group);
        }

        private static void BuildTouchRipple(GameObject canvasGO, Camera cam)
        {
            const int poolSize = 8;
            var go = new GameObject("TouchRipple",
                typeof(RectTransform), typeof(TouchRipple));
            go.transform.SetParent(canvasGO.transform, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            var rects = new RectTransform[poolSize];
            var images = new Image[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var item = new GameObject("Ripple_" + i,
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                item.transform.SetParent(go.transform, false);
                var r = (RectTransform)item.transform;
                r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.sizeDelta = new Vector2(220f, 220f);
                var img = item.GetComponent<Image>();
                img.sprite = sprite;
                img.raycastTarget = false;
                item.SetActive(false);
                rects[i] = r;
                images[i] = img;
            }

            var ripple = go.GetComponent<TouchRipple>();
            SetField(ripple, "_canvasRect", (RectTransform)canvasGO.transform);
            SetField(ripple, "_camera", cam);
            SetField(ripple, "_poolRects", rects);
            SetField(ripple, "_poolImages", images);
        }

        private static void BuildTutorialNudge(Transform canvasParent, GameRoot root)
        {
            var go = new GameObject("TutorialNudge",
                typeof(RectTransform), typeof(CanvasGroup), typeof(TutorialNudge));
            go.transform.SetParent(canvasParent, false);

            var rect = (RectTransform)go.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 200f);
            rect.sizeDelta = new Vector2(900f, 350f);

            var hintText = CreateUiText(go.transform, "Hint", "TAP THE ENEMY", 80,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
                new Vector2(900f, 200f), color: Color.white);
            hintText.fontStyle = FontStyle.Bold;

            var group = go.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;

            var nudge = go.GetComponent<TutorialNudge>();
            SetField(nudge, "_root", root);
            SetField(nudge, "_group", group);
        }

        private static void BuildTapToStartSplash(Transform canvasParent)
        {
            var go = new GameObject("TapToStartSplash",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
                typeof(CanvasGroup), typeof(Button), typeof(TapToStartSplash));
            go.transform.SetParent(canvasParent, false);
            go.transform.SetAsLastSibling();

            var rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.85f);
            image.raycastTarget = true;

            var group = go.GetComponent<CanvasGroup>();
            group.alpha = 1f;
            group.blocksRaycasts = true;

            var button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;

            var ctaText = CreateUiText(go.transform, "CTA Pulse", "TAP TO PLAY", 110,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
                new Vector2(900f, 220f), color: Color.white);
            var ctaRect = ctaText.rectTransform;

            var splash = go.GetComponent<TapToStartSplash>();
            SetField(splash, "_group", group);
            SetField(splash, "_tapButton", button);
            SetField(splash, "_ctaPulse", ctaRect);
        }

        private static void BuildMuteToggle(Transform canvasParent)
        {
            var go = new GameObject("MuteToggle",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
                typeof(Button), typeof(MuteToggle));
            go.transform.SetParent(canvasParent, false);

            var rect = (RectTransform)go.transform;
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-32f, -15f);
            rect.sizeDelta = new Vector2(120f, 120f);

            var bg = go.GetComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.45f);
            bg.raycastTarget = true;
            bg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

            var iconGO = new GameObject("Icon",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconGO.transform.SetParent(go.transform, false);
            var iconRect = (RectTransform)iconGO.transform;
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(80f, 80f);

            var iconImage = iconGO.GetComponent<Image>();
            iconImage.color = Color.white;
            iconImage.raycastTarget = false;

            var onSprite = LoadIconSprite("volume_up_white_24dp.png");
            var offSprite = LoadIconSprite("volume_off_white_24dp.png");
            iconImage.sprite = onSprite;

            var button = go.GetComponent<Button>();
            button.targetGraphic = bg;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.selectedColor = Color.white;
            button.colors = colors;

            var toggle = go.GetComponent<MuteToggle>();
            SetField(toggle, "_button", button);
            SetField(toggle, "_icon", iconImage);
            SetField(toggle, "_onSprite", onSprite);
            SetField(toggle, "_offSprite", offSprite);
        }

        private static CanvasGroup CreateHintGroup(Transform parent)
        {
            var go = new GameObject("Hint", typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -200f);
            rect.sizeDelta = new Vector2(800f, 100f);
            var hint = CreateUiText(go.transform, "Text", "TAP TARGET", 60,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 100f),
                color: new Color(1f, 1f, 1f, 0.8f));
            return go.GetComponent<CanvasGroup>();
        }

        private static void WireUiClickSound(Button button, AudioService audio)
        {
            if (button == null) return;
            var sound = button.gameObject.AddComponent<UiSoundButton>();
            SetField(sound, "_audio", audio);
            SetField(sound, "_button", button);
            SetField(sound, "_key", "click");
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchored, bool small = false)
        {
            var go = new GameObject(name,
                typeof(RectTransform), typeof(Image), typeof(Button), typeof(TweenButton));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = small ? new Vector2(360f, 110f) : new Vector2(520f, 160f);
            go.GetComponent<Image>().color = small
                ? new Color(0.3f, 0.3f, 0.3f, 0.9f)
                : new Color(0.95f, 0.65f, 0.15f, 1f);
            var labelTmp = CreateUiText(go.transform, "Label", label, small ? 50f : 60f,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
                small ? new Vector2(360f, 110f) : new Vector2(520f, 160f), Color.white);
            return go.GetComponent<Button>();
        }

        private static void DisableShadowsInScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var r in root.GetComponentsInChildren<Renderer>(includeInactive: true))
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    r.receiveShadows = false;
                }
            }
        }

        private static Scene OpenOrCreateScene()
        {
            if (File.Exists(ScenePath))
                return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            if (target == null) return;
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);
            if (field == null)
            {
                Debug.LogWarning($"[SceneBuilder] Поле '{fieldName}' не найдено в {target.GetType().Name}");
                return;
            }
            field.SetValue(target, value);
            if (target is Object uo) EditorUtility.SetDirty(uo);
        }

        private static void CreateBackground(Transform parent, Camera cam)
        {
            Texture2D bg = null;
            var guids = AssetDatabase.FindAssets("t:Texture2D BG", new[] { "Assets/_Project/Art" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("clean", System.StringComparison.OrdinalIgnoreCase)) continue;
                bg = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (bg != null) break;
            }
            if (bg == null)
            {
                foreach (var guid in guids)
                {
                    bg = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));
                    if (bg != null) break;
                }
            }

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "BG";
            Object.DestroyImmediate(quad.GetComponent<Collider>());

            const float distance = 60f;
            var attachTo = cam != null ? cam.transform : parent;
            quad.transform.SetParent(attachTo, false);
            quad.transform.localPosition = new Vector3(0f, 0f, distance);
            quad.transform.localRotation = Quaternion.identity;

            var fov = cam != null ? cam.fieldOfView : 50f;
            var aspect = 9f / 16f;
            var height = 2f * distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            var width  = height * aspect;
            quad.transform.localScale = new Vector3(width * 1.05f, height * 1.05f, 1f);

            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Standard");
            var mat = new Material(shader) { name = "BG_Material" };
            if (bg != null && mat.HasProperty("_MainTex")) mat.mainTexture = bg;
            mat.color = bg != null ? new Color(0.6f, 0.6f, 0.65f, 1f) : new Color(0.12f, 0.18f, 0.28f, 1f);
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private static FloatingNumber CreateFloatingNumberTemplate(Transform parent)
        {
            var go = new GameObject("FloatingNumberTemplate",
                typeof(Canvas), typeof(CanvasGroup), typeof(FloatingNumber));
            go.transform.SetParent(parent);
            go.SetActive(false);

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;
            ((RectTransform)go.transform).sizeDelta = new Vector2(2f, 1f);
            go.transform.localScale = Vector3.one * 0.01f;

            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(go.transform, false);
            var rect = (RectTransform)textGO.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var ui = textGO.AddComponent<Text>();
            ui.font = LegacyFont();
            ui.alignment = TextAnchor.MiddleCenter;
            ui.fontSize = 64;
            ui.fontStyle = FontStyle.Bold;
            ui.color = new Color(0.55f, 1f, 0.55f, 1f);
            ui.text = "+0";
            ui.horizontalOverflow = HorizontalWrapMode.Overflow;
            ui.verticalOverflow = VerticalWrapMode.Overflow;

            var fn = go.GetComponent<FloatingNumber>();
            SetField(fn, "_text", ui);
            SetField(fn, "_group", go.GetComponent<CanvasGroup>());
            return fn;
        }
    }
}

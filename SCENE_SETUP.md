# Сборка сцены Main.unity

В проекте сцена собирается **в один клик** через `Playable → Tools → Build Main Scene`.
Этот документ описывает что именно билдер делает и что нужно подкрутить руками после.

> Все пути относительны корня `Assets/`.

---

## TL;DR

```
1. Открыть проект в Unity 2022.3.62f2.
2. Playable → Tools → Build Main Scene
3. Подвесить префабы в UnitsBank.asset и VfxBank.asset (если ещё не).
4. Подвесить аудио-клипы в AudioBank.asset.
5. Press Play.
```

Если префабы и банки уже заполнены — шаги 3–4 можно пропустить.

---

## 0. Импорт ассетов

Исходники лежат в `_unpacked/rar/Plb_008/`. Перенесённые в проект:

```
Assets/_Project/Art/
  Heroes/        Hero_*.fbx, DF_Knight_2_HQ.png, DF_Knight_2_HQ_gold.png
  Mobs/          Goblin_*.fbx, FB_Ogre*.fbx, FB_Goblin*.png, ...
  Chest/         Chest.FBX, ChestShopLegendary.png
  Backgrounds/   BG_plb_clean.jpg          (генерится через _make_bg.py)
  Sequences/     Sparks 012/, Hit 013/, Electricity 014/, Energy 016/,
                 Liquid 009/, Liquid 051/, Liquid 056/
  FX/            (источник — не используется в рантайме, sequences сами по себе)
Assets/_Project/Audio/
  music_loop.ogg, sfx_tap.ogg, sfx_hit.ogg, sfx_fail.ogg,
  sfx_chest.ogg, sfx_upgrade.ogg, sfx_win.ogg
Assets/_Project/Plugins/WebGL/
  PlayableBridge.jslib                     (мост MRAID — нельзя удалять)
```

**Импорт-настройки FBX:**
- Model: `Mesh Compression = High`, `Read/Write = off`, `Optimize Game Objects = on`.
- Rig: `Generic`. Если у тебя несколько FBX от одного риг-меша — выдели их и
  запусти `Playable → Tools → Sync Avatars in Selection` (без этого animation-clips
  с разных FBX не играют на одном меше).
- Animation: `Loop Time = on` для idle/run; `Has Root Motion = off`.
- Materials: `Use External Materials`. Если попался URP/HDRP-shader —
  `Playable → Tools → Fix Pink Materials` переведёт всё на `Mobile/Diffuse`.
  Затем `Playable → Tools → Auto Fill Material Textures` подтянет `_MainTex`
  по совпадению имён.

**Текстуры:** `RGBA Crunched`, `q = 50`, `Max = 512`.
PNG-секвенции в `Art/Sequences/` и `Art/FX/` обрабатывает
`SequenceTexturePostprocessor` автоматически.

**Аудио:** mono, 22050 Hz, Vorbis q = 2 (sfx) / q = 3 (music).
4.1 MB исходников → 103 KB после конвертации.

---

## 1. SO-конфиги

`SceneBuilder` создаёт дефолты в `Assets/_Project/Configs/` если их нет.
Если уже существуют — **не перезаписывает** (твои правки сохранятся между прогонами).

| Файл | Что внутри |
|---|---|
| `BalanceConfig.asset` | MoveSpeed, AttackWindup/Impact/Recover, FailedAttackBounce, DeathAnim/Fade, Upgrade timings, FloatingNumber duration, EndCardDelay, SuperAttackThreshold = 10. |
| `LevelConfig.asset` | Player.Position+Power, Targets[Kind, Position, Power, PrefabKey]. |
| `UnitsBank.asset` | PlayerPrefab, ChestPrefab, EnemyByKey ("goblin", "ogre"). |
| `VfxBank.asset` | hit / block / chest_open / super_hit / death / power_gain / upgrade. |
| `AudioBank.asset` | tap / hit / fail / chest / upgrade / win + music loop. |

**Дефолтная расстановка `LevelConfig`:**

| Kind  | Position    | Power | PrefabKey |
|-------|-------------|-------|-----------|
| Chest | (-2, -1)    | 3     | (пусто)   |
| Enemy | ( 2,  0)    | 4     | goblin    |
| Enemy | ( 0,  2)    | 7     | ogre      |
| Enemy | (-1.5, 3.5) | 9     | ogre      |

Под полную сессию ~15–25 сек.

**Маппинг VFX → секвенции (что куда):**

| Key            | Секвенция                  |
|----------------|----------------------------|
| `hit`          | Sparks 012 Explosion       |
| `block`        | Electricity 014 Charge     |
| `chest_open`   | Energy 016 Explosion       |
| `super_hit`    | Hit 013 Explosion          |
| `death`        | Liquid 051 Explosion       |
| `power_gain`   | Liquid 009 Explosion       |
| `upgrade`      | Liquid 056 Explosion       |

---

## 2. Префабы юнитов

Все префабы — `Assets/_Project/Art/Prefabs/`.

### `Player.prefab`
- Корень: `[Animator] [BoxCollider≈1×2×1] [PlayerView]`
- Дочерние Transform-маркеры (важны для VFX/движения):
  - `StopPoint` — точка, до которой бежит соперник, чтобы атаковать игрока.
  - `VfxPoint` — где спавнятся партиклы по игроку.
  - `AnchorPoint` — где висит `PowerLabel` над головой.
- `BodyRenderer` ← SkinnedMeshRenderer тела
- `DefaultMaterial` ← `DF_Knight_2_HQ.mat`
- `UpgradedMaterial` ← `DF_Knight_2_HQ_gold.mat` (тот же шейдер, gold-текстура)
- `Animator Controller`: триггеры/стейты `Idle / Run / Attack / SuperAttack / Hit / Death / Victory / Upgrade`
- `FootstepEmitter` (опц.) с дочерним префабом отпечатка ноги

### `Enemy_*.prefab` (Goblin, Ogre)
- Корень: `[Animator] [BoxCollider] [EnemyView]`
- Дочерние: `StopPoint`, `VfxPoint`, `AnchorPoint`
- (опц.) `WarningRing` — красное кольцо под ногами для preview "сильнее тебя"
- Animator: `Idle / Attack / Hit / Death`

### `Chest.prefab`
- Корень: `[Animator] [BoxCollider] [ChestView]`
- Дочерние: `StopPoint`, `VfxPoint`, `AnchorPoint`
- Animator: `Closed / Open` (либо собрать в Animation Window из позиций крышки)
- `IdleBobAmplitude = 0.05`, `IdleBobSpeed = 2`

### `FloatingNumber.prefab`
- Корень: `Canvas (World Space) + CanvasGroup + FloatingNumber`
- Дочерний: `TextMeshPro Text (UI)` — крупный жирный шрифт
- В рантайме Floating-Number поворачивается лицом к камере (billboard).

### Vfx-префабы (`Assets/_Project/Art/Prefabs/Vfx/`)
- Корень: `[SpriteRenderer] [SpriteSequencePlayer]`
- `_frames` ← массив спрайтов из соответствующей `Sequences/...`
- `_fps = 15`, `_loop = false`, `_disableOnFinish = true`
- VfxService спавнит из пула, поэтому `disableOnFinish` обязательно.

`UnitsBank` хранит все ссылки. Spawner читает `PrefabKey` и достаёт нужный
тип Enemy. Если `PrefabKey` пуст — берёт дефолт.

---

## 3. Сцена — что собирает SceneBuilder

```
Main.unity
├─ [GameRoot]              [DefaultExecutionOrder(-1000)]
├─ Main Camera             Perspective, FOV 35, position (0, 40, -32), rotation (50,0,0)
│   ├─ ScreenShake
│   └─ BG                  Quad-child, BG_plb_clean.jpg, Unlit/Texture
├─ Directional Light       Intensity 0.55, цвет тёплый (0.99, 0.95, 0.85), без теней
│                          (RenderSettings.ambientLight = (0.32, 0.30, 0.28))
├─ Level
│   └─ UnitSpawner         читает LevelConfig + UnitsBank
│                          Spawn rotation = (-30, -180, 0) — юниты лежат в плоскости BG
├─ BattleFlow
│   ├─ TapInput            press-and-release, preview при зажатии
│   ├─ TargetIndicator     LineRenderer, dashed, Sprites/Default
│   │                      _enemyArrowColor = красный (сильнее)
│   │                      _winnableArrowColor = синий (победим)
│   │                      _chestArrowColor = зелёный (сундук)
│   ├─ VfxService
│   └─ FloatingNumbersRoot
├─ Audio
│   ├─ Music_Source        AudioSource, loop = true
│   └─ Sfx_0..3            пул из 4 AudioSource, playOnAwake = false
├─ Integration
│   ├─ MraidBridge         GameObject.name строго "MraidBridge"
│   └─ AnalyticsService
├─ Canvas (Screen Space - Overlay, 1080×1920, Match = 0.5)
│   ├─ HUD
│   │   ├─ PlayerPower (TMP)
│   │   └─ Hint (CanvasGroup + текст "Tap")
│   └─ EndCard
│       ├─ Panel, Title, CTA_Button (TweenButton), Retry_Button
└─ EventSystem
```

`SceneBuilder` сам прокидывает все `[SerializeField]`-ссылки между нодами,
включая `_root` (GameRoot) у клиентских компонентов (HudPresenter,
EndCardPresenter, AudioService, AnalyticsService, MraidBridge).

### Что НЕ делает SceneBuilder
- Не трогает уже созданные SO (`LevelConfig`, `BalanceConfig` и т.д.) — твои поля сохраняются.
- Не подвешивает префабы в `UnitsBank` и `VfxBank` — это делается **один раз руками**.
- Не подвешивает аудио-клипы в `AudioBank` — тоже один раз.

---

## 4. Player Settings и Build Settings

`Playable → Build → Configure Player Only` выставляет всё ниже без сборки:

- `Switch Platform → WebGL`
- `Scripting Backend = IL2CPP`, `Code Optimization = Master`,
  `Code Generation = OptimizeSize`, `Managed Stripping = High`.
- `WebGL.compressionFormat = Brotli`, `decompressionFallback = true`,
  `exceptionSupport = None`, `dataCaching = false`,
  `threadsSupport = false`, `linkerTarget = Wasm`, `memorySize = 64`.
- `Color Space = Gamma`, `gpuSkinning = false`,
  `stripUnusedMeshComponents = true`, `bakeCollisionMeshes = true`.
- `WebGL Template = PROJECT:Playable` (portrait, autostart, без оверлеев).
- `Active Input Handling = Input Manager (Old)` — по дефолту, новый InputSystem не подключён.

---

## 5. Прогон

`Edit → Play` — должен выйти HUD с числом 5, четыре цели на сцене с числами,
press-and-hold показывает стрелку нужного цвета, тап атакует/собирает сундук,
после уничтожения всех врагов — Win-EndCard. Если ткнуть в врага сильнее себя —
Lose-EndCard с Retry.

**Финальная сборка**: `Playable → Build → WebGL → Build And Run (test)` —
оптимизированный билд + автозапуск через локальный Unity-сервер.
Папка `Build/WebGL/` — рабочий deliverable (zip-ом, как принято
в AppLovin/Vungle/IronSource).

Single-file inline: `Playable → Build → WebGL → Single HTML` собирает
`Build/Playable/index.html` — один файл, запускается двойным кликом без сети.
Под капотом — brotli → gzip → base64 + `DecompressionStream` API.
Подробности в README, раздел «Single-file inline — как это работает».

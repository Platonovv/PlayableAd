# Сборка сцены Main.unity

Пошаговый гайд: от пустого проекта до играбельной сцены. Делается **один раз** —
дальше всё крутится через ScriptableObject-конфиги без правки сцены.

> Все пути относительны корня `Assets/`.

---

## 0. Импорт ассетов из материалов

В `_unpacked/rar/Plb_008/` лежат FBX/PNG/WAV. Перенеси нужное в `Assets/_Project/Art/`
и `Assets/_Project/Audio/` через проводник или drag-and-drop в Unity:

```
Assets/_Project/Art/
  Heroes/         Hero_idle.fbx, Hero_run.fbx, Hero_sword_attack.fbx, Hero_death.fbx,
                   DF_Knight_2_HQ.png, DF_Knight_2_HQ_gold.png
  Mobs/           FB_Ogre2.fbx, Goblin_red_idle.fbx, FB_Goblin1 copy.png ...
  Chest/          Chest.FBX, ChestShopLegendary.png
  Backgrounds/    BG_plb.jpg
  Sequences/      Liquid 051 Explosion/, Sparks 012 Explosion/, Hit 013 Explosion/
                   ↑ сюда — postprocessor сам выставит crunch + 512px

Assets/_Project/Audio/
  music_loop.ogg            (короткий лоуп — обрезать io-music-transition-chase.wav до ~10–12 сек)
  sfx_tap.ogg               ← Click_Selection_Сlop.wav  или  UI Bubble Pop Text.wav
  sfx_hit.ogg               ← Sledgehammer_Pickaxe_hit_03.wav  или  Destr Hit Metal With Hammer.wav
  sfx_fail.ogg              ← man male grunt hurt pain shout 2.wav  или  Metal Can Hit 01.wav
  sfx_chest.ogg             ← Bright Sparkle Pop.wav  +  Magic Gun Shot 1.wav
  sfx_upgrade.ogg           ← Super Charged Game Power Up 2.wav  или  Cinematic Upgrade 2.wav
  sfx_win.ogg               ← Male Yelling Victorious Yes 1.wav

Музыка лежит во **внешнем** zip: `_unpacked/Plb_008/ Materials/Sound/io-music-transition-chase.wav`.
SFX — в распакованном rar: `_unpacked/rar/Plb_008/Sound/`.
```

**Импорт-настройки FBX (вручную в инспекторе):**
- Model: `Scale Factor = 0.01` (если FBX в см) или `1` (метры). Подбери на глаз.
  `Mesh Compression = High`. `Read/Write = off`. `Optimize Game Objects = on`.
- Rig: `Animation Type = Generic` (не Humanoid — рыцарь не из Mecanim).
- Animation: для каждого FBX — выставить `Loop Time` для idle/run, `Has Root Motion = off`.
- Materials: `Material Creation Mode = Standard`, `Location = Use External Materials`.
  Шейдер материалов потом перевести на `Mobile/Diffuse` или `Unlit/Texture`.

**Импорт-настройки PNG-фонов:** `Texture Type = Sprite`, `Max Size = 1024`, `Crunched = on, q=50`.

**Импорт-настройки аудио:** см. ниже в шаге 5.

---

## 1. Создать SO-конфиги

`Project → Assets/_Project/Configs/` (создай папку если нет):

| Меню | Имя файла |
|---|---|
| `Create → Playable → Balance Config` | `BalanceConfig.asset` |
| `Create → Playable → Level Config`   | `LevelConfig.asset` |
| `Create → Playable → Vfx Bank`       | `VfxBank.asset` |
| `Create → Playable → Audio Bank`     | `AudioBank.asset` |

**`BalanceConfig.asset`** — оставить дефолты, поправишь по ощущению.

**`LevelConfig.asset`:**
- `Player.Position = (0, -3)`, `Power = 5`
- `Targets`: добавить 4 элемента
  | Kind  | Position    | Power | PrefabKey |
  |-------|-------------|-------|-----------|
  | Chest | (-2, -1)    | 3     | (пусто)   |
  | Enemy | ( 2, 0)     | 4     | goblin    |
  | Enemy | ( 0, 2)     | 7     | ogre      |
  | Enemy | (-1.5, 3.5) | 9     | ogre      |

(Разная стоимость заставляет идти сначала по слабым+сундук, потом по сильному.)

**`VfxBank.asset` Entries:**
| Key            | Prefab (создаём в шаге 4)                |
|----------------|------------------------------------------|
| `hit`          | `Vfx_Hit.prefab` (sparks explosion seq)  |
| `block`        | `Vfx_Block.prefab` (electricity charge)  |
| `chest_open`   | `Vfx_ChestOpen.prefab` (energy explosion)|

**`AudioBank.asset`:**
- `Music = music_loop.ogg`, `MusicVolume = 0.5`
- `Sfxs`:
  | Key       | Clip            | Volume |
  |-----------|-----------------|--------|
  | `tap`     | sfx_tap.ogg     | 0.6    |
  | `hit`     | sfx_hit.ogg     | 0.8    |
  | `fail`    | sfx_fail.ogg    | 0.6    |
  | `chest`   | sfx_chest.ogg   | 0.8    |
  | `upgrade` | sfx_upgrade.ogg | 0.7    |
  | `win`     | sfx_win.ogg     | 0.8    |

---

## 2. Создать сцену

`File → New Scene → Empty`. Сохранить как `Assets/_Project/Scenes/Main.unity`.
Удалить дефолтные `Main Camera` и `Directional Light` — пересоздадим вручную.

`File → Build Profiles → Scene List → Add Open Scenes`.

---

## 3. Префаб игрока (PlayerView)

1. В сцене: `Create Empty` → переименовать в `Player_TMP`.
2. Дочерним: `Hero_idle.fbx` (drag из проекта). Это даст mesh + skeleton.
3. На корне `Player_TMP` добавить компоненты:
   - `Animator` (Controller назначим ниже)
   - `BoxCollider` (≈ 1×2×1, Center подгоняешь под модель) — нужен для рейкастов тапа
   - `PlayerView` (наш скрипт)
4. Дочерним: `Create Empty` → `Anchor` (где-то на уровне головы, +1.5 по Y).
5. Дочерним под `Anchor`: `UI → Text — TextMeshPro` → переименовать в `PowerLabel`.
   Вручную сделать его World Space:
   - Заменить компонент `RectTransform` на `Transform` нельзя, поэтому проще:
     `Create Empty → PowerLabelRoot` (просто Transform), на нём `PowerLabel` (наш скрипт).
     Внутри него — `Canvas (Render Mode = World Space)` с дочерним TMP-текстом.
   - Привязать TMP к полю `_text` в `PowerLabel`.
6. Дочерним: `Create Empty` → `HighlightRing` (туда положишь спрайт круга подсветки;
   изначально SetActive=off — сам код включает).
7. **Animator Controller** (`Assets/_Project/Art/Animations/Player.controller`):
   - Параметры (триггеры): `Idle`, `Run`, `Attack`, `Hit`, `Victory`.
   - Стейты: `Idle` (animation = Hero_idle clip, Loop on), `Run` (Hero_run, Loop on),
     `Attack` (Hero_sword_attack), `Hit` (часть Hero_death или Hero_idle),
     `Victory` (Hero_idle2). Переходы: Any State → state по триггеру.
8. На `PlayerView` инспектор:
   - `Label` ← перетащить `PowerLabel` (компонент)
   - `AnchorPoint` ← `Anchor` (Transform)
   - `HighlightRing` ← (можно оставить пустым)
   - `Animator` ← Animator на корне
   - `BodyRenderer` ← SkinnedMeshRenderer тела рыцаря
   - `DefaultMaterial` ← материал с `DF_Knight_2_HQ.png`
   - `UpgradedMaterial` ← такой же материал, но с `DF_Knight_2_HQ_gold.png`
   - `UpgradeBurst` ← (опционально) ParticleSystem с золотыми искрами
9. **Сохранить как префаб** — drag в `Assets/_Project/Prefabs/Player.prefab`. Из сцены удалить.

---

## 4. Префабы врагов и сундука

**`Enemy_Goblin.prefab`** — аналогично Player:
- Корень: пустой GO + `Goblin_red_idle.fbx` дочерним
- Компоненты на корне: `Animator`, `BoxCollider`, `EnemyView`
- `Anchor` + `PowerLabel` дочерними
- Animator triggers: `Idle`, `Hit`, `Death`
- Поле `VfxOnDeath` ← (можно оставить пустым)

**`Enemy_Ogre.prefab`** — то же самое с `FB_Ogre2.fbx`.

**`Chest.prefab`**:
- Корень: пустой GO + `Chest.FBX` дочерним
- `Animator`, `BoxCollider`, `ChestView`
- Animator triggers: `Open` (анимация открытия — можно собрать из позиций крышки в Animation Window).
- `OpenVfx` ← дочерний ParticleSystem (опционально)
- `IdleBobAmplitude = 0.05`, `IdleBobSpeed = 2`

**`FloatingNumber.prefab`:**
- Корень: пустой GO с `Canvas` (World Space) + `CanvasGroup` + `FloatingNumber` (наш скрипт)
- Дочерний: `TextMeshPro Text (UI)` с большим жирным шрифтом
- `_text` ← TMP, `_group` ← CanvasGroup

**`Vfx_Hit.prefab`, `Vfx_Block.prefab`, `Vfx_ChestOpen.prefab`:**
- Корень: пустой GO + `SpriteRenderer` (sortingLayer = Default или выше)
  + `SpriteSequencePlayer` (наш скрипт)
- `_renderer` ← тот же SpriteRenderer
- `_frames` ← массив спрайтов из `Sequences/...`
- `_fps = 15`, `_loop = false`, `_disableOnFinish = true`

**`UnitSpawner` использует только Player/Enemy/Chest** — вариативность префабов
по `PrefabKey` на этапе MVP не реализована. Если хочешь два типа Enemy (goblin и ogre) —
дай знать, добавлю lookup по ключу в `UnitSpawner`.

---

## 5. Сборка сцены — иерархия

```
Main.unity
├── [GameRoot]                      (Empty, скрипт GameRoot)
├── Main Camera                     (Tag=MainCamera, Orthographic, Size≈6)
│   └── ScreenShake                 (компонент на самой камере)
├── Directional Light               (Shadows = No Shadows)
├── Level
│   ├── BG_Quad                     (Quad с BG_plb.jpg, плоско на фоне)
│   └── UnitSpawner                 (Empty + UnitSpawner)
├── BattleFlow                      (Empty)
│   ├── BattleFlow (script)
│   ├── TapInput (script)
│   ├── TargetIndicator             (Empty + LineRenderer + TargetIndicator)
│   ├── VfxService (script)
│   └── FloatingNumbersRoot         (Empty)
├── Audio                           (Empty + AudioService)
│   ├── Music_Source                (AudioSource, loop=true)
│   ├── Sfx_0..3                    (4 AudioSource, playOnAwake=false)
├── Integration                     (Empty)
│   ├── MraidBridge (script)        ← name на GameObject строго "MraidBridge"
│   └── AnalyticsService (script)
└── UI
    ├── Canvas (Screen Space - Overlay, CanvasScaler: ScaleWithScreenSize, 1080×1920, Match=0.5)
    │   ├── HUD                     (Empty + HudView + HudPresenter)
    │   │   ├── PlayerPower         (TMP_Text)
    │   │   └── Hint                (CanvasGroup + текст "Tap")
    │   └── EndCard                 (Empty + EndCardView + EndCardPresenter)
    │       ├── Panel               (Image, dark overlay)
    │       ├── Title               (TMP_Text)
    │       ├── CTA_Button          (Button + TweenButton)
    │       └── Retry_Button        (Button + TweenButton)
    └── EventSystem                 (Standalone Input Module)
```

### Настройки конкретных нод

**[GameRoot]** (порядок выполнения уже в коде = -1000):
- `LevelConfig` ← `LevelConfig.asset`
- `BalanceConfig` ← `BalanceConfig.asset`
- `Spawner` ← `UnitSpawner` (со сцены)
- `BattleFlow` ← `BattleFlow` (со сцены)
- `Camera` ← `Main Camera` (или оставить пусто — возьмёт `Camera.main`)

**Main Camera**:
- `Projection = Orthographic`, `Size = 6`
- `Position = (0, 4, -8)`, `Rotation = (35, 0, 0)` — лёгкий top-down угол
- `Clear Flags = Solid Color` (чёрный)
- `Background` color, `Culling Mask = Default`
- `Allow HDR = off`, `Allow MSAA = off`, `Post Processing = off`

**Directional Light**: `Shadow Type = No Shadows`, `Intensity ≈ 1.1`, направить под ~45°.

**Level → BG_Quad**:
- 3D `Quad`, текстура — `BG_plb.jpg`. Материал — `Unlit/Texture`. Развернуть лицом к камере, увеличить scale, чтобы заполнял кадр.

**UnitSpawner**:
- `PlayerPrefab` ← `Player.prefab`
- `EnemyPrefab` ← `Enemy_Goblin.prefab` (или `Enemy_Ogre.prefab` — текущий MVP-spawner один для всех врагов)
- `ChestPrefab` ← `Chest.prefab`
- `Root` ← сам `Level` Transform или дочерний `Level/Spawned`

**TargetIndicator** (под BattleFlow):
- LineRenderer: `Material = Sprites/Default`, `Width = 0.08`, `Use World Space = on`,
  `Texture Mode = Stretch`, `Positions Count = 2`. Material — простой цветной (жёлтый).
- `_selectionGlow` ← (опционально) префаб круга-glow под целью

**BattleFlow** (script, на ноде BattleFlow):
- `Input` ← `TapInput` (на этой же ноде)
- `Indicator` ← `TargetIndicator`
- `Vfx` ← `VfxService`
- `Shake` ← `ScreenShake` (на камере)
- `FloatingNumberPrefab` ← `FloatingNumber.prefab`
- `FloatingNumbersRoot` ← `FloatingNumbersRoot` Transform

**TapInput** (на той же ноде что BattleFlow):
- `Camera` ← `Main Camera`
- `TappableMask` ← `Default` или отдельный layer `Tappable` (если решишь развести)

**VfxService** (на той же ноде):
- `Bank` ← `VfxBank.asset`
- `AutoDestroy = 2.5`

**ScreenShake** — компонент прямо на `Main Camera`, без полей.

**AudioService** (на ноде Audio):
- `Bank` ← `AudioBank.asset`
- `Music` ← `Music_Source` AudioSource
- `SfxPool` ← массив из 4 AudioSource

**Audio Source настройки** (для всех):
- `Force To Mono = on` (для импорта клипов в Inspector)
- `Quality = 30–50%` (для Vorbis), `Sample Rate = 22050 Hz Override`
- `Bypass Effects = on`

**MraidBridge** — обязательно `GameObject.name = "MraidBridge"` (по нему JS ищет ноду через SendMessage).
- `Audio` ← `AudioService`
- `CtaUrl` ← https://your-store-link.example

**AnalyticsService** — без полей.

**Canvas → HUD**:
- `HudView`: `_playerPower` ← TMP_Text, `_hint` ← CanvasGroup
- `HudPresenter`: `_view` ← `HudView`

**Canvas → EndCard**:
- `EndCardView`: `_group` ← CanvasGroup на самом EndCard, `_ctaButton` ← Button, `_retryButton` ← Button (или null), `_title` ← TMP_Text
- `EndCardPresenter`: `_view` ← `EndCardView`, `_winTitle = "Победа!"`

**TweenButton** — повесить на `CTA_Button` и `Retry_Button` (RectTransform-only, никаких ссылок).

---

## 6. Player Settings и Build Settings

1. `File → Build Profiles → WebGL → Switch Platform`.
2. `Player → Resolution and Presentation → WebGL Template = PROJECT/Playable`.
3. `Player → Other Settings → Active Input Handling = Input Manager (Old)` или `Both`
   (мы используем `Input.GetMouseButtonDown`, новый InputSystem не подключён).
4. `Player → Other Settings → Color Space = Gamma`.
5. Убедиться, что `Main.unity` стоит первой в Scene List.

Остальное (`Brotli`, `Strip High`, `IL2CPP Master` и т.д.) выставит сам
`Playable → Build → WebGL → Single HTML` через `BuildPipeline.ConfigurePlayer()`.

---

## 7. Прогон

`Edit → Play` — должен выйти HUD с числом 5, четыре цели на сцене с числами,
тап атакует/собирает сундук, после уничтожения всех врагов появляется EndCard.

**Тесты домена**: `Window → General → Test Runner → EditMode → Run All` —
5 тестов в `Project.Domain.Tests` должны быть зелёными.

**Финальная сборка**: `Playable → Build → WebGL → Single HTML`. По окончании
`Build/Playable/index.html` должен быть около 4–4.5 MB и запускаться
двойным кликом без интернета.

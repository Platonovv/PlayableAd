# Playable Ad — Knight vs Mobs

HTML5 playable: рыцарь выбирает цель, ориентируясь на числовую силу. Сначала сундук — апгрейд, потом враги. Победа — все враги уничтожены, после чего показывается end-card с CTA. Если игрок попытается ударить врага сильнее себя — погибает, появляется Lose-карточка с Retry.

**Целевая платформа:** WebGL, portrait, 60 FPS, бюджет ≤ 5 MB, длительность 15–30 сек.

---

## Содержание

- [Стек](#стек)
- [Архитектура](#архитектура)
- [Геймплей-флоу](#геймплей-флоу)
- [Сборка](#сборка)
- [Локальный запуск](#локальный-запуск)
- [Edit-time утилиты](#edit-time-утилиты)
- [Анимации в Unity Playworks](#анимации-в-unity-playworks)
- [Unity Playworks SDK интеграция](#unity-playworks-sdk-интеграция)
- [Оптимизации размера](#оптимизации-размера)
- [Что бы улучшил при наличии времени](#что-бы-улучшил-при-наличии-времени)
- [Граф сцены](#граф-сцены)

---

## Стек

| Решение | Версия | Обоснование |
|---|---|---|
| **Unity** | 2022.3.62f2 LTS | LTS-ветка с минимальным wasm-следом под playable. Unity 6 на пустом проекте даёт +2-3 MB к билду — выбор сознательный. |
| **C# language** | 9.0 | Records, target-typed `new`, pattern matching, `init`-сеттеры. Поддерживается Unity 2022.3 без отдельной настройки. C# 10/11 не используются ради совместимости с Mono-компилятором Unity и трансляцией Unity Playworks Plugin в JS. |
| **API Compatibility** | .NET Standard 2.1 | Минимальный wasm-след. |
| **Built-in Render Pipeline** | — | URP в WebGL даёт +200–400 KB к wasm и десятки лишних shader variants. Сцена плоская — берём минимум. |
| **Unity Playworks Plugin** | 7.2.0 | Пайплайн транспиляции C# → JS для финального playable-билда (single HTML, ≤5 MB). Подключён как `file:../../UnityPlug/scripts`. |
| **Legacy `Animation`** | — | Mecanim/Animator у Unity Playworks Plugin в WebGL фризит skinned mesh в bind-pose. Legacy `UnityEngine.Animation` обходит баг и работает 100%. См. [Анимации в Unity Playworks](#анимации-в-unity-playworks). |
| **Tween** (свой, на корутинах) | — | DOTween не подключаем — экономия ~150 KB. Покрываем нужные кривые (linear/outQuad/outBack/outSine). Все методы устойчивы к уничтожению цели во время анимации. |
| **Без TextMeshPro** | — | TMP даёт +6.5 MB шрифтов/шейдеров и ломается при стрипе Unity Playworks Plugin. Вместо неё стандартный `UnityEngine.UI.Text` со встроенным шрифтом. |
| **Без VContainer/Zenject** | — | Граф зависимостей маленький — `GameRoot` собирает его руками через `[SerializeField]`. Никаких singleton'ов и `Find`. |
| **Без LeoEcsProto / DOTS** | — | Под core loop из 5–6 действий ECS — overengineering. Domain-модель — pure C# без UnityEngine. |
| **Без Addressables** | — | Single-file билд, грузить нечего. |

### Unity-модули, оставленные в `Packages/manifest.json`

`animation`, `audio`, `jsonserialize`, `particlesystem`, `physics`, `ui`, `ugui`. Всё лишнее (URP, TextMeshPro, VS, Timeline, AI Nav, Cloth, Terrain, Vehicles, XR, Tilemap, Multiplayer, AR, Postprocessing, Cinemachine, Test Framework, ImageConversion, IMGUI, UIElements, UnityWebRequest) удалено.

---

## Архитектура

```
Domain  (без UnityEngine, asmdef noEngineReferences)
  → Battle, Unit, Power, IBattleAction, BattleEvents, generic StateMachine

Core (Runtime asmdef)
  → GameRoot (composition root), SignalBus, Pool<T>, Tween/Ease, Signals

Gameplay
  → Units:    PlayerView, EnemyView, ChestView, UnitView (Legacy Animation), PowerLabel
  → Targeting: TapInput (press-and-release), TargetIndicator (dashed line),
              ITappable
  → Vfx:      VfxService, VfxBank, FloatingNumber (flying numbers),
              SpriteSequencePlayer, FootstepEmitter
  → Camera:   ScreenShake, CameraFollow (опционально, по дефолту off)
  → Flow:     BattleFlow + 6 состояний (Idle/Moving/Attack/ChestOpen/Won/Lost)
  → UnitSpawner (читает LevelConfig + UnitsBank)

UI       → HudView/HudPresenter (опц. счётчик силы), EndCardView/EndCardPresenter
Audio    → AudioBank (SO), AudioService (подписка на сигналы)
Integration → MraidBridge (WebGL JSLib + Unity Playworks SDK через рефлексию + mock-фолбэк),
              AnalyticsService
Configs   → BalanceConfig, LevelConfig, UnitsBank, VfxBank, AudioBank (все SO)
Editor    → SceneBuilder, SequenceTexturePostprocessor, MaterialFixer,
            MaterialAutoFill, AnimationClipRenamer, FbxSlimmer, BuildSizeAudit
```

**Ключевые паттерны:**
- **Domain/View разделение** — Domain pure C#, View = MB. Бой логически тестируется без сцены.
- **State Machine** — generic `StateMachine<TContext>`, 6 состояний боя, контекст инжектится явно через `BattleFlowContext`.
- **MVP-lite** в UI: View рендерит, Presenter подписан на SignalBus.
- **SignalBus** — типизированный pub/sub без реактивных фреймворков.
- **ScriptableObject configs** — все балансы, расстановка уровня, банки префабов крутятся в инспекторе.
- **Press-and-release input** — `TapInput` показывает preview при зажатии, коммит на отпускании.
- **Все ссылки через `[SerializeField]`** — без singleton'ов, `Find*`, или `GetComponent` (кроме одного raycast-вызова).

---

## Геймплей-флоу

1. **Idle** — TapInput слушает зажатие. При press-and-hold над врагом/сундуком включается preview: подсветка, scale-up, dashed-стрелка к цели. Цвет стрелки: зелёный — сундук, синий — враг которого победим, красный — враг сильнее нас.
2. **MovingToTarget** — игрок бежит к точке `view.Stop` (отдельная Transform на префабе, не центр). Trail-следы отпечатываются за ним через `FootstepEmitter`.
3. **AttackState** —
   - Враг разворачивается лицом к игроку.
   - Если `player.power >= enemy.power`: игрок бьёт (или `SuperAttack` если power > порога), враг получает Hit, +N летит к лейблу игрока, лейбл pop'ит, игрок делает рандомный flair (Punch/Hop/Squash/DoubleHop), враг проигрывает Death-анимацию и исчезает.
   - Иначе: враг сам контратакует, игрок получает урон, проигрывает Death → GoLost.
4. **ChestOpenState** — сундук «дёргается» (anticipation), затем открывается; меч из сундука летит по дуге к игроку с боковым wobble в апексе; на приземлении бон-меч активируется + игрок проигрывает Upgrade с flair.
5. **WonState/LostState** — терминальные, EndCard показывает соответствующую карточку (Win с CTA + Retry; Lose только с Retry). При входе в любой из терминалов вызывается `Luna.Unity.LifeCycle.GameEnded()` — обязательное требование рекламных сетей.

---

## Сборка

Финальный playable собирается **через Unity Playworks Plugin** — это даёт single-file inline HTML под бюджет ≤5 MB рекламных сетей.

### Шаги

1. **Unity:** `Window → Unity Playworks Plugin` (Ctrl+E) → вкладка **«Сборка»**, кнопка «Собрать проект» (быстрая локальная итерация) или сразу вкладка **«Загрузка в Creative Library»** для production-залива.
2. **Перед загрузкой обязательно:** в **Playable Settings** на Concept-уровне в Hub заполнить **Apple URL** и **Google Play URL** — без них рекламные сети отклоняют плейбл.
3. **Hub (браузер, [create.lunalabs.io](https://create.lunalabs.io)):** открой свой Application → Concept `playable_v1` → версия → **Publish or Download**.
4. В Publish-панели включи нужные ad-сети (Unity Ads / Google Ads / AppLovin / Mintegral / IronSource / Vungle / Meta) → **Download Creatives** → ZIP с папками per-network → внутри `index.html` каждой сети — это финальный single-file inline playable. Размер каждого экспорта Hub показывает прямо в UI — выбираешь нужный.

Перед сборкой проверь вкладку **«Диагностика проекта»** — все блоки должны быть зелёными или с минимальными warning'ами. Особенно:
- **Главная функциональность** — критично, ошибки тут блокируют сборку;
- **Рекламные сети** — закрывается интеграцией Luna SDK (см. ниже);
- **Технические характеристики / Лучшие практики** — желтые треугольники не блокируют, но косметика для модерации.

---

## Локальный запуск

1. Открыть проект в Unity 2022.3.62f2 LTS (Unity Hub → Open).
2. Открыть `Assets/_Project/Scenes/Main.unity`.
3. Press Play.

Если сцены нет (свежий клон) — `Playable → Tools → Build Main Scene` соберёт её одной кнопкой (иерархия, компоненты, ссылки, дефолтные SO).

---

## Edit-time утилиты

| Меню | Что делает |
|---|---|
| `Playable → Tools → Build Main Scene` | Собирает сцену в один клик: иерархия, компоненты, ссылки, дефолтные SO. |
| `Playable → Tools → Build Size Audit` | Сканит ассеты под `Assets/_Project/`, печатает неиспользуемые с размерами, предлагает удалить. |
| `Playable → Tools → Auto Fill Material Textures` | Прогоняет материалы под `Art/`, заполняет пустые `_MainTex` по совпадению имён, фиксит URP-шейдеры. |
| `Playable → Tools → Fix Pink Materials` | Переводит URP/HDRP-шейдеры на `Mobile/Diffuse`. |
| `Playable → Tools → Sanitize FBX Animation Clip Names` | Чистит имена клипов внутри FBX от символов, недопустимых в путях Windows (типа `|` из `Armature|Run`) — Unity Playworks Plugin без этого падает на упаковке. |
| `Playable → Tools → FBX Slimmer` | Ужимает FBX-импорт: компрессия мешей, оптимальные skin-weights. |
| `Window → Unity Playworks Plugin` (Ctrl+E) | Главное окно плагина: Сборка / Загрузка в Creative Library / Анализ размера / Настройки. |

---

## Анимации в Unity Playworks

**Проблема.** Unity Playworks Plugin 7.2.0 не корректно поднимает Mecanim Animator в WebGL: state-machine тикает, клипы резолвятся, кости присутствуют — но bone-transforms не применяются к skinned mesh, модель замирает в bind-pose.

**Решение — Legacy Animation.** В проекте все юниты переведены на `UnityEngine.Animation` (legacy):

1. **FBX импорт:** Rig → **Animation Type = Legacy**, Strip Bones = OFF, Animation Compression = Off.
2. **На префабе** на той же ноде где был `Animator` (внутри FBX, типа `Hero_idle_sword_02`) добавлен компонент `Animation`. В его массив `Animations` положены все нужные клипы заранее (Unity Playworks Plugin стрипает `AddClip/GetClip` — динамически добавлять нельзя).
3. **В UnitView** добавлено поле `LegacyClipMap` — массив `[StateName, Clip, Loop]`. Код `PlayAnim(state)` находит клип по StateName и вызывает `LegacyAnim.Play(clip.name)`.
4. **WrapMode** клипов задаётся в Awake через `clip.wrapMode = Loop ? WrapMode.Loop : WrapMode.Once` по флагу из маппинга. Это работает потому что Unity Playworks Plugin стрипает `AnimationState.wrapMode`, но не стрипает `AnimationClip.wrapMode`.

При добавлении нового юнита: положи в `Animation.Animations` нужные клипы и в `UnitView.LegacyClipMap` пропиши маппинг `Idle → ClipName`, `Attack → ClipName` и т.д.

---

## Unity Playworks SDK интеграция

Рекламные сети требуют двух вызовов SDK:

| API | Когда вызывается | Зачем |
|---|---|---|
| `Luna.Unity.Playable.InstallFullGame()` | На клик по CTA в end-card | Сеть открывает свой нативный store-CTA вместо нашего `OpenStore(url)`. |
| `Luna.Unity.LifeCycle.GameEnded()` | На `BattleWonSignal`/`BattleLostSignal` | Сеть переключается на свой нативный end-card / показывает install-prompt. |

Оба вызова идут через **рефлексию** в [`MraidBridge.cs`](Assets/_Project/Scripts/Integration/MraidBridge.cs) (`System.Type.GetType("Luna.Unity.Playable, Unity.Luna")…` — это namespace и сборка поставляются плагином). В Editor рефлексия молча возвращает `false` → код падает на `Playable_OpenStore(url)` mock. В Playworks-сборке сети находят правильные методы и плейбл проходит валидаторы.

---

## Оптимизации размера

| Источник | Что сделано |
|---|---|
| **Unity package set** | Удалены TextMeshPro (-6.5 MB), URP, Visual Scripting, Timeline, AI Navigation, Cloth, Terrain, Vehicles, XR/VR, Tilemap, Multiplayer, AR, Postprocessing, Cinemachine, Test Framework, ImageConversion, IMGUI, UIElements, UnityWebRequest. |
| **IL2CPP / WebGL Player** | `Code Optimization = Master`, `Code Generation = OptimizeSize`, `Exception Support = None`, `Compression = Brotli`, `Memory = 64 MB`, `linkerTarget = Wasm`, `dataCaching = false`, `threadsSupport = false`. |
| **Color space** | Gamma — экономия shader variants. |
| **Render** | Built-in RP, без shadows, без AA, без anisotropic, без realtime reflections. `gpuSkinning = false`. |
| **3D-ассеты** | Низкополигональные skinned meshes с unlit-материалами (Mobile/Diffuse). Текстуры — `RGBA Crunched`, q=50, max 512 px. |
| **VFX** | `SequenceTexturePostprocessor` авто-настраивает PNG-секвенции в `Art/Sequences/` и `Art/FX/`: max 512 px, crunch q=50. |
| **Audio** | mono, 22050 Hz, Vorbis q=2 (sfx) / q=3 (music). 4.1 MB исходников → 103 KB после конвертации. |
| **BG** | Процедурно генерируется через `_make_bg.py` (gradient + tree silhouettes + dock + waves). ~50 KB JPG. |
| **link.xml** | Сохранены только используемые модули (`UnityEngine.AnimationModule`, `UnityEngine.UI`, `UnityEngine.UIModule`, `UnityEngine.IMGUIModule`, `UnityEngine.TextRenderingModule`). |
| **Tests removed** | Юнит-тесты и test-framework пакет вырезаны из проекта. |

---

## Что бы улучшил при наличии времени

- **Локализация** под несколько языков (сейчас один).
- **Адаптивная вёрстка** UI под экстремальные aspect ratio (9:21+).
- **Pre-bake hero animations в sprite atlas** — снимет ещё 200–400 KB и уберёт остаточные skinned-mesh-нюансы Unity Playworks Plugin.
- **A/B-варианты конфигов уровня** через query-string.
- **Расширенная аналитика**: тайминги между взаимодействиями, hit-rate первого тапа, drop-off по фазам.
- **Туториал-нудж** (анимированная стрелка) если игрок не сделал тап за 3 секунды.

---

## Граф сцены

```
[GameRoot]                         (composition root, [DefaultExecutionOrder(-1000)])
├─ Main Camera                     (Perspective, FOV 35, position (0, 40, -32), rotation (50,0,0))
│   ├─ ScreenShake (component, FX: shake / FOV pulse / push-offset)
│   └─ BG (Quad child, перпендикуляр камеры, текстура BG_plb_clean.jpg)
├─ Directional Light               (Intensity 0.55, тёплый dim, без теней)
├─ Level
│   └─ UnitSpawner                 (читает LevelConfig + UnitsBank)
├─ BattleFlow                      (FSM + сервисы)
│   ├─ TapInput
│   ├─ VfxService
│   ├─ TargetIndicator (dashed line)
│   └─ FloatingNumbersRoot
├─ Audio
│   ├─ Music_Source (AudioSource)
│   └─ Sfx_0..3 (4 AudioSource pool)
├─ Integration
│   ├─ MraidBridge (Unity Playworks SDK + JS bridge)
│   └─ AnalyticsService
├─ Canvas (Screen Space — Overlay, Reference 1080×1920)
│   ├─ HUD          (HudView + HudPresenter)
│   └─ EndCard      (EndCardView + EndCardPresenter, Win/Lose варианты)
└─ EventSystem
```


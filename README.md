# Playable Ad — Knight vs Mobs

HTML5 playable: рыцарь выбирает цель, ориентируясь на числовую силу. Сначала сундук — апгрейд, потом враги. Победа = все враги уничтожены, после чего показывается end-card с CTA. Если игрок попытается ударить врага сильнее себя — погибает, появляется Lose-карточка с Retry.

Целевая платформа: WebGL, portrait, ~5 MB, 60 FPS.

---

## Стек

| Решение | Обоснование |
|---|---|
| **Unity 2022.3.62f2 LTS** | LTS-ветка с минимальным wasm под playable. Unity 6 даёт +2-3 MB к билду на пустом проекте — это выбор сознательный. |
| **Built-in Render Pipeline** | URP в WebGL даёт +200–400 KB к wasm и десятки лишних shader variants. Сцена плоская — берём минимум. |
| **UniTask** | Async-цепочки (move → attack → flair) без аллокаций. |
| **Своя `Tween`** (UniTask + Ease curves) | DOTween не подключаем — экономия ~150 KB. Покрываем только нужные кривые (linear/outQuad/outBack/outSine). Все методы устойчивы к уничтожению цели во время анимации. |
| **Без VContainer/Zenject** | Граф зависимостей маленький — `GameRoot` собирает его руками через `[SerializeField]`. Никаких singleton'ов и `Find`. |
| **Без LeoEcsProto / DOTS** | Под core loop из 5–6 действий ECS — overengineering. Domain-модель — pure C# без UnityEngine. |
| **Без Addressables** | Single-file билд, грузить нечего. |

## Архитектура

```
Domain  (без UnityEngine, asmdef noEngineReferences)
  → Battle, Unit, Power, IBattleAction, BattleEvents, generic StateMachine

Core (Runtime asmdef)
  → GameRoot (composition root), SignalBus, Pool<T>, Tween/Ease, Signals

Gameplay
  → Units:    PlayerView, EnemyView, ChestView, UnitView, PowerLabel
  → Targeting: TapInput (press-and-release), TargetIndicator (dashed line),
              ITappable
  → Vfx:      VfxService, VfxBank, FloatingNumber (flying numbers),
              SpriteSequencePlayer, FootstepEmitter
  → Camera:   ScreenShake, CameraFollow (опционально, по дефолту off)
  → Flow:     BattleFlow + 6 состояний (Idle/Moving/Attack/ChestOpen/Won/Lost)
  → UnitSpawner (читает LevelConfig + UnitsBank)

UI       → HudView/HudPresenter (опц. счётчик силы), EndCardView/EndCardPresenter
Audio    → AudioBank (SO), AudioService (подписка на сигналы)
Integration → MraidBridge (WebGL JSLib + mock-фолбэк), AnalyticsService
Configs   → BalanceConfig, LevelConfig, UnitsBank, VfxBank, AudioBank (все SO)
Editor    → SceneBuilder, BuildPipeline, SingleFileInliner,
            SequenceTexturePostprocessor, MaterialFixer, MaterialAutoFill,
            AvatarSync, BuildSizeAudit
```

**Ключевые паттерны:**
- **Domain/View разделение** — Domain pure C#, View = MB. Бой логически тестируется без сцены.
- **State Machine** — generic `StateMachine<TContext>`, 6 состояний боя, контекст инжектится явно через `BattleFlowContext`.
- **MVP-lite** в UI: View рендерит, Presenter подписан на SignalBus.
- **SignalBus** — типизированный pub/sub без реактивных фреймворков.
- **ScriptableObject configs** — все балансы, расстановка уровня, банки префабов крутятся в инспекторе.
- **Press-and-release input** — `TapInput` показывает preview при зажатии, коммит на отпускании.
- **Все ссылки через `[SerializeField]`** — без singleton'ов, `Find*`, или `GetComponent` (кроме одного raycast-вызова).

## Геймплей-флоу

1. **Idle** — TapInput слушает зажатие. При press-and-hold над врагом/сундуком включается preview: подсветка, scale-up, dashed-стрелка к цели. Цвет стрелки: 🟢 сундук, 🔵 враг которого победим, 🔴 враг сильнее нас.
2. **MovingToTarget** — игрок бежит к точке `view.Stop` (отдельная Transform на префабе, не центр). Trail-следы отпечатываются за ним через `FootstepEmitter`.
3. **AttackState** —
   - Враг разворачивается лицом к игроку.
   - Если `player.power >= enemy.power`: игрок бьёт (или `SuperAttack` если power > порога), враг получает Hit, +N летит к лейблу игрока, лейбл pop'ит, игрок делает рандомный flair (Punch/Hop/Spin), враг проигрывает Death-анимацию и исчезает.
   - Иначе: враг сам контратакует, игрок получает урон, проигрывает Death → GoLost.
4. **ChestOpenState** — игрок Victory-анимация во время открытия, +N летит, апгрейд (Upgrade-state + смена материала на gold + рандомный flair).
5. **WonState/LostState** — терминальные, EndCard показывает соответствующую карточку (Win с CTA + Retry; Lose только с Retry).

## Как собрать и запустить

### Локальный запуск из Editor
1. Открыть проект в Unity 2022.3.62f2 (Unity Hub → Open).
2. `Playable → Tools → Build Main Scene` — генерирует `Main.unity` со всей иерархией и SO-конфигами.
3. Открыть `Assets/_Project/Scenes/Main.unity`.
4. Перед Play подкрутить префабы (см. `SCENE_SETUP.md` если они ещё не настроены).
5. Press Play.

### WebGL build & run
- **`Playable → Build → WebGL → Build Single HTML`** — собирает оптимизированный WebGL-билд, склеивает его в один `Build/Playable/index.html` через `SingleFileInliner` и сразу открывает в дефолтном браузере по `file://`. Это и есть deliverable «один HTML-файл». Под капотом: brotli из Unity-сборки декомпрессится в C#, пережимается в gzip и инлайнится base64-ом. На рантайме браузер распаковывает через `DecompressionStream('gzip')` (нативный API, Chromium ≥80, Safari ≥16.4, Firefox ≥113).

### Edit-time утилиты

| Меню | Что делает |
|---|---|
| `Playable → Tools → Build Main Scene` | Собирает сцену в один клик: иерархия, компоненты, ссылки, дефолтные SO. |
| `Playable → Tools → Build Size Audit` | Сканит все ассеты под `Assets/_Project/`, печатает неиспользуемые с размерами, предлагает удалить. |
| `Playable → Tools → Auto Fill Material Textures` | Прогоняет материалы под `Art/`, заполняет пустые `_MainTex` по совпадению имён, фиксит URP-шейдеры. |
| `Playable → Tools → Fix Pink Materials` | То же, но более агрессивно — переводит URP/HDRP-шейдеры на `Mobile/Diffuse`. |
| `Playable → Tools → Sync Avatars in Selection` | Для Generic-rig'ов: первый выделенный FBX становится «мастером» Avatar, остальные копируют. Без этого animation-clips из разных FBX не играют на одном меше. |
| `Playable → Build → Configure Player Only` | Применяет настройки Player без сборки. |

## Применённые оптимизации размера

| Источник | Что сделано |
|---|---|
| **Unity package set** | Удалены URP, Visual Scripting, Timeline, AI Navigation, Cloth, Terrain, Vehicles, XR/VR, Tilemap, Multiplayer, AR, Postprocessing, Cinemachine, Test Framework, ImageConversion, IMGUI, UIElements, UnityWebRequest. |
| **IL2CPP** | `Code Optimization = Master`, `Code Generation = OptimizeSize`, `Managed Stripping = High`, `Exception Support = None`. |
| **WebGL** | `Brotli`, `Memory = 64 MB`, `linkerTarget = Wasm`, `dataCaching = false`, `threadsSupport = false`. |
| **Color space** | Gamma — экономия shader variants. |
| **Render** | Built-in RP, без shadows, без AA, без anisotropic, без realtime reflections. `gpuSkinning = false`. |
| **3D-ассеты** | Низкополигональные skinned meshes с unlit-материалами (Mobile/Diffuse). Текстуры — `RGBA Crunched`, q=50, max 512 px. |
| **VFX** | `SequenceTexturePostprocessor` авто-настраивает PNG-секвенции в `/Art/Sequences/` и `/Art/FX/`: max 512 px, crunch q=50. |
| **Audio** | mono, 22050 Hz, Vorbis q=2 (sfx) / q=3 (music). 4.1 MB исходников → 103 KB после конвертации. |
| **BG** | Процедурно генерируется через `_make_bg.py` — простой gradient + tree silhouettes + dock + waves. ~50 KB JPG. |
| **Tests removed** | Из проекта вырезаны юнит-тесты и test-framework пакет. |

## Single-file inline — как это работает

`SingleFileInliner` склеивает loader/framework/wasm/data в один `Build/Playable/index.html`. Проблема Unity 2022.3 brotli (loader детектит компрессию по расширению URL, а `data:` URI расширения не имеет) обходится так:

1. Brotli-файлы из `Build/WebGL/Build/` декомпрессируются в C# (`System.IO.Compression.BrotliStream`).
2. Пережимаются в gzip (`GZipStream`, Optimal).
3. Инлайнятся в HTML как base64 в трёх переменных `window.__playable_data/framework/code`.
4. На рантайме helper в `<script>` распаковывает их через `new DecompressionStream('gzip')` — нативный браузерный API (Chromium ≥80, Safari ≥16.4, Firefox ≥113), без js-полифилов.
5. Получившиеся `Blob` оборачиваются в `URL.createObjectURL`, эти blob-URL подменяют `dataUrl/frameworkUrl/codeUrl` в config'е перед `createUnityInstance`.

Размер итогового HTML ≈ brotli × (gzip/brotli ≈ 1.05–1.10) × base64 (1.33). На наших ассетах укладываемся в 5 MB. Альтернативный deliverable — папка `Build/WebGL/` через zip-пакет (стандарт для AppLovin/Vungle/IronSource).

## Что бы улучшил при наличии времени

- **Локализация** под несколько языков (сейчас один).
- **Адаптивная вёрстка** UI под экстремальные aspect ratio (9:21+).
- **Pre-bake hero animations в sprite atlas** — снимет ещё 200–400 KB.
- **A/B-варианты конфигов уровня** через query-string.
- **Расширенная аналитика**: тайминги между взаимодействиями, hit-rate первого тапа, drop-off по фазам.
- **Туториал-нудж** (анимированная стрелка) если игрок не сделал тап за 3 секунды.

## Граф сцены

```
[GameRoot]                         (composition root, [DefaultExecutionOrder(-1000)])
├─ Main Camera                     (Perspective, FOV 35, position (0, 40, -32), rotation (50,0,0))
│   ├─ ScreenShake (component)
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
│   ├─ MraidBridge
│   └─ AnalyticsService
├─ Canvas (Screen Space — Overlay, Reference 1080×1920)
│   ├─ HUD          (HudView + HudPresenter)
│   └─ EndCard      (EndCardView + EndCardPresenter, Win/Lose варианты)
└─ EventSystem
```

SceneBuilder автоматически создаёт всё это и привязывает все ссылки `[SerializeField]`. После прогона остаётся только подвесить префабы юнитов в `UnitsBank` и VFX-префабы в `VfxBank` — это делается один раз.

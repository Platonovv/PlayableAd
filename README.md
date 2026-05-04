# Playable Ad — Knight vs Mobs

HTML5 playable: игрок-рыцарь выбирает цель, ориентируясь на числовую силу. Сначала
сундук, потом враг — иначе отскок без потери очков. Победа = все враги уничтожены,
после чего показывается end-card с CTA.

Целевая платформа: WebGL, portrait, ~5 MB, 60 FPS.

---

## Стек и почему именно так

| Решение | Обоснование |
|---|---|
| **Unity 6000.3.9f1** | Уже установлен у команды; стабильный WebGL-пайплайн с brotli + IL2CPP. |
| **Built-in Render Pipeline** (не URP) | URP в WebGL даёт +200–400 KB к wasm и десятки лишних shader variants. Сцена плоская, 2D Lights и Shader Graph не нужны — берём минимум. |
| **UniTask** | Для async-цепочек (move → attack → hit). Альтернатива coroutines, но без аллокаций. |
| **Своя `Tween`** (UniTask + AnimationCurve) | DOTween не подключаем — экономия ~150 KB. Покрываем только реально нужные кривые. |
| **Без VContainer/Zenject** | Граф зависимостей маленький (см. `GameRoot`). Ручная композиция читается лучше и весит меньше, чем рефлексивная инъекция. |
| **Без LeoEcsProto / DOTS** | Под core loop из 5–6 действий ECS — overengineering. Domain-модель хранится в чистом C# и тестируется без сцены. |
| **Без Addressables** | Билд single-file, грузить нечего; пакет только раздул бы wasm. |

## Архитектура

```
Domain  (без UnityEngine)        → Battle, Unit, Power, IBattleAction, BattleEvents, generic StateMachine
Core    (Runtime asmdef)         → GameRoot, SignalBus, Pool<T>, Tween, Signals
Gameplay                         → PlayerView, EnemyView, ChestView, TapInput, TargetIndicator,
                                    BattleFlow + 5 состояний (Idle/Moving/Attack/ChestOpen/Won),
                                    UnitSpawner, VfxService, ScreenShake, FloatingNumber
UI                               → HudView/HudPresenter, EndCardView/EndCardPresenter (MVP-lite)
Audio                            → AudioBank (SO), AudioService — подписка на сигналы
Integration                      → MraidBridge (WebGL JSLib + mock-фолбэк), AnalyticsService
Configs                          → BalanceConfig, LevelConfig, VfxBank, AudioBank (SO)
Editor                           → BuildPipeline, SingleFileInliner, SequenceTexturePostprocessor
```

**Ключевые паттерны.** Domain/View разделение (Domain-слой — pure C# без UnityEngine, можно покрыть
тестами без поднятия сцены); generic State Machine с context-параметром; типизированный SignalBus
вместо `UnityEvent`; ScriptableObject configs вместо хардкода; `ITappable` — стратегия, чтобы
добавление нового типа цели не трогало `Battle`.

## Как собрать и запустить

### Локальный запуск из Editor
1. Открыть проект в Unity 6000.3.9f1 (Unity Hub → Open).
2. Открыть `Assets/_Project/Scenes/Main.unity`.
3. Press Play.

### WebGL single-file сборка
1. `Playable → Build → WebGL → Single HTML` в меню Editor.
2. Подождать сборку IL2CPP (5–10 минут на холодную).
3. Готовый файл — `Build/Playable/index.html`. Открывается двойным кликом
   (нет внешних зависимостей, грузится из `data:`-URI).

### Размер
Итоговый HTML включает в себя:
- `loader.js` — встраивается как script-блок;
- `framework.js`, `wasm`, `data` — base64 от brotli-сжатых файлов.

Бюджет, на который рассчитывали: ~4.4 MB (см. ниже).

## Применённые оптимизации размера

| Источник | Что сделано |
|---|---|
| **Unity package set** | Удалены URP, Visual Scripting, Timeline, AI Navigation, Cloth, Terrain, Vehicles, XR/VR, Tilemap, Multiplayer, AR, Postprocessing, Cinemachine. Минимум модулей — только `animation`, `audio`, `imageconversion`, `particlesystem`, `physics`, `ui`, `unitywebrequest`. |
| **IL2CPP** | `Code Optimization = Master`, `Code Generation = OptimizeSize`, `Managed Stripping = High`, `Exception Support = None`. |
| **WebGL** | `Brotli`, `decompressionFallback = true` (нужен для inlined data:-URI), `Memory = 64 MB`, `linkerTarget = Wasm`, `dataCaching = false`, `threadsSupport = false`. |
| **Color space** | Gamma (не Linear) — мобильный таргет, разница в качестве на материалах сцены незаметна, экономия в shader variants. |
| **Render** | Built-in RP, без shadows, без anti-aliasing, без anisotropic, без realtime reflections. `gpuSkinning = false` (CPU skinning выгоднее на мобильном WebGL). |
| **3D-ассеты** | Hero/Enemy/Chest — низкополигональные skinned meshes с unlit-материалами. Текстуры — `RGBA Crunched`, quality 50, max 512 px. |
| **VFX** | Спрайт-секвенции прогоняются через `SequenceTexturePostprocessor`: max 512 px, crunch q=50. Помогает экономить мегабайты на сырых PNG из материалов. |
| **Audio** | Vorbis quality 0.3, mono, 22050 Hz. Остаются только используемые клипы; всё остальное из материалов вырезано. |
| **Build pipeline** | `SingleFileInliner` собирает WebGL-вывод в один HTML без сетевых запросов — играется даже из `file://`. |

## Что бы улучшил при наличии времени

- **Локализация** под несколько языков (сейчас текст только на одном).
- **Адаптивная вёрстка** UI под экстремальные aspect ratio (телефоны 9:21+) с пересчётом сейф-зон.
- **Pre-bake hero animations в sprite atlas** — потенциально снимет ещё 200–400 KB и
  уберёт необходимость в SkinnedMeshRenderer на WebGL.
- **A/B-варианты конфигов уровня** через query-string (например, `?level=hard`),
  чтобы маркетинг крутил эксперименты без ребилда.
- **Расширенная аналитика**: тайминги между взаимодействиями, hit-rate первого тапа,
  drop-off по фазам — добавил бы только обвязку, сами события уже эмитятся через `SignalBus`.
- **Полноценный туториал-нудж** (анимированная стрелка) для первых 3 секунд, если игрок
  не сделал тап.

## Сборка сцены

Сцена `Main.unity` ожидает следующий граф:

```
[GameRoot]
  ├─ Camera (orthographic, portrait fit)
  │   └─ ScreenShake (на ноду камеры)
  ├─ Lighting (Directional Light, без shadows)
  ├─ Level
  │   └─ UnitSpawner (PlayerPrefab, EnemyPrefab, ChestPrefab, Root)
  ├─ BattleFlow (TapInput, TargetIndicator, VfxService, FloatingNumberPrefab + Root)
  ├─ Audio (AudioService с AudioBank, AudioSource×4 пул)
  ├─ Integration (MraidBridge, AnalyticsService)
  └─ UI (Canvas + HudView/Presenter, EndCardView/Presenter)
```

Конфиги: `Configs/BalanceConfig.asset`, `Configs/LevelConfig.asset`,
`Configs/VfxBank.asset`, `Configs/AudioBank.asset` — создаются через
`Assets → Create → Playable → ...`.


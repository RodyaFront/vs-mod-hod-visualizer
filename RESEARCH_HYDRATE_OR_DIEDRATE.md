# Исследование мода Hydrate Or Diedrate

## Что сделано
- Репозиторий склонирован в `vs-moding-workspace/HydrateOrDiedrate`.
- Проведен обзор структуры, точек входа, статусов игрока и данных, пригодных для визуализации через `PlayerStatusStrip`.

## Базовая информация
- `modid`: `hydrateordiedrate`
- Версия в `modinfo.json`: `2.4.6`
- Назначение: система жажды, перегрева/охлаждения, водной инфраструктуры (колодцы, насосы, трубы), совместимости с другими модами.
- Основной `ModSystem`: `HydrateOrDiedrateModSystem`
- Основной сетевой канал: `hydrateordiedrate`

## Архитектура (высокоуровнево)
- `HydrateOrDiedrateModSystem`:
  - грузит конфиг (`ConfigManager.EnsureModConfigLoaded`);
  - применяет Harmony-патчи (включая категориальные для совместимости);
  - регистрирует entity behaviors, блоки/блок-сущности, сетевые пакеты и HUD-элементы;
  - на сервере добавляет поведения игроку (`HoD:thirst`, `HoD:bodytemperaturehot`, `HoD:liquidencumbrance`) в зависимости от конфига.
- `EntityBehaviorThirst`:
  - отвечает за деградацию гидратации, урон от обезвоживания, штраф скорости, hydration delay, nutrition deficit;
  - хранит данные в `WatchedAttributes` tree `thirst`.
- `EntityBehaviorBodyTemperatureHot`:
  - влияет на скорость жажды через `IThirstRateModifier`;
  - считает охлаждение (одежда, сырость, помещение, тень, низкая освещенность) и пишет в `WatchedAttributes` tree `hodCooling`.
- `EntityBehaviorLiquidEncumbrance`:
  - проверяет инвентарь на перегруз жидкостями;
  - накладывает модификатор скорости `entity.Stats.Set("walkspeed", "liquidEncumbrancePenalty", ...)`.

## Ключевые статусы для визуализации

### 1) Жажда (основной кандидат)
- Источник: `EntityBehaviorThirst`
- Где читать:
  - `CurrentThirst`
  - `MaxThirst`
  - `ThirstRate`
  - `NutritionDeficitAmount`
  - `HydrationLossDelay`
- Физическое хранилище: `WatchedAttributes["thirst"]`
  - `currentThirst`, `maxThirst`, `thirstRate`, `nutritionDeficitAmount`, `hydrationLossDelay`
- Текущий HUD мода уже рисует thirst bar (`HudElementThirstBar`) и nutrition deficit bar (`HudElementNutritionDeficitBar`).

### 2) Охлаждение/жара (дополнительный статус)
- Источник: `EntityBehaviorBodyTemperatureHot`
- Где читать:
  - `WatchedAttributes["hodCooling"]`:
    - `gearCooling`
    - `totalCooling`
    - `wetBonus`, `roomBonus`, `lowSunBonus`, `shadeBonus`
- Важно:
  - Актуально только если `HeatAndCooling.HarshHeat == true`.
  - Используется как множитель к скорости жажды.

### 3) Перегруз жидкостями (опционально)
- Источник: `EntityBehaviorLiquidEncumbrance`
- Что есть:
  - прямого публичного watched-значения для UI нет;
  - накладывается штраф скорости через stat-модификатор.
- Для визуализации придется:
  - либо вычислять состояние по инвентарю аналогично логике мода;
  - либо читать итоговую скорость/модификаторы из `entity.Stats` и выводить как индикатор penalty.

## Данные предметов/жидкостей, влияющие на статусы
- Центр доступа: `HydrationManager`
  - `GetHydration(ItemStack)`
  - `GetBlockHydration(ICoreAPI, Block)`
  - `GetNutritionDeficit(...)`
  - `GetHealing(...)`
  - `IsBoiling(...)`
- Ключи атрибутов:
  - `hydration`
  - `nutritionDeficit`
  - `HoDisBoiling`
  - `health`
- Значения массово назначаются через patch-систему (`ItemHydrationPatch`, `BlockHydrationPatch`).

## Сеть и клиентские сигналы
- Пакет `DrinkProgressPacket` (`Progress`, `IsDrinking`, `IsDangerous`) используется для оверлея процесса питья.
- Для `PlayerStatusStrip` это можно использовать как временный статус "пьет / опасная вода", если нужен реактивный индикатор.

## Конфигурация и условия включения
- Файл конфига: `HydrateOrDiedrateConfig.json`
- Критичные переключатели:
  - `Thirst.Enabled`
  - `HeatAndCooling.HarshHeat`
  - `LiquidEncumbrance.Enabled`
- На стороне клиента значения конфига подхватываются из world config, синхронизированного сервером.

## Практический вывод для мода-визуализатора на базе PlayerStatusStrip
- Рекомендуемый минимальный набор статусов:
  1. `Thirst %` = `CurrentThirst / MaxThirst`
  2. `HydrationLossDelay` (сек/формат времени)
  3. `NutritionDeficitAmount`
- Рекомендуемый расширенный набор:
  4. `ThirstRate` (текущий rate относительно базового из конфига)
  5. `Cooling total` (если harsh heat включен)
  6. Бинарный статус "Dangerous drink" во время питья по `DrinkProgressPacket.IsDangerous`

## Риски интеграции
- Поведения могут отсутствовать (отключены конфигом), это нужно обрабатывать без ошибок.
- Часть логики завязана на другие моды (например, `xlib`, `HardcoreWater`, `ACulinaryArtillery`, `hudshelf`) и должна проверяться через `IsModEnabled`.
- Для некоторых показателей (encumbrance) нет готового watched-поля, потребуется косвенное вычисление.

## Полезные точки в коде
- `src/HydrateOrDiedrateModSystem.cs`
- `src/Thirst/EntityBehaviorThirst.cs`
- `src/Thirst/EntityBehaviorThirst.Properties.cs`
- `src/Hot Weather/EntityBehaviorsBodyTemperatureHot.cs`
- `src/Hot Weather/EntityBehaviorsBodyTemperatureHot.Properties.cs`
- `src/Encumbrance/EntityBehaviorLiquidEncumbrance.cs`
- `src/Hydration/HydrationManager.cs`
- `src/Commands/ThirstCommands.cs`
- `src/Patches/CharacterExtraDialogsPatch.cs`

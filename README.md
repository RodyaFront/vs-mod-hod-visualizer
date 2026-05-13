# HOD Visualizer

`HOD Visualizer` (`hodvisualizer`) adds clear status icons and tooltips for key mechanics from
`Hydrate Or Diedrate`, rendered through `Player Status Strip`.

## Version

Current release: **1.0.0**

## Requirements

- Vintage Story (game)
- `hydrateordiedrate`
- `playerstatusstrip`

## What this mod shows

- **Thirst**
  - visible when hydration is in debuff ranges
  - breakpoints:
    - `0..600`: thirst movement-speed penalty status
    - `<= 0`: critical thirst status with movement penalty + periodic damage warning
- **Heat / Cold impact on thirst**
  - `You are hot` when thirst drains faster
  - `You are cold` when thirst drains slower
  - hidden when the real effect is effectively zero
- **Liquid encumbrance**
  - highlights movement penalty from carrying too much liquid
- **Nutrition deficit (optional HoD mechanic)**
  - shows accumulated nutrition deficit value

## UX goals

- Game-friendly, non-technical tooltip text
- Stable icon semantics for quick recognition
- No noise: statuses only appear when they have meaningful impact

## Changelog

See `CHANGELOG.md` for release notes.

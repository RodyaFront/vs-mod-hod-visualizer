# Changelog

All notable changes to this project are documented in this file.

## [Unreleased]

## [1.0.0] - 2026-05-13

### Added
- Initial `hodvisualizer` mod scaffold with status-strip provider integration for Hydrate Or Diedrate.
- Domain evaluators and tests for thirst tiers, heat/cold effect, liquid encumbrance, and nutrition deficit.
- Localized tooltip/title strings and status icon assets for supported effects.
- Smoke-testing and implementation docs (`SMOKE_TESTING.md`, `IMPLEMENTATION_PLAN.md`, `STATUS_LOGIC_SPEC.md`).

### Changed
- Updated thirst visual logic to use hydration-unit breakpoints (`0..600` and `<= 0`) instead of percentage-only display.
- Refined heat/cold visibility to hide near-zero thirst-rate effects.
- Adjusted tooltip VTML formatting to remove top empty space and leading text spacing artifacts.

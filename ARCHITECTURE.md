# ARCHITECTURE

## Modules
Core: boot, state machine, services, save, settings.
Input: touch, controller, accessibility remapping.
Player: movement, locomotion, combat intent, local presentation.
Combat: health, damage, hit resolution, status effects, aggro.
Abilities: cooldowns, activation, effects.
Items: definitions, affixes, inventory, equipment, loot tables.
AI: sensing, state/behavior logic, navigation, encounter director.
World: room chunks, doors, checkpoints, mission flow.
Network: session, join code, player spawning, authoritative combat, reconnect.
UI: HUD, inventory, lobby, results, settings.
Presentation: VFX, audio, animation, camera shake, damage numbers.
Telemetry/QA: performance markers, automated smoke tests, reproducible seeds.

## Authority rules
- Local client: raw input, camera, local UI, prediction/presentation.
- Host/server: enemy state, damage validation, deaths, loot, objective state, revive state.
- Shared deterministic data: item/enemy definitions by stable IDs.

## Scene plan
Boot -> Camp -> Mission.
Test scenes are separate and lightweight.

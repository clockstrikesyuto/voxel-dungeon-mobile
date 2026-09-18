# AI RULES

## Product intent
Build a polished original mobile co-op dungeon crawler with a gameplay loop broadly inspired by accessible action-RPG dungeon crawlers, without copying protected characters, maps, names, UI artwork, audio, story, textures, or other distinctive expression from existing games.

## Non-negotiables
1. Mobile-first: iOS/Android landscape, touch controls, readable UI, scalable quality.
2. 1–4 players: every gameplay system must work in solo and network co-op from the beginning.
3. Server/host authoritative for combat-critical state: health, damage, drops, mission progress, enemy death.
4. Never introduce a system that only works for Player 1.
5. Never block the main thread with avoidable synchronous work during gameplay.
6. Avoid per-frame allocations in hot paths.
7. Pool enemies, projectiles, VFX, damage numbers, and common pickups.
8. Do not create one GameObject per terrain voxel.
9. All tuning values must be data-driven or serialized; avoid magic numbers scattered through code.
10. Every bug fix must add or update a regression check when practical.
11. Do not change game feel, UX hierarchy, or art direction just to make implementation easier.
12. Do not ask the user to perform coding/debugging work that can be done by the AI/tooling.

## Definition of done for a task
- Compiles with zero errors.
- No new Console exceptions during the relevant test path.
- Solo path tested.
- 4-player simulation considered/tested when networking is affected.
- Touch UI safe-area checked when UI is affected.
- Performance impact considered.
- Acceptance test updated if behavior changed.
- CHANGELOG/BUG_HISTORY updated for meaningful changes.

## AI change loop
1. Read GAME_SPEC.md + DESIGN_RULES.md + relevant code.
2. State the behavior to preserve.
3. Make the smallest coherent change.
4. Compile.
5. Run automated tests.
6. Run Play Mode smoke test.
7. Inspect Console/logs.
8. Fix failures.
9. Re-run regression tests.
10. Commit with a specific message.

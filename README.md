# Voxel Dungeon Mobile — AI-first Unity Starter

Mobile-first 1–4 player co-op action RPG / dungeon crawler prototype foundation.

## Target
- Unity 6.3 LTS (latest 6.3 patch recommended)
- iOS / Android first, PC later
- Landscape orientation
- URP
- Solo + 2–4 player online co-op
- High-quality original voxel/3D art direction

## First boot
1. Create a **Unity 6.3 LTS URP** project.
2. Copy this repository contents over the new project root.
3. Open Unity.
4. Run **Tools > Voxel Dungeon > Install Required Packages**.
5. Run **Tools > Voxel Dungeon > Apply Mobile Defaults**.
6. Run `python Tools/validate_repo.py` before every push.

## Multiplayer stack
- Netcode for GameObjects
- Multiplayer Services
- Relay / Lobby / Sessions
- Multiplayer Play Mode
- Multiplayer Tools

## AI workflow
The project is designed for: spec -> implementation -> compile/test -> inspect logs -> fix -> regression test -> commit.
See `AI_RULES.md`, `ARCHITECTURE.md`, and `ACCEPTANCE_TESTS.md`.

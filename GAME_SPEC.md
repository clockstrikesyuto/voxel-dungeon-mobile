# GAME SPEC — baseline

## Core fantasy
Fast, readable, satisfying top-down action in compact handcrafted/procedural hybrid dungeons. The game is original in world, characters, enemies, equipment, UI and narrative.

## Players
- 1 to 4 players.
- Solo is fully supported.
- Online co-op uses room/join-code flow; QR can be added in the app shell later.
- Reconnect and drop-out tolerance are required before public test.

## Camera
- Isometric/top-down 3D.
- Camera is primarily fixed relative to the player; no mandatory manual rotation on mobile.
- Multiplayer camera remains local to each player.

## Controls
Left thumb: virtual stick.
Right thumb: melee, ranged, dodge, skill 1, skill 2, skill 3, potion.
Context interactions use a single contextual action when needed.
Aim assist is required for mobile melee and ranged attacks.

## Combat
- Melee + ranged + dodge + 3 active abilities + potion.
- High enemy density with clear telegraphs.
- Hit stop, impact VFX, damage numbers, knockback and audio are part of game feel, but must be scalable by quality tier.
- Host/server is authoritative for damage, death and rewards.

## Equipment
- Melee weapon
- Ranged weapon
- Armor
- 3 relic/ability slots
- Data-driven power level and affix/enchantment system
- Distinct rarity tiers with original naming

## Mission loop
Camp -> choose mission -> dungeon -> combat/events -> loot -> boss -> result -> camp -> upgrade -> higher difficulty.

## Dungeon composition
Use authored room chunks connected by procedural rules, not fully random noise.
Room types: combat, traversal, treasure, event, elite, miniboss, boss, secret.

## Multiplayer scaling
Do not scale only HP. Increase a mix of enemy count, elite chance, behavior complexity, and HP.

## Down/revive
Multiplayer: downed state + teammate revive.
Solo: limited revive/life system during mission.

## Mobile performance target
- Baseline target: stable 30 FPS on supported mid-range devices.
- Optional 60 FPS performance mode on capable devices.
- Quality tiers: Low / Medium / High.
- Dynamic reduction for expensive VFX/shadows/enemy cosmetics is allowed, gameplay state is not.

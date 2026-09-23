# Changelog

## 0.0.9 - Adventure Overhaul

- Redesigned the player character with friendlier facial proportions, layered clothing and cleaner armor silhouettes.
- Reduced oversized headgear and removed chest/headgear overlap flicker.
- Rebuilt melee weapons around a hand pivot so blades stand upright instead of looking flat.
- Refined melee, dagger and ranged animation poses.
- Synchronized melee damage and ranged projectile release with the visible attack animation.
- Added enemy crowd separation for clearer close-range combat.
- Made boss encounters engage faster and improved opening telegraphs.
- Added visible boss phase color changes and cleared telegraphs immediately on defeat.
- Lifted the boss HP HUD and retained dynamic enemy health-bar placement.
- Replaced large colored boss-floor squares with subtle glowing inlay lines.
- Made visible outdoor and stage floor surfaces consistently walkable.
- Changed fall recovery to allow a readable fall into the abyss without endless falling.
- Improved checkpoint recovery by sampling nearby verified floor positions.
- Added consumables to mission-clear reward summaries.
- Preserved the bright-stage, open-cave, world-map and Japanese/English systems from the visual overhaul.


## 0.0.8 - Character Motion & Combat Polish

- Added alternating leg and arm movement while walking.
- Added visible melee weapon swing animations with weapon-specific motion.
- Added ranged weapon ready/release animations.
- Added player hit-reaction animation.
- Grouped equipped melee and ranged visuals into animation rigs.
- Raised normal enemy health bars dynamically above character bounds.
- Moved the boss health HUD slightly higher.
- Made visible Crystal Crypt, Ashen Forge and Void Garden floor slabs collidable so decorative floor areas no longer hide accidental fall-through gaps.
- Fixed unlocked solo stages to launch directly instead of entering the multiplayer lobby.


## 0.0.7 - Adventure Expansion Polish

- Fixed actor visual roots so feet align with the ground plane.
- Fixed equipped boots and armor alignment.
- Boss-fight falls now respawn the player safely inside the active boss arena.
- Checkpoints snap to verified ground to prevent recovery loops.
- Replaced the flashing purple boss barrier with subtle transparent glass and edge feedback.
- Expanded boss barrier collision so the doorway cannot be bypassed from the sides or above.
- Removed boss world-space health bars; bosses now use only the safe-area top HUD.
- Improved visibility of mobile FIRE / ICE / TONIC and other action buttons.

## 0.0.6 - Equipment & Void Garden Expansion

- Added persistent melee/ranged equipment inventory and loadouts.
- Added weapon rarity, power and attack-speed modifiers.
- Added equipment drops to Crystal Crypt, Ashen Forge and Void Garden.
- Added boss-exclusive weapons including Warden Cleaver, Colossus Maul and Void Edge.
- Added Frontier Arsenal for changing equipped weapons in the physical hub.
- Converted Frontier Merchant into a working equipment shop.
- Added Archivist Luma with a progression quest and reward.
- Added live player weapon silhouettes that update when loadouts change.
- Added equipped weapon names to the mission HUD.
- Added a complete third mission, Void Garden, with white ruins, reflecting pools, garden lighting and dedicated encounters.
- Added Astral Warden with a dedicated three-phase boss profile.
- Connected Void Garden to the physical hub gate and Frontier Map.
- Added Void Garden completion persistence.
- Expanded world progression to Crystal Crypt -> Ashen Forge -> Void Garden.

## 0.0.5 - Solo Adventure Expansion

- Reworked Crystal Crypt floor construction to remove overlapping coplanar tiles and reduce movement flicker.
- Added room-based encounter activation so enemies wake only when the player enters their combat area.
- Added sealed boss routes: clear regular encounters, enter the arena, close the rear barrier, then spawn the boss.
- Added boss entrance title presentation.
- Added three-phase boss combat with faster pursuit, stronger pulses and radial projectile bursts.
- Added a complete second playable mission, Ashen Forge, with foundries, lava channels, machinery, stronger encounters, treasure and Forge Colossus.
- Added Ashen Forge completion persistence and Void Garden route discovery.
- Connected Ashen Forge to the physical hub gate and Frontier Map.
- Added stage-specific journey banners and scene identity.
- Retry now reloads the current mission rather than always returning to Crystal Crypt.

## 0.0.4 - Frontier Beauty Update

- Added 64x64 procedural pixel textures with point filtering across generated materials.
- Rebuilt Frontier Haven as a bright daytime hub with water, bridge, trees, flowers, lamps, buildings and distant scenery.
- Added separate bright cinematic post-processing profiles for hub and dungeon.
- Rebuilt Crystal Crypt into a long multi-zone stage: Entry Court, Crystal Hall, Crossing, Gallery, Inner Sanctum and boss arena.
- Added accessible side routes and three treasure chests.
- Redistributed enemies across the full stage and expanded the boss encounter.
- Increased player, enemy and boss visual detail with layered armor, faces, weapons and silhouette differences.
- Added stage journey banners.
- Hid gameplay HUD behind mission result UI.
- Enabled HDR/MSAA on game cameras.

# CHANGELOG

## 0.0.3
- Added melee slash visual and combat knockback.
- Added ranged projectile attack with lightweight aim assist.
- Added potion healing with cooldown.
- Added enemy attack windup/telegraph and death shrink animation.
- Added mobile ranged and potion buttons.

## 0.0.2
- Added functional top-down movement, camera follow, dodge, melee combat and simple enemy pursuit.
- Added mobile joystick/action buttons and player/enemy health UI.
- Added visible combat feedback with damage numbers, hit pulse and hit flash.

## 0.0.1
- Added AI-first Unity project foundation.
- Added mobile/multiplayer product constraints and acceptance tests.

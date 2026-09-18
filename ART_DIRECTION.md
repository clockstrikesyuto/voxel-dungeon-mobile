# ART DIRECTION — Mobile Premium Voxel Fantasy

## Goal
A high-quality original stylized 3D look with voxel/block influence, readable on a phone and distinct from Minecraft's specific visual identity.

## Shape language
- Chunky readable proportions.
- Characters use softened block forms rather than literal cubes everywhere.
- Weapons are 15–25% oversized for combat readability.
- Enemy classes must be recognizable by silhouette before color.
- Bosses use one dominant shape + one signature animated feature.

## Surface language
- Painted/stylized PBR, restrained texture noise.
- Large material regions instead of tiny pixel noise.
- Roughness and emissive contrast are important.
- Avoid photorealism; preserve clean readability during 4-player combat.

## Lighting
- One dominant key direction per biome.
- Soft fill so characters never disappear into terrain.
- Local emissive accents for interactables, danger and loot.
- High tier may use additional shadow/VFX quality; low tier keeps the same gameplay readability.

## VFX hierarchy
1. Enemy danger telegraph
2. Player action feedback
3. Loot rarity feedback
4. Ambient world effects

Never let ambient VFX obscure danger telegraphs.

## Biome palette examples
- Verdant Ruins: moss green / warm stone / cyan magical accents
- Ember Foundry: charcoal / copper / orange-red heat
- Frost Vault: blue-white / dark slate / violet crystal accents
- Hollow Citadel: desaturated stone / deep indigo / gold corruption

## Mobile readability rules
- Critical interactables need value/brightness separation from the floor.
- Damage telegraphs remain visible under four-player VFX.
- Avoid thin geometry as the main identity of an enemy.
- UI icons should still read around 48–64 px.
- Loot beams/glows scale by rarity but do not fill the screen.

## Performance construction
- Modular mesh chunks, not per-voxel GameObjects.
- LOD where it saves real GPU/CPU cost.
- Instancing for repeated props.
- Baked/static lighting where compatible with level goals.
- Pooled transient VFX.
- Texture atlases where sensible; do not sacrifice art iteration for premature packing.

## Originality guard
Do not copy existing game's:
- character silhouettes,
- mob designs,
- item artwork/names,
- HUD layout/art,
- texture sets,
- named locations,
- story beats,
- sound effects or music.
The target is familiar genre usability with an original visual identity.

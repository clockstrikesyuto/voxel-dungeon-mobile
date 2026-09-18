# DESIGN RULES

## Visual direction
High-quality original stylized 3D with voxel/block influence, not a direct Minecraft look.
- Strong silhouettes.
- Large readable weapons.
- Controlled palette per biome.
- Premium lighting and VFX within mobile budgets.
- Enemies must be identifiable at phone-screen size.

## UI
- Landscape-first.
- Touch targets >= 48 logical px equivalent where possible.
- Respect safe areas/notches.
- Combat HUD should be understandable in under 3 seconds.
- Do not cover the player's combat focus with loot popups.

## Feel
- Input response first.
- Dodge must feel immediate.
- Attack magnetism/aim assist should reduce frustration without feeling automatic.
- Loot pickup for currency/materials is automatic.
- Equipment drops notify but do not force inventory interruption.

## Content construction
- Data assets for enemies, weapons, abilities, loot tables, encounters and rooms.
- Prefer reusable modular geometry over bespoke huge scenes.
- Pool repeated runtime objects.

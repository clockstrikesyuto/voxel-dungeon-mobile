# ACCEPTANCE TESTS

## Foundation
- [ ] App boots into a valid scene with zero exceptions.
- [ ] Landscape orientation is enforced on device.
- [ ] Safe-area-aware HUD root exists.
- [ ] Quality tier can switch at runtime.

## Player movement
- [ ] Touch stick moves player in camera-relative XZ plane.
- [ ] Releasing stick stops acceleration correctly.
- [ ] Dodge cannot get stuck when input is released mid-frame.
- [ ] Movement is stable at 30 and 60 FPS.

## Combat
- [ ] Melee hits one valid target only per configured strike window.
- [ ] Melee slash visual appears when attacking.
- [ ] Hits visibly react with damage number, flash and knockback.
- [ ] Ranged attack launches a projectile and can use aim assist.
- [ ] Potion heals only when health is missing and respects cooldown.
- [ ] Enemy attack shows a readable windup before damage.
- [ ] Enemy death completes once and removes the enemy.
- [ ] Damage cannot be applied twice from duplicated network messages.
- [ ] Player death/down transitions exactly once.

## Mobile HUD
- [ ] Joystick controls movement with mouse/touch.
- [ ] Attack button triggers melee.
- [ ] Dodge button triggers dodge.
- [ ] Ranged button triggers ranged attack.
- [ ] Potion button heals when health is missing.
- [ ] Player HP bar updates after damage/healing.
- [ ] Enemy HP bars update after damage.

## Multiplayer
- [ ] 1 player completes the smoke mission.
- [ ] 2 players can join and see each other move.
- [ ] 4 simulated players can join one session.
- [ ] Host authoritative health stays consistent on all clients.
- [ ] Client disconnect does not crash host.
- [ ] Rejoin plan is covered before public test.

## Performance
- [ ] No repeating GC allocation from player movement loop.
- [ ] 20 enemies in test arena do not produce Console spam.
- [ ] VFX quality can be reduced without changing gameplay.

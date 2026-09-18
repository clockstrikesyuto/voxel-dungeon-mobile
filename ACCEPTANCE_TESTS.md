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
- [ ] Ranged aim assist selects only valid targets.
- [ ] Damage cannot be applied twice from duplicated network messages.
- [ ] Player death/down transitions exactly once.

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

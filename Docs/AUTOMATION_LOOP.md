# Automation loop

The intended autonomous loop is:
1. Pull latest branch.
2. Read AI_RULES.md + task acceptance criteria.
3. Edit a focused set of files.
4. Run repository validation.
5. Open Unity in batch/editor automation environment.
6. Compile scripts.
7. Run EditMode tests.
8. Run PlayMode smoke scene.
9. Capture Console errors and performance counters.
10. Fix failures automatically.
11. Re-run the same checks.
12. Commit only after green checks.

For network changes, include a 4-player Multiplayer Play Mode scenario before merging.
For rendering/UI changes, include representative-device screenshots and a performance sample.

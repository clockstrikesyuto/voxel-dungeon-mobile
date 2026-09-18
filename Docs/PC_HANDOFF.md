# PC HANDOFF CHECKLIST

When a Windows or Mac computer is available:

1. Install Unity Hub.
2. Install Unity 6.3 LTS (latest 6.3 patch).
3. Add Android Build Support + SDK/NDK + OpenJDK.
4. Clone `clockstrikesyuto/voxel-dungeon-mobile`.
5. Create/open the repository as a Universal 3D (URP) Unity project.
6. Let Unity generate Library/Temp locally; never commit them.
7. In Unity run: Tools > Voxel Dungeon > Install Required Packages.
8. Then run: Tools > Voxel Dungeon > Apply Mobile Defaults.
9. Create Boot, Camp and Mission_Test scenes.
10. Add GameBootstrap to Boot.
11. Capture the first Console output. Fix every compile error before gameplay implementation continues.
12. Enable Multiplayer Play Mode after packages install and prepare 4-player editor simulation.

The user's job at handoff is mainly to open/install Unity and share the first Editor/Console state. Coding and debugging should remain AI-led whenever tools permit.

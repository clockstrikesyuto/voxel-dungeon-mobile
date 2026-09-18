# Unity setup

Use Unity 6.3 LTS, preferably the latest 6.3 patch available in Unity Hub.
Create a Universal 3D/URP project, then overlay this starter.

In Unity:
1. Tools > Voxel Dungeon > Install Required Packages
2. Tools > Voxel Dungeon > Apply Mobile Defaults
3. Create Boot, Camp, Mission_Test scenes
4. Add a GameBootstrap object to Boot
5. Add Boot to Build Settings / Build Profiles first

Package bootstrap requests the latest compatible releases for:
- Input System
- Netcode for GameObjects
- Multiplayer Services
- Multiplayer Play Mode
- Multiplayer Tools

Do not add production networking code until package installation finishes without errors.

#!/usr/bin/env python3
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
required = [
    "AI_RULES.md", "GAME_SPEC.md", "DESIGN_RULES.md", "ARCHITECTURE.md",
    "ACCEPTANCE_TESTS.md", "BUG_HISTORY.md", "FEEDBACK_HISTORY.md",
    "Assets/_Game/Scripts/Core/GameBootstrap.cs",
    "Assets/_Game/Scripts/Player/TopDownPlayerMotor.cs",
]
errors = []
for rel in required:
    if not (ROOT / rel).exists():
        errors.append(f"missing required file: {rel}")

for path in ROOT.rglob("*"):
    if path.is_file() and path.stat().st_size > 25 * 1024 * 1024:
        errors.append(f"oversized file (>25MB): {path.relative_to(ROOT)}")

for forbidden in ["Library", "Temp", "Logs", "UserSettings"]:
    if (ROOT / forbidden).exists():
        errors.append(f"generated Unity folder should not be committed: {forbidden}")

if errors:
    print("VALIDATION FAILED")
    for e in errors:
        print(" -", e)
    sys.exit(1)
print("VALIDATION OK")

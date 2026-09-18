using UnityEngine;
using VoxelDungeon.Combat;
using VoxelDungeon.Player;
using VoxelDungeon.UI;

namespace VoxelDungeon.Core
{
    public sealed class MissionController : MonoBehaviour
    {
        [SerializeField] private Health[] regularEnemies;
        [SerializeField] private GameObject bossRoot;
        [SerializeField] private Health bossHealth;
        [SerializeField] private PlayerProgress playerProgress;
        [SerializeField] private GameObject bossEntryGate;

        private int remaining;
        private bool bossActivated;
        private bool bossArenaEntered;
        private bool cleared;

        public void Configure(
            Health[] regular,
            GameObject bossObject,
            Health boss,
            PlayerProgress progress,
            GameObject entryGate = null)
        {
            regularEnemies = regular;
            bossRoot = bossObject;
            bossHealth = boss;
            playerProgress = progress;
            bossEntryGate = entryGate;
        }

        private void Start()
        {
            remaining = 0;

            if (regularEnemies != null)
            {
                foreach (Health enemy in regularEnemies)
                {
                    if (enemy == null)
                        continue;

                    remaining++;
                    enemy.Died += OnRegularEnemyDied;
                }
            }

            if (bossRoot != null)
                bossRoot.SetActive(false);

            if (bossHealth != null)
                bossHealth.Died += OnBossDied;

            if (bossEntryGate != null)
                bossEntryGate.SetActive(remaining > 0);

            if (remaining == 0)
                OpenBossRoute();
        }

        private void OnDestroy()
        {
            if (regularEnemies != null)
            {
                foreach (Health enemy in regularEnemies)
                {
                    if (enemy != null)
                        enemy.Died -= OnRegularEnemyDied;
                }
            }

            if (bossHealth != null)
                bossHealth.Died -= OnBossDied;
        }

        private void OnRegularEnemyDied()
        {
            remaining = Mathf.Max(0, remaining - 1);
            if (remaining == 0)
                Invoke(nameof(OpenBossRoute), 0.35f);
        }

        public void NotifyBossArenaEntered()
        {
            bossArenaEntered = true;
            if (remaining == 0)
                ActivateBoss();
        }

        private void OpenBossRoute()
        {
            if (bossEntryGate != null)
                bossEntryGate.SetActive(false);

            if (bossArenaEntered)
                ActivateBoss();
        }

        private void ActivateBoss()
        {
            if (bossActivated || !bossArenaEntered || remaining > 0)
                return;

            bossActivated = true;
            if (bossRoot != null)
                bossRoot.SetActive(true);
        }

        private void OnBossDied()
        {
            if (cleared)
                return;

            cleared = true;
            Invoke(nameof(ShowResult), 0.8f);
        }

        private void ShowResult()
        {
            MissionResultUI.Show(playerProgress);
        }
    }
}

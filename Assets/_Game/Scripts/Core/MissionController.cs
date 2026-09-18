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

        private int remaining;
        private bool bossActivated;
        private bool cleared;

        public void Configure(
            Health[] regular,
            GameObject bossObject,
            Health boss,
            PlayerProgress progress)
        {
            regularEnemies = regular;
            bossRoot = bossObject;
            bossHealth = boss;
            playerProgress = progress;
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

            if (remaining == 0)
                ActivateBoss();
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
                Invoke(nameof(ActivateBoss), 0.8f);
        }

        private void ActivateBoss()
        {
            if (bossActivated)
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

using UnityEngine;
using VoxelDungeon.AI;
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
        [SerializeField] private GameObject arenaBarrier;

        private bool bossActivated;
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

        public void RegisterArenaBarrier(GameObject barrier)
        {
            arenaBarrier = barrier;
            if (arenaBarrier != null)
                arenaBarrier.SetActive(false);
        }

        private void Start()
        {
            // The boss route is always open. Exploration enemies are optional.
            if (bossEntryGate != null)
                bossEntryGate.SetActive(false);

            if (arenaBarrier != null)
                arenaBarrier.SetActive(false);

            if (bossRoot != null)
                bossRoot.SetActive(false);

            if (bossHealth != null)
                bossHealth.Died += OnBossDied;
        }

        private void OnDestroy()
        {
            if (bossHealth != null)
                bossHealth.Died -= OnBossDied;
        }

        public void NotifyBossArenaEntered()
        {
            ActivateBoss();
        }

        private void ActivateBoss()
        {
            if (bossActivated)
                return;

            bossActivated = true;

            // Enemies skipped on the route stay behind the arena barrier and
            // cannot leak into the boss fight.
            if (regularEnemies != null)
            {
                foreach (Health enemy in regularEnemies)
                {
                    if (enemy != null && !enemy.IsDead)
                        enemy.gameObject.SetActive(false);
                }
            }

            if (arenaBarrier != null)
                arenaBarrier.SetActive(true);

            if (bossRoot == null)
                return;

            bossRoot.SetActive(true);

            SimpleEnemyBrain brain = bossRoot.GetComponent<SimpleEnemyBrain>();
            if (brain != null)
                brain.BeginEncounter();

            BossPulseAttack pulse = bossRoot.GetComponent<BossPulseAttack>();
            if (pulse != null)
                pulse.BeginEncounter();

            BossIntroUI.Show(bossRoot.name);
            BossHealthUI.Show(bossRoot.name, bossHealth);
        }

        private void OnBossDied()
        {
            if (cleared)
                return;

            cleared = true;

            if (arenaBarrier != null)
                arenaBarrier.SetActive(false);

            Invoke(nameof(ShowResult), 0.8f);
        }

        private void ShowResult()
        {
            MissionResultUI.Show(playerProgress);
        }
    }
}

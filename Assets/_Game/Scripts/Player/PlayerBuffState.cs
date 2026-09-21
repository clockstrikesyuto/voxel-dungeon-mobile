using UnityEngine;

namespace VoxelDungeon.Player
{
    public sealed class PlayerBuffState : MonoBehaviour
    {
        private float powerUntil;
        private float guardUntil;
        private float hasteUntil;

        public float DamageMultiplier => Time.time < powerUntil ? 1.35f : 1f;
        public int DefenseBonus => Time.time < guardUntil ? 14 : 0;
        public float MoveSpeedBonus => Time.time < hasteUntil ? 0.20f : 0f;

        public void ActivatePower(float duration = 10f)
        {
            powerUntil = Mathf.Max(powerUntil, Time.time + duration);
        }

        public void ActivateGuard(float duration = 10f)
        {
            guardUntil = Mathf.Max(guardUntil, Time.time + duration);
        }

        public void ActivateHaste(float duration = 10f)
        {
            hasteUntil = Mathf.Max(hasteUntil, Time.time + duration);
        }
    }
}

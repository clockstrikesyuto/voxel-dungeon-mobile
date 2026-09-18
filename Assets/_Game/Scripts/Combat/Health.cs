using System;
using UnityEngine;

namespace VoxelDungeon.Combat
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        private int currentHealth;
        private bool dead;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public bool IsDead => dead;
        public event Action<int, int> Changed;
        public event Action Died;

        private void Awake() => ResetHealth();

        public void ResetHealth()
        {
            dead = false;
            currentHealth = maxHealth;
            Changed?.Invoke(currentHealth, maxHealth);
        }

        public void ApplyDamage(int amount)
        {
            if (dead || amount <= 0) return;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            Changed?.Invoke(currentHealth, maxHealth);
            if (currentHealth == 0)
            {
                dead = true;
                Died?.Invoke();
            }
        }
    }
}

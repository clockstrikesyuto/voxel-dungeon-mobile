using UnityEngine;

namespace VoxelDungeon.AI
{
    [CreateAssetMenu(menuName = "Voxel Dungeon/AI/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string stableId = "enemy.id";
        [SerializeField] private string displayName = "New Enemy";
        [SerializeField, Min(1)] private int maxHealth = 20;
        [SerializeField, Min(0)] private int contactDamage = 5;
        [SerializeField, Min(0.1f)] private float moveSpeed = 2.5f;
        [SerializeField, Min(0.1f)] private float aggroRange = 8f;
        [SerializeField, Min(0.1f)] private float attackRange = 1.5f;

        public string StableId => stableId;
        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
        public int ContactDamage => contactDamage;
        public float MoveSpeed => moveSpeed;
        public float AggroRange => aggroRange;
        public float AttackRange => attackRange;

#if UNITY_EDITOR
        private void OnValidate()
        {
            stableId = stableId.Trim();
            if (string.IsNullOrWhiteSpace(stableId))
                stableId = name.ToLowerInvariant().Replace(' ', '.');
        }
#endif
    }
}

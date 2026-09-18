using UnityEngine;

namespace VoxelDungeon.Combat
{
    public readonly struct DamagePayload
    {
        public readonly GameObject Source;
        public readonly int Amount;
        public readonly Vector3 HitPoint;
        public readonly Vector3 Direction;
        public readonly bool Critical;

        public DamagePayload(GameObject source, int amount, Vector3 hitPoint, Vector3 direction, bool critical)
        {
            Source = source;
            Amount = amount;
            HitPoint = hitPoint;
            Direction = direction;
            Critical = critical;
        }
    }
}

namespace VoxelDungeon.Combat
{
    public interface IDamageReceiver
    {
        bool CanReceiveDamage { get; }
        void ReceiveDamage(in DamagePayload payload);
    }
}

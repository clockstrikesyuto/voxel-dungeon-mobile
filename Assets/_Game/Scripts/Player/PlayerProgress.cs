using System;
using UnityEngine;

namespace VoxelDungeon.Player
{
    public sealed class PlayerProgress : MonoBehaviour
    {
        [SerializeField] private int gold;
        [SerializeField] private int meleePowerBonus;
        [SerializeField] private int rangedPowerBonus;
        [SerializeField] private int lootCount;

        public int Gold => gold;
        public int MeleePowerBonus => meleePowerBonus;
        public int RangedPowerBonus => rangedPowerBonus;
        public int LootCount => lootCount;

        public event Action Changed;

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            gold += amount;
            Changed?.Invoke();
        }

        public void AddMeleePower(int amount)
        {
            if (amount <= 0) return;
            meleePowerBonus += amount;
            lootCount++;
            Changed?.Invoke();
        }

        public void AddRangedPower(int amount)
        {
            if (amount <= 0) return;
            rangedPowerBonus += amount;
            lootCount++;
            Changed?.Invoke();
        }
    }
}

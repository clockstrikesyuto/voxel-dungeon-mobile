using System.Collections;
using UnityEngine;

namespace VoxelDungeon.Loot
{
    public sealed class TreasureChest : MonoBehaviour
    {
        [SerializeField] private Transform lid;
        [SerializeField, Min(0.5f)] private float openRadius = 1.7f;
        [SerializeField] private int goldReward = 12;
        [SerializeField] private int powerReward = 5;

        private Transform player;
        private bool opened;

        public void Configure(Transform lidTransform, int gold, int power)
        {
            lid = lidTransform;
            goldReward = Mathf.Max(1, gold);
            powerReward = Mathf.Max(1, power);
        }

        private void Update()
        {
            if (opened)
                return;

            if (player == null)
            {
                GameObject go = GameObject.Find("Player_Debug");
                if (go != null)
                    player = go.transform;
                return;
            }

            Vector3 delta = player.position - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude <= openRadius * openRadius)
                Open();
        }

        private void Open()
        {
            opened = true;

            if (lid != null)
                StartCoroutine(OpenLid());

            LootPickup.Spawn(transform.position + transform.forward * 0.8f, LootPickup.LootKind.Gold, goldReward);

            LootPickup.LootKind kind = Random.value < 0.5f
                ? LootPickup.LootKind.MeleePower
                : LootPickup.LootKind.RangedPower;

            LootPickup.Spawn(transform.position + transform.right * 0.75f, kind, powerReward);
        }

        private IEnumerator OpenLid()
        {
            Quaternion start = lid.localRotation;
            Quaternion end = start * Quaternion.Euler(-70f, 0f, 0f);
            float duration = 0.3f;
            float age = 0f;

            while (age < duration)
            {
                age += Time.deltaTime;
                lid.localRotation = Quaternion.Slerp(start, end, Mathf.Clamp01(age / duration));
                yield return null;
            }

            lid.localRotation = end;
        }
    }
}

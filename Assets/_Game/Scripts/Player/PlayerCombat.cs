using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;
using VoxelDungeon.Core;
using VoxelDungeon.Items;
using VoxelDungeon.Loot;
using VoxelDungeon.Presentation;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(Health))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        [Header("Melee")]
        [SerializeField, Min(1)] private int meleeDamage = 34;
        [SerializeField, Min(0.1f)] private float meleeRadius = 1.15f;
        [SerializeField, Min(0f)] private float meleeForwardOffset = 1.1f;
        [SerializeField, Min(0.05f)] private float meleeCooldown = 0.45f;

        [Header("Ranged")]
        [SerializeField, Min(1)] private int rangedDamage = 22;
        [SerializeField, Min(0.1f)] private float rangedSpeed = 12f;
        [SerializeField, Min(0.05f)] private float rangedCooldown = 0.7f;
        [SerializeField, Min(1f)] private float aimAssistRange = 9f;
        [SerializeField, Range(-1f, 1f)] private float aimAssistMinDot = 0.15f;

        [Header("Potion")]
        [SerializeField, Min(1)] private int potionHeal = 45;
        [SerializeField, Min(0.1f)] private float potionCooldown = 8f;

        [Header("Feedback")]
        [SerializeField, Min(1f)] private float attackPulseScale = 1.12f;
        [SerializeField, Min(0.01f)] private float attackPulseDuration = 0.07f;

        private readonly Collider[] hitBuffer = new Collider[24];
        private readonly Collider[] aimBuffer = new Collider[32];

        private Health health;
        private PlayerProgress progress;
        private PlayerBuffState buffs;
        private PlayerActionAnimator actionAnimator;
        private float nextMeleeTime;
        private float nextRangedTime;
        private float nextPotionTime;
        private float nextItemTime;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;

        private void Awake()
        {
            health = GetComponent<Health>();
            progress = GetComponent<PlayerProgress>();
            buffs = GetComponent<PlayerBuffState>();
            if (buffs == null)
                buffs = gameObject.AddComponent<PlayerBuffState>();

            actionAnimator = GetComponent<PlayerActionAnimator>();
            if (actionAnimator == null)
                actionAnimator = gameObject.AddComponent<PlayerActionAnimator>();

            baseScale = transform.localScale;
        }

        private void Start()
        {
            if (health != null)
            {
                int maxHealth = 100 + ProfileProgress.TotalVitality + (ProfileProgress.Level - 1) * 3;
                health.ConfigureMaxHealth(maxHealth, true);
            }
        }

        public bool TryMelee()
        {
            if (Time.time < nextMeleeTime)
                return false;

            EquipmentRecord meleeItem = ProfileProgress.EquippedMelee;
            nextMeleeTime = Time.time + meleeCooldown * Mathf.Max(0.35f, meleeItem.CooldownMultiplier);

            actionAnimator?.PlayMelee(meleeItem.Id);
            StartCoroutine(ResolveMeleeAfterWindup(meleeItem));
            return true;
        }

        private IEnumerator ResolveMeleeAfterWindup(EquipmentRecord meleeItem)
        {
            // Match the damage frame to the visible weapon strike.
            yield return new WaitForSeconds(0.09f);

            PlayAttackPulse();
            MeleeArcVisual.Spawn(transform);

            float attackRadius = meleeRadius;
            float forwardOffset = meleeForwardOffset;
            float weaponCritBonus = 0f;
            float weaponDamageMultiplier = 1f;

            switch (meleeItem.Id)
            {
                case "crystal_daggers":
                    attackRadius = 0.95f;
                    forwardOffset = 0.90f;
                    weaponCritBonus = 0.12f;
                    break;

                case "forge_spear":
                    attackRadius = 1.0f;
                    forwardOffset = 1.65f;
                    break;

                case "colossus_maul":
                    attackRadius = 1.55f;
                    forwardOffset = 1.18f;
                    weaponDamageMultiplier = 1.12f;
                    break;

                case "ember_axe":
                    weaponDamageMultiplier = 1.08f;
                    break;

                case "void_edge":
                    weaponCritBonus = 0.05f;
                    weaponDamageMultiplier = 1.10f;
                    break;
            }

            Vector3 center = transform.position + Vector3.up * 0.9f + transform.forward * forwardOffset;
            int count = Physics.OverlapSphereNonAlloc(
                center,
                attackRadius,
                hitBuffer,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider hit = hitBuffer[i];
                if (hit == null || hit.transform.IsChildOf(transform))
                    continue;

                HealthDamageReceiver receiver = hit.GetComponentInParent<HealthDamageReceiver>();
                if (receiver == null || !receiver.CanReceiveDamage || receiver.gameObject == gameObject)
                    continue;

                Vector3 direction = hit.transform.position - transform.position;
                direction.y = 0f;

                int baseDamage =
                    meleeDamage
                    + (progress != null ? progress.MeleePowerBonus : 0)
                    + ProfileProgress.MeleePower
                    + meleeItem.Power
                    + ProfileProgress.EquippedWeaponRollPower(EquipmentSlot.Melee)
                    + ProfileProgress.MightRank * 2;

                bool critical = Random.value <
                    Mathf.Clamp01(ProfileProgress.TotalCritChance + weaponCritBonus);

                float buffMultiplier = buffs != null ? buffs.DamageMultiplier : 1f;
                int finalDamage = Mathf.RoundToInt(
                    (critical ? baseDamage * 1.65f : baseDamage) *
                    buffMultiplier *
                    weaponDamageMultiplier);

                receiver.ReceiveDamage(new DamagePayload(
                    gameObject,
                    finalDamage,
                    hit.ClosestPoint(center),
                    direction.sqrMagnitude > 0.001f ? direction.normalized : transform.forward,
                    critical));

                if (meleeItem.Id == "warden_cleaver" && health != null)
                    health.Heal(2);
            }
        }

        public bool TryRanged()
        {
            if (Time.time < nextRangedTime)
                return false;

            EquipmentRecord rangedItem = ProfileProgress.EquippedRanged;
            nextRangedTime = Time.time + rangedCooldown * Mathf.Max(0.35f, rangedItem.CooldownMultiplier);
            PlayAttackPulse();
            actionAnimator?.PlayRanged(rangedItem.Id);

            Vector3 direction = FindAimDirection();

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "PlayerProjectile";
            projectile.transform.position = transform.position + Vector3.up * 0.9f + direction * 0.9f;
            projectile.transform.localScale = Vector3.one * 0.32f;

            Collider primitiveCollider = projectile.GetComponent<Collider>();
            if (primitiveCollider != null)
                Destroy(primitiveCollider);

            Renderer renderer = projectile.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = new Color(0.35f, 0.8f, 1f, 1f);

            SimpleProjectile projectileLogic = projectile.AddComponent<SimpleProjectile>();
            int baseDamage =
                rangedDamage
                + (progress != null ? progress.RangedPowerBonus : 0)
                + ProfileProgress.RangedPower
                + rangedItem.Power
                + ProfileProgress.EquippedWeaponRollPower(EquipmentSlot.Ranged)
                + ProfileProgress.MightRank * 2;

            float projectileSpeed = rangedSpeed;
            float projectileRadius = 0.28f;
            float rangedCritBonus = 0f;
            float rangedDamageMultiplier = 1f;

            switch (rangedItem.Id)
            {
                case "ember_repeater":
                    projectileSpeed += 4f;
                    projectileRadius = 0.24f;
                    break;
                case "void_staff":
                    projectileRadius = 0.42f;
                    rangedDamageMultiplier = 1.08f;
                    break;
                case "starbow":
                    projectileSpeed += 2f;
                    rangedCritBonus = 0.08f;
                    break;
                case "astral_crossbow":
                    projectileSpeed += 3f;
                    projectileRadius = 0.34f;
                    rangedDamageMultiplier = 1.18f;
                    break;
            }

            bool critical = Random.value <
                Mathf.Clamp01(ProfileProgress.TotalCritChance + rangedCritBonus);

            float buffMultiplier = buffs != null ? buffs.DamageMultiplier : 1f;
            int finalDamage = Mathf.RoundToInt(
                (critical ? baseDamage * 1.65f : baseDamage) *
                buffMultiplier *
                rangedDamageMultiplier);

            projectileLogic.Initialize(
                gameObject,
                direction,
                projectileSpeed,
                finalDamage,
                3.0f,
                projectileRadius,
                critical);

            transform.forward = direction;
            return true;
        }

        public bool TryPotion()
        {
            if (Time.time < nextPotionTime || health == null)
                return false;

            if (ProfileProgress.GetConsumable("healing_potion") <= 0)
                return false;

            if (!health.Heal(potionHeal))
                return false;

            if (!ProfileProgress.ConsumeItem("healing_potion"))
                return false;

            nextPotionTime = Time.time + potionCooldown;
            StartCoroutine(PotionPulse());
            return true;
        }

        public bool TryPowerTonic()
        {
            if (ProfileProgress.GetConsumable("power_tonic") <= 0)
                return false;

            if (!ProfileProgress.ConsumeItem("power_tonic"))
                return false;

            buffs?.ActivatePower();
            return true;
        }

        public bool TryGuardTonic()
        {
            if (ProfileProgress.GetConsumable("guard_tonic") <= 0)
                return false;

            if (!ProfileProgress.ConsumeItem("guard_tonic"))
                return false;

            buffs?.ActivateGuard();
            return true;
        }

        public bool TryHasteTonic()
        {
            if (ProfileProgress.GetConsumable("haste_tonic") <= 0)
                return false;

            if (!ProfileProgress.ConsumeItem("haste_tonic"))
                return false;

            buffs?.ActivateHaste();
            return true;
        }

        public bool TryUtilityTonic()
        {
            if (TryPowerTonic()) return true;
            if (TryGuardTonic()) return true;
            return TryHasteTonic();
        }

        public bool TryFireBomb()
        {
            return TryThrowAdventureItem(
                "fire_bomb",
                false,
                44 + ProfileProgress.MightRank * 2,
                3.2f);
        }

        public bool TryFrostFlask()
        {
            return TryThrowAdventureItem(
                "frost_flask",
                true,
                24 + ProfileProgress.MightRank,
                3.6f);
        }

        private bool TryThrowAdventureItem(
            string itemId,
            bool frost,
            int damage,
            float radius)
        {
            if (Time.time < nextItemTime ||
                ProfileProgress.GetConsumable(itemId) <= 0)
                return false;

            Vector3 direction = FindAimDirection();

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = frost ? "FrostFlask" : "FireBomb";
            projectile.transform.position =
                transform.position + Vector3.up * 0.85f + direction * 0.85f;
            projectile.transform.localScale = Vector3.one * 0.34f;

            Collider collider = projectile.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Renderer renderer = projectile.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = frost
                    ? new Color(0.26f, 0.78f, 1f)
                    : new Color(1f, 0.30f, 0.06f);
            }

            AdventureBombProjectile bomb =
                projectile.AddComponent<AdventureBombProjectile>();
            bomb.Initialize(
                gameObject,
                direction,
                damage,
                radius,
                9.5f,
                frost);

            ProfileProgress.ConsumeItem(itemId);
            nextItemTime = Time.time + 1.15f;
            transform.forward = direction;
            return true;
        }

        private Vector3 FindAimDirection()
        {
            Vector3 forward = transform.forward;
            Vector3 bestDirection = forward;
            float bestScore = aimAssistMinDot;

            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                aimAssistRange,
                aimBuffer,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider candidate = aimBuffer[i];
                if (candidate == null || candidate.transform.IsChildOf(transform))
                    continue;

                HealthDamageReceiver receiver = candidate.GetComponentInParent<HealthDamageReceiver>();
                if (receiver == null || !receiver.CanReceiveDamage || receiver.gameObject == gameObject)
                    continue;

                Vector3 toTarget = receiver.transform.position - transform.position;
                toTarget.y = 0f;

                float distance = toTarget.magnitude;
                if (distance < 0.001f)
                    continue;

                Vector3 dir = toTarget / distance;
                float dot = Vector3.Dot(forward, dir);
                float distanceBonus = 1f - Mathf.Clamp01(distance / aimAssistRange);
                float score = dot + distanceBonus * 0.25f;

                if (dot >= aimAssistMinDot && score > bestScore)
                {
                    bestScore = score;
                    bestDirection = dir;
                }
            }

            return bestDirection.normalized;
        }

        private void PlayAttackPulse()
        {
            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);

            pulseRoutine = StartCoroutine(AttackPulse());
        }

        private IEnumerator AttackPulse()
        {
            transform.localScale = baseScale * attackPulseScale;
            yield return new WaitForSeconds(attackPulseDuration);
            transform.localScale = baseScale;
            pulseRoutine = null;
        }

        private IEnumerator PotionPulse()
        {
            Vector3 original = transform.localScale;
            transform.localScale = original * 1.18f;
            yield return new WaitForSeconds(0.12f);
            transform.localScale = original;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position + Vector3.up * 0.9f + transform.forward * meleeForwardOffset,
                meleeRadius);

            Gizmos.DrawWireSphere(transform.position, aimAssistRange);
        }
#endif
    }
}

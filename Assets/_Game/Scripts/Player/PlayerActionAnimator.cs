using System.Collections;
using UnityEngine;

namespace VoxelDungeon.Player
{
    public sealed class PlayerActionAnimator : MonoBehaviour
    {
        [SerializeField] private float meleeDuration = 0.30f;
        [SerializeField] private float rangedDuration = 0.28f;
        [SerializeField] private float hitDuration = 0.18f;

        private Transform visual;
        private Transform rightArm;
        private Transform leftArm;
        private Transform meleeRig;
        private Transform rangedRig;

        private Quaternion rightArmBaseRotation;
        private Quaternion leftArmBaseRotation;
        private Quaternion meleeBaseRotation;
        private Quaternion rangedBaseRotation;
        private Vector3 meleeBasePosition;
        private Vector3 rangedBasePosition;
        private Quaternion visualBaseRotation;

        private Coroutine upperBodyRoutine;
        private Coroutine hitRoutine;

        public bool UpperBodyBusy { get; private set; }

        private void Start()
        {
            ResolveParts();
        }

        public void PlayMelee(string weaponId)
        {
            ResolveParts();

            if (upperBodyRoutine != null)
                StopCoroutine(upperBodyRoutine);

            upperBodyRoutine = StartCoroutine(MeleeRoutine(weaponId));
        }

        public void PlayRanged(string weaponId)
        {
            ResolveParts();

            if (upperBodyRoutine != null)
                StopCoroutine(upperBodyRoutine);

            upperBodyRoutine = StartCoroutine(RangedRoutine(weaponId));
        }

        public void PlayHitReaction()
        {
            ResolveParts();

            if (hitRoutine != null)
                StopCoroutine(hitRoutine);

            hitRoutine = StartCoroutine(HitRoutine());
        }

        private void ResolveParts()
        {
            visual = transform.Find("PlayerVisual");
            if (visual != null)
            {
                rightArm = visual.Find("Arm_R");
                leftArm = visual.Find("Arm_L");
                visualBaseRotation = Quaternion.identity;

                if (rightArm != null && !UpperBodyBusy)
                    rightArmBaseRotation = rightArm.localRotation;

                if (leftArm != null && !UpperBodyBusy)
                    leftArmBaseRotation = leftArm.localRotation;
            }

            Transform loadout = transform.Find("LoadoutVisual");
            if (loadout != null)
            {
                meleeRig = loadout.Find("MeleeRig");
                rangedRig = loadout.Find("RangedRig");

                if (meleeRig != null && !UpperBodyBusy)
                {
                    meleeBasePosition = meleeRig.localPosition;
                    meleeBaseRotation = meleeRig.localRotation;
                }

                if (rangedRig != null && !UpperBodyBusy)
                {
                    rangedBasePosition = rangedRig.localPosition;
                    rangedBaseRotation = rangedRig.localRotation;
                }
            }
        }

        private IEnumerator MeleeRoutine(string weaponId)
        {
            UpperBodyBusy = true;
            ResolveParts();

            float windupAngle = weaponId == "colossus_maul" ? -72f :
                                weaponId == "forge_spear" ? -32f : -48f;

            float strikeAngle = weaponId == "crystal_daggers" ? 76f :
                                weaponId == "forge_spear" ? 26f :
                                weaponId == "colossus_maul" ? 98f : 88f;

            float windup = meleeDuration * 0.30f;
            float strike = meleeDuration * 0.34f;
            float recover = meleeDuration * 0.36f;

            yield return AnimateUpperBody(
                windup,
                Quaternion.Euler(windupAngle, 0f, -18f),
                Quaternion.Euler(16f, 0f, 10f),
                Quaternion.Euler(-18f, -8f, -30f),
                meleeBasePosition + new Vector3(0f, 0.03f, -0.04f));

            yield return AnimateUpperBody(
                strike,
                Quaternion.Euler(strikeAngle, 0f, 24f),
                Quaternion.Euler(-18f, 0f, -8f),
                Quaternion.Euler(strikeAngle * 0.78f, 12f, 35f),
                meleeBasePosition + new Vector3(0.02f, -0.04f, 0.18f));

            yield return AnimateUpperBody(
                recover,
                rightArmBaseRotation,
                leftArmBaseRotation,
                meleeBaseRotation,
                meleeBasePosition);

            ResetUpperBody();
        }

        private IEnumerator RangedRoutine(string weaponId)
        {
            UpperBodyBusy = true;
            ResolveParts();

            float raise = rangedDuration * 0.42f;
            float release = rangedDuration * 0.22f;
            float recover = rangedDuration * 0.36f;

            Vector3 readyPosition = rangedBasePosition + new Vector3(0.52f, 0.02f, 0.74f);
            Quaternion readyRotation = weaponId == "void_staff"
                ? Quaternion.Euler(-18f, 0f, -22f)
                : Quaternion.Euler(0f, -12f, -82f);

            yield return AnimateRanged(
                raise,
                Quaternion.Euler(-58f, 0f, -12f),
                Quaternion.Euler(-42f, 0f, 14f),
                readyRotation,
                readyPosition);

            yield return AnimateRanged(
                release,
                Quaternion.Euler(-72f, 0f, -6f),
                Quaternion.Euler(-34f, 0f, 8f),
                readyRotation,
                readyPosition + new Vector3(0f, 0f, 0.08f));

            yield return AnimateRanged(
                recover,
                rightArmBaseRotation,
                leftArmBaseRotation,
                rangedBaseRotation,
                rangedBasePosition);

            ResetUpperBody();
        }

        private IEnumerator HitRoutine()
        {
            if (visual == null)
                yield break;

            float half = hitDuration * 0.5f;
            float age = 0f;

            while (age < half)
            {
                age += Time.deltaTime;
                float t = Mathf.Clamp01(age / half);
                visual.localRotation = Quaternion.Slerp(
                    visualBaseRotation,
                    Quaternion.Euler(-10f, 0f, 6f),
                    t);
                yield return null;
            }

            age = 0f;
            while (age < half)
            {
                age += Time.deltaTime;
                float t = Mathf.Clamp01(age / half);
                visual.localRotation = Quaternion.Slerp(
                    Quaternion.Euler(-10f, 0f, 6f),
                    visualBaseRotation,
                    t);
                yield return null;
            }

            visual.localRotation = visualBaseRotation;
            hitRoutine = null;
        }

        private IEnumerator AnimateUpperBody(
            float duration,
            Quaternion rightTarget,
            Quaternion leftTarget,
            Quaternion meleeTarget,
            Vector3 meleePositionTarget)
        {
            Quaternion rightStart = rightArm != null ? rightArm.localRotation : Quaternion.identity;
            Quaternion leftStart = leftArm != null ? leftArm.localRotation : Quaternion.identity;
            Quaternion rigStart = meleeRig != null ? meleeRig.localRotation : Quaternion.identity;
            Vector3 rigPositionStart = meleeRig != null ? meleeRig.localPosition : Vector3.zero;

            float age = 0f;
            while (age < duration)
            {
                age += Time.deltaTime;
                float t = Ease(Mathf.Clamp01(age / Mathf.Max(0.01f, duration)));

                if (rightArm != null) rightArm.localRotation = Quaternion.Slerp(rightStart, rightTarget, t);
                if (leftArm != null) leftArm.localRotation = Quaternion.Slerp(leftStart, leftTarget, t);

                if (meleeRig != null)
                {
                    meleeRig.localRotation = Quaternion.Slerp(rigStart, meleeTarget, t);
                    meleeRig.localPosition = Vector3.Lerp(rigPositionStart, meleePositionTarget, t);
                }

                yield return null;
            }
        }

        private IEnumerator AnimateRanged(
            float duration,
            Quaternion rightTarget,
            Quaternion leftTarget,
            Quaternion rangedTarget,
            Vector3 rangedPositionTarget)
        {
            Quaternion rightStart = rightArm != null ? rightArm.localRotation : Quaternion.identity;
            Quaternion leftStart = leftArm != null ? leftArm.localRotation : Quaternion.identity;
            Quaternion rigStart = rangedRig != null ? rangedRig.localRotation : Quaternion.identity;
            Vector3 rigPositionStart = rangedRig != null ? rangedRig.localPosition : Vector3.zero;

            float age = 0f;
            while (age < duration)
            {
                age += Time.deltaTime;
                float t = Ease(Mathf.Clamp01(age / Mathf.Max(0.01f, duration)));

                if (rightArm != null) rightArm.localRotation = Quaternion.Slerp(rightStart, rightTarget, t);
                if (leftArm != null) leftArm.localRotation = Quaternion.Slerp(leftStart, leftTarget, t);

                if (rangedRig != null)
                {
                    rangedRig.localRotation = Quaternion.Slerp(rigStart, rangedTarget, t);
                    rangedRig.localPosition = Vector3.Lerp(rigPositionStart, rangedPositionTarget, t);
                }

                yield return null;
            }
        }

        private void ResetUpperBody()
        {
            if (rightArm != null) rightArm.localRotation = rightArmBaseRotation;
            if (leftArm != null) leftArm.localRotation = leftArmBaseRotation;

            if (meleeRig != null)
            {
                meleeRig.localRotation = meleeBaseRotation;
                meleeRig.localPosition = meleeBasePosition;
            }

            if (rangedRig != null)
            {
                rangedRig.localRotation = rangedBaseRotation;
                rangedRig.localPosition = rangedBasePosition;
            }

            UpperBodyBusy = false;
            upperBodyRoutine = null;
        }

        private static float Ease(float t)
        {
            return t * t * (3f - 2f * t);
        }

        private void OnDisable()
        {
            UpperBodyBusy = false;
        }
    }
}

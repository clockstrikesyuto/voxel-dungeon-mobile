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
        private Transform offhandMeleeRig;
        private Transform rangedRig;

        private Quaternion rightArmBaseRotation;
        private Quaternion leftArmBaseRotation;
        private Quaternion meleeBaseRotation;
        private Quaternion offhandBaseRotation;
        private Quaternion rangedBaseRotation;
        private Vector3 meleeBasePosition;
        private Vector3 offhandBasePosition;
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
                offhandMeleeRig = loadout.Find("OffhandMeleeRig");
                rangedRig = loadout.Find("RangedRig");

                if (meleeRig != null && !UpperBodyBusy)
                {
                    meleeBasePosition = meleeRig.localPosition;
                    meleeBaseRotation = meleeRig.localRotation;
                }

                if (offhandMeleeRig != null && !UpperBodyBusy)
                {
                    offhandBasePosition = offhandMeleeRig.localPosition;
                    offhandBaseRotation = offhandMeleeRig.localRotation;
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

            float windupAngle = weaponId == "colossus_maul" ? -76f :
                                weaponId == "forge_spear" ? -30f :
                                weaponId == "crystal_daggers" ? -42f : -52f;

            float strikeAngle = weaponId == "crystal_daggers" ? 72f :
                                weaponId == "forge_spear" ? 24f :
                                weaponId == "colossus_maul" ? 102f : 88f;

            float windup = meleeDuration * 0.30f;
            float strike = meleeDuration * 0.34f;
            float recover = meleeDuration * 0.36f;

            Quaternion rightWindup = rightArmBaseRotation * Quaternion.Euler(windupAngle, 0f, -14f);
            Quaternion leftWindup = leftArmBaseRotation * Quaternion.Euler(12f, 0f, 8f);
            Quaternion rigWindup = meleeBaseRotation * Quaternion.Euler(windupAngle * 0.72f, -8f, -24f);

            yield return AnimateUpperBody(
                windup,
                rightWindup,
                leftWindup,
                rigWindup,
                meleeBasePosition + new Vector3(0f, 0.04f, -0.04f));

            Quaternion rightStrike = rightArmBaseRotation * Quaternion.Euler(strikeAngle, 0f, 18f);
            Quaternion leftStrike = leftArmBaseRotation * Quaternion.Euler(-14f, 0f, -6f);
            Quaternion rigStrike = meleeBaseRotation * Quaternion.Euler(strikeAngle * 0.88f, 12f, 32f);

            yield return AnimateUpperBody(
                strike,
                rightStrike,
                leftStrike,
                rigStrike,
                meleeBasePosition + new Vector3(0.04f, -0.03f, 0.22f));

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
            Quaternion readyRotation = rangedBaseRotation * (
                weaponId == "void_staff"
                    ? Quaternion.Euler(-18f, 0f, -22f)
                    : Quaternion.Euler(0f, -12f, -82f));

            Quaternion rightReady = rightArmBaseRotation * Quaternion.Euler(-58f, 0f, -12f);
            Quaternion leftReady = leftArmBaseRotation * Quaternion.Euler(-42f, 0f, 14f);

            yield return AnimateRanged(
                raise,
                rightReady,
                leftReady,
                readyRotation,
                readyPosition);

            yield return AnimateRanged(
                release,
                rightArmBaseRotation * Quaternion.Euler(-72f, 0f, -6f),
                leftArmBaseRotation * Quaternion.Euler(-34f, 0f, 8f),
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
            Quaternion offhandStart = offhandMeleeRig != null ? offhandMeleeRig.localRotation : Quaternion.identity;
            Vector3 offhandPositionStart = offhandMeleeRig != null ? offhandMeleeRig.localPosition : Vector3.zero;

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

                if (offhandMeleeRig != null)
                {
                    Quaternion offhandTarget = offhandBaseRotation * Quaternion.Inverse(meleeBaseRotation) * meleeTarget;
                    offhandTarget = Quaternion.Euler(
                        offhandTarget.eulerAngles.x,
                        -offhandTarget.eulerAngles.y,
                        -offhandTarget.eulerAngles.z);

                    Vector3 delta = meleePositionTarget - meleeBasePosition;
                    Vector3 offhandTargetPosition = offhandBasePosition + new Vector3(-delta.x, delta.y, delta.z);

                    offhandMeleeRig.localRotation = Quaternion.Slerp(offhandStart, offhandTarget, t);
                    offhandMeleeRig.localPosition = Vector3.Lerp(offhandPositionStart, offhandTargetPosition, t);
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

            if (offhandMeleeRig != null)
            {
                offhandMeleeRig.localRotation = offhandBaseRotation;
                offhandMeleeRig.localPosition = offhandBasePosition;
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

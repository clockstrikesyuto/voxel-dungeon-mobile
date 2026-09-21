using UnityEngine;

namespace VoxelDungeon.Player
{
    public sealed class SimpleWalkAnimator : MonoBehaviour
    {
        [SerializeField] private float strideAngle = 22f;
        [SerializeField] private float strideSpeed = 9f;
        [SerializeField] private float armSwing = 15f;
        [SerializeField] private float stepDistance = 0.10f;
        [SerializeField] private float settleSpeed = 11f;

        private Transform leftLeg;
        private Transform rightLeg;
        private Transform leftArm;
        private Transform rightArm;
        private Transform leftBoot;
        private Transform rightBoot;

        private Vector3 leftLegBase;
        private Vector3 rightLegBase;
        private Vector3 leftBootBase;
        private Vector3 rightBootBase;
        private float phase;
        private bool basesCaptured;
        private PlayerActionAnimator actions;

        private void Start()
        {
            actions = GetComponent<PlayerActionAnimator>();
            ResolveParts();
        }

        private void Update()
        {
            if (leftLeg == null || rightLeg == null || leftBoot == null || rightBoot == null)
                ResolveParts();

            CharacterController controller = GetComponent<CharacterController>();
            Vector3 planarVelocity = controller != null ? controller.velocity : Vector3.zero;
            planarVelocity.y = 0f;

            float speed = planarVelocity.magnitude;
            bool moving = speed > 0.08f;

            if (moving)
            {
                phase += Time.deltaTime * strideSpeed * Mathf.Clamp(speed / 4.5f, 0.65f, 1.55f);

                float wave = Mathf.Sin(phase);
                float legAngle = wave * strideAngle;
                float armAngle = wave * armSwing;
                float step = wave * stepDistance;

                ApplyRotation(leftLeg, legAngle);
                ApplyRotation(rightLeg, -legAngle);

                if (actions == null || !actions.UpperBodyBusy)
                {
                    ApplyRotation(leftArm, -armAngle);
                    ApplyRotation(rightArm, armAngle);
                }

                ApplyRotation(leftBoot, legAngle * 0.65f);
                ApplyRotation(rightBoot, -legAngle * 0.65f);

                if (basesCaptured)
                {
                    ApplyStep(leftLeg, leftLegBase, step);
                    ApplyStep(rightLeg, rightLegBase, -step);
                    ApplyStep(leftBoot, leftBootBase, step);
                    ApplyStep(rightBoot, rightBootBase, -step);
                }
            }
            else
            {
                SettleRotation(leftLeg);
                SettleRotation(rightLeg);

                if (actions == null || !actions.UpperBodyBusy)
                {
                    SettleRotation(leftArm);
                    SettleRotation(rightArm);
                }

                SettleRotation(leftBoot);
                SettleRotation(rightBoot);

                if (basesCaptured)
                {
                    SettlePosition(leftLeg, leftLegBase);
                    SettlePosition(rightLeg, rightLegBase);
                    SettlePosition(leftBoot, leftBootBase);
                    SettlePosition(rightBoot, rightBootBase);
                }
            }
        }

        private void ResolveParts()
        {
            Transform visual = transform.Find("PlayerVisual");
            if (visual != null)
            {
                leftLeg = visual.Find("Leg_L");
                rightLeg = visual.Find("Leg_R");
                leftArm = visual.Find("Arm_L");
                rightArm = visual.Find("Arm_R");
            }

            Transform loadout = transform.Find("LoadoutVisual");
            if (loadout != null)
            {
                leftBoot = loadout.Find("ArmorBoot_L");
                rightBoot = loadout.Find("ArmorBoot_R");
            }

            if (leftLeg != null && rightLeg != null)
            {
                leftLegBase = leftLeg.localPosition;
                rightLegBase = rightLeg.localPosition;

                if (leftBoot != null) leftBootBase = leftBoot.localPosition;
                if (rightBoot != null) rightBootBase = rightBoot.localPosition;

                basesCaptured = true;
            }
        }

        private static void ApplyRotation(Transform part, float angle)
        {
            if (part == null) return;
            part.localRotation = Quaternion.Slerp(
                part.localRotation,
                Quaternion.Euler(angle, 0f, 0f),
                1f - Mathf.Exp(-18f * Time.deltaTime));
        }

        private static void ApplyStep(Transform part, Vector3 basePosition, float step)
        {
            if (part == null) return;
            Vector3 target = basePosition + new Vector3(0f, Mathf.Abs(step) * 0.18f, step);
            part.localPosition = Vector3.Lerp(
                part.localPosition,
                target,
                1f - Mathf.Exp(-18f * Time.deltaTime));
        }

        private void SettleRotation(Transform part)
        {
            if (part == null) return;
            part.localRotation = Quaternion.Slerp(
                part.localRotation,
                Quaternion.identity,
                1f - Mathf.Exp(-settleSpeed * Time.deltaTime));
        }

        private void SettlePosition(Transform part, Vector3 target)
        {
            if (part == null) return;
            part.localPosition = Vector3.Lerp(
                part.localPosition,
                target,
                1f - Mathf.Exp(-settleSpeed * Time.deltaTime));
        }
    }
}

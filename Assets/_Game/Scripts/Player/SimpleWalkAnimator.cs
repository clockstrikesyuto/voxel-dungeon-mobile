using UnityEngine;

namespace VoxelDungeon.Player
{
    public sealed class SimpleWalkAnimator : MonoBehaviour
    {
        [SerializeField] private float strideAngle = 24f;
        [SerializeField] private float strideSpeed = 9f;
        [SerializeField] private float armSwing = 16f;
        [SerializeField] private float settleSpeed = 10f;

        private TopDownPlayerMotor motor;
        private Transform leftLeg;
        private Transform rightLeg;
        private Transform leftArm;
        private Transform rightArm;
        private float phase;

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
        }

        private void Start()
        {
            ResolveParts();
        }

        private void Update()
        {
            if (leftLeg == null || rightLeg == null)
                ResolveParts();

            Vector3 planarVelocity = Vector3.zero;
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                planarVelocity = controller.velocity;
                planarVelocity.y = 0f;
            }

            float speed = planarVelocity.magnitude;
            bool moving = speed > 0.08f;

            if (moving)
            {
                phase += Time.deltaTime * strideSpeed * Mathf.Clamp(speed / 4.5f, 0.65f, 1.55f);
                float leg = Mathf.Sin(phase) * strideAngle;
                float arm = Mathf.Sin(phase) * armSwing;

                ApplyRotation(leftLeg, leg);
                ApplyRotation(rightLeg, -leg);
                ApplyRotation(leftArm, -arm);
                ApplyRotation(rightArm, arm);
            }
            else
            {
                Settle(leftLeg);
                Settle(rightLeg);
                Settle(leftArm);
                Settle(rightArm);
            }
        }

        private void ResolveParts()
        {
            Transform visual = transform.Find("PlayerVisual");
            if (visual == null)
                return;

            leftLeg = visual.Find("Leg_L");
            rightLeg = visual.Find("Leg_R");
            leftArm = visual.Find("Arm_L");
            rightArm = visual.Find("Arm_R");
        }

        private void ApplyRotation(Transform part, float angle)
        {
            if (part == null) return;
            part.localRotation = Quaternion.Slerp(
                part.localRotation,
                Quaternion.Euler(angle, 0f, 0f),
                1f - Mathf.Exp(-18f * Time.deltaTime));
        }

        private void Settle(Transform part)
        {
            if (part == null) return;
            part.localRotation = Quaternion.Slerp(
                part.localRotation,
                Quaternion.identity,
                1f - Mathf.Exp(-settleSpeed * Time.deltaTime));
        }
    }
}

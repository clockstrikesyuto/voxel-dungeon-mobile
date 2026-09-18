using UnityEngine;

namespace VoxelDungeon.Combat
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class KnockbackMotor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float decay = 16f;

        private CharacterController controller;
        private Vector3 velocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public void AddImpulse(Vector3 direction, float strength)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f || strength <= 0f)
                return;

            velocity += direction.normalized * strength;
        }

        private void Update()
        {
            if (velocity.sqrMagnitude < 0.001f || controller == null || !controller.enabled)
                return;

            controller.Move(velocity * Time.deltaTime);
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, decay * Time.deltaTime);
        }
    }
}

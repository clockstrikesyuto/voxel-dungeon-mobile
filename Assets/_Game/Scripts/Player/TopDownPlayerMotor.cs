using UnityEngine;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class TopDownPlayerMotor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
        [SerializeField, Min(0f)] private float acceleration = 28f;
        [SerializeField, Min(0f)] private float gravity = 25f;

        private CharacterController controller;
        private Vector2 moveInput;
        private Vector3 planarVelocity;
        private float verticalVelocity;

        public Vector2 MoveInput => moveInput;

        private void Awake() => controller = GetComponent<CharacterController>();

        public void SetMoveInput(Vector2 input)
        {
            moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        private void Update()
        {
            Vector3 desired = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
            planarVelocity = Vector3.MoveTowards(planarVelocity, desired, acceleration * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            else
                verticalVelocity -= gravity * Time.deltaTime;

            Vector3 motion = planarVelocity + Vector3.up * verticalVelocity;
            controller.Move(motion * Time.deltaTime);

            Vector3 facing = new Vector3(planarVelocity.x, 0f, planarVelocity.z);
            if (facing.sqrMagnitude > 0.05f)
                transform.forward = Vector3.Slerp(transform.forward, facing.normalized, 18f * Time.deltaTime);
        }
    }
}

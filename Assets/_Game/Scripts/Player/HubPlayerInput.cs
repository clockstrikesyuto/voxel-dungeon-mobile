using UnityEngine;
using VoxelDungeon.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class HubPlayerInput : MonoBehaviour
    {
        [SerializeField] private MobileJoystick mobileJoystick;

        private TopDownPlayerMotor motor;

        public void SetMobileJoystick(MobileJoystick joystick)
        {
            mobileJoystick = joystick;
        }

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
        }

        private void Update()
        {
            Vector2 move = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1f;
            }

            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.sqrMagnitude > move.sqrMagnitude)
                    move = stick;
            }
#endif

            if (mobileJoystick != null && mobileJoystick.Value.sqrMagnitude > move.sqrMagnitude)
                move = mobileJoystick.Value;

            motor.SetMoveInput(move);
        }
    }
}

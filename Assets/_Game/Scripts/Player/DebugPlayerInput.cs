using UnityEngine;
using VoxelDungeon.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(TopDownPlayerMotor))]
    [RequireComponent(typeof(PlayerCombat))]
    public sealed class DebugPlayerInput : MonoBehaviour
    {
        [SerializeField] private MobileJoystick mobileJoystick;

        private TopDownPlayerMotor motor;
        private PlayerCombat combat;

        public void SetMobileJoystick(MobileJoystick joystick)
        {
            mobileJoystick = joystick;
        }

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
            combat = GetComponent<PlayerCombat>();
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

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    combat.TryMelee();

                if (Keyboard.current.leftShiftKey.wasPressedThisFrame ||
                    Keyboard.current.rightShiftKey.wasPressedThisFrame)
                    motor.TryDodge();
            }

            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.sqrMagnitude > move.sqrMagnitude)
                    move = stick;

                if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                    combat.TryMelee();

                if (Gamepad.current.buttonEast.wasPressedThisFrame)
                    motor.TryDodge();
            }
#endif

            if (mobileJoystick != null && mobileJoystick.Value.sqrMagnitude > move.sqrMagnitude)
                move = mobileJoystick.Value;

            motor.SetMoveInput(move);
        }
    }
}

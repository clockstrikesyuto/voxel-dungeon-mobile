using UnityEngine;

namespace VoxelDungeon.Input
{
    public sealed class MobileInputState : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public bool MeleePressed { get; private set; }
        public bool RangedPressed { get; private set; }
        public bool DodgePressed { get; private set; }
        public bool PotionPressed { get; private set; }
        public bool Skill1Pressed { get; private set; }
        public bool Skill2Pressed { get; private set; }
        public bool Skill3Pressed { get; private set; }

        public void SetMove(Vector2 value) => Move = Vector2.ClampMagnitude(value, 1f);
        public void PressMelee() => MeleePressed = true;
        public void PressRanged() => RangedPressed = true;
        public void PressDodge() => DodgePressed = true;
        public void PressPotion() => PotionPressed = true;
        public void PressSkill1() => Skill1Pressed = true;
        public void PressSkill2() => Skill2Pressed = true;
        public void PressSkill3() => Skill3Pressed = true;

        private void LateUpdate()
        {
            MeleePressed = false;
            RangedPressed = false;
            DodgePressed = false;
            PotionPressed = false;
            Skill1Pressed = false;
            Skill2Pressed = false;
            Skill3Pressed = false;
        }
    }
}

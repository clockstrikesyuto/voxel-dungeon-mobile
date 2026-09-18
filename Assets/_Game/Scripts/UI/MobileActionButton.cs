using UnityEngine;
using UnityEngine.EventSystems;
using VoxelDungeon.Player;

namespace VoxelDungeon.UI
{
    public sealed class MobileActionButton : MonoBehaviour, IPointerDownHandler
    {
        public enum ActionKind
        {
            Attack,
            Dodge
        }

        [SerializeField] private ActionKind action;
        [SerializeField] private TopDownPlayerMotor motor;
        [SerializeField] private PlayerCombat combat;

        public void Configure(ActionKind kind, TopDownPlayerMotor playerMotor, PlayerCombat playerCombat)
        {
            action = kind;
            motor = playerMotor;
            combat = playerCombat;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (action)
            {
                case ActionKind.Attack:
                    combat?.TryMelee();
                    break;
                case ActionKind.Dodge:
                    motor?.TryDodge();
                    break;
            }
        }
    }
}

using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
        private StarterAssetsInputs _starterAssetsInputs;

        public void SetStarterAssetsInputs(StarterAssetsInputs starterAssetsInputs)
        {
            _starterAssetsInputs = starterAssetsInputs;
        }

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            _starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            _starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            _starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            _starterAssetsInputs.SprintInput(virtualSprintState);
        }
        
    }

}

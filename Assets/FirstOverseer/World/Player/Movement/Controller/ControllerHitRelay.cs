using System;
using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class ControllerHitRelay : MonoBehaviour
    {
        public event Action<ControllerColliderHit> OnHit;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            OnHit?.Invoke(hit);
        }
    }
}
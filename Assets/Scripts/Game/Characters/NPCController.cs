using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace WhaleFall
{
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class NPCController : RTSPlayerController
    {
        public NPCManager nPCManager;

        public KinematicCharacterMotor KinematicMotor
        {
            get
            {
                return Motor;
            }
        }

        protected override void UpdateInput()
        {

        }

        protected override void Update()
        {
            base.Update();

            Vector3 targetDirection = GetTargetDirection();
            if (targetDirection == Vector3.zero)
            {
                MoveTo(nPCManager.GetNextWayPoint());
            }
        }
    }
}
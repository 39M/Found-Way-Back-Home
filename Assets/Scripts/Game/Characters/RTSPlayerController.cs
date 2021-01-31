using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace WhaleFall
{
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class RTSPlayerController : PlayerController
    {
        protected override Vector3 GetTargetDirection()
        {
            if (targetPos == Vector3.zero)
            {
                return Vector3.zero;
            }
            else if ((targetPos - transform.position).sqrMagnitude > .1f)
            {
                var targetDirection = targetPos - transform.position;
                targetDirection.y = 0;
                return targetDirection;
            }
            else
            {
                targetPos = Vector3.zero;
                return Vector3.zero;
            }
        }

        protected override void Update()
        {
            Vector3 targetDirection = GetTargetDirection();

            // Handle animation
            _forwardAxis = Mathf.Lerp(_forwardAxis, targetDirection.z, 1f - Mathf.Exp(-ForwardAxisSharpness * Time.deltaTime));
            _rightAxis = Mathf.Lerp(_rightAxis, targetDirection.x, 1f - Mathf.Exp(-TurnAxisSharpness * Time.deltaTime));

            if (targetDirection != Vector3.zero)
            {
                animator.SetBool(animIdRunning, true);
            }
            else
            {
                animator.SetBool(animIdRunning, false);
            }

            UpdateInput();
        }

        protected virtual void UpdateInput()
        {
            if (Input.GetMouseButton(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    //划出射线，只有在scene视图中才能看到
                    Debug.DrawLine(ray.origin, hitInfo.point);
                    GameObject gameObj = hitInfo.collider.gameObject;
                    Debug.Log("click object name is " + gameObj.name);
                }
                MoveTo(hitInfo.point);
            }
        }

        private Vector3 targetPos = Vector3.zero;
        public void MoveTo(Vector3 targetPos)
        {
            this.targetPos = new Vector3(targetPos.x, this.targetPos.y, targetPos.z);
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            Vector3 targetDirection = GetTargetDirection();
            if (targetDirection != Vector3.zero)
            {
                transform.forward = targetDirection.normalized;
            }
            currentRotation = transform.rotation;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace WhaleFall
{
    public class PlayerController : MonoBehaviour, ICharacterController
    {
        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;

        [Header("Animation Parameters")]
        public float ForwardAxisSharpness = 10;
        public float TurnAxisSharpness = 5;

        [Header("Misc")]
        public Vector3 Gravity = new Vector3(0, -30f, 0);

        protected float _forwardAxis;
        protected float _rightAxis;
        protected Vector3 _rootMotionPositionDelta;
        protected Quaternion _rootMotionRotationDelta;

        protected KinematicCharacterMotor Motor;

        public float turnLerpValue = .5f;

        protected Animator animator { get; set; }
        Vector3 moveDirection;
        protected int animIdRunning;
        protected int animIdInAir;

        protected virtual void Awake()
        {
            Motor = gameObject.GetOrAddComponent<KinematicCharacterMotor>();
            // Assign to motor
            Motor.CharacterController = this;
        }

        protected virtual void Start()
        {
            ClothPhysicsManager.ins.Init();
            _rootMotionPositionDelta = Vector3.zero;
            _rootMotionRotationDelta = Quaternion.identity;
            animator = GetComponent<Animator>();
            animIdRunning = Animator.StringToHash("Running");
            animIdInAir = Animator.StringToHash("InAir");
        }

        protected const string HORIZONTAL_INPUT_STR = "Horizontal";
        protected const string VERTICAL_INPUT_STR = "Vertical";
        protected virtual Vector3 GetTargetDirection()
        {
            return new Vector3
            {
                x = Input.GetAxisRaw(HORIZONTAL_INPUT_STR),
                y = 0.0f,
                z = Input.GetAxisRaw(VERTICAL_INPUT_STR),
            };
        }

        protected virtual void Update()
        {
            Vector3 targetDirection = GetTargetDirection();

            // Handle animation
            _forwardAxis = Mathf.Lerp(_forwardAxis, targetDirection.z, 1f - Mathf.Exp(-ForwardAxisSharpness * Time.deltaTime));
            _rightAxis = Mathf.Lerp(_rightAxis, targetDirection.x, 1f - Mathf.Exp(-TurnAxisSharpness * Time.deltaTime));

            float y = Camera.main.transform.rotation.eulerAngles.y;
            moveDirection = Quaternion.Euler(0, y, 0) * targetDirection;

            if (targetDirection != Vector3.zero)
            {
                Vector3 newforward = transform.forward.Lerp(moveDirection, turnLerpValue);
                _rootMotionRotationDelta = Quaternion.FromToRotation(transform.forward, newforward);
                animator.SetBool(animIdRunning, true);
            }
            else
            {
                animator.SetBool(animIdRunning, false);
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            currentRotation = _rootMotionRotationDelta * currentRotation;
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround)
            {
                if (deltaTime > 0)
                {
                    // The final velocity is the velocity from root motion reoriented on the ground plane
                    currentVelocity = _rootMotionPositionDelta / deltaTime;
                    currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
                }
                else
                {
                    // Prevent division by zero
                    currentVelocity = Vector3.zero;
                }
            }
            else
            {
                if (_forwardAxis > 0f)
                {
                    // If we want to move, add an acceleration to the velocity
                    Vector3 targetMovementVelocity = Motor.CharacterForward * _forwardAxis * MaxAirMoveSpeed;
                    Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                    currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                }

                // Gravity
                currentVelocity += Gravity * deltaTime;

                // Drag
                currentVelocity *= (1f / (1f + (Drag * deltaTime)));
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            // Reset root motion deltas
            _rootMotionPositionDelta = Vector3.zero;
            _rootMotionRotationDelta = Quaternion.identity;
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        protected void OnLanded()
        {
            animator.SetBool(animIdInAir, false);
        }

        protected void OnLeaveStableGround()
        {
            animator.SetBool(animIdInAir, true);
        }

        public void AddVelocity(Vector3 velocity)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        private void OnAnimatorMove()
        {
            // Accumulate rootMotion deltas between character updates 
            _rootMotionPositionDelta += animator.deltaPosition;
            //_rootMotionRotationDelta = animator.deltaRotation * _rootMotionRotationDelta;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}
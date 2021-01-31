using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhaleFall;

namespace KartGame.KartSystems
{
    /// <summary>
    /// A basic keyboard implementation of the IInput interface for all the input information a kart needs.
    /// </summary>
    public class KeyboardInput : MonoBehaviour, IInput
    {
        public float Acceleration
        {
            get { return m_Acceleration; }
        }
        public float Steering
        {
            get { return m_Steering; }
        }
        public bool HopPressed
        {
            get { return m_HopPressed; }
        }
        public bool HopHeld
        {
            get { return m_HopHeld; }
        }

        float m_Acceleration;
        float m_Steering;
        bool m_HopPressed;
        bool m_HopHeld;

        bool m_FixedUpdateHappened;

        const string HORIZONTAL_INPUT_STR = "Horizontal";
        const string VERTICAL_INPUT_STR = "Vertical";

        void Update ()
        {
            Vector3 input = new Vector3(Input.GetAxis(HORIZONTAL_INPUT_STR), 0, Input.GetAxis(VERTICAL_INPUT_STR)).normalized;
            if (Vector3.Dot(input, transform.forward) < 0)
            {
                input = transform.forward.Lerp(input, .6f);
            }

            m_Acceleration = Vector3.Dot(input, transform.forward);
            m_Steering = Vector3.Dot(input, transform.right);
            m_HopHeld = Input.GetKey(KeyCode.LeftShift);

            if (m_FixedUpdateHappened)
            {
                m_FixedUpdateHappened = false;

                m_HopPressed = false;
            }

            m_HopPressed |= Input.GetKeyDown (KeyCode.LeftShift);
        }

        void FixedUpdate ()
        {
            m_FixedUpdateHappened = true;
        }
    }
}
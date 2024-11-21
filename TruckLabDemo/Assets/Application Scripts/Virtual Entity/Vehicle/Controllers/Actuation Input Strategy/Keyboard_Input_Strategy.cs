using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Actuation_Input_Strategy
{
    /// <summary>
    /// An input strategy that uses keyboard inputs for controlling the vehicle.
    /// </summary>
    [CreateAssetMenu(fileName = "Keyboard_Input_Strategy", menuName = "Input Strategies/Keyboard")]
    public class Keyboard_Input_Strategy : Actuation_Input_Strategy
    {
        /// <summary>
        /// Fixed velocity for forward and backward movement using keyboard input.
        /// </summary>
        public float KeyboardInputVelocity = 5f;

        /// <summary>
        /// Steering rate (in radians per second) for left and right turns.
        /// </summary>
        public float SteerRate = 30f * Mathf.Deg2Rad;

        /// <summary>
        /// Initializes the input strategy. No additional setup required for manual input.
        /// </summary>
        /// <param name="twistSubscriber">Ignored for manual input strategy.</param>
        public override void Initialize(TwistSubscriber twistSubscriber = null) { }

        /// <summary>
        /// Retrieves input values for velocity and steering angle based on keyboard input.
        /// </summary>
        /// <returns>A tuple containing velocity and steering angle.</returns>
        public override (float velocity, float steerAngle) GetInput()
        {
            float inputVelocity = 0;
            float inputSteerAngle = 0;

            // Forward and backward movement
            if (Input.GetKey(KeyCode.W))
            {
                inputVelocity = KeyboardInputVelocity;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                inputVelocity = -KeyboardInputVelocity;
            }

            // Steering left and right
            if (Input.GetKey(KeyCode.A))
            {
                inputSteerAngle += SteerRate * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                inputSteerAngle -= SteerRate * Time.deltaTime;
            }

            return (inputVelocity, inputSteerAngle);
        }
    }
}

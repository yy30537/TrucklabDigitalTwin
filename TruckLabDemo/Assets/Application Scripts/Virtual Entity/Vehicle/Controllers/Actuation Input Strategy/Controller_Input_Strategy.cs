using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Actuation_Input_Strategy
{
    /// <summary>
    /// An input strategy that retrieves velocity and steering data from a ROS topic.
    /// Designed for use with game controllers or external control systems.
    /// </summary>
    [CreateAssetMenu(fileName = "Controller_Input_Strategy", menuName = "Input Strategies/Controller")]
    public class Controller_Input_Strategy : Actuation_Input_Strategy
    {
        /// <summary>
        /// The name of the ROS topic to subscribe to for control input.
        /// </summary>
        public string TopicName;

        /// <summary>
        /// Reference to the TwistSubscriber for receiving ROS twist messages.
        /// </summary>
        private TwistSubscriber twistSubscriber;

        /// <summary>
        /// Initializes the input strategy with the specified ROS topic.
        /// </summary>
        /// <param name="sharedTwistSubscriber">Shared TwistSubscriber instance for receiving ROS messages.</param>
        public override void Initialize(TwistSubscriber sharedTwistSubscriber)
        {
            twistSubscriber = sharedTwistSubscriber;
            twistSubscriber.Topic = TopicName; // Assign the ROS topic to the subscriber
        }

        /// <summary>
        /// Retrieves input values for velocity and steering angle from the ROS topic.
        /// </summary>
        /// <returns>A tuple containing velocity and steering angle.</returns>
        public override (float velocity, float steerAngle) GetInput()
        {
            float inputVelocity = 0;
            float inputSteerAngle = 0;

            // Retrieve data from the twist subscriber if available
            if (twistSubscriber != null)
            {
                inputVelocity = twistSubscriber.linearVelocity.y; // Forward/backward movement
                inputSteerAngle = twistSubscriber.angularVelocity.x; // Steering angle
            }

            return (inputVelocity, inputSteerAngle);
        }
    }
}

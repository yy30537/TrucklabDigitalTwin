using RosSharp.Scripts.RosBridgeClient.RosCommuncation;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle.Controllers.ActuationInputStrategy
{
    /// <summary>
    /// Abstract base class for actuation input strategies.
    /// Defines a framework for retrieving input data (e.g., velocity and steering angle).
    /// </summary>
    public abstract class Actuation_Input_Strategy : ScriptableObject
    {
        /// <summary>
        /// Name of the strategy, used for identification and debugging.
        /// </summary>
        public string StrategyName;

        /// <summary>
        /// Initializes the input strategy. Allows optional TwistSubscriber configuration.
        /// </summary>
        /// <param name="twistSubscriber">Optional shared TwistSubscriber for ROS integration.</param>
        public abstract void Initialize(TwistSubscriber twistSubscriber = null);

        /// <summary>
        /// Retrieves the current input values for velocity and steering angle.
        /// </summary>
        /// <returns>A tuple containing velocity and steering angle.</returns>
        public abstract (float velocity, float steerAngle) GetInput();
    }
}
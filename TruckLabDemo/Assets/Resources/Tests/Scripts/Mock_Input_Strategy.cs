// Assets/Scripts/Tests/MockInputStrategy.cs
using UnityEngine;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers.ActuationInputStrategy;
using RosSharp.Scripts.RosBridgeClient.RosCommuncation;

namespace Tests.Scripts
{
    /// <summary>
    /// Mock actuation input strategy for testing purposes.
    /// Returns predefined velocity and steering angle values.
    /// </summary>
    [CreateAssetMenu(fileName = "Mock_Input_Strategy", menuName = "Input Strategies/Mock")]
    public class Mock_Input_Strategy : Actuation_Input_Strategy
    {
        public float MockVelocity;
        public float MockSteerAngle;

        public override void Initialize(TwistSubscriber twistSubscriber = null)
        {
            
        }

        public override (float velocity, float steerAngle) GetInput()
        {
            return (MockVelocity, MockSteerAngle);
        }
    }
}

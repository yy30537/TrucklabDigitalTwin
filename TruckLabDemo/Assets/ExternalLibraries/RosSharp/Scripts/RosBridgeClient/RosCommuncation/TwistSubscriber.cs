/*
© CentraleSupelec, 2017
Author: Dr. Jeremy Fix (jeremy.fix@centralesupelec.fr)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain A copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Adjustments to new Publication Timing and Execution Framework
// © Siemens AG, 2018, Dr. Martin Bischoff (martin.bischoff@siemens.com)

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class TwistSubscriber : UnitySubscriber<MessageTypes.Geometry.Twist>
    {
        //public Transform SubscribedTransform;

        private float previousRealTime;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;

        
        protected override void ReceiveMessage(MessageTypes.Geometry.Twist message)
        {
            linearVelocity = ToVector3(message.linear).Ros2Unity();
            angularVelocity = -ToVector3(message.angular).Ros2Unity();
        }

        private static Vector3 ToVector3(MessageTypes.Geometry.Vector3 geometryVector3)
        {
            return new Vector3((float)geometryVector3.x, (float)geometryVector3.y, (float)geometryVector3.z);
        }

        private void Update()
        {
          //  if (isMessageReceived)
          //      ProcessMessage();
          //  previousRealTime = Time.realtimeSinceStartup;


        }
        
        /*
        private void ProcessMessage()
        {
            float deltaTime = Time.realtimeSinceStartup - previousRealTime;

            SubscribedTransform.Translate(linearVelocity * deltaTime);
            SubscribedTransform.Rotate(Vector3.forward, angularVelocity.X1 * deltaTime);
            SubscribedTransform.Rotate(Vector3.up, angularVelocity.Y1 * deltaTime);
            SubscribedTransform.Rotate(Vector3.left, angularVelocity.z * deltaTime);

            isMessageReceived = false;
        }
        */
    }
}
﻿/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

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

// Added allocation free alternatives
// UoK , 2019, Odysseas Doumas (od79@kent.ac.uk / odydoum@gmail.com)

using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.Scripts.Extensions;
using Quaternion = UnityEngine.Quaternion;
using Transform = UnityEngine.Transform;
using Vector3 = UnityEngine.Vector3;

namespace RosSharp.Scripts.RosBridgeClient.RosCommuncation
{
    public class PoseStampedPublisher : UnityPublisher<PoseStamped>
    {
        public Transform PublishedTransform;
        public string FrameId = "Unity";

        private global::RosSharp.RosBridgeClient.MessageTypes.Geometry.PoseStamped message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new global::RosSharp.RosBridgeClient.MessageTypes.Geometry.PoseStamped
            {
                header = new global::RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            GetGeometryPoint(PublishedTransform.position.Unity2Ros(), message.pose.position);
            GetGeometryQuaternion(PublishedTransform.rotation.Unity2Ros(), message.pose.orientation);

            Publish(message);
        }

        private static void GetGeometryPoint(Vector3 position, global::RosSharp.RosBridgeClient.MessageTypes.Geometry.Point geometryPoint)
        {
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
        }

        private static void GetGeometryQuaternion(Quaternion quaternion, global::RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            geometryQuaternion.x = quaternion.x;
            geometryQuaternion.y = quaternion.y;
            geometryQuaternion.z = quaternion.z;
            geometryQuaternion.w = quaternion.w;
        }

    }
}

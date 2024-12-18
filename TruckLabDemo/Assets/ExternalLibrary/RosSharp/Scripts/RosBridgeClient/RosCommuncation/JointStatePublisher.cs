﻿/*
© Siemens AG, 2017-2019
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

using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.Scripts.Extensions;
using RosSharp.Scripts.RosBridgeClient.MessageHandling;

namespace RosSharp.Scripts.RosBridgeClient.RosCommuncation
{
    public class JointStatePublisher : UnityPublisher<JointState>
    {
        public List<JointStateReader> JointStateReaders;
        public string FrameId = "Unity";

        private global::RosSharp.RosBridgeClient.MessageTypes.Sensor.JointState message;    
        
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
            int jointStateLength = JointStateReaders.Count;
            message = new global::RosSharp.RosBridgeClient.MessageTypes.Sensor.JointState
            {
                header = new global::RosSharp.RosBridgeClient.MessageTypes.Std.Header { frame_id = FrameId },
                name = new string[jointStateLength],
                position = new double[jointStateLength],
                velocity = new double[jointStateLength],
                effort = new double[jointStateLength]
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            for (int i = 0; i < JointStateReaders.Count; i++)
                UpdateJointState(i);

            Publish(message);
        }

        private void UpdateJointState(int i)
        {

            JointStateReaders[i].Read(
                out message.name[i],
                out float position,
                out float velocity,
                out float effort);

            message.position[i] = position;
            message.velocity[i] = velocity;
            message.effort[i] = effort;
        }


    }
}

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

using UnityEngine;

namespace RosSharp.Scripts.RosBridgeClient.MessageHandling
{
    [RequireComponent(typeof(HingeJoint))]
    public class JoyAxisJointMotorWriter : JoyAxisWriter
    {
        public float MaxVelocity;

        private HingeJoint _hingeJoint;
        private JointMotor jointMotor;
        private float targetVelocity;
        private bool isMessageReceived;

        private void Start()
        {
            _hingeJoint = GetComponent<HingeJoint>();
            _hingeJoint.useMotor = true;
        }

        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        private void ProcessMessage()
        {
            jointMotor = _hingeJoint.motor;
            jointMotor.targetVelocity = targetVelocity;
            _hingeJoint.motor = jointMotor;
            isMessageReceived = false;
        }

        public override void Write(float value)
        {
            targetVelocity = value * MaxVelocity;
            isMessageReceived = true;
        }
    }

}
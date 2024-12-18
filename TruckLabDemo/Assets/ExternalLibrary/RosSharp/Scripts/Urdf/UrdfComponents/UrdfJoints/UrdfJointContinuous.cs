﻿/*
© Siemens AG, 2018-2019
Author: Suzannah Smith (suzannah.smith@siemens.com)

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
using Joint = RosSharp.Urdf.Joint;

namespace RosSharp.Scripts.Urdf.UrdfComponents.UrdfJoints
{
    public class UrdfJointContinuous : UrdfJoint
    {
        public static UrdfJoint Create(GameObject linkObject)
        {
            UrdfJointContinuous urdfJoint = linkObject.AddComponent<UrdfJointContinuous>();
            urdfJoint._jointType = JointTypes.Continuous;
            urdfJoint.UnityJoint = linkObject.AddComponent<HingeJoint>();
            urdfJoint.UnityJoint.autoConfigureConnectedAnchor = true;
            linkObject.AddComponent<HingeJointAngleCalculator>();

            return urdfJoint;
        }

        #region Runtime

        public override float GetPosition()
        {
            return -GetComponent<HingeJointAngleCalculator>().Angle * Mathf.Deg2Rad;
        }
        public override float GetVelocity()
        {
            return -((HingeJoint)UnityJoint).velocity * Mathf.Deg2Rad;
        }
        public override float GetEffort()
        {
            return -((HingeJoint)UnityJoint).motor.force;
        }

        protected override void OnUpdateJointState(float deltaState)
        {
            Quaternion rot = Quaternion.AngleAxis(-deltaState * Mathf.Rad2Deg, UnityJoint.axis);
            transform.rotation = transform.rotation * rot;
        }

        #endregion

        protected override void ImportJointData(Joint joint)
        {
            UnityJoint.axis = (joint.axis != null) ? GetAxis(joint.axis) : GetDefaultAxis();

            if (joint.dynamics != null)
                ((HingeJoint)UnityJoint).spring = GetJointSpring(joint.dynamics);
        }

        protected override Joint ExportSpecificJointData(Joint joint)
        {
            joint.axis = GetAxisData(UnityJoint.axis);
            joint.dynamics = new Joint.Dynamics(
                ((HingeJoint)UnityJoint).spring.damper, 
                ((HingeJoint)UnityJoint).spring.spring);
            
            return joint;
        }
    }
}


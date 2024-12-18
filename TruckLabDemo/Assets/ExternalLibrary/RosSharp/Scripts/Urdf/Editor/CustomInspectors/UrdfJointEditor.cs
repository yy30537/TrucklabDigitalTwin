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

using RosSharp.Scripts.Urdf.UrdfComponents.UrdfJoints;
using UnityEditor;
using UnityEngine;

namespace RosSharp.Scripts.Urdf.Editor.CustomInspectors
{
    [CustomEditor(typeof(UrdfJoint), true)]
    public class UrdfJointEditor : UnityEditor.Editor
    {
        private UrdfJoint urdfJoint;
        private bool showDetails;

        protected virtual void OnEnable()
        {
            urdfJoint = (UrdfJoint)serializedObject.targetObject;    
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);

            UrdfJoint.JointTypes newJointType = urdfJoint.JointType;

            EditorGUILayout.BeginVertical("HelpBox");
            newJointType = (UrdfJoint.JointTypes)EditorGUILayout.EnumPopup(
                "Type of joint", newJointType);
            if (newJointType != urdfJoint.JointType)
            {
                if (EditorUtility.DisplayDialog("Confirm joint type change",
                    "Are you sure you want to change the joint type? This will erase all information currently stored in the joint.",
                    "Continue", "Cancel"))
                {
                    UrdfJoint.ChangeJointType(urdfJoint.gameObject, newJointType);
                }
            }
            EditorGUILayout.EndVertical();

            showDetails = EditorGUILayout.Foldout(showDetails, "Joint URDF Configuration", true);
            if (showDetails)
            {
                urdfJoint.JointName = EditorGUILayout.TextField("SpaceName", urdfJoint.JointName);

                if (urdfJoint.JointType != UrdfJoint.JointTypes.Fixed)
                    GUILayout.BeginVertical("HelpBox");
                switch (urdfJoint.JointType)
                {
                    case UrdfJoint.JointTypes.Fixed:
                        break;
                    case UrdfJoint.JointTypes.Continuous:
                        DisplayDynamicsMessage("HingeJoint > Spring > Damper (for damping) and Spring (for friction)");
                        DisplayAxisMessage("HingeJoint > Axis");
                        break;
                    case UrdfJoint.JointTypes.Revolute:
                        DisplayDynamicsMessage("HingeJoint > Spring > Damper (for damping) and Spring (for friction)");
                        DisplayAxisMessage("HingeJoint > Axis");
                        DisplayRequiredLimitMessage("Hinge Joint Limits Application > Large Angle Limit  / Max");
                        break;
                    case UrdfJoint.JointTypes.Floating:
                        DisplayDynamicsMessage("ConfigurableJoint > xDrive > Position Damper (for Damping) and Position Spring (for friction)");
                        break;
                    case UrdfJoint.JointTypes.Prismatic:
                        DisplayDynamicsMessage("ConfigurableJoint > xDrive > Position Damper (for Damping) and Position Spring (for friction)");
                        DisplayAxisMessage("ConfigurableJoint > Axis");
                        DisplayRequiredLimitMessage("Prismatic Joint Limits Application > Position Limit Min / Max");
                        break;
                    case UrdfJoint.JointTypes.Planar:
                        DisplayDynamicsMessage("ConfigurableJoint > xDrive > Position Damper (for Damping) and Position Spring (for friction)");
                        DisplayAxisMessage("ConfigurableJoint > Axis and Secondary Axis");
                        DisplayRequiredLimitMessage("ConfigurableJoint > Linear Limit > Limit");
                        break;
                }

                if (urdfJoint.JointType != UrdfJoint.JointTypes.Fixed)
                    GUILayout.EndVertical();
            }
        }

        private void DisplayDynamicsMessage(string dynamicsLocation)
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Joint Dynamics (optional)");

            EditorGUILayout.HelpBox("To define damping and friction values, edit the fields " + dynamicsLocation + ".", MessageType.Info);

        }

        private void DisplayAxisMessage(string axisLocation)
        {
            GUILayout.Space(5);
            
            EditorGUILayout.LabelField("Joint Axis");

            EditorGUILayout.HelpBox("An axis is required for this joint type. Remember to define an axis in " + axisLocation + ".", MessageType.Info);
        }

        public void DisplayRequiredLimitMessage(string limitLocation)
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Joint Limits");

            urdfJoint.EffortLimit = EditorGUILayout.DoubleField("Effort Limit", urdfJoint.EffortLimit);
            urdfJoint.VelocityLimit = EditorGUILayout.DoubleField("currentVelocity Limit", urdfJoint.VelocityLimit);

            if (!urdfJoint.AreLimitsCorrect())
                EditorGUILayout.HelpBox("Limits are required for this joint type. Please enter valid limit values in " + limitLocation + ".", MessageType.Warning);

            GUILayout.Space(5);
        }
    }
}

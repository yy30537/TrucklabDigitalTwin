using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Core
{
    public class VehicleKinematics : VehicleComponent
    {
        public bool isActive = false;
        
        public ActuationInputSource actuationInputSource;
        public KinematicsSource kinematicsSource;
        
        public float inputVelocity;
        public float inputSteerAngle;

        private float keyboardInputVelocity = 2f;
        private float steerRate = 30f * Mathf.Deg2Rad;
        
        [Header("Variables")]
        public float v1, a, delta, gamma;
        public float x0, y0, x1, y1, psi1, x1C, y1C, x2, y2, psi2;
        public float x1Prev, y1Prev, psi1Prev, x1dot, y1dot, psi1dot, x2dot, y2dot, psi2dot, v2, psi2Prev;

        void FixedUpdate()
        {
            if (isActive)
            {
                // Update actuation input variables
                UpdateActuationInputs();

                // Update vehicle kinematic variables 
                if (VehicleData.VehicleConfig.isMocapAvaialbe && kinematicsSource == KinematicsSource.motioncapture)
                {
                    MotionCapture();
                }
                else if (kinematicsSource == KinematicsSource.actuation)
                {
                    Actuation();
                    Intermedstates();
                }

                // Update vehicle data
                UpdateVehicleData();
            }
        }

        public override void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            base.Initialize(vehicleProduct, vehicleData);
            x1 = vehicleProduct.vehicleConfig.initialTractorPosX;
            y1 = vehicleProduct.vehicleConfig.initialTractorPosY;
            psi1 = vehicleProduct.vehicleConfig.initialTractorAngle * Mathf.Deg2Rad;
            psi2 = vehicleProduct.vehicleConfig.initialTrailerAngle * Mathf.Deg2Rad;
            v1 = vehicleProduct.vehicleConfig.initialVelocity;
            a = vehicleProduct.vehicleConfig.initialAcceleration;
            delta = vehicleProduct.vehicleConfig.initialSteeringAngle;
            
            kinematicsSource = vehicleProduct.vehicleConfig.kinematicsSource;
            actuationInputSource = vehicleProduct.vehicleConfig.actuationInputSource;
        }
        
        private void UpdateActuationInputs()
        {
            if (VehicleData.VehicleConfig.isRosAvaialbe)
            {
                if (actuationInputSource == ActuationInputSource.controller)
                {
                    ControllerInput();
                }
                else if (actuationInputSource == ActuationInputSource.thrustmaster)
                {
                    ThrustmasterInput();
                }
            }
            if (actuationInputSource == ActuationInputSource.keyboard)
            {
                KeyboardInput();
            }
        }
        private void MotionCapture()
        {
            x1 = vehicleProduct.tractorRigidBody.position.z * VehicleData.VehicleConfig.scale;
            y1 = -vehicleProduct.tractorRigidBody.position.x * VehicleData.VehicleConfig.scale;
            psi1 = -vehicleProduct.tractorRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;
            
            x2 = vehicleProduct.trailerRigidBody.position.z * VehicleData.VehicleConfig.scale;
            y2 = -vehicleProduct.trailerRigidBody.position.x * VehicleData.VehicleConfig.scale;
            psi2 = -vehicleProduct.trailerRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;
            
            // front axle position
            x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
            y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);
            
            // 5th wheel tractor(coupling to semi-trailer) position
            x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
            y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);
        }
        private void Actuation()
        {
            // Update previous variable for new loop

            // front axle position
            x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
            y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

            // rear axle position
            x1 = x1Prev + x1dot * Time.deltaTime;
            y1 = y1Prev + y1dot * Time.deltaTime;

            // 5th wheel tractor(coupling to semi-trailer) position
            x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
            y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);

            // semi trailer center axle
            x2 = x1C - VehicleData.l2 * Mathf.Cos(psi2);
            y2 = y1C - VehicleData.l2 * Mathf.Sin(psi2);
        }
        
        private void Intermedstates()
        {
            x1Prev = x1;
            y1Prev = y1;
            psi1Prev = psi1;
            psi2Prev = psi2;

            // Update Actuation
            v1 = inputVelocity; // m/s
            delta = inputSteerAngle; // rad

            gamma = psi1 - psi2;

            // Intermediate states
            psi1dot = (v1 / VehicleData.l1) * Mathf.Tan(delta);
            psi1 = psi1Prev + (psi1dot * Time.deltaTime);

            x1dot = v1 * Mathf.Cos(psi1);
            y1dot = v1 * Mathf.Sin(psi1);

            psi2dot = (v1 * Mathf.Sin(gamma) + psi1dot * VehicleData.l1C * Mathf.Cos(gamma)) / VehicleData.l2;
            psi2 = psi2Prev + (psi2dot * Time.deltaTime);

            v2 = v1 * Mathf.Cos(gamma) - psi1dot * VehicleData.l1C * Mathf.Sin(gamma);
        }
        
        private void ControllerInput()
        {
            // TODO: update ros topic
            vehicleProduct.twistSubscriber.Topic = vehicleProduct.vehicleConfig.twistSubscriberTopicController;
            //inputVelocity = vehicleProduct.twistSubscriber.linearVelocity.y;
            //inputSteerAngle = vehicleProduct.twistSubscriber.angularVelocity.x;
        }
        private void ThrustmasterInput()
        {
            // TODO: update ros topic
            vehicleProduct.twistSubscriber.Topic = vehicleProduct.vehicleConfig.twistSubscriberTopicThrustmaster;
            //inputVelocity = vehicleProduct.twistSubscriber.linearVelocity.y;
            //inputSteerAngle = vehicleProduct.twistSubscriber.angularVelocity.x;
        }
        private void KeyboardInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                inputVelocity = keyboardInputVelocity;
            } else if (Input.GetKey(KeyCode.S))
            {
                inputVelocity = keyboardInputVelocity * (-1);
            }
            else
            {
                inputVelocity = 0;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputSteerAngle += steerRate * Time.deltaTime;
            } else if (Input.GetKey(KeyCode.D))
            {
                inputSteerAngle -= steerRate * Time.deltaTime;
            }
            else
            {
                inputSteerAngle = 0;
            }
            
        }
        
        public void SetVehiclePosition(float x, float y, float psi1Degrees, float psi2Degrees)
        {
            // Update internal state variables
            x1 = x * VehicleData.VehicleConfig.scale; // Adjust as needed for scale
            y1 = y * VehicleData.VehicleConfig.scale; // Adjust as needed for scale
            psi1 = psi1Degrees * Mathf.Deg2Rad;
            psi2 = psi2Degrees * Mathf.Deg2Rad;
            Intermedstates();
        }
        public void SetTractorAngle(float inputValue, float angleAdjustmentRate)
        {
            psi1 += inputValue * angleAdjustmentRate;
            Intermedstates();
        }
        public void SetTrailerAngle(float inputValue, float angleAdjustmentRate)
        {
            psi2 += inputValue * angleAdjustmentRate;
            Intermedstates();
        }
        private void UpdateVehicleData()
        {
            VehicleData.x0 = x0;
            VehicleData.y0 = y0;
            VehicleData.x1 = x1;
            VehicleData.y1 = y1;
            VehicleData.psi1 = psi1;
            VehicleData.x1C = x1C;
            VehicleData.y1C = y1C;
            VehicleData.x2 = x2;
            VehicleData.y2 = y2;
            VehicleData.psi2 = psi2;
            VehicleData.v1 = v1;
            VehicleData.a = a;
            VehicleData.delta = delta;
            VehicleData.gamma = gamma;
            VehicleData.x1Prev = x1Prev;
            VehicleData.y1Prev = y1Prev;
            VehicleData.psi1Prev = psi1Prev;
            VehicleData.x1dot = x1dot;
            VehicleData.y1dot = y1dot;
            VehicleData.psi1dot = psi1dot;
            VehicleData.x2dot = x2dot;
            VehicleData.y2dot = y2dot;
            VehicleData.psi2dot = psi2dot;
            VehicleData.v2 = v2;
            VehicleData.psi2Prev = psi2Prev;
        }
        
        public void InitPathSimulation(Path path)
        {
            Vector2 frontAxlePosition = path.frontaxle[0];
            Vector2 rearAxlePosition = path.rearaxle[0];
            Vector2 trailerAxlePosition = path.traileraxle[0];
            Vector2 orientation = path.psi[0];

            // Update vehicle positions and orientations
            x1 = frontAxlePosition.x;
            y1 = frontAxlePosition.y;
            psi1 = orientation.x;

            x2 = trailerAxlePosition.x;
            y2 = trailerAxlePosition.y;
            psi2 = orientation.y;

            // Update intermediate states
            x0 = frontAxlePosition.x + VehicleData.l1 * Mathf.Cos(VehicleData.psi1);
            y0 = frontAxlePosition.y + VehicleData.l1 * Mathf.Sin(VehicleData.psi1);
            x1C = rearAxlePosition.x + VehicleData.l1C * Mathf.Cos(VehicleData.psi1);
            y1C = rearAxlePosition.y + VehicleData.l1C * Mathf.Sin(VehicleData.psi1);

            Intermedstates();
            Actuation();
            UpdateVehicleData();
        }
        private IEnumerator VisualizePathCoroutine(Path path)
        {
            for (int i = 0; i < path.time.Count; i++)
            {
                Vector2 frontAxlePosition = path.frontaxle[i];
                Vector2 rearAxlePosition = path.rearaxle[i];
                Vector2 trailerAxlePosition = path.traileraxle[i];
                Vector2 orientation = path.psi[i];
                
                //inputVelocity = path.velocities[i]; 
                //inputSteerAngle = path.steeringAngles[i];

                
                // front axle position
                x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
                y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

                // Update vehicle positions and orientations
                x1 = frontAxlePosition.x;
                y1 = frontAxlePosition.y;
                psi1 = orientation.x;

                // 5th wheel tractor(coupling to semi-trailer) position
                x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
                y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);

                x2 = trailerAxlePosition.x;
                y2 = trailerAxlePosition.y;
                psi2 = orientation.y;
                
                Intermedstates();
                UpdateVehicleData();

                // Wait for the next frame
                float waitTime = path.time[i] - (i > 0 ? path.time[i - 1] : 0);
                yield return new WaitForSeconds(waitTime);
            }
        }
        public void VisualizePath(Path path)
        {
            StartCoroutine(VisualizePathCoroutine(path));
        }

    }       
}

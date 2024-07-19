using System;
using System.Collections;
using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Core
{
    /// <summary>
    /// Handles the kinematics of the vehicle, managing its movement and physical properties.
    /// </summary>
    public class VehicleKinematics : VehicleComponent
    {
        public bool isActive = false;

        public ActuationInputSource actuationInputSource;
        public KinematicsSource kinematicsSource;

        public float inputVelocity;
        public float inputSteerAngle;

        private float keyboardInputVelocity = 5f;
        private float steerRate = 30f * Mathf.Deg2Rad;

        [Header("Variables")]
        public float v1, a, delta, gamma;
        public float x0, y0, x1, y1, psi1, x1C, y1C, x2, y2, psi2;
        public float x1Prev, y1Prev, psi1Prev, x1dot, y1dot, psi1dot, x2dot, y2dot, psi2dot, v2, psi2Prev;

        private bool isBraking = false;

        /// <summary>
        /// FixedUpdate is called at fixed intervals, ideal for handling physics and kinematics.
        /// </summary>
        private void FixedUpdate()
        {
            if (isActive)
            {
                //if (isBraking)
                //{
                //    Brake();
                //}
                
                UpdateActuationInputs();
                if (VehicleData.VehicleConfig.IsMoCapAvailable && kinematicsSource == KinematicsSource.MotionCapture)
                {
                    MotionCapture();
                }
                else if (kinematicsSource == KinematicsSource.Actuation)
                {
                    Actuation();
                    Intermedstates();
                }
                

                UpdateVehicleData();
            }
        }

        /// <summary>
        /// Initializes the kinematics component with the given vehicle and data.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product instance.</param>
        /// <param name="vehicleData">The vehicle data instance.</param>
        public override void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            base.Initialize(vehicleProduct, vehicleData);
            kinematicsSource = vehicleProduct.Config.KinematicsSource;
            actuationInputSource = vehicleProduct.Config.ActuationInputSource;

            if (kinematicsSource != KinematicsSource.MotionCapture)
            {
                x1 = vehicleProduct.Config.InitialTractorPosX;
                y1 = vehicleProduct.Config.InitialTractorPosY;
                psi1 = vehicleProduct.Config.InitialTractorAngle * Mathf.Deg2Rad;
                psi2 = vehicleProduct.Config.InitialTrailerAngle * Mathf.Deg2Rad;
                Intermedstates();

                v1 = vehicleProduct.Config.InitialVelocity;
                a = vehicleProduct.Config.InitialAcceleration;
                delta = vehicleProduct.Config.InitialSteeringAngle;

                UpdateVehicleData();
            }

        }

        /// <summary>
        /// Updates the actuation input variables based on the input source.
        /// </summary>
        private void UpdateActuationInputs()
        {
            if (VehicleData.VehicleConfig.IsRosAvailable)
            {
                switch (actuationInputSource)
                {
                    case ActuationInputSource.Controller:
                        ControllerInput();
                        break;
                    case ActuationInputSource.ThrustMaster:
                        ThrustmasterInput();
                        break;
                }
            }
            if (actuationInputSource == ActuationInputSource.Keyboard)
            {
                KeyboardInput();
            }
        }

        /// <summary>
        /// Updates the vehicle state using motion capture data.
        /// </summary>
        private void MotionCapture()
        {
            x1 = VehicleProduct.TractorRigidBody.position.z * VehicleData.VehicleConfig.Scale;
            y1 = -VehicleProduct.TractorRigidBody.position.x * VehicleData.VehicleConfig.Scale;
            psi1 = -VehicleProduct.TractorRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;

            x2 = VehicleProduct.TrailerRigidBody.position.z * VehicleData.VehicleConfig.Scale;
            y2 = -VehicleProduct.TrailerRigidBody.position.x * VehicleData.VehicleConfig.Scale;
            psi2 = -VehicleProduct.TrailerRigidBody.rotation.eulerAngles.y * Mathf.Deg2Rad;

            x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
            y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

            x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
            y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);
        }

        /// <summary>
        /// Updates the vehicle state using actuation input data.
        /// </summary>
        private void Actuation()
        {
            x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
            y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

            x1 = x1Prev + x1dot * Time.deltaTime;
            y1 = y1Prev + y1dot * Time.deltaTime;

            x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
            y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);

            x2 = x1C - VehicleData.l2 * Mathf.Cos(psi2);
            y2 = y1C - VehicleData.l2 * Mathf.Sin(psi2);
        }

        /// <summary>
        /// Updates intermediate states of the vehicle.
        /// </summary>
        private void Intermedstates()
        {
            x1Prev = x1;
            y1Prev = y1;
            psi1Prev = psi1;
            psi2Prev = psi2;

            v1 = inputVelocity; // m/s
            delta = inputSteerAngle; // rad

            gamma = psi1 - psi2;

            psi1dot = (v1 / VehicleData.l1) * Mathf.Tan(delta);
            psi1 = psi1Prev + (psi1dot * Time.deltaTime);

            x1dot = v1 * Mathf.Cos(psi1);
            y1dot = v1 * Mathf.Sin(psi1);

            psi2dot = (v1 * Mathf.Sin(gamma) + psi1dot * VehicleData.l1C * Mathf.Cos(gamma)) / VehicleData.l2;
            psi2 = psi2Prev + (psi2dot * Time.deltaTime);

            v2 = v1 * Mathf.Cos(gamma) - psi1dot * VehicleData.l1C * Mathf.Sin(gamma);
        }

        /// <summary>
        /// Updates the vehicle state using controller input data.
        /// </summary>
        private void ControllerInput()
        {
            VehicleProduct.TwistSubscriber.Topic = VehicleProduct.Config.TwistSubscriberTopicController;
            // inputVelocity = vehicleProduct.TwistSubscriber.linearVelocity.y;
            // inputSteerAngle = vehicleProduct.TwistSubscriber.angularVelocity.x;
        }

        /// <summary>
        /// Updates the vehicle state using ThrustMaster input data.
        /// </summary>
        private void ThrustmasterInput()
        {
            VehicleProduct.TwistSubscriber.Topic = VehicleProduct.Config.TwistSubscriberTopicThrustMaster;
            // inputVelocity = vehicleProduct.TwistSubscriber.linearVelocity.y;
            // inputSteerAngle = vehicleProduct.TwistSubscriber.angularVelocity.x;
        }

        /// <summary>
        /// Updates the vehicle state using keyboard input data.
        /// </summary>
        private void KeyboardInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                inputVelocity = keyboardInputVelocity;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                inputVelocity = -keyboardInputVelocity;
            }
            else
            {
                inputVelocity = 0;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputSteerAngle += steerRate * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                inputSteerAngle -= steerRate * Time.deltaTime;
            }
            else
            {
                inputSteerAngle = 0;
            }
        }

        /// <summary>
        /// Perform Braking (initial set up)
        /// </summary>
        private void Brake()
        {
            // TODO more advanced braking logic
            // inputVelocity = 0;
        }

        /// <summary>
        /// Sets the vehicle position and orientation.
        /// </summary>
        /// <param name="x">The x-coordinate of the tractor.</param>
        /// <param name="y">The y-coordinate of the tractor.</param>
        /// <param name="psi1Degrees">The orientation of the tractor in degrees.</param>
        /// <param name="psi2Degrees">The orientation of the trailer in degrees.</param>
        public void SetVehiclePosition(float x, float y, float psi1Degrees, float psi2Degrees)
        {
            // Apply scale
            var scale = VehicleData.VehicleConfig.Scale;
            x1 = x * scale;
            y1 = y * scale;

            psi1 = psi1Degrees * Mathf.Deg2Rad;
            psi2 = psi2Degrees * Mathf.Deg2Rad;

            // Calculate and update the front axle position
            x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
            y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

            // Update the fifth wheel position
            x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
            y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);

            // Update the semitrailer center axle position
            x2 = x1C - VehicleData.l2 * Mathf.Cos(psi2);
            y2 = y1C - VehicleData.l2 * Mathf.Sin(psi2);

            Intermedstates();
            UpdateVehicleData();
        }


        /// <summary>
        /// Sets the tractor angle.
        /// </summary>
        /// <param name="inputValue">The input value for the angle adjustment.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle is adjusted.</param>
        public void SetTractorAngle(float inputValue, float angleAdjustmentRate)
        {
            psi1 += inputValue * angleAdjustmentRate;
            Intermedstates();
        }

        /// <summary>
        /// Sets the trailer angle.
        /// </summary>
        /// <param name="inputValue">The input value for the angle adjustment.</param>
        /// <param name="angleAdjustmentRate">The rate at which the angle is adjusted.</param>
        public void SetTrailerAngle(float inputValue, float angleAdjustmentRate)
        {
            psi2 += inputValue * angleAdjustmentRate;
            Intermedstates();
        }

        /// <summary>
        /// Updates the vehicle data with the current state.
        /// </summary>
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

        /// <summary>
        /// Initializes the path visualization with the given path.
        /// </summary>
        /// <param name="path">The path to visualize.</param>
        public void InitPathVisualization(Path path)
        {
            Vector2 frontAxlePosition = path.frontaxle[0];
            Vector2 rearAxlePosition = path.rearaxle[0];
            Vector2 trailerAxlePosition = path.traileraxle[0];
            Vector2 orientation = path.psi[0];

            x1 = frontAxlePosition.x;
            y1 = frontAxlePosition.y;
            psi1 = orientation.x;

            x2 = trailerAxlePosition.x;
            y2 = trailerAxlePosition.y;
            psi2 = orientation.y;

            x0 = frontAxlePosition.x + VehicleData.l1 * Mathf.Cos(VehicleData.psi1);
            y0 = frontAxlePosition.y + VehicleData.l1 * Mathf.Sin(VehicleData.psi1);
            x1C = rearAxlePosition.x + VehicleData.l1C * Mathf.Cos(VehicleData.psi1);
            y1C = rearAxlePosition.y + VehicleData.l1C * Mathf.Sin(VehicleData.psi1);

            Intermedstates();
            Actuation();
            UpdateVehicleData();
        }


        /// <summary>
        /// Visualizes the path by updating the vehicle's state over time.
        /// </summary>
        /// <param name="path">The path to visualize.</param>
        public void VisualizePath(Path path)
        {
            StartCoroutine(VisualizePathCoroutine(path));
        }

        /// <summary>
        /// Coroutine to visualize the path by updating the vehicle's state at each time step.
        /// </summary>
        /// <param name="path">The path to visualize.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private IEnumerator VisualizePathCoroutine(Path path)
        {
            for (int i = 0; i < path.time.Count; i++)
            {
                Vector2 frontAxlePosition = path.frontaxle[i];
                Vector2 rearAxlePosition = path.rearaxle[i];
                Vector2 trailerAxlePosition = path.traileraxle[i];
                Vector2 orientation = path.psi[i];

                x0 = x1 + VehicleData.l1 * Mathf.Cos(psi1);
                y0 = y1 + VehicleData.l1 * Mathf.Sin(psi1);

                x1 = frontAxlePosition.x;
                y1 = frontAxlePosition.y;
                psi1 = orientation.x;

                x1C = x1 + VehicleData.l1C * Mathf.Cos(psi1);
                y1C = y1 + VehicleData.l1C * Mathf.Sin(psi1);

                x2 = trailerAxlePosition.x;
                y2 = trailerAxlePosition.y;
                psi2 = orientation.y;

                Intermedstates();
                UpdateVehicleData();

                float waitTime = path.time[i] - (i > 0 ? path.time[i - 1] : 0);
                yield return new WaitForSeconds(waitTime);
            }
        }

        /// <summary>
        /// Applies brake to the vehicle
        /// </summary>
        public void ApplyBrakes()
        {
            isBraking = true;
        }

        /// <summary>
        /// Releases the brake
        /// </summary>
        public void ReleaseBrakes()
        {
            isBraking = false;
        }

    }
}

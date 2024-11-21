using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Application_Scripts.Manager.Path_Manager
{
    /// <summary>
    /// Represents a path for VeVehicle simulation, containing axle positions, steering data, 
    /// and other simulation-related properties.
    /// </summary>
    [System.Serializable]
    public class Path
    {
        /// <summary>
        /// Name of the path, used for identification.
        /// </summary>
        public string PathName;

        /// <summary>
        /// Unique identifier for the path.
        /// </summary>
        public int PathId;

        /// <summary>
        /// List of positions for the front axle throughout the path.
        /// </summary>
        public List<Vector2> FrontAxle;

        /// <summary>
        /// List of positions for the pivot point throughout the path.
        /// </summary>
        public List<Vector2> Pivot;

        /// <summary>
        /// List of positions for the rear axle throughout the path.
        /// </summary>
        public List<Vector2> RearAxle;

        /// <summary>
        /// List of positions for the trailer axle throughout the path.
        /// </summary>
        public List<Vector2> TrailerAxle;

        /// <summary>
        /// List of yaw angles (orientation) throughout the path.
        /// </summary>
        public List<Vector2> Psi;

        /// <summary>
        /// The starting pose of the vehicle at the beginning of the path.
        /// </summary>
        public List<float> StartPose;

        /// <summary>
        /// The ending pose of the vehicle at the end of the path.
        /// </summary>
        public List<float> EndPose;

        /// <summary>
        /// List of time steps corresponding to the simulation path.
        /// </summary>
        public List<float> Time;

        /// <summary>
        /// List of velocities recorded throughout the path.
        /// </summary>
        public List<float> Velocities;

        /// <summary>
        /// List of steering angles recorded throughout the path.
        /// </summary>
        public List<float> SteeringAngles;

        /// <summary>
        /// Simulation input data, including steering and velocity inputs.
        /// </summary>
        public SimulationInput SimInput;

        /// <summary>
        /// Indicates whether the path data is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        /// <param name="json">The JSON string representing the path data.</param>
        /// <param name="id">The unique ID of the path.</param>
        /// <param name="name">The name of the path.</param>
        public Path(string json, int id, string name)
        {
            PathId = id;
            PathName = name;
            Deserialize(json);
        }

        /// <summary>
        /// Deserializes the JSON string into the path object, populating its fields.
        /// </summary>
        /// <param name="json">The JSON string representing the path data.</param>
        private void Deserialize(string json)
        {
            try
            {
                // Handle empty JSON string as an empty path.
                if (string.IsNullOrEmpty(json))
                {
                    InitializeEmptyPath();
                    IsValid = true;
                }
                else
                {
                    // Configure settings for JSON deserialization.
                    var settings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Vector2_Converter() }, // Custom converter for Vector2
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Prevent circular references
                    };

                    // Populate this object with data from the JSON string.
                    JsonConvert.PopulateObject(json, this, settings);
                    IsValid = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to deserialize path data: {e.Message}");
                IsValid = false;
            }
        }

        /// <summary>
        /// Initializes an empty path with default values for all fields.
        /// </summary>
        private void InitializeEmptyPath()
        {
            FrontAxle = new List<Vector2>();
            Pivot = new List<Vector2>();
            RearAxle = new List<Vector2>();
            TrailerAxle = new List<Vector2>();
            Psi = new List<Vector2>();
            StartPose = new List<float>();
            EndPose = new List<float>();
            Time = new List<float>();
            Velocities = new List<float>();
            SteeringAngles = new List<float>();
            SimInput = new SimulationInput();
        }

        /// <summary>
        /// Represents the input data used for simulation, including steering and velocity inputs.
        /// </summary>
        [System.Serializable]
        public class SimulationInput
        {
            /// <summary>
            /// Matrix of steering angle inputs over time.
            /// </summary>
            public float[][] SteerInput;

            /// <summary>
            /// Matrix of velocity inputs over time.
            /// </summary>
            public float[][] VxInput;

            /// <summary>
            /// Constant velocity value for the simulation.
            /// </summary>
            public float Vx;

            /// <summary>
            /// Maximum time duration for the simulation.
            /// </summary>
            public float MaxTime;
        }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ApplicationScripts.Manager.PathManager
{
    /// <summary>
    /// Represents a referencePath for VeVehicle simulation, containing axle positions, steering data, 
    /// and other simulation-related properties.
    /// </summary>
    [System.Serializable]
    public class Reference_Path
    {
        /// <summary>
        /// Name of the referencePath, used for identification.
        /// </summary>
        public string PathName;

        /// <summary>
        /// Unique identifier for the referencePath.
        /// </summary>
        public int PathId;

        /// <summary>
        /// List of positions for the front axle throughout the referencePath.
        /// </summary>
        public List<Vector2> FrontAxle;

        /// <summary>
        /// List of positions for the pivot point throughout the referencePath.
        /// </summary>
        public List<Vector2> Pivot;

        /// <summary>
        /// List of positions for the rear axle throughout the referencePath.
        /// </summary>
        public List<Vector2> RearAxle;

        /// <summary>
        /// List of positions for the trailer axle throughout the referencePath.
        /// </summary>
        public List<Vector2> TrailerAxle;

        /// <summary>
        /// List of yaw angles (orientation) throughout the referencePath.
        /// </summary>
        public List<Vector2> Psi;

        /// <summary>
        /// The starting pose of the vehicle at the beginning of the referencePath.
        /// </summary>
        public List<float> StartPose;

        /// <summary>
        /// The ending pose of the vehicle at the end of the referencePath.
        /// </summary>
        public List<float> EndPose;

        /// <summary>
        /// List of time steps corresponding to the simulation referencePath.
        /// </summary>
        public List<float> Time;


        /// <summary>
        /// Simulation input data, including steering and velocity inputs.
        /// </summary>
        [JsonProperty("siminput")]
        public SimulationInput SimInput;

        /// <summary>
        /// Indicates whether the referencePath data is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference_Path"/> class.
        /// </summary>
        /// <param name="json">The JSON string representing the referencePath data.</param>
        /// <param name="id">The unique ID of the referencePath.</param>
        /// <param name="name">The name of the referencePath.</param>
        public Reference_Path(string json, int id, string name)
        {
            PathId = id;
            PathName = name;
            Deserialize(json);
        }

        /// <summary>
        /// Deserializes the JSON string into the referencePath object, populating its fields.
        /// </summary>
        /// <param name="json">The JSON string representing the referencePath data.</param>
        private void Deserialize(string json)
        {
            try
            {
                // Handle empty JSON string as an empty referencePath.
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
                Debug.LogError($"Failed to deserialize referencePath data: {e.Message}");
                IsValid = false;
            }
        }

        /// <summary>
        /// Initializes an empty referencePath with default values for all fields.
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
            [JsonProperty("steer_input")]
            public float[][] SteerInput;

            /// <summary>
            /// Matrix of velocity inputs over time.
            /// </summary>
            [JsonProperty("Vx_input")]
            public float[][] VxInput;

            /// <summary>
            /// Constant velocity value for the simulation.
            /// </summary>
            [JsonProperty("Vx")]
            public float Vx;

            /// <summary>
            /// Maximum time duration for the simulation.
            /// </summary>
            [JsonProperty("tmax")]
            public float MaxTime;
        }
    }
}

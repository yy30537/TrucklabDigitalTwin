using System.Collections.Generic;
using Application_Scripts.Virtual_Entity.Vehicle;
using Newtonsoft.Json;
using UnityEngine;

namespace Application_Scripts.Manager.Path_Manager
{
    /// <summary>
    /// Manages paths for VeVehicle simulation, including loading from JSON files, 
    /// recording new paths, and saving them to disk.
    /// </summary>
    public class Path_Manager : MonoBehaviour
    {
        /// <summary>
        /// List of JSON files containing predefined paths.
        /// Assignable in the Unity Inspector.
        /// </summary>
        public List<TextAsset> JsonFiles;

        /// <summary>
        /// List of deserialized paths available for simulation.
        /// </summary>
        public List<Path> Paths;

        // Internal variables for managing the path recording process
        private Path recordingPath;
        private bool isRecording = false;
        private float recordStartTime;
        private VE_Vehicle currentVeVehicle;
        private Vehicle_Data vehicleData;

        /// <summary>
        /// Initializes the Path_Manager by deserializing paths from JSON files.
        /// </summary>
        void Awake()
        {
            Paths = new List<Path>();
            int id = 0; // Unique identifier for each path

            // Loop through all assigned JSON files and deserialize their content
            foreach (var jsonFile in JsonFiles)
            {
                if (jsonFile != null)
                {
                    string jsonContent = jsonFile.text; // Read the JSON content
                    string pathName = jsonFile.name;   // Use file name as the path name
                    Path path = new Path(jsonContent, id++, pathName);

                    if (path.IsValid)
                    {
                        Paths.Add(path); // Add valid paths to the list
                    }
                    else
                    {
                        Debug.LogError($"Failed to initialize path from JSON file: {jsonFile.name}");
                    }
                }
                else
                {
                    Debug.LogError("JSON file is null");
                }
            }
        }

        /// <summary>
        /// Starts recording a new path for the specified VeVehicle.
        /// </summary>
        /// <param name="veVehicle">The VeVehicle for which to record the path.</param>
        public void StartPathRecording(VE_Vehicle veVehicle)
        {
            currentVeVehicle = veVehicle; // Set the vehicle being recorded
            recordingPath = new Path("", Paths.Count, "RecordedPath_" + Paths.Count); // Initialize a new path
            recordStartTime = Time.time; // Record the start time
            vehicleData = currentVeVehicle.Data; // Cache the vehicle's data reference
            isRecording = true; // Start recording
        }

        /// <summary>
        /// Stops recording the current path and saves it to disk as a JSON file.
        /// </summary>
        public void StopPathRecording()
        {
            if (currentVeVehicle != null)
            {
                isRecording = false; // Stop the recording state

                // Serialize the recorded path to JSON
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new Vector2_Converter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(recordingPath, settings);

                // Save the JSON to disk (update path as needed)
                //File.WriteAllText($"", json);
                Paths.Add(recordingPath); // Add the path to the list of paths
                Debug.Log("Path recorded and saved.");
                currentVeVehicle = null; // Clear the reference to the vehicle
            }
        }

        /// <summary>
        /// Updates the recording process during the FixedUpdate loop.
        /// </summary>
        void FixedUpdate()
        {
            if (isRecording)
            {
                RecordPath(); // Continuously record path data
            }
        }

        /// <summary>
        /// Records the current state of the VeVehicle during the recording process.
        /// Captures axle positions, angles, velocity, and steering inputs.
        /// </summary>
        private void RecordPath()
        {
            if (currentVeVehicle != null)
            {
                // Capture the front axle position (X0, Y0)
                recordingPath.FrontAxle.Add(new Vector2(vehicleData.X0, vehicleData.Y0));

                // Capture the pivot position (X1, Y1)
                recordingPath.Pivot.Add(new Vector2(vehicleData.X1, vehicleData.Y1));

                // Capture the rear axle position (X1C, Y1C)
                recordingPath.RearAxle.Add(new Vector2(vehicleData.X1C, vehicleData.Y1C));

                // Capture the trailer axle position (X2, Y2)
                recordingPath.TrailerAxle.Add(new Vector2(vehicleData.X2, vehicleData.Y2));

                // Capture the yaw angles for the tractor (Psi1) and trailer (Psi2)
                recordingPath.Psi.Add(new Vector2(vehicleData.Psi1, vehicleData.Psi2));

                // Record the elapsed time since the start of recording
                recordingPath.Time.Add(Time.time - recordStartTime);

                // Record the current velocity and steering angle
                recordingPath.Velocities.Add(vehicleData.V1);
                recordingPath.SteeringAngles.Add(vehicleData.Delta);

                // Uncomment for debugging timestamps
                // Debug.Log($"{(Time.time - recordStartTime) * 1000}");
            }
        }
    }
}

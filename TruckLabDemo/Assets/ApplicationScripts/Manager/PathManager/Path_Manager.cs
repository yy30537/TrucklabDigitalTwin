using System;
using System.Collections.Generic;
using System.IO; // For file operations
using Newtonsoft.Json;
using UnityEngine;
using ApplicationScripts.VirtualEntity.Vehicle.Controllers;

using ApplicationScripts.VirtualEntity.Vehicle;


#if UNITY_EDITOR
using UnityEditor; // For AssetDatabase.Refresh() if needed in the future
#endif

namespace ApplicationScripts.Manager.PathManager
{
    /// <summary>
    /// Manages reference paths for VeVehicle simulation, including loading from JSON files, 
    /// recording new paths, and saving them to persistent storage.
    /// </summary>
    public class Path_Manager : MonoBehaviour
    {
        /// <summary>
        /// List of JSON files containing predefined reference paths.
        /// Assignable in the Unity Inspector.
        /// </summary>
        public List<TextAsset> JsonFiles;

        /// <summary>
        /// List of deserialized reference paths available for simulation.
        /// </summary>
        public List<Reference_Path> Paths;

        // Internal variables for managing the reference path recording process
        private Reference_Path recordingReferencePath;
        private bool isRecording = false;
        private float recordStartTime;
        private VE_Vehicle currentVeVehicle;
        private Vehicle_Data vehicleData;
        private Vehicle_Kinematics_Controller kinematicsController;

        // Lists to store simulation input data
        private List<float> steerTimesList;
        private List<float> steerValuesList;
        private List<float> vxTimesList;
        private List<float> vxValuesList;

        // Previous input values to detect changes
        private float previousSteerAngle;
        private float previousVx;

        /// <summary>
        /// The directory path where recorded reference paths are saved.
        /// </summary>
        private string saveDirectory;

        /// <summary>
        /// Initializes the Path_Manager by deserializing paths from JSON files and setting up the save directory.
        /// Also loads any existing recorded reference paths from persistent storage.
        /// </summary>
        void Awake()
        {
            Paths = new List<Reference_Path>();
            int id = 0; // Unique identifier for each referencePath

            // Load predefined reference paths from assigned JSON TextAssets
            foreach (var jsonFile in JsonFiles)
            {
                if (jsonFile != null)
                {
                    string jsonContent = jsonFile.text; // Read the JSON content
                    string pathName = jsonFile.name;     // Use file name as the referencePath name
                    Reference_Path referencePath = new Reference_Path(jsonContent, id++, pathName);

                    if (referencePath.IsValid)
                    {
                        Paths.Add(referencePath); // Add valid paths to the list
                        Debug.Log($"Loaded referencePath: {referencePath.PathName}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to initialize referencePath from JSON file: {jsonFile.name}");
                    }
                }
                else
                {
                    Debug.LogError("JSON file is null");
                }
            }

            // Set up the save directory in persistentDataPath
            saveDirectory = System.IO.Path.Combine(Application.persistentDataPath, "VehicleReferencePaths");
            if (!System.IO.Directory.Exists(saveDirectory))
            {
                System.IO.Directory.CreateDirectory(saveDirectory);
                Debug.Log($"Created directory for saving reference paths: {saveDirectory}");
            }

            // Load any existing recorded reference paths from persistentDataPath
            LoadRecordedReferencePaths();
        }

        /// <summary>
        /// Loads all recorded reference paths from the persistent storage directory.
        /// </summary>
        private void LoadRecordedReferencePaths()
        {
            string[] files = System.IO.Directory.GetFiles(saveDirectory, "*.json");
            foreach (string file in files)
            {
                try
                {
                    string jsonContent = System.IO.File.ReadAllText(file);
                    // Assuming the file name follows the format: Path_{PathId}_{PathName}_{Timestamp}.json
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    string[] parts = fileName.Split('_');
                    if (parts.Length >= 4)
                    {
                        // Extract PathId and reconstruct PathName
                        if (int.TryParse(parts[1], out int pathId))
                        {
                            // Reconstruct PathName by joining parts[2] to parts[n-2]
                            string pathName = string.Join("_", parts, 2, parts.Length - 3);
                            Reference_Path referencePath = new Reference_Path(jsonContent, pathId, pathName);

                            if (referencePath.IsValid)
                            {
                                Paths.Add(referencePath);
                                Debug.Log($"Loaded recorded referencePath: {referencePath.PathName} from {file}");
                            }
                            else
                            {
                                Debug.LogError($"Failed to initialize referencePath from JSON file: {file}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"Invalid PathId in file name: {fileName}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Unexpected file naming format for reference path file: {file}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading reference path from {file}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Starts recording a new reference path for the specified VeVehicle.
        /// Utilizes InvokeRepeating to record path data at 0.1-second intervals.
        /// </summary>
        /// <param name="veVehicle">The VeVehicle for which to record the reference path.</param>
        public void StartPathRecording(VE_Vehicle veVehicle)
        {
            if (isRecording)
            {
                Debug.LogWarning("Reference_Path recording is already in progress.");
                return;
            }

            if (veVehicle == null)
            {
                Debug.LogError("Cannot start recording. VeVehicle is null.");
                return;
            }

            currentVeVehicle = veVehicle; // Set the vehicle being recorded
            kinematicsController = currentVeVehicle.GetComponent<Vehicle_Kinematics_Controller>();
            if (kinematicsController == null)
            {
                Debug.LogError("Vehicle_Kinematics_Controller not found on the VeVehicle.");
                return;
            }

            // Initialize a new Reference_Path with empty JSON to start recording
            recordingReferencePath = new Reference_Path(string.Empty, Paths.Count, "RecordedPath_" + Paths.Count);
            recordStartTime = Time.time; // Record the start time
            vehicleData = currentVeVehicle.Data; // Cache the vehicle's data reference
            isRecording = true; // Start recording

            // Initialize simulation input lists
            steerTimesList = new List<float>();
            steerValuesList = new List<float>();
            vxTimesList = new List<float>();
            vxValuesList = new List<float>();

            // Initialize previous input values
            previousSteerAngle = kinematicsController.InputSteerAngle;
            previousVx = kinematicsController.InputVelocity;

            // Record the initial steering and velocity inputs at t=0
            float initialTime = 0f;
            steerTimesList.Add(initialTime);
            steerValuesList.Add(previousSteerAngle);
            vxTimesList.Add(initialTime);
            vxValuesList.Add(previousVx);

            // **Record StartPose**
            recordingReferencePath.StartPose = new List<float>
            {
                vehicleData.X1,
                vehicleData.Y1,
                vehicleData.Psi1,
                vehicleData.Psi2
            };
            Debug.Log($"Recorded StartPose at t={initialTime:F3}s: X1={vehicleData.X1}, Y1={vehicleData.Y1}, Psi1={vehicleData.Psi1}, Psi2={vehicleData.Psi2}");

            // Start invoking the RecordPath method every 0.1 seconds
            InvokeRepeating(nameof(RecordPath), 0.0f, 0.1f);
            Debug.Log("Reference_Path recording started.");
        }

        /// <summary>
        /// Stops recording the current reference path and saves it to disk as a JSON file in persistentDataPath.
        /// </summary>
        public void StopPathRecording()
        {
            if (!isRecording)
            {
                Debug.LogWarning("No referencePath is currently being recorded.");
                return;
            }

            if (currentVeVehicle == null || kinematicsController == null)
            {
                Debug.LogError("Cannot stop recording. VeVehicle or KinematicsController is null.");
                return;
            }

            isRecording = false; // Stop the recording state

            // Stop invoking the RecordPath method
            CancelInvoke(nameof(RecordPath));

            // **Record EndPose**
            recordingReferencePath.EndPose = new List<float>
            {
                vehicleData.X1,
                vehicleData.Y1,
                vehicleData.Psi1,
                vehicleData.Psi2
            };
            Debug.Log($"Recorded EndPose: X1={vehicleData.X1}, Y1={vehicleData.Y1}, Psi1={vehicleData.Psi1}, Psi2={vehicleData.Psi2}");

            // Populate simulation input with recorded data
            recordingReferencePath.SimInput = new Reference_Path.SimulationInput
            {
                SteerInput = new float[][]
                {
                    steerTimesList.ToArray(),
                    steerValuesList.ToArray()
                },
                VxInput = new float[][]
                {
                    vxTimesList.ToArray(),
                    vxValuesList.ToArray()
                },
                Vx = kinematicsController.InputVelocity, // Final velocity or default
                MaxTime = recordingReferencePath.Time.Count > 0 ? recordingReferencePath.Time[recordingReferencePath.Time.Count - 1] : 0f
            };

            // Serialize the recorded referencePath to JSON
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new Vector2_Converter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented // For readable JSON
            };
            string json = JsonConvert.SerializeObject(recordingReferencePath, settings);

            // **Save the JSON to persistentDataPath**
            string sanitizedPathName = PathNameSanitizer(recordingReferencePath.PathName);
            string fileName = $"Path_{recordingReferencePath.PathId}_{sanitizedPathName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json";
            string fullPath = System.IO.Path.Combine(saveDirectory, fileName);

            try
            {
                File.WriteAllText(fullPath, json);
                Debug.Log($"Reference_Path recorded and saved to {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save referencePath to {fullPath}: {e.Message}");
            }

            // Add to Paths list
            Paths.Add(recordingReferencePath);
            Debug.Log($"Reference_Path recorded: {recordingReferencePath.PathName} with ID {recordingReferencePath.PathId}");

            // Clear references
            currentVeVehicle = null;
            kinematicsController = null;
        }

        /// <summary>
        /// Records the current state of the VeVehicle during the recording process.
        /// Captures axle positions, angles, velocity, and steering inputs.
        /// Invoked every 0.1 seconds via InvokeRepeating.
        /// </summary>
        private void RecordPath()
        {
            if (currentVeVehicle != null && kinematicsController != null)
            {
                float currentTime = Time.time - recordStartTime;

                // Capture the front axle position (X0, Y0)
                recordingReferencePath.FrontAxle.Add(new Vector2(vehicleData.X0, vehicleData.Y0));

                // Capture the pivot position (X1, Y1)
                recordingReferencePath.Pivot.Add(new Vector2(vehicleData.X1, vehicleData.Y1));

                // Capture the rear axle position (X1C, Y1C)
                recordingReferencePath.RearAxle.Add(new Vector2(vehicleData.X1C, vehicleData.Y1C));

                // Capture the trailer axle position (X2, Y2)
                recordingReferencePath.TrailerAxle.Add(new Vector2(vehicleData.X2, vehicleData.Y2));

                // Capture the yaw angles for the tractor (Psi1) and trailer (Psi2)
                recordingReferencePath.Psi.Add(new Vector2(vehicleData.Psi1, vehicleData.Psi2));

                // Record the elapsed time since the start of recording
                recordingReferencePath.Time.Add(currentTime);

                // Handle input steering angle and velocity recording
                float currentSteerAngle = kinematicsController.InputSteerAngle;
                float currentVx = kinematicsController.InputVelocity;

                // Check for steering angle change
                if (!Mathf.Approximately(currentSteerAngle, previousSteerAngle))
                {
                    steerTimesList.Add(currentTime);
                    steerValuesList.Add(currentSteerAngle);
                    Debug.Log($"Recorded Steering Change at t={currentTime:F3}s: {currentSteerAngle}");
                    previousSteerAngle = currentSteerAngle;
                }

                // Check for velocity change
                if (!Mathf.Approximately(currentVx, previousVx))
                {
                    vxTimesList.Add(currentTime);
                    vxValuesList.Add(currentVx);
                    Debug.Log($"Recorded Vx Change at t={currentTime:F3}s: {currentVx}");
                    previousVx = currentVx;
                }
            }
            else
            {
                Debug.LogError("Cannot record referencePath. VeVehicle or KinematicsController is null.");
            }
        }

        /// <summary>
        /// Retrieves a specific referencePath by its ID.
        /// </summary>
        /// <param name="pathId">The unique ID of the referencePath.</param>
        /// <returns>The Reference_Path object if found; otherwise, null.</returns>
        public Reference_Path GetPathById(int pathId)
        {
            return Paths.Find(p => p.PathId == pathId);
        }

        /// <summary>
        /// Sanitizes the referencePath name to be used as a filename by removing or replacing invalid characters.
        /// </summary>
        /// <param name="pathName">The original referencePath name.</param>
        /// <returns>A sanitized referencePath name safe for use as a filename.</returns>
        private string PathNameSanitizer(string pathName)
        {
            // Use System.IO.Path to get invalid file name characters
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                pathName = pathName.Replace(c, '_');
            }
            return pathName;
        }


    }
}

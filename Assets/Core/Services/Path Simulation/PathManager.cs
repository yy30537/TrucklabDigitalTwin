using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Core
{
    public class PathManager : MonoBehaviour
    {
        public List<TextAsset> jsonFiles; // Assign JSON files via the Unity Inspector
        public List<Path> paths;

        private Path recordingPath;
        private bool isRecording = false;
        private float recordStartTime;
        private VehicleProduct recordingVehicle;
        
        void Awake()
        {
            paths = new List<Path>();
            int id = 0;

            foreach (var jsonFile in jsonFiles)
            {
                if (jsonFile != null)
                {
                    string jsonContent = jsonFile.text;
                    string pathName = jsonFile.name; // Use the file name as the path name
                    Path path = new Path(jsonContent, id++, pathName);
                    if (path.isValid)
                    {
                        paths.Add(path);
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

        public void StartRecording(VehicleProduct vehicle)
        {
            recordingVehicle = vehicle;
            recordingPath = new Path("", paths.Count, "RecordedPath_" + paths.Count);
            isRecording = true;
            recordStartTime = Time.time;
        }

        public void StopRecording()
        {
            if (recordingVehicle != null)
            {
                isRecording = false;
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new Vector2Converter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(recordingPath, settings);
                File.WriteAllText("D:/Yang/TruckLabDemo/Assets/Core/Virtual Entity Components/Vehicle/Reference Paths/RecordedPath_" + recordingPath.pathID + ".json", json);
                paths.Add(recordingPath);
                Debug.Log("Path recorded and saved.");
                recordingVehicle = null;
            }
        }

        void FixedUpdate()
        {
            if (isRecording)
            {
                RecordCurrentState();
            }
        }

        private void RecordCurrentState()
        {
            if (recordingVehicle != null)
            {
                VehicleData vehicleData = recordingVehicle.vehicleData;
                recordingPath.frontaxle.Add(new Vector2(vehicleData.x0, vehicleData.y0));
                recordingPath.pivot.Add(new Vector2(vehicleData.x1, vehicleData.y1));
                recordingPath.rearaxle.Add(new Vector2(vehicleData.x1C, vehicleData.y1C));
                recordingPath.traileraxle.Add(new Vector2(vehicleData.x2, vehicleData.y2));
                recordingPath.psi.Add(new Vector2(vehicleData.psi1, vehicleData.psi2));
                recordingPath.time.Add(Time.time - recordStartTime);
                recordingPath.velocities.Add(vehicleData.v1); // Record the current velocity
                recordingPath.steeringAngles.Add(vehicleData.delta); // Record the current steering angle
                //Debug.Log($"{(Time.time - recordStartTime) * 1000} ");
            }
        }
    }
}

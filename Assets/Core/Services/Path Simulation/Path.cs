using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Core
{
    [System.Serializable]
    public class Path
    {
        public string pathName;
        public int pathID;

        public List<Vector2> frontaxle;
        public List<Vector2> pivot;
        public List<Vector2> rearaxle;
        public List<Vector2> traileraxle;
        public List<Vector2> psi;
        public List<float> startPose;
        public List<float> endPose;
        public List<float> time;
        public List<float> velocities; // List for recording velocities
        public List<float> steeringAngles; // List for recording steering angles
        public SimulationInput simInput;

        public bool isValid { get; private set; }

        public Path(string json, int id, string name)
        {
            pathID = id;
            pathName = name;
            Deserialize(json);
        }

        private void Deserialize(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    InitializeEmptyPath();
                    isValid = true;
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new Vector2Converter() },
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    JsonConvert.PopulateObject(json, this, settings);
                    isValid = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to deserialize path data: {e.Message}");
                isValid = false;
            }
        }

        private void InitializeEmptyPath()
        {
            frontaxle = new List<Vector2>();
            pivot = new List<Vector2>();
            rearaxle = new List<Vector2>();
            traileraxle = new List<Vector2>();
            psi = new List<Vector2>();
            startPose = new List<float>();
            endPose = new List<float>();
            time = new List<float>();
            velocities = new List<float>();
            steeringAngles = new List<float>();
            simInput = new SimulationInput();
        }

        [System.Serializable]
        public class SimulationInput
        {
            public float[][] steer_input;
            public float[][] Vx_input;
            public float Vx;
            public float tmax;
        }
    }
}

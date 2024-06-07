using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpaceProduct : Product
    {
        public SpaceConfig spaceConfig;
        public Dictionary<int, VehicleProduct> vehiclesInside;
        public SpaceDashboard dashboard;
        public MeshRenderer meshRenderer;
        private VehicleFactory vehicleFactory;

        private void Awake()
        {
            vehiclesInside = new Dictionary<int, VehicleProduct>();
        }

        public void Init(SpaceConfig config, GameObject instance, Camera cam, GameObject uiObserverParent, Transform dashboardParent)
        {
            base.Init(config.spaceID, config.spaceName, instance, cam, uiObserverParent, dashboardParent);
            spaceConfig = config;
        }

        protected override void InitComponents(GameObject uiObserverParent, Transform dashboardParent)
        {
            vehicleFactory = FindObjectOfType<VehicleFactory>();
            ConfigureMeshComponents();
            InitializeSpaceUIObserver(uiObserverParent, dashboardParent);
        }

        private void ConfigureMeshComponents()
        {
            Vector3 centerPoint = CalculateCenterPoint(spaceConfig.spacePoints);
            productInstance.transform.position = centerPoint;

            Vector3[] offsetVertices = new Vector3[spaceConfig.spacePoints.Length];
            for (int i = 0; i < spaceConfig.spacePoints.Length; i++)
            {
                offsetVertices[i] = spaceConfig.spacePoints[i] - centerPoint;
            }

            var meshFilter = productInstance.AddComponent<MeshFilter>();
            meshRenderer = productInstance.AddComponent<MeshRenderer>();
            var meshCollider = productInstance.AddComponent<MeshCollider>();
            var rb = productInstance.AddComponent<Rigidbody>();

            Mesh spaceMesh = CreateMesh(offsetVertices);

            meshFilter.mesh = spaceMesh;
            meshRenderer.material = spaceConfig.spaceMaterial;
            meshCollider.sharedMesh = spaceMesh;
            meshCollider.convex = false;
            meshCollider.isTrigger = false;

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void Update()
        {
            foreach (var vehicle in vehicleFactory.productLookupTable.Values)
            {
                Vector3[] tractorBoundingBox = vehicle.vehicleData.GetTractorBoundingBox();
                Vector3[] trailerBoundingBox = vehicle.vehicleData.GetTrailerBoundingBox();

                if (IsTruckBoundingBoxInSpace(tractorBoundingBox, trailerBoundingBox, spaceConfig.spacePoints))
                {
                    if (!vehiclesInside.ContainsKey(vehicle.productID))
                    {
                        vehiclesInside.Add(vehicle.productID, vehicle);
                        systemLog.LogEvent(productName + ": " + vehicle.productName + " entered.");
                        NotifyUIObservers();
                    }
                }
                else
                {
                    if (vehiclesInside.ContainsKey(vehicle.productID))
                    {
                        vehiclesInside.Remove(vehicle.productID);
                        systemLog.LogEvent(productName + ": " + vehicle.productName + " exited.");
                        NotifyUIObservers();
                    }
                }
            }
        }

        private void InitializeSpaceUIObserver(GameObject uiObserverParent, Transform dashboardParent)
        {
            var uiObserverInstance = new GameObject("SpaceDashboard");
            uiObserverInstance.transform.SetParent(uiObserverParent.transform);
            var uiObserver = uiObserverInstance.AddComponent<SpaceDashboard>();
            uiObserver.Initialize(this, dashboardParent);
            RegisterObserver(uiObserver);
        }

        private void NotifyUIObservers()
        {
            dashboard?.UpdateDashboard();
        }

        public void RegisterObserver(SpaceDashboard observer)
        {
            dashboard = observer;
        }

        public void RemoveObserver(SpaceDashboard observer)
        {
            dashboard = null;
        }

        private Mesh CreateMesh(Vector3[] vertices)
        {
            Mesh mesh = new Mesh
            {
                vertices = vertices
            };

            int[] triangles = new int[(vertices.Length - 2) * 3];
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private Vector3 CalculateCenterPoint(Vector3[] vertices)
        {
            Vector3 center = Vector3.zero;
            foreach (Vector3 vertex in vertices)
            {
                center += vertex;
            }
            center /= vertices.Length;
            return center;
        }

        private bool IsTruckBoundingBoxInSpace(Vector3[] tractorBoundingBox, Vector3[] trailerBoundingBox, Vector3[] polygon)
        {
            foreach (var point in tractorBoundingBox)
            {
                if (IsPointInGoalArea(point, polygon))
                {
                    return true;
                }
            }

            foreach (var point in trailerBoundingBox)
            {
                if (IsPointInGoalArea(point, polygon))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPointInGoalArea(Vector3 point, Vector3[] polygon)
        {
            float a1 = Vector3.Distance(polygon[0], polygon[1]);
            float a2 = Vector3.Distance(polygon[1], polygon[2]);
            float a3 = Vector3.Distance(polygon[2], polygon[3]);
            float a4 = Vector3.Distance(polygon[3], polygon[0]);

            float b1 = Vector3.Distance(polygon[0], point);
            float b2 = Vector3.Distance(polygon[1], point);
            float b3 = Vector3.Distance(polygon[2], point);
            float b4 = Vector3.Distance(polygon[3], point);

            float A = Mathf.Round(a1 * a2);
            float u1 = (a1 + b1 + b2) / 2;
            float u2 = (a2 + b2 + b3) / 2;
            float u3 = (a3 + b3 + b4) / 2;
            float u4 = (a4 + b4 + b1) / 2;

            float A1 = Mathf.Sqrt(u1 * (u1 - a1) * (u1 - b1) * (u1 - b2));
            float A2 = Mathf.Sqrt(u2 * (u2 - a2) * (u2 - b2) * (u2 - b3));
            float A3 = Mathf.Sqrt(u3 * (u3 - a3) * (u3 - b3) * (u3 - b4));
            float A4 = Mathf.Sqrt(u4 * (u4 - a4) * (u4 - b4) * (u4 - b1));

            float area = Mathf.Round(A1 + A2 + A3 + A4);
            return area == A;
        }
    }
}

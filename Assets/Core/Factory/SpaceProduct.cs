using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SpaceProduct : Product
    {
        public SpaceConfig spaceConfig;
        public Dictionary<int, VehicleProduct> vehiclesInside;
        public SpaceDashboardObserver dashboardObserver;
        public MeshRenderer meshRenderer;
        public VehicleFactory vehicleFactory;
        
        private void Awake()
        {
            vehiclesInside = new Dictionary<int, VehicleProduct>();
        }
        
        public void Init(SpaceConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject, VehicleFactory vehicleFactory)
        {
            base.Init(config.spaceID, config.spaceName, instance, cam, systemLog, getClickedObject);
            this.vehicleFactory = vehicleFactory;
            spaceConfig = config;
            InitComponents();
        }

        public override void InitComponents()
        {
            // Calculate the center point and set the product instance's position
            Vector3 centerPoint = CalculateCenterPoint(spaceConfig.spacePoints);
            productInstance.transform.position = centerPoint;

            // Offset the vertices relative to the center point
            Vector3[] offsetVertices = new Vector3[spaceConfig.spacePoints.Length];
            for (int i = 0; i < spaceConfig.spacePoints.Length; i++)
            {
                offsetVertices[i] = spaceConfig.spacePoints[i] - centerPoint;
            }

            // Initialize the mesh components
            var meshFilter = productInstance.AddComponent<MeshFilter>();
            meshRenderer = productInstance.AddComponent<MeshRenderer>();
            var meshCollider = productInstance.AddComponent<MeshCollider>();
            var rb = productInstance.AddComponent<Rigidbody>();

            Mesh spaceMesh = CreateMesh(offsetVertices);

            meshFilter.mesh = spaceMesh;
            meshRenderer.material = spaceConfig.spaceMaterial;
            meshCollider.sharedMesh = spaceMesh;
            meshCollider.convex = false;  // Set convex to false to create a non-convex collider
            meshCollider.isTrigger = false;  // Set as trigger collider

            // Configure Rigidbody
            rb.isKinematic = true;  // Set isKinematic to true so it doesn't interfere with the physics simulation
            rb.useGravity = false;  // Disable gravity
        }

        private void Update()
        {
            foreach (var vehicle in vehicleFactory.productLookupTable)
            {
                Vector3[] tractorBoundingBox = vehicle.Value.vehicleData.GetTractorBoundingBox();
                Vector3[] trailerBoundingBox = vehicle.Value.vehicleData.GetTrailerBoundingBox();
                if (IsBoundingBoxInSpace(tractorBoundingBox, spaceConfig.spacePoints) || IsBoundingBoxInSpace(trailerBoundingBox, spaceConfig.spacePoints))
                {
                    if (!IsVehicleProductInSpace(vehicle.Value.productID))
                    {
                        vehiclesInside.Add(vehicle.Value.productID, vehicle.Value);
                        systemLog.LogEvent(productName + ": " + vehicle.Value.productName + " entered.");
                        NotifyUIObservers();
                    }
                }
                else
                {
                    if (IsVehicleProductInSpace(vehicle.Value.productID))
                    {
                        vehiclesInside.Remove(vehicle.Value.productID);
                        systemLog.LogEvent(productName + ": " + vehicle.Value.productName + " exited.");
                        NotifyUIObservers();
                    }
                }
            }
        }

        public bool IsVehicleProductInSpace(int id)
        {
            return vehiclesInside.ContainsKey(id);
        }

        private void NotifyUIObservers()
        {
            dashboardObserver.UpdateDashboard();
        }

        public void RegisterObserver(SpaceDashboardObserver observer)
        {
            dashboardObserver = observer;
        }

        public void RemoveObserver(SpaceDashboardObserver observer)
        {
            dashboardObserver = null;
        }

        private Mesh CreateMesh(Vector3[] vertices)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            // Create two triangles for the quad
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

        private bool IsBoundingBoxInSpace(Vector3[] boundingBox, Vector3[] polygon)
        {
            foreach (var point in boundingBox)
            {
                if (IsPointInPolygon(point, polygon))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointInPolygon(Vector3 point, Vector3[] polygon)
        {
            bool isInside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; j = i++)
            {
                if (((polygon[i].z > point.z) != (polygon[j].z > point.z)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}

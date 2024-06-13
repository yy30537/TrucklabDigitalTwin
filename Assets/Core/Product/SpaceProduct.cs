using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a space product in the simulation.
    /// </summary>
    public class SpaceProduct : Product
    {
        public SpaceConfig spaceConfig { get; private set; }
        public Dictionary<int, VehicleProduct> vehiclesInside { get; private set; }
        public SpaceDashboardObserver dashboardObserver { get; private set; }
        public MeshRenderer meshRenderer { get; private set; }
        public VehicleFactory vehicleFactory { get; private set; }

        private void Awake()
        {
            vehiclesInside = new Dictionary<int, VehicleProduct>();
        }

        /// <summary>
        /// Initializes the space product with the provided configuration and dependencies.
        /// </summary>
        /// <param name="config">Space configuration.</param>
        /// <param name="instance">GameObject instance of the space.</param>
        /// <param name="cam">Main camera.</param>
        /// <param name="systemLog">System log for logging events.</param>
        /// <param name="getClickedObject">Service for detecting clicked objects.</param>
        /// <param name="vehicleFactory">Vehicle factory instance.</param>
        public void Init(SpaceConfig config, GameObject instance, Camera cam, SystemLog systemLog, GetClickedObject getClickedObject, VehicleFactory vehicleFactory)
        {
            base.Init(config.spaceID, config.spaceName, instance, cam, systemLog, getClickedObject);
            this.vehicleFactory = vehicleFactory;
            spaceConfig = config;
            InitComponents();
        }

        /// <summary>
        /// Initializes the components of the space product.
        /// </summary>
        public override void InitComponents()
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

        /// <summary>
        /// Checks if a vehicle product is inside the space.
        /// </summary>
        /// <param name="id">Vehicle product ID.</param>
        /// <returns>True if the vehicle product is inside the space, false otherwise.</returns>
        public bool IsVehicleProductInSpace(int id)
        {
            return vehiclesInside.ContainsKey(id);
        }

        /// <summary>
        /// Notifies UI observers of changes.
        /// </summary>
        private void NotifyUIObservers()
        {
            dashboardObserver.UpdateDashboard();
        }

        /// <summary>
        /// Registers an observer for the space product.
        /// </summary>
        /// <param name="observer">Observer to register.</param>
        public void RegisterObserver(SpaceDashboardObserver observer)
        {
            dashboardObserver = observer;
        }

        /// <summary>
        /// Removes an observer from the space product.
        /// </summary>
        /// <param name="observer">Observer to remove.</param>
        public void RemoveObserver(SpaceDashboardObserver observer)
        {
            dashboardObserver = null;
        }

        /// <summary>
        /// Creates a mesh from the provided vertices.
        /// </summary>
        /// <param name="vertices">Array of vertices.</param>
        /// <returns>Generated mesh.</returns>
        private Mesh CreateMesh(Vector3[] vertices)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

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

        /// <summary>
        /// Calculates the center point of the given vertices.
        /// </summary>
        /// <param name="vertices">Array of vertices.</param>
        /// <returns>Center point of the vertices.</returns>
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

        /// <summary>
        /// Checks if a bounding box is within the space defined by a polygon.
        /// </summary>
        /// <param name="boundingBox">Array of bounding box vertices.</param>
        /// <param name="polygon">Array of polygon vertices.</param>
        /// <returns>True if bounding box is within the polygon, false otherwise.</returns>
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

        /// <summary>
        /// Checks if a point is within a polygon.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="polygon">Array of polygon vertices.</param>
        /// <returns>True if point is within the polygon, false otherwise.</returns>
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

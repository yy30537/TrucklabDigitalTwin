using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a region product in the simulation.
    /// Manages the initialization, configuration, and vehicle tracking within the region.
    /// </summary>
    public class RegionProduct : BaseProduct
    {
        /// <summary>
        /// Gets or sets the configuration for the region.
        /// </summary>
        public RegionConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the vehicle factory associated with the region.
        /// </summary>
        public VehicleFactory VehicleFactory { get; set; }

        /// <summary>
        /// Gets the dictionary of vehicles currently in the region.
        /// </summary>
        public Dictionary<int, VehicleProduct> VehiclesInRegion { get; private set; }

        /// <summary>
        /// Gets the mesh renderer for the region.
        /// </summary>
        public MeshRenderer MeshRenderer { get; private set; }

        /// <summary>
        /// Gets the UI component for the region.
        /// </summary>
        public RegionUI Ui { get; private set; }

        /// <summary>
        /// Gets the observer component for the region.
        /// </summary>
        public RegionObserver Observer { get; private set; }

        /// <summary>
        /// Gets or sets the x-coordinate of the region's position.
        /// </summary>
        public float x { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the region's position.
        /// </summary>
        public float y { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle psi1 in radians.
        /// </summary>
        public float psi1Rad { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle psi2 in radians.
        /// </summary>
        public float psi2Rad { get; set; }

        /// <summary>
        /// Initializes the region product.
        /// </summary>
        private void Awake()
        {
            VehiclesInRegion = new Dictionary<int, VehicleProduct>();
        }

        /// <summary>
        /// Initializes the region product.
        /// </summary>
        public override void Initialize()
        {
            VehicleFactory = FindObjectOfType<VehicleFactory>();
            InitializeRegionMesh();
            base.Initialize();
            Observer.StartObserving();

            x = Config.x;
            y = Config.y;
            psi1Rad = Config.psi1Rad;
            psi2Rad = Config.psi2Rad;
            //SystemLogWindow.LogEvent($"Initialized Region: {ProductName}");
        }

        /// <summary>
        /// Initializes the region's mesh based on its configuration.
        /// </summary>
        private void InitializeRegionMesh()
        {
            Vector3 centerPoint = CalculateCenterPoint(Config.RegionPoints);
            ProductInstance.transform.position = centerPoint;

            Vector3[] offsetVertices = new Vector3[Config.RegionPoints.Length];
            for (int i = 0; i < Config.RegionPoints.Length; i++)
            {
                offsetVertices[i] = Config.RegionPoints[i] - centerPoint;
            }

            var meshFilter = ProductInstance.AddComponent<MeshFilter>();
            MeshRenderer = ProductInstance.AddComponent<MeshRenderer>();
            var meshCollider = ProductInstance.AddComponent<MeshCollider>();
            var rb = ProductInstance.AddComponent<Rigidbody>();

            Mesh regionMesh = CreateMesh(offsetVertices);

            meshFilter.mesh = regionMesh;
            MeshRenderer.material = Config.RegionMaterial;
            meshCollider.sharedMesh = regionMesh;
            meshCollider.convex = false;
            meshCollider.isTrigger = false;

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        /// <summary>
        /// Initializes the UI components for the region.
        /// </summary>
        protected override void InitializeUI()
        {
            var uiInstance = Instantiate(Config.RegionUiPrefab, UiParent);
            Ui = uiInstance.GetComponent<RegionUI>();
            uiInstance.name = Config.RegionName;
            if (Ui == null)
            {
                Ui = uiInstance.AddComponent<RegionUI>();
            }
            Ui.Initialize(UiParent);
            Ui.SetRegionData(this);
            //SystemLogWindow.LogEvent($"Initialized Region UI for: {ProductName}");
        }

        /// <summary>
        /// Initializes the observer component for the region.
        /// </summary>
        protected override void InitializeObserver()
        {
            Observer = gameObject.AddComponent<RegionObserver>();
            Observer.Initialize(this, Ui);
        }

        /// <summary>
        /// Updates the state of the region.
        /// </summary>
        private void Update()
        {
            UpdateVehiclesInRegion();
        }

        /// <summary>
        /// Updates the list of vehicles currently in the region.
        /// </summary>
        private void UpdateVehiclesInRegion()
        {
            if (VehicleFactory.ProductLookupTable.Count > 0)
            {
                foreach (var vehicle in VehicleFactory.ProductLookupTable)
                {
                    Vector3[] tractorBoundingBox = vehicle.Value.Data.GetTractorBoundingBox();
                    Vector3[] trailerBoundingBox = vehicle.Value.Data.GetTrailerBoundingBox();
                    bool isInRegion = IsBoundingBoxInRegion(tractorBoundingBox, Config.RegionPoints) ||
                                      IsBoundingBoxInRegion(trailerBoundingBox, Config.RegionPoints);

                    if (isInRegion && !IsVehicleProductInRegion(vehicle.Value.ProductID))
                    {
                        VehiclesInRegion.Add(vehicle.Value.ProductID, vehicle.Value);
                    }
                    else if (!isInRegion && IsVehicleProductInRegion(vehicle.Value.ProductID))
                    {
                        VehiclesInRegion.Remove(vehicle.Value.ProductID);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a vehicle is currently in the region.
        /// </summary>
        /// <param name="id">The ID of the vehicle to check.</param>
        /// <returns>True if the vehicle is in the region, false otherwise.</returns>
        public bool IsVehicleProductInRegion(int id)
        {
            return VehiclesInRegion.ContainsKey(id);
        }

        /// <summary>
        /// Performs cleanup when the region product is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            //if (Observer != null)
            //{
            //    Observer.StopObserving();
            //    Destroy(Observer);
            //}
            //if (Ui != null)
            //{
            //    Destroy(Ui.gameObject);
            //}
            //SystemLogWindow.LogEvent($"Destroyed Region: {ProductName}");
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
        private bool IsBoundingBoxInRegion(Vector3[] boundingBox, Vector3[] polygon)
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

using ApplicationScripts.Manager;
using ApplicationScripts.UIController.ApplicationUI;
using ApplicationScripts.VirtualEntity.Space.Controllers;
using ApplicationScripts.VirtualEntity.Vehicle;
using TMPro;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Space
{
    public class VE_Space : VE
    {
        /// <summary>
        /// Configuration for the space entity, containing initialization settings.
        /// </summary>
        [Header("Space Data")]
        public Space_Config Config;

        /// <summary>
        /// Runtime data associated with the space, managing its state and contents.
        /// </summary>
        public Space_Data SpaceData;

        /// <summary>
        /// Mesh renderer for the space visualization.
        /// </summary>
        public MeshRenderer SpaceMesh;

        /// <summary>
        /// Space controller managing the space's behavior and interactions.
        /// </summary>
        [Header("Space Controllers")]
        public Space_Controller SpaceController;

        /// <summary>
        /// UI dashboard controller for the space.
        /// </summary>
        public Space_Dashboard_Controller SpaceDashboardController;

        /// <summary>
        /// Label GameObject used to display the space name visually.
        /// </summary>
        [Header("Space Label")]
        private GameObject spaceLabel;

        /// <summary>
        /// Camera manager for handling camera-related operations in the space.
        /// </summary>
        [Header("Application Dependencies")]
        public Camera_Manager CameraManager;

        /// <summary>
        /// Controller for logging system events related to the space.
        /// </summary>
        public UI_Controller_SystemLog SystemLogUiController;

        /// <summary>
        /// Utility for detecting objects via mouse clicks.
        /// </summary>
        public VE_OnClick_Getter VeOnClickGetter;

        /// <summary>
        /// Reference to the vehicle creator responsible for spawning vehicles in the space.
        /// </summary>
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Parent transform under which space-related GameObjects will be instantiated.
        /// </summary>
        public Transform VeUiInstanceParentTransform;

        /// <summary>
        /// Parent transform for the space instance in the scene.
        /// </summary>
        public Transform VeInstanceParentTransform;


        /// <summary>
        /// Updates the material of the space's mesh based on the presence of vehicles.
        /// </summary>
        private void Update()
        {
            if (SpaceMesh)
            {
                // Check if there are vehicles present in the space
                if (SpaceData.VehiclesPresentInSpace != null && SpaceData.VehiclesPresentInSpace.Count > 0)
                {
                    SpaceMesh.material = Config.GroundMaterialActive;
                }
                else
                {
                    SpaceMesh.material = Config.GroundMaterial;
                }
            }
        }


        /// <summary>
        /// Sets the dependencies required by the space.
        /// </summary>
        /// <param name="ve_instance">The instance of the space GameObject.</param>
        /// <param name="space_config">The configuration for the space.</param>
        /// <param name="vehicle_creator">Reference to the vehicle creator.</param>
        /// <param name="ve_instance_parent_transform">Parent transform for the space instance.</param>
        /// <param name="ve_ui_parent_transform">Parent transform for the UI elements.</param>
        /// <param name="camera_manager">The camera manager for camera-related operations.</param>
        /// <param name="systemLog_ui_controller">System log UI controller for event logging.</param>
        /// <param name="ve_onclick_getter">Utility for detecting mouse click interactions.</param>
        public void SetDependencies(
            GameObject ve_instance,
            Space_Config space_config,
            Vehicle_Creator vehicle_creator,
            Transform ve_instance_parent_transform,
            Transform ve_ui_parent_transform,
            Camera_Manager camera_manager,
            UI_Controller_SystemLog systemLog_ui_controller,
            VE_OnClick_Getter ve_onclick_getter)
        {
            Config = space_config;
            Instance = ve_instance;
            CameraManager = camera_manager;
            VehicleCreator = vehicle_creator;
            SystemLogUiController = systemLog_ui_controller;
            VeOnClickGetter = ve_onclick_getter;
            VeInstanceParentTransform = ve_instance_parent_transform;
            VeUiInstanceParentTransform = ve_ui_parent_transform;
        }


        /// <summary>
        /// Initializes the space and its components.
        /// </summary>
        public override void Init()
        {
            InitializeCommonProperties();
            InitializeSpaceData();
            InitializeControllers();
            InitializeDashboard();
            CreateMesh();
            CreateSpaceLabel();

            SystemLogUiController.LogEvent($"Initialized Space: {Name}");
        }

        /// <summary>
        /// Initializes common properties like name and parent transforms.
        /// </summary>
        private void InitializeCommonProperties()
        {
            Id = Config.Id;
            Name = Config.Name;
            Instance.transform.SetParent(VeInstanceParentTransform);
            Instance.name = Name;
            Instance.tag = "Space";
        }

        /// <summary>
        /// Initializes the space data and links it to the configuration.
        /// </summary>
        private void InitializeSpaceData()
        {
            SpaceData = Instance.AddComponent<Space_Data>();
            SpaceData.Init(Config);
        }

        /// <summary>
        /// Initializes the space controllers.
        /// </summary>
        private void InitializeControllers()
        {
            SpaceController = Instance.AddComponent<Space_Controller>();
            SpaceController.Init(SpaceData, VehicleCreator);
        }

        /// <summary>
        /// Initializes the UI dashboard for the space.
        /// </summary>
        private void InitializeDashboard()
        {
            var ui_instance = Instantiate(Config.UiTemplate, VeUiInstanceParentTransform);
            SpaceDashboardController = ui_instance.AddComponent<Space_Dashboard_Controller>();
            SpaceDashboardController.SetDependencies(
                this,
                Config,
                ui_instance,
                VeUiInstanceParentTransform,
                SystemLogUiController
            );
            SpaceDashboardController.Init();
        }

        /// <summary>
        /// Creates A label above the space to display its name.
        /// </summary>
        private void CreateSpaceLabel()
        {
            // Create A quad as the label background
            spaceLabel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            spaceLabel.name = $"{Name}_Label";
            spaceLabel.transform.SetParent(Instance.transform);

            // Position and rotation of the label
            float labelHeight = 0.1f; // Height offset above the mesh
            spaceLabel.transform.position = new Vector3(Config.LabelX, labelHeight, Config.LabelY);
            spaceLabel.transform.rotation = Quaternion.Euler(90f, Config.LabelPsi, 0);

            // Set scale and add text
            spaceLabel.transform.localScale = new Vector3(2f, 2f, 1f);
            TextMeshPro textMesh = spaceLabel.AddComponent<TextMeshPro>();
            textMesh.text = Name;
            textMesh.fontSize = 4;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Disable collisions for the label
            DestroyImmediate(spaceLabel.GetComponent<MeshCollider>());
        }

        /// <summary>
        /// Creates the mesh for the space using its configured space markings.
        /// </summary>
        private void CreateMesh()
        {
            // Calculate the center point of the space markings to position the instance.
            Vector3 centerPoint = CalculateCenterPoint(Config.SpaceMarkings);
            Instance.transform.position = centerPoint;

            // Adjust vertices relative to the center point.
            Vector3[] offsetVertices = new Vector3[Config.SpaceMarkings.Length];
            for (int i = 0; i < Config.SpaceMarkings.Length; i++)
            {
                offsetVertices[i] = Config.SpaceMarkings[i] - centerPoint;
            }

            // Add MeshFilter and MeshRenderer components to the space instance.
            var meshFilter = Instance.AddComponent<MeshFilter>();
            SpaceMesh = Instance.AddComponent<MeshRenderer>();

            // Generate and assign the mesh using the offset vertices.
            Mesh spaceMesh = CreateMesh(offsetVertices);
            meshFilter.mesh = spaceMesh;

            // Set the default material for the space mesh.
            if (Application.isPlaying)
            {
                SpaceMesh.material = Config.GroundMaterial; // Use material in Play Mode
            }
            else
            {
                SpaceMesh.sharedMaterial = Config.GroundMaterial; // Use sharedMaterial in Edit Mode
            }

        }

        /// <summary>
        /// Generates A mesh from the given vertices.
        /// </summary>
        /// <param name="vertices">The vertices defining the mesh.</param>
        /// <returns>A new mesh generated from the vertices.</returns>
        private Mesh CreateMesh(Vector3[] vertices)
        {
            // Create A new mesh and assign its vertices.
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            // Define triangles for the mesh based on vertex indices.
            int[] triangles = new int[(vertices.Length - 2) * 3];
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            mesh.triangles = triangles;

            // Recalculate normals and bounds for accurate rendering.
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }

        /// <summary>
        /// Calculates the geometric center point of A set of vertices.
        /// </summary>
        /// <param name="vertices">The vertices defining the space markings.</param>
        /// <returns>The calculated center point of the vertices.</returns>
        private Vector3 CalculateCenterPoint(Vector3[] vertices)
        {
            Vector3 center = Vector3.zero;

            // Sum all vertices to find the average position.
            foreach (Vector3 vertex in vertices)
            {
                center += vertex;
            }

            // Divide by the number of vertices to compute the center.
            center /= vertices.Length;
            return center;
        }

    }
}

using System;
using System.Collections.Generic;
using Application_Scripts.Manager;
using Application_Scripts.UI_Controller.Application_UI;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity
{
    /// <summary>
    /// Abstract base class for creating and managing Virtual Entities (VE).
    /// This class provides common methods and properties for creation, deletion, and registration.
    /// </summary>
    /// <typeparam name="VE_Type">The type of VE that this creator manages.</typeparam>
    /// <typeparam name="VE_Config_Type">The configuration type used to initialize the VE.</typeparam>

    public abstract class VE_Creator<VE_Type, VE_Config_Type> : MonoBehaviour
        where VE_Type : VE
        where VE_Config_Type : ScriptableObject
    {
        /// <summary>
        /// Transform to which newly instantiated VE instances will be parented in the scene.
        /// </summary>
        [Header("Main Scene VE Instance Parent")]
        [SerializeField] protected Transform VeInstanceParentTransform;

        /// <summary>
        /// Transform to which VE-related UI elements will be parented in the scene.
        /// </summary>
        [SerializeField] protected Transform VeUiParentTransform;

        /// <summary>
        /// Camera manager responsible for camera-related operations.
        /// </summary>
        [Header("Application Dependencies")]
        [SerializeField] protected Camera_Manager CameraManager;

        /// <summary>
        /// Reference to the system log UI controller for logging system events.
        /// </summary>
        [SerializeField] protected UI_Controller_SystemLog SystemLogUiController;

        /// <summary>
        /// Lookup table storing all VE instances created by this factory, indexed by their unique Id.
        /// </summary>
        public Dictionary<int, VE_Type> LookupTable { get; } = new Dictionary<int, VE_Type>();

        /// <summary>
        /// Abstract method to create and return A new VE based on the provided configuration.
        /// Implementing classes must define the logic for instantiating and initializing the VE.
        /// </summary>
        /// <param name="ve_config">The configuration used to initialize the VE.</param>
        /// <returns>The created VE instance.</returns>
        public abstract VE_Type Create_VE(VE_Config_Type ve_config);

        /// <summary>
        /// Abstract method to delete A VE instance from the factory and handle its removal from the lookup table.
        /// Implementing classes must define the logic for cleanup and event handling.
        /// </summary>
        /// <param name="ve_id">The unique Id of the VE to delete.</param>
        public abstract void Delete_VE(int ve_id);

        /// <summary>
        /// Registers A VE instance in the lookup table and logs the registration event.
        /// </summary>
        /// <param name="ve">The VE instance to register.</param>
        /// <param name="ve_config">The configuration associated with the VE instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if the VE instance is null.</exception>
        protected virtual void Register_VE(VE_Type ve, VE_Config_Type ve_config)
        {
            if (ve == null) throw new ArgumentNullException(nameof(ve));

            LookupTable[ve.Id] = ve;
            SystemLogUiController.LogEvent($"Registered {typeof(VE_Type).Name}: {ve.Name}");
        }

        /// <summary>
        /// Retrieves A VE instance from the lookup table by its unique Id.
        /// </summary>
        /// <param name="ve_id">The unique Id of the VE to retrieve.</param>
        /// <returns>The VE instance if found; otherwise, null.</returns>
        public VE_Type Lookup_VE(int ve_id)
        {
            LookupTable.TryGetValue(ve_id, out var ve);
            return ve;
        }
    }
}
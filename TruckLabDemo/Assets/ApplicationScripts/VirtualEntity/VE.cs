using UnityEngine;

namespace ApplicationScripts.VirtualEntity
{
    /// <summary>
    /// Abstract base class representing A Virtual Entity (VE) in the simulation environment.
    /// Provides common properties and initialization logic for derived entities.
    /// </summary>
    public abstract class VE : MonoBehaviour
    {
        /// <summary>
        /// Unique identifier for the virtual entity.
        /// </summary>
        public int Id;

        /// <summary>
        /// Display name or description of the virtual entity.
        /// </summary>
        public string Name;

        /// <summary>
        /// Reference to the GameObject instance representing this entity in the scene.
        /// </summary>
        public GameObject Instance;

        /// <summary>
        /// Abstract method to initialize the virtual entity.
        /// Must be implemented by derived classes to define entity-specific initialization behavior.
        /// </summary>
        public abstract void Init();
    }
}
﻿using System.Collections.Generic;
using UnityEngine;

namespace VehiclePhysics.Scripts.Vehicle.Misc
{
    /// <summary>
    /// Allows character to enter or exit vehicleTransform. Can be used with any first or 3rd person object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VehicleChanger))]
    public class CharacterVehicleChanger : MonoBehaviour
    {
        /// <summary>
        /// Maximum distance at which the character will be able to enter the vehicleTransform.
        /// </summary>
        [Tooltip("Maximum distance at which the character will be able to enter the vehicleTransform.")]
        [Range(0.2f, 3f)]
        public float enterDistance = 2f;

        /// <summary>
        /// Maximum speed at which the character will be able to enter / exit the vehicleTransform.
        /// </summary>
        [Tooltip("Maximum speed at which the character will be able to enter / exit the vehicleTransform.")]
        public float maxEnterExitVehicleSpeed = 2f;

        /// <summary>
        /// Tag of the object representing the point from which the enter distance will be measured. Useful if you want to enable you character to enter only when near the door.
        /// </summary>
        [Tooltip("Tag of the object representing the point from which the enter distance will be measured. Useful if you want to enable you character to enter only when near the door.")]
        public string enterExitTag = "EnterExitPoint";

        /// <summary>
        /// Game object representing A character. Can also be another vehicleTransform.
        /// </summary>
        [Tooltip("Game object representing A character. Can also be another vehicleTransform.")]
        public GameObject characterControllerObject;

        /// <summary>
        /// True when character can enter the vehicleTransform.
        /// </summary>
        [HideInInspector]
        public bool nearVehicle = false;

        private bool insideVehicle = false;
        private VehicleChanger vehicleChanger;
        private VehicleController nearestVehicle;
        private GameObject nearestEnterExitObject;
        private Vector3 relativeEnterPosition;
        private bool showWarning = true;

        void Awake()
        {
            if (this.enabled)
            {
                vehicleChanger = GetComponent<VehicleChanger>();
                vehicleChanger.CharacterBased = true;
                vehicleChanger.DeactivateAll = true;
            }
        }

        void Update()
        {
            if (!insideVehicle)
            {
                nearVehicle = false;

                if (!characterControllerObject.activeSelf)
                    characterControllerObject.SetActive(true);

                if (!vehicleChanger.DeactivateAll)
                    vehicleChanger.DeactivateAll = true;

                nearestVehicle = vehicleChanger.NearestVehicleFrom(characterControllerObject);

                // Only check if vehicleTransform close enough
                if (Vector3.Distance(characterControllerObject.transform.position, nearestVehicle.transform.position) < 10f)
                {
                    List<GameObject> enterExitObjects = new List<GameObject>();

                    // Find enter exit points
                    foreach (Transform child in nearestVehicle.transform)
                    {
                        if (child.CompareTag(enterExitTag))
                        {
                            enterExitObjects.Add(child.gameObject);
                        }
                    }

                    // Enter exit objects exist
                    if (enterExitObjects.Count > 0)
                    {
                        // Only one point, return it
                        if (enterExitObjects.Count == 1)
                        {
                            nearestEnterExitObject = enterExitObjects[0];
                        }
                        else
                        {
                            float minDistance = Mathf.Infinity;

                            for (int i = 0; i < enterExitObjects.Count; i++)
                            {
                                float distance = Vector3.Distance(enterExitObjects[i].transform.position, characterControllerObject.transform.position);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    nearestEnterExitObject = enterExitObjects[i];
                                }
                            }
                        }

                        if (Vector3.Distance(nearestEnterExitObject.transform.position, characterControllerObject.transform.position) < enterDistance)
                        {
                            nearVehicle = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("IVirtualEntityProduct does not have enter/exit points so it can not be entered.");
                    }
                }
            }

            bool changeVehicle = false;
            try
            {
                changeVehicle = UnityEngine.Input.GetButtonDown("ChangeVehicle");
            }
            catch
            {
                if (showWarning)
                {
                    Debug.LogWarning("ChangeVehicle button not set up inside input manager. Falling back to default.");
                    showWarning = false;
                }

                changeVehicle = UnityEngine.Input.GetKeyDown(KeyCode.V);
            }

            // Enter IVirtualEntityProduct
            if (nearVehicle && changeVehicle && !insideVehicle && nearestVehicle.Speed < maxEnterExitVehicleSpeed)
            {
                characterControllerObject.SetActive(false);

                vehicleChanger.DeactivateAll = false;
                relativeEnterPosition = nearestVehicle.transform.InverseTransformPoint(characterControllerObject.transform.position);
                insideVehicle = true;
                vehicleChanger.ChangeVehicle(nearestVehicle);

                nearVehicle = false;
            }
            // Exit vehicleTransform
            else if (insideVehicle && changeVehicle && nearestVehicle.Speed < maxEnterExitVehicleSpeed)
            {
                insideVehicle = false;
                characterControllerObject.transform.position = nearestVehicle.transform.TransformPoint(relativeEnterPosition);
                vehicleChanger.DeactivateAll = true;

                characterControllerObject.SetActive(true);
            }
        }
    }
}


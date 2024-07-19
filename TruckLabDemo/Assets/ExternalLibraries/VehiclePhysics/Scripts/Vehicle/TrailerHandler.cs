using UnityEngine;
using System.Collections;

namespace NWH.VehiclePhysics
{
    /// <summary>
    /// Everthing related to a trailerTransform.
    /// </summary>
    [System.Serializable]
    public class TrailerHandler
    {
        /// <summary>
        /// Set to true if the vehicleTransform is a trailerTransform, otherwise set to false.
        /// </summary>
        [Tooltip("Set to true if vehicleTransform is a trailerTransform, otherwise set to false.")]
        public bool isTrailer = false;

        /// <summary>
        /// If the vehicleTransform is a trailerTransform, this is the object placed at the point at which it will connect to the towing vehicleTransform.
        /// If the vehicleTransform is towing, this is the object placed at point at which trailerTransform will be coneected.
        /// </summary>
        [Tooltip("If the vehicleTransform is a trailerTransform, this is the object placed at the point at which it will connect to the towing vehicleTransform." +
            " If the vehicleTransform is towing, this is the object placed at point at which trailerTransform will be coneected.")]
        public GameObject attachmentPoint = null;

        /// <summary>
        /// Tag which will be taken into consideration when searching for a trailerTransform in the scene.
        /// </summary>
        public string trailersTag = "trailerTransform";

        /// <summary>
        /// Maximum distance between towing vehicleTransform's attachment point and trailerTransform's attachment point.
        /// </summary>
        public float attachDistanceThreshold = 0.5f;

        /// <summary>
        /// If a trailerTransform is in range when the scene is started it will be attached.
        /// </summary>
        public bool attachOnPlay = false;

        /// <summary>
        /// Breaking force of the generated joint.
        /// </summary>
        public float breakForce = Mathf.Infinity;

        /// <summary>
        /// Is trailerTransform's attachment point close enough to be attached to the towing vehicleTransform?
        /// </summary>
        public bool trailerInRange = false;

        /// <summary>
        /// Object that will be disabled when trailerTransform is attached and disabled when trailerTransform is detached.
        /// </summary>
        public GameObject trailerStand;

        /// <summary>
        /// True if object is trailerTransform and is attached to a towing vehicleTransform and also true if towing vehicleTransform and has trailerTransform attached.
        /// </summary>
        [HideInInspector]
        public bool attached;

        /// <summary>
        /// Power reduction that will be applied when vehicleTransform has no trailerTransform to avoid wheel spin when controlled with a binary Controller.
        /// </summary>
        public float maxNoTrailerPowerReduction = 0f;

        private VehicleController nearestTrailerVC = null;
        private VehicleController trailerVC = null;
        private ConfigurableJoint cj = null;
        private GameObject[] trailerGOs;
        private VehicleController otherController;

        private VehicleController vc;

        public void Initialize(VehicleController vc)
        {
            this.vc = vc;

            try
            {
                trailerGOs = GameObject.FindGameObjectsWithTag(trailersTag);
            }
            catch
            {
                Debug.LogWarning("'" + trailersTag + "' tag does not exist.");
            }
        }

        /// <summary>
        /// Return 0 if no trailerTransform and noTrailerPowerReduction value if trailerTransform attached.
        /// </summary>
        public float NoTrailerPowerReduction
        {
            get
            {
                if(!isTrailer && !attached)
                {
                    return maxNoTrailerPowerReduction;
                }
                return 0;
            }
        }

        // Update is called once per frame
        public void Update()
        {
            if(!isTrailer)
            {
                UpdateTractor();
            }    
            else
            {
                UpdateTrailer();
            }

            // Wait for physics to settle before attaching trailerTransform
            if(Time.frameCount == 15f)
            {
                if (attachOnPlay && !isTrailer)
                {
                    UpdateTractor();
                    AttachTrailer();
                }
            }
        }

        private void UpdateTrailer()
        {
            if (!attached) vc.effects.lights.enabled = false;
        }

        private void UpdateTractor()
        {
            // Try to find trailers if none attached
            if (trailerVC == null)
            {
                if (!vc.trailer.isTrailer && attachmentPoint != null)
                {
                    if (trailerGOs.Length > 0)
                    {
                        nearestTrailerVC = null;
                        trailerInRange = false;
                        foreach (GameObject trailerGO in trailerGOs)
                        {
                            otherController = trailerGO.GetComponent<VehicleController>();
                            if (otherController != null && otherController.trailer.isTrailer && otherController.enabled)
                            {
                                Vector3 dir = otherController.trailer.attachmentPoint.transform.position - vc.trailer.attachmentPoint.transform.position;
                                float dist = dir.sqrMagnitude;

                                if (dist < attachDistanceThreshold)
                                {
                                    nearestTrailerVC = otherController;
                                    trailerInRange = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        nearestTrailerVC = null;
                        trailerInRange = false;
                    }
                }
            }

            // Attach trailerTransform
            if (nearestTrailerVC != null && trailerVC == null && vc.input.trailerAttachDetach)
            {
                AttachTrailer();
            }
            // Detach trailerTransform
            else if (trailerVC != null && vc.input.trailerAttachDetach)
            {
                DetachTrailer();
            }

            // Detach trailerTransform if joint broke
            if (trailerVC != null && cj == null)
            {
                DetachTrailer();
            }

            if(trailerVC != null && attached)
            {
                trailerVC.transmission.Gear = vc.transmission.Gear;
            }

            vc.input.trailerAttachDetach = false;
        }

        private void AttachTrailer()
        {
            if(nearestTrailerVC != null)
            {
                trailerVC = nearestTrailerVC;
                trailerVC.transform.position = trailerVC.transform.position - (trailerVC.trailer.attachmentPoint.transform.position - vc.trailer.attachmentPoint.transform.position);

                cj = trailerVC.gameObject.GetComponent<ConfigurableJoint>();
                if (cj == null)
                    cj = trailerVC.gameObject.AddComponent<ConfigurableJoint>();
                cj.connectedBody = vc.vehicleRigidbody;
                cj.anchor = trailerVC.transform.InverseTransformPoint(attachmentPoint.transform.position);
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;
                cj.enableCollision = true;
                cj.breakForce = breakForce;

                trailerVC.trailer.attached = true;
                vc.trailer.attached = true;

                trailerVC.Active = true;
                vc.input.trailerAttachDetach = false;

                if (trailerVC.trailer.trailerStand != null)
                    trailerVC.trailer.trailerStand.SetActive(false);

                trailerVC.input = vc.input;
                trailerVC.effects.lights.enabled = vc.effects.lights.enabled;
            }
        }


        private void DetachTrailer()
        {
            if (trailerVC.trailer.trailerStand != null)
                trailerVC.trailer.trailerStand.SetActive(true);
            trailerVC.trailer.attached = false;
            //trailerVC.input = null;
            trailerVC.effects.lights.enabled = false;
            vc.trailer.attached = false;

            GameObject.Destroy(cj);
            cj = null;
            trailerVC.Active = false;
            trailerVC = null;

            vc.input.trailerAttachDetach = false;
        }

        private void RemoveAllJoints()
        {
            var joints = vc.gameObject.GetComponents<ConfigurableJoint>();
            foreach(ConfigurableJoint joint in joints)
            {
                GameObject.Destroy(joint);
            }
        }
    }
}


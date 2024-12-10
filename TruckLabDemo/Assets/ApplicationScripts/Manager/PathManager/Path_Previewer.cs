using System.Collections.Generic;
using UnityEngine;

namespace ApplicationScripts.Manager.PathManager
{
    /// <summary>
    /// Handles the visualization of paths for VeVehicle simulation using LineRenderers.
    /// </summary>
    public class Path_Previewer : MonoBehaviour
    {
        /// <summary>
        /// LineRenderer used for visualizing the front axle referencePath of the vehicle.
        /// </summary>
        [Header("Reference_Path Renderers")]
        public LineRenderer FrontAxleRenderer;

        // Uncomment and assign if additional axles are to be visualized
        // public LineRenderer pivotRenderer;
        // public LineRenderer rearAxleRenderer;
        // public LineRenderer trailerAxleRenderer;

        /// <summary>
        /// Draws the specified referencePath using LineRenderers.
        /// </summary>
        /// <param name="referencePath">The referencePath object containing points to visualize.</param>
        public void DrawPath(Reference_Path referencePath)
        {
            // Activate the LineRenderer for the front axle and draw its referencePath
            FrontAxleRenderer.gameObject.SetActive(true);
            DrawLine(referencePath.FrontAxle, FrontAxleRenderer);

            // Uncomment to draw additional axles if required
            // DrawLine(referencePath.Pivot, pivotRenderer);
            // DrawLine(referencePath.RearAxle, rearAxleRenderer);
            // DrawLine(referencePath.TrailerAxle, trailerAxleRenderer);
        }

        /// <summary>
        /// Draws a line between a series of points using the specified LineRenderer.
        /// </summary>
        /// <param name="points">List of 2D points that define the line.</param>
        /// <param name="lineRenderer">The LineRenderer to be used for drawing the line.</param>
        private void DrawLine(List<Vector2> points, LineRenderer lineRenderer)
        {
            // Set the number of positions the LineRenderer will use
            lineRenderer.positionCount = points.Count;

            // Use world coordinates for rendering the line
            lineRenderer.useWorldSpace = true;

            // Convert 2D points to 3D positions for the LineRenderer
            Vector3[] positions = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                // Set a slight elevation (0.1f) to ensure lines are visible above ground level
                positions[i] = new Vector3(points[i].x, 0.2f, points[i].y);
            }

            // Assign the calculated positions to the LineRenderer
            lineRenderer.SetPositions(positions);
        }

        /// <summary>
        /// Toggles the visibility of the referencePath visualization.
        /// </summary>
        /// <param name="status">True to show the referencePath, false to hide it.</param>
        public void Toggle(bool status)
        {
            // Toggle the active state of the front axle LineRenderer
            FrontAxleRenderer.gameObject.SetActive(status);

            // Uncomment to toggle visibility of additional axles
            // if (pivotRenderer != null) pivotRenderer.gameObject.SetActive(status);
            // if (rearAxleRenderer != null) rearAxleRenderer.gameObject.SetActive(status);
            // if (trailerAxleRenderer != null) trailerAxleRenderer.gameObject.SetActive(status);
        }
    }
}

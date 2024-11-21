using System.Collections.Generic;
using UnityEngine;

namespace Application_Scripts.Manager.Path_Manager
{
    /// <summary>
    /// Handles the visualization of paths for VeVehicle simulation using LineRenderers.
    /// </summary>
    public class Path_Previewer : MonoBehaviour
    {
        /// <summary>
        /// LineRenderer used for visualizing the front axle path of the vehicle.
        /// </summary>
        [Header("Path Renderers")]
        public LineRenderer FrontAxleRenderer;

        // Uncomment and assign if additional axles are to be visualized
        // public LineRenderer pivotRenderer;
        // public LineRenderer rearAxleRenderer;
        // public LineRenderer trailerAxleRenderer;

        /// <summary>
        /// Draws the specified path using LineRenderers.
        /// </summary>
        /// <param name="path">The path object containing points to visualize.</param>
        public void DrawPath(Path path)
        {
            // Activate the LineRenderer for the front axle and draw its path
            FrontAxleRenderer.gameObject.SetActive(true);
            DrawLine(path.FrontAxle, FrontAxleRenderer);

            // Uncomment to draw additional axles if required
            // DrawLine(path.Pivot, pivotRenderer);
            // DrawLine(path.RearAxle, rearAxleRenderer);
            // DrawLine(path.TrailerAxle, trailerAxleRenderer);
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
                positions[i] = new Vector3(points[i].x, 0.1f, points[i].y);
            }

            // Assign the calculated positions to the LineRenderer
            lineRenderer.SetPositions(positions);
        }

        /// <summary>
        /// Toggles the visibility of the path visualization.
        /// </summary>
        /// <param name="status">True to show the path, false to hide it.</param>
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

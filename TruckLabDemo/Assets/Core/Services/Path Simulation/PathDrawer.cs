using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Draws paths for vehicle simulation using LineRenderers.
    /// </summary>
    public class PathDrawer : MonoBehaviour
    {
        public LineRenderer frontAxleRenderer;
        //public LineRenderer pivotRenderer;
        //public LineRenderer rearAxleRenderer;
        //public LineRenderer trailerAxleRenderer;
        private bool isActive;

        /// <summary>
        /// Draws the specified path.
        /// </summary>
        /// <param name="path">The path to be drawn.</param>
        public void DrawPath(Path path)
        {
            frontAxleRenderer.gameObject.SetActive(true);
            DrawLine(path.frontaxle, frontAxleRenderer);
            //DrawLine(path.pivot, pivotRenderer);
            //DrawLine(path.rearaxle, rearAxleRenderer);
            //DrawLine(path.traileraxle, trailerAxleRenderer);
        }

        /// <summary>
        /// Draws a line using the given points and LineRenderer.
        /// </summary>
        /// <param name="points">The points that define the line.</param>
        /// <param name="lineRenderer">The LineRenderer to be used.</param>
        private void DrawLine(List<Vector2> points, LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.useWorldSpace = true; // Set to true if using world coordinates
            Vector3[] positions = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                positions[i] = new Vector3(points[i].x, 0.1f, points[i].y);
            }
            lineRenderer.SetPositions(positions);
        }

        /// <summary>
        /// Toggles the visibility of the path.
        /// </summary>
        /// <param name="status">The visibility status.</param>
        public void Toggle(bool status)
        {
            frontAxleRenderer.gameObject.SetActive(status);
        }
    }
}

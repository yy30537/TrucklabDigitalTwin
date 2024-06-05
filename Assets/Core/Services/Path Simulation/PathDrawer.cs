using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class PathDrawer : MonoBehaviour
    {
        public LineRenderer frontAxleRenderer;
        //public LineRenderer pivotRenderer;
        //public LineRenderer rearAxleRenderer;
        //public LineRenderer trailerAxleRenderer;
        private bool isActive;
        
        public void DrawPath(Path path)
        {
            frontAxleRenderer.gameObject.SetActive(true);
            DrawLine(path.frontaxle, frontAxleRenderer);
            //DrawLine(path.pivot, pivotRenderer);
            //DrawLine(path.rearaxle, rearAxleRenderer);
            //DrawLine(path.traileraxle, trailerAxleRenderer);
        }
        
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

        public void Toggle(bool status)
        {
            frontAxleRenderer.gameObject.SetActive(status);
        }
        
        
    }
}

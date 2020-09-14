using System.Collections.Generic;
using System.Linq;
using Framework.Extensions;
using Sirenix.OdinInspector;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace DashTerritory
{
    public class TerritoryGrid : MonoBehaviour
    {
        public float territorySize = 1;
        public float tileHeight = 0.1f;
        public float tileGap = 0.1f;
        public Color gizmoColor;
        
        private Bounds bounds;
        private Vector3[] corners;
        
        public Vector3[] Subdivisions { get; private set; }

        private void Start()
        {
            var gridCenter = transform.position;
            var gridSize = transform.localScale;
            gridCenter.y += gridSize.y * 0.5f;
            bounds = new Bounds(gridCenter, gridSize);

            corners = CalculateCorners();
            Subdivisions = CalculateSubdivisions().ToArray();
            
            BuildGrid();
        }

        private void BuildGrid()
        {
            var groundTransform = new GameObject("Ground").AddComponent<BoxCollider>().transform;
            var scale = transform.localScale;
            scale.y = 2;
            groundTransform.localScale = scale;
            
            var position = transform.position;
            position.y -= scale.y * 0.5f;
            position.y += tileHeight;
            groundTransform.position = position;

            groundTransform.rotation = transform.rotation;
            
            groundTransform.SetParent(transform);
            groundTransform.gameObject.layer = LayerMask.NameToLayer("Ground");
            
            TerritoryTileManager.Instance.RegisterTerritoryGrid(this);
        }
        
        private void OnDrawGizmos()
        {
            DrawWireBox();
            
            if (Application.isPlaying) return; 
            
            Subdivisions = CalculateSubdivisions().ToArray();
            foreach (var subdivision in Subdivisions)
            {
                Gizmos.DrawCube(subdivision, new Vector3
                {
                    x = territorySize - tileGap * 0.5f, 
                    y = tileHeight,
                    z = territorySize - tileGap * 0.5f
                });
            }
        }

        private void DrawWireBox()
        {
            var gridCenter = transform.position;
            var gridSize = transform.localScale;
            gridCenter.y += gridSize.y * 0.5f;
            
            bounds = new Bounds(gridCenter, gridSize);
            var corners = CalculateCorners();

            Gizmos.color = gizmoColor;

            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[0], corners[3]);

            Gizmos.DrawLine(corners[4], corners[5]);
            Gizmos.DrawLine(corners[5], corners[6]);
            Gizmos.DrawLine(corners[6], corners[7]);
            Gizmos.DrawLine(corners[4], corners[7]);

            Gizmos.DrawLine(corners[0], corners[4]);
            Gizmos.DrawLine(corners[1], corners[5]);
            Gizmos.DrawLine(corners[2], corners[6]);
            Gizmos.DrawLine(corners[3], corners[7]);
        }
        
        private Vector3[] CalculateCorners()
        {
            var c = new Vector3[8];

            // Add bottom corners
            c[0] = bounds.min;
            c[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            c[2] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            c[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);

            // Add top corners
            c[4] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            c[5] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            c[6] = bounds.max;
            c[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);

            for (var i = 0; i < c.Length; i++)
            {
                c[i] = c[i].RotateAround(bounds.center, transform.rotation);
            }
            return c;
        }
        
        private IEnumerable<Vector3> CalculateSubdivisions()
        {
            var subdivisionCenters = new List<Vector3>();
            var subdivisionsX = Mathf.FloorToInt(bounds.size.x / territorySize);
            var subdivisionsZ = Mathf.FloorToInt(bounds.size.z / territorySize);

            for (var x = 0; x < subdivisionsX; x++)
            {
                for (var z = 0; z < subdivisionsZ; z++)
                {
                    var center = new Vector3
                    {
                        x = x * territorySize + territorySize / 2,
                        z = z * territorySize + territorySize / 2
                    };
                    center += bounds.min;
                    subdivisionCenters.Add(center);
                }
            }

            for (var c = 0; c < subdivisionCenters.Count; c++)
            {
                var subdivision = subdivisionCenters[c];
                subdivision = subdivision.RotateAround(bounds.center, transform.rotation);
                subdivision.y += tileHeight * 0.5f;
                subdivisionCenters[c] = subdivision;
            }

            return subdivisionCenters;
        }
    }

    public sealed class GridElement
    {
        public Vector3 center;
        public Quaternion rotation;
    }
}

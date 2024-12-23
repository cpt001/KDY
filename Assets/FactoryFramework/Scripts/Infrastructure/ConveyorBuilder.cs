using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using UnityEngine.Splines;

namespace FactoryFramework
{
    public class ConveyorBuilder : MonoBehaviour
    {
        public BeltMeshSO beltMesh;
        public BeltMeshSO frameMesh;

        public Vector3[] points = new Vector3[]
        {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 0, 1),
            new Vector3 (1, 0, 3),
            new Vector3 (-1, 0, 5),
            new Vector3 (0, 0, 7),
            new Vector3 (0, 0, 8),
        };

        private ConveyorBelt _conveyor;

        private void Start()
        {

            Build();
        }

        void Build()
        {
            _conveyor = GetComponent<ConveyorBelt>();

            if (ConveyorLogisticsUtils.settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SMART)
            {
                _conveyor.AssignPath<RoundedPolygon>(points);
            }
            else if (ConveyorLogisticsUtils.settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SPLINE)
            {
                _conveyor.AssignPath<PointSplinePath>(points);
            }
            if (beltMesh != null && frameMesh != null)
                _conveyor.UpdateMesh(beltMesh, frameMesh);
            else
                _conveyor.UpdateMesh();
        }

        private void OnDrawGizmos()
        {
            // draw the points
            for (int i = 0; i < points.Length-1; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
        private void OnDrawGizmosSelected()
        {
            // draw the points
            for (int i = 0; i < points.Length ; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(points[i], .15f);
            }
        }

        //private void OnValidate()
        //{
        //    Build();
        //}
    }
}

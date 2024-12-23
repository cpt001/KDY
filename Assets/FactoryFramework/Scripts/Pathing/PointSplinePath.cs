using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Jobs;
using UnityEngine.UIElements;

namespace FactoryFramework
{
    public struct PointSplinePath : IPath, IPathMeshGenerator, IDisposable
    {
        public NativeArray<float3> Points;
        public NativeArray<quaternion> Tangents;
        private NativeArray<float3> _constructorPoints;

        private bool _validCorner;
        public bool IsValid => _validCorner;
        private float _length;
        public float TotalLength => _length;

        public float3[] PathPoints => Points.ToArray();
        public quaternion[] PathRotations => Tangents.ToArray();
        public Vector3[] OriginalPoints => _constructorPoints.ToArray().Select(v => (Vector3)v).ToArray();

        private float3 QuaternionToForward(quaternion q) => math.mul(q, math.forward());

        public PointSplinePath(Vector3[] pathPoints, float cornerRadius, int cornerResolution)
        {
            List<BezierKnot> points = new List<BezierKnot>();
            for (int i = 0; i < pathPoints.Length; i++)
            {
                // Calculate tangent positions
                Vector3 tangentIn = Vector3.zero;
                Vector3 tangentOut = Vector3.zero;

                // WORK IN PROGRESS
                //if (i > 0)
                //{
                //    // Tangent in is towards the previous point
                //    tangentIn = (pathPoints[i] - pathPoints[i - 1]).normalized * cornerRadius * cornerResolution;
                //}

                //if (i < pathPoints.Length - 1)
                //{
                //    // Tangent out is towards the next point
                //    tangentOut = (pathPoints[i + 1] - pathPoints[i]).normalized * cornerRadius * cornerResolution;
                //}

                // Create the knot with the position and tangents
                BezierKnot knot = new BezierKnot(pathPoints[i]);
                //BezierKnot knot = new BezierKnot(pathPoints[i], tangentIn, tangentOut);
                // Calculate the rotation to ensure the up vector is world up
                Vector3 forward = (tangentOut).normalized;
                Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);

                // Apply the rotation to the knot
                knot.Rotation = rotation;
                points.Add(knot);
            }

            var spline = new Spline(points);
            spline.SetAutoSmoothTension(new SplineRange(0, spline.Count - 1), 1f);
            spline.SetTangentMode(TangentMode.AutoSmooth);
            _validCorner = spline != null;
            //spline.SetAutoSmoothTension(new SplineRange(0, spline.Count), 0.85f);
            var knotPoints = spline.Knots.Select(k => k.Position).ToArray();

            _length = spline.GetLength();
            _constructorPoints = new NativeArray<float3>(knotPoints, Allocator.Persistent);


            var sampledPoints = new List<Vector3>();
            float t = 0;
            float step = .002f;
            float threshold = 5f;
            float distanceThreshold = 3f;

            float lastT = 0;

            Vector3 _tempPos = spline.EvaluatePosition(step);
            Vector3 _position = spline.EvaluatePosition(0);
            Vector3 lastTangent = (_tempPos - _position).normalized;
            sampledPoints.Add(_position);

            t += step;

            while (t < 1 - step)
            {
                Vector3 tempPos = spline.EvaluatePosition(t + step);
                Vector3 position = spline.EvaluatePosition(t);

                Vector3 curTangent = (tempPos - position).normalized;
                if (Vector3.Angle(curTangent, lastTangent) > threshold || Vector3.Distance(tempPos, position) > distanceThreshold)
                {
                    lastTangent = curTangent;
                    lastT = t;
                    sampledPoints.Add(position );
                }
                t += step;
            }
            sampledPoints.Add(spline.EvaluatePosition(1f));

            Points = new NativeArray<float3>(sampledPoints.Count(), Allocator.Persistent);
            Tangents = new NativeArray<quaternion>(sampledPoints.Count(), Allocator.Persistent);

            for (int i = 0; i < sampledPoints.Count(); i++)
            {
                float perc = i / (float)(sampledPoints.Count()-1);
                Points[i] = spline.EvaluatePosition(perc);
                Tangents[i] = quaternion.LookRotationSafe(spline.EvaluateTangent(perc), math.up());
            }
        }


        public float3 GetPositionAtPoint(float targetDistance)
        {
            //float targetDistance = TotalLength * distance;
            float accumulatedDistance = 0f;
            for (int i = 0; i < Points.Length - 1; i++)
            {
                float3 p0 = Points[i];
                float3 p1 = Points[i + 1];
                float length = math.distance(p0, p1);

                if (accumulatedDistance + length >= targetDistance)
                {
                    float remainingDistance = targetDistance - accumulatedDistance;
                    return math.lerp(p0, p1, remainingDistance / length);
                }
                accumulatedDistance += length;
            }
            // If the distance exceeds the path length, return the last point
            return Points[Points.Length - 1];
        }

        // Get the tangent vector at a specific distance along the path
        public float3 GetTangentAtDistance(float targetDistance)
        {
            float accumulatedDistance = 0f;
            float3 fwd;

            for (int i = 0; i < Points.Length - 1; i++)
            {
                float segmentLength = math.distance(Points[i], Points[i + 1]);
                if (accumulatedDistance + segmentLength >= targetDistance)
                {
                    // Return the normalized direction vector of the current segment
                    //fwd = math.normalize((Points[i + 1] - Points[i]));
                    //return math.mul(quaternion.LookRotation(fwd, math.up()), math.forward());
                    return math.normalize((Points[i + 1] - Points[i]));
                }
                accumulatedDistance += segmentLength;
            }
            return math.normalize(Points[Points.Length - 2] - Points[Points.Length - 1]);
            //fwd = math.normalize(Points[Points.Length - 2] - Points[Points.Length - 1]);
            //return math.mul(quaternion.LookRotation(fwd, math.up()), math.forward());
        }
        public quaternion GetRotationAtPoint(float targetDistance)
        {
            float accumulatedDistance = 0f;

            for (int i = 0; i < Points.Length - 1; i++)
            {
                float segmentLength = math.distance(Points[i], Points[i + 1]);
                if (accumulatedDistance + segmentLength >= targetDistance)
                {
                    // Return the normalized direction vector of the current segment
                    //float t = (targetDistance - accumulatedDistance) / segmentLength;
                    //return math.slerp(Tangents[i], Tangents[i + 1], math.pow(t,3));
                    return this.Tangents[i];
                }
                accumulatedDistance += segmentLength;
            }

            //fwd = math.normalize(Points[Points.Length -2] - Points[Points.Length - 1]);
            //right = math.cross(fwd, math.up());
            //up = math.cross(right, fwd);
            return this.Tangents[this.Tangents.Length - 1];
        }

        public (float3, float) GetClosestPoint(float3 worldPoint)
        {
            // get closest point to the path
            float minDistance = float.MaxValue;
            float3 closestPoint = float3.zero;
            float closestDistance = 0f;
            for (int i = 0; i < Points.Length - 1; i++)
            {
                float3 p0 = Points[i];
                float3 p1 = Points[i + 1];
                float3 dir = math.normalize(p1 - p0);
                float t = math.dot(worldPoint - p0, dir);
                float3 closest = p0 + math.clamp(t, 0, 1) * dir;
                float distance = math.distance(closest, worldPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closest;
                    closestDistance = math.distance(Points[0], closest);
                }
            }
            return (closestPoint, closestDistance);
        }

        public float3 GetStart()
        {
            return Points[0];
        }

        public float3 GetEnd()
        {
            return Points[Points.Length - 1];
        }

        public float3 GetDirectionAtPoint(float targetDistance)
        {
            return math.mul(GetRotationAtPoint(targetDistance), math.forward());
        }

        public float3 GetRightAtPoint(float targetDistance)
        {
            return math.mul(GetRotationAtPoint(targetDistance), math.right());
        }

        public float3 GetUpAtPoint(float targetDistance)
        {
            return math.mul(GetRotationAtPoint(targetDistance), math.up());
        }

        public (float3, float3, float3) GetPathVectors(float targetDistance)
        {
            var rot = GetRotationAtPoint(targetDistance);
            return (math.mul(rot, math.forward()), math.mul(rot, math.right()), math.mul(rot, math.up()));
        }

        public void CleanUp()
        {
            if (Points.IsCreated) Points.Dispose();
            if (Tangents.IsCreated) Tangents.Dispose();
            if (_constructorPoints.IsCreated) _constructorPoints.Dispose();
        }

        public void Dispose()
        {
            if (Points.IsCreated) Points.Dispose();
            if (Tangents.IsCreated) Tangents.Dispose();
            if (_constructorPoints.IsCreated) _constructorPoints.Dispose();
        }

        public JobHandle RunMeshGenJob(ref BeltMeshGenerator.NativeMeshGroup inputMesh, ref BeltMeshGenerator.NativeMesh outputMesh, ref BeltMeshGenerator.MeshGenParams settings)
        {
            var MGJ = BeltMeshGenerator.MeshGenApi.CreateMeshGenJob(this, ref inputMesh, ref outputMesh, ref settings);
            return MGJ.Run();
        }
    }
}

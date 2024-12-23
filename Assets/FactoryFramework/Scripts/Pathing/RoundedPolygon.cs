using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace FactoryFramework
{
    public struct RoundedPolygon : IPath, IPathMeshGenerator, IDisposable
    {
        public NativeArray<float3> Points;
        public NativeArray<quaternion> Tangents;
        private NativeArray<float3> _constructorPoints;

        private float cornerRadius;

        private bool _validCorner;
        public bool IsValid => _validCorner;
        private float _length;
        public float TotalLength => _length;

        public float3[] PathPoints => Points.ToArray();
        public quaternion[] PathRotations => Tangents.ToArray();
        public Vector3[] OriginalPoints => _constructorPoints.ToArray().Select(v=>(Vector3)v).ToArray();

        private float3 QuaternionToForward(quaternion q) => math.mul(q, math.forward());

        public RoundedPolygon(Vector3[] Polygon, float cornerRadius, int cornerResolution)
        {
            _validCorner = true;
            this.cornerRadius = cornerRadius;
            var points = new List<float3>();
            var tangents = new List<quaternion>();
            
            float accumulatedDistance = 0f;
            points.Add(Polygon[0]);
            tangents.Add(quaternion.LookRotation(math.normalize(Polygon[1] - Polygon[0]), math.up()));
            for (int i = 1; i < Polygon.Length - 1; i++)
            {
                float3 p0 = Polygon[i - 1];
                float3 p1 = Polygon[i];
                float3 p2 = Polygon[i + 1];

                //flatten the points around p1
                p0.y = p1.y;
                p2.y = p1.y;

                float3 v1 = math.normalize(p0 - p1);
                float3 v2 = math.normalize(p2 - p1);

                float cross = math.cross(v1, v2).y;
                bool isConvex = cross > 0;

                // calculate the bisector
                float3 bisector = math.normalize(v1 + v2);

                float angle = math.acos(math.dot(v1, v2));
                float distance = cornerRadius / math.tan(angle / 2);

                //ensure distance is not greater than half of the edge length
                float edge1Length = math.distance(p0, p1);
                float edge2Length = math.distance(p1, p2);
                if (distance > math.min(edge1Length, edge2Length) / 2)
                    _validCorner = false;
                //distance = math.min(distance, edge1Length / 2);

                // get start and end of arc
                float3 arcStart = p1 + v1 * distance;
                float3 arcEnd = p1 + v2 * distance;

                if (math.abs(math.dot(v1, v2)) >= .99)
                    continue;

                // calculate the arc center
                float arcDistance = cornerRadius / math.sin(angle / 2f);
                float3 arcCenter = p1 + bisector * arcDistance;

                // calcuate the arc points
                float startAngle = math.atan2((arcStart - arcCenter).z, (arcStart - arcCenter).x);
                float endAngle = Mathf.Atan2((arcEnd - arcCenter).z, (arcEnd - arcCenter).x);

                // Correct end angle
                if (isConvex && endAngle < startAngle)
                    endAngle += math.PI * 2f;
                else if (!isConvex && endAngle > startAngle)
                    startAngle += math.PI * 2f;

                if (cornerRadius == 0)
                {
                    points.Add(p1);
                    tangents.Add(quaternion.LookRotation(math.normalize(Polygon[i + 1] - Polygon[i]), math.up()));
                    accumulatedDistance += math.distance(points[points.Count - 1], points[points.Count - 2]);
                } else
                {
                    //float angleStep = (endAngle - startAngle) / cornerResolution;
                    float angleStep = isConvex ? (endAngle - startAngle) / cornerResolution :
                        (startAngle - endAngle) / cornerResolution;

                    // add arc points
                    for (int j = 1; j <= cornerResolution; j++)
                    {
                        float angleToUse = isConvex ? startAngle + j * angleStep : startAngle - j * angleStep;
                        float3 point = new float3(arcCenter.x + cornerRadius * math.cos(angleToUse), p1.y, arcCenter.z + cornerRadius * math.sin(angleToUse));
                        points.Add(point);
                        //float radian = math.radians(angleToUse);
                        float3 tangent = new float3(math.cos(angleToUse), 0, math.sin(angleToUse));
                        if (!isConvex)
                            tangent = -tangent;
                        float3 forward = math.cross(tangent, math.up());
                        float3 up = math.cross(forward, tangent);
                        tangents.Add(quaternion.LookRotation(forward, up));
                        accumulatedDistance += math.distance(points[points.Count - 1], points[points.Count - 2]);
                    }
                }
               
            }
            tangents.Add(quaternion.LookRotation(math.normalize(Polygon[Polygon.Length-1] - Polygon[Polygon.Length - 2]), math.up()));
            points.Add(Polygon[Polygon.Length - 1]);
            accumulatedDistance += math.distance(points[points.Count - 1], points[points.Count - 2]);

            _constructorPoints = new NativeArray<float3>(Polygon.Select(x => (float3)x).ToArray(), Allocator.Persistent);
            Points = new NativeArray<float3>(points.ToArray(), Allocator.Persistent);
            Tangents = new NativeArray<quaternion>(tangents.ToArray(), Allocator.Persistent);
            _length = accumulatedDistance;
        }

        // Get a point on the path at a specific distance
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

            for (int i = 0; i < Points.Length - 1; i++)
            {
                float segmentLength = math.distance(Points[i], Points[i + 1]);
                if (accumulatedDistance + segmentLength >= targetDistance)
                {
                    // Return the normalized direction vector of the current segment
                    return math.normalize((Points[i + 1] - Points[i]));
                }
                accumulatedDistance += segmentLength;
            }
            return math.normalize(Points[Points.Length - 2] - Points[Points.Length - 1]);
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
            return Points[Points.Length-1];
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
            return (math.mul(rot,math.forward()), math.mul(rot, math.right()), math.mul(rot, math.up()));
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

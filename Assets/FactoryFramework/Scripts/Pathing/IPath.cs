using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace FactoryFramework
{
    public interface IPath
    {
        public bool IsValid { get; }
        public float TotalLength { get; }

        public (float3, float) GetClosestPoint(float3 worldPoint);

        public float3 GetStart();
        public float3 GetEnd();

        public float3 GetPositionAtPoint(float targetDistance);
        public float3 GetDirectionAtPoint(float targetDistance);
        public float3 GetRightAtPoint(float targetDistance);
        public float3 GetUpAtPoint(float targetDistance);

        //Forward, Right, Up
        public (float3, float3, float3) GetPathVectors(float targetDistance);

        public quaternion GetRotationAtPoint(float targetDistance);
        public void CleanUp();

        public Vector3[] OriginalPoints { get; } 
        public float3[] PathPoints { get; }
        public quaternion[] PathRotations { get; }
    }

    // This interface designates that an object supports Jobs based mesh generation
    public interface IPathMeshGenerator
    {
        // Create a MeshGenConfig<pathstruct> job, then return the handle to it
        public JobHandle RunMeshGenJob(ref BeltMeshGenerator.NativeMeshGroup inputMesh, ref BeltMeshGenerator.NativeMesh outputMesh, ref BeltMeshGenerator.MeshGenParams settings);
    }
}
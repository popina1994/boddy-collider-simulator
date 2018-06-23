using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    class MeshUpgrade
    {
        public const int NUM_POINTS = 3;

        // Assumes that density is 1.
        public static float CalculateMass(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3 initCenter = new Vector3(0, 0, 0);

            float mass = 0.0f;
            foreach (var vertexId in mesh.triangles)
            {
                initCenter += vertices[vertexId];
            }
            initCenter /= mesh.triangles.Length;

            for (int idx = 0; idx < mesh.triangles.Length; idx += NUM_POINTS)
            {
                Tetrahedron tetrahedron = new Tetrahedron(vertices[mesh.triangles[idx]], vertices[mesh.triangles[idx + 1]],
                    vertices[mesh.triangles[idx + 2]], initCenter);
                mass += tetrahedron.CalculateVolume();
            }

            return mass;
        }

        public static Vector3 CalculateCenterOfMass(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3 initCenter = new Vector3(0, 0, 0);
            Vector3 center = new Vector3(0, 0, 0);
            float mass = CalculateMass(mesh);
            foreach (var vertexId in mesh.triangles)
            {
                initCenter += vertices[vertexId];
            }
            initCenter /= mesh.triangles.Length;

            for (int idx = 0; idx < mesh.triangles.Length; idx += NUM_POINTS)
            {
                Tetrahedron tetrahedron = new Tetrahedron(vertices[mesh.triangles[idx]], vertices[mesh.triangles[idx + 1]],
                    vertices[mesh.triangles[idx + 2]], initCenter);
                center += tetrahedron.CalculateCenterOfMass() * tetrahedron.CalculateVolume();
            }

            center /= mass;

            return center;
        }
    }
}

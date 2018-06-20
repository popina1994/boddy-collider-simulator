using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class MeshBuilder
    {
        private LinkedList<int> triangles;
        private LinkedList<Vector3> vertices;
        public MeshBuilder()
        {
            triangles = new LinkedList<int>();
            vertices = new LinkedList<Vector3>();
        }

        public void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            int idxVertex = vertices.Count;
            vertices.AddLast(point1);
            vertices.AddLast(point2);
            vertices.AddLast(point3);
            triangles.AddLast(idxVertex);
            triangles.AddLast(idxVertex + 1);
            triangles.AddLast(idxVertex + 2);
        }

        public Mesh Build()
        {
            return null;
        }
    }
}

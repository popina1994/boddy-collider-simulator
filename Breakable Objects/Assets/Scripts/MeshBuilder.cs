using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

            Debug.Log(String.Format("Vertex {0} {1} {2}", point1.ToString(), point2.ToString(), 
                                                          point3.ToString()));
        }

        public Mesh Build()
        {
            Mesh mesh = new Mesh();
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            for (int idx = 0; idx < mesh.triangles.Length; idx+=3)
            {
                Debug.Log(String.Format("Triangle {0} {1} {2}", mesh.vertices[mesh.triangles[idx]].ToString(),
                                                         mesh.vertices[mesh.triangles[idx + 1]].ToString(),
                                                        mesh.vertices[mesh.triangles[idx + 2]].ToString()));
            }
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}

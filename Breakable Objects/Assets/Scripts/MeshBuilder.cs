using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathUpgrade = Assets.Scripts.Utility.MathUpgrade;

namespace Assets.Scripts
{
    class MeshBuilder
    {
        private LinkedList<int> triangles;
        private LinkedList<Vector3> vertices;
        private List<Segment> borderEdges;
        private LinkedList<LinkedList<Vector3>> vertexLoops;
        private Plane plane;
        public MeshBuilder(Plane plane)
        {
            triangles = new LinkedList<int>();
            vertices = new LinkedList<Vector3>();
            borderEdges = new List<Segment>();
            vertexLoops = new LinkedList<LinkedList<Vector3>>();
            this.plane = plane;
        }

        private void AddBorderEdge(Vector3 startPoint, Vector3 endPoint)
        {
            Segment segment = new Segment(startPoint, endPoint);
            if (segment.IsOnPlane(plane) && (MathUpgrade.AreEqual(plane.GetDistanceToPoint(startPoint), 0f)))
            {
                borderEdges.Add(segment);
                Debug.Log(String.Format("Edge {0} {1}", startPoint.ToString(), endPoint.ToString()));
            }
        }

        private void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3, bool addBorderEdge)
        {
            int idxVertex = vertices.Count;
            vertices.AddLast(point1);
            vertices.AddLast(point2);
            vertices.AddLast(point3);

            triangles.AddLast(idxVertex);
            triangles.AddLast(idxVertex + 1);
            triangles.AddLast(idxVertex + 2);

            if (addBorderEdge)
            {
                AddBorderEdge(point1, point2);
                AddBorderEdge(point2, point3);
                AddBorderEdge(point3, point1);
            }
            
            Debug.Log(String.Format("Vertex {0} {1} {2}", point1.ToString(), point2.ToString(),
                point3.ToString()));
            
        }

        public void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            AddTriangle(point1, point2, point3, true);
        }

        /// <summary>
        /// Iterates through border edges (edges that were created by plane cutting the 3d object) and 
        /// extract one loop from them.
        /// </summary>
        /// <param name="borderEdges"></param>
        /// <returns></returns>
        private static LinkedList<Vector3> ExtractVertexLoop(List<Segment> borderEdges)
        {
            LinkedList<Vector3> vertexLoop = new LinkedList<Vector3>();
            Vector3 endPointVertex = borderEdges[0].StartPoint;
            vertexLoop.AddLast(borderEdges[0].EndPoint);
            borderEdges.RemoveAt(0);

            while (true)
            {
                int idx = 0;
                bool found = false;
                for (; idx < borderEdges.Count; idx ++)
                {
                    Segment edge = borderEdges[idx];
                    if (vertexLoop.Last.Value == edge.StartPoint)
                    {
                        vertexLoop.AddLast(edge.EndPoint);
                        borderEdges.RemoveAt(idx);
                        found = true;
                        if (edge.EndPoint == endPointVertex)
                        {
                            return vertexLoop;
                        }
                        break;
                    }
                }
                //TODO: Remove this when concave fragmentation support is added. 
                if ( (idx  == borderEdges.Count) && !found)
                { 
                    return vertexLoop;
                }
            }
        }

        private void TriangulateCaps()
        {
            while (borderEdges.Count != 0)
            {
                vertexLoops.AddLast(ExtractVertexLoop(borderEdges));
            }
            // TODO: Add concave case of triangulation.
            foreach (var vertexLoop in vertexLoops)
            {
                Vector3[] vertexLoopArray = vertexLoop.ToArray();
                for (int idx = 1; idx < vertexLoop.Count - 1; idx++)
                {
                    // Why does this work? Well, the logic the other side shouldn't be seen 
                    // in the oposite triangulation.
                    AddTriangle(vertexLoopArray[idx + 1], vertexLoopArray[idx], vertexLoopArray[0], false);
                }
            }
        }

        public Mesh Build()
        {
            Mesh mesh = new Mesh();

            TriangulateCaps();
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

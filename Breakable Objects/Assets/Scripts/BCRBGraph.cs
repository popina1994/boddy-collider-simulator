using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{

    class BCRBGraph
    {
        private const float HIT_POINTS_MAX = 1;
        private List<MeshTreeNode> _fragments;

        /// <summary>
        /// Represents connections among different fragments
        /// </summary>
        private List<LinkedList<BCRBGraphEdge>> _edges;

        private float _mass;
        private Vector3 _speed;

        public BCRBGraph(MeshTreeNode root)
        {
            LinkedList<MeshTreeNode> nodes = new LinkedList<MeshTreeNode>();
            nodes.AddLast(root);

            Fragments = new List<MeshTreeNode>();
            Edges = new List<LinkedList<BCRBGraphEdge>>();
            while (nodes.Count != 0)
            {
                MeshTreeNode node = nodes.First.Value;
                nodes.RemoveFirst();
                if (node.IsLeaf())
                {
                    Fragments.Add(node);
                    Edges.Add(new LinkedList<BCRBGraphEdge>());
                }
                if (node.Left != null)
                {
                    nodes.AddLast(node.Left);
                }
                if (node.Right != null)
                {
                    nodes.AddLast(node.Right);
                }
            }

            foreach (var fragment in Fragments)
            {
                int idx = 0;
                foreach (var fragmentCollision in Fragments)
                {
                    if ((fragment != fragmentCollision) && fragment.Collides(fragmentCollision))
                    {
                        Edges[idx].AddLast(new BCRBGraphEdge(HIT_POINTS_MAX, fragmentCollision));
                    }
                    idx++;
                }
            }
        }

        private BCRBGraph()
        {
            Fragments = new List<MeshTreeNode>();
            Edges = new List<LinkedList<BCRBGraphEdge>>();
        }

        public List<MeshTreeNode> Fragments
        {
            get { return _fragments;}
            private set { _fragments = value; }
        }

        public List<LinkedList<BCRBGraphEdge>> Edges
        {
            get { return _edges; }
            private set { _edges = value; }
        }

        private static void RemoveBrokenEdges(List<LinkedList<BCRBGraphEdge>> edges)
        {
            foreach (var adjacentList in edges)
            {
                var currentNode = adjacentList.First;
                while (currentNode != null)
                {
                    if (currentNode.Value.IsBroken())
                    {
                        adjacentList.Remove(currentNode);
                    }
                    currentNode = currentNode.Next;
                }
            }
        }

        private static void InitIndicesToVertices(List<MeshTreeNode> vertices)
        {
            int idx = 0;
            foreach (var vertex in vertices)
            {
                vertex.VertexIdx = idx;
                idx++;
            }
        }

        /// <summary>
        /// Iterates through graph and returns newly formed connected components. 
        /// </summary>
        /// <returns></returns>
        private List<BCRBGraph> GetConnectivityGraphs()
        {
            List<BCRBGraph> connectivityComponents = new List<BCRBGraph>();
            LinkedList<int> fragmentsToVisit = new LinkedList<int>();
            List<bool> visitedFragments = new List<bool>(Fragments.Count);
            for (int idx = 0; idx < Fragments.Count; idx++)
            {
                visitedFragments.Add(false);
            }
            InitIndicesToVertices(Fragments);

            for (int idxFragment = 0; idxFragment < Fragments.Count; idxFragment++)
            {
                if (!visitedFragments[idxFragment])
                {
                    BCRBGraph graph = new BCRBGraph();
                    fragmentsToVisit.AddLast(idxFragment);
                    graph.Fragments.Add(Fragments[idxFragment]);
                    graph.Edges.Add(Edges[idxFragment]);

                    while (fragmentsToVisit.Count != 0)
                    {
                        int idxFragmentToVisit = fragmentsToVisit.First.Value;
                        visitedFragments[idxFragmentToVisit] = true;
                        fragmentsToVisit.RemoveFirst();
                        foreach (var edge in Edges[idxFragmentToVisit])
                        {
                            if (!visitedFragments[edge.Fragment.VertexIdx])
                            {
                                fragmentsToVisit.AddLast(edge.Fragment.VertexIdx);
                                graph.Fragments.Add(Fragments[edge.Fragment.VertexIdx]);
                                graph.Edges.Add(Edges[edge.Fragment.VertexIdx]);
                            }
                        }
                    }
                    connectivityComponents.Add(graph);
                }
            }
            return connectivityComponents;
        }

        public bool OnCollisionDamaged(ContactPoint contactPoint, float impulseIntensity)
        {
            float damage;
            float s;
            int idx = 0;
            // TODO: Add real damage calculation.
            bool damaged = false;
            foreach (var adjacentList in Edges)
            {
                foreach (var edge in adjacentList)
                {
                    damaged = true;
                    Vector3 edgePosition = (Fragments[idx].CenterOfMass + edge.Fragment.CenterOfMass) / 2;
                    s = Vector3.Distance(edgePosition, contactPoint.point);
                    //edge.Damage(impulseIntensity / (s * s * s));
                    edge.Damage(HIT_POINTS_MAX);
                }

                idx++;
            }

            return damaged;
        }

        public List<BCRBGraph> RecomputeConnectivity()
        {
            RemoveBrokenEdges(Edges);
            return GetConnectivityGraphs();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class MeshTreeNode
    {
        private Mesh _mesh;
        private MeshTreeNode _left;
        private MeshTreeNode _right;
        private MeshTreeNode _parent;
        private int _vertexIdx;
        private bool _isMinmal;
        private Vector3 _centerOfMass;
        private float _mass;

        public MeshTreeNode(Mesh mesh)
        {
            Mesh = mesh;
        }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public MeshTreeNode Left
        {
            get { return _left; }
            set { _left = value;
                value._parent = this;
            }
        }

        public MeshTreeNode Right
        {
            get { return _right; }
            set { _right = value; value._parent= this; }
        }

        public MeshTreeNode Parent
        {
            get { return _parent; }
        }

        public int VertexIdx
        {
            get { return _vertexIdx; }
            set { _vertexIdx = value; }
        }

        public bool IsMinmal
        {
            get { return _isMinmal; }
            set { _isMinmal = value; }
        }

        public Vector3 CenterOfMass
        {
            get { return _centerOfMass; }
            set { _centerOfMass = value; }
        }

        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        public void Reset()
        {
            VertexIdx = -1;
            IsMinmal = false;
        }

        public bool IsLeaf()
        {
            return (Left == null) && (Right == null);
        }

        public bool Collides(MeshTreeNode meshTreeNode)
        {
            return Mesh.bounds.Intersects(meshTreeNode.Mesh.bounds);
        }
    }
}

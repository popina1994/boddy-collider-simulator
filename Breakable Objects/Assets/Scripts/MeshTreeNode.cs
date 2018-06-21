using System;
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
            set { _left = value; }
        }

        public MeshTreeNode Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public MeshTreeNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }
}

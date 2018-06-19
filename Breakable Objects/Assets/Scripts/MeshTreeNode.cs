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

        public MeshTreeNode(Mesh mesh)
        {
            Mesh = mesh;
        }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }
    }
}

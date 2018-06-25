using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class BCRBGraphEdge
    {
        private float _hitPoints;
        private MeshTreeNode _fragment;


        public BCRBGraphEdge(float hitPoints, MeshTreeNode fragment)
        {
            _hitPoints = hitPoints;
            _fragment = fragment;
            
        }

        public float HitPoints
        {
            get { return _hitPoints; }
        }

        public MeshTreeNode Fragment
        {
            get { return _fragment; }
        }

        public void Damage(float damage)
        {
            _hitPoints -= damage;
        }

        public bool IsBroken()
        {
            return _hitPoints <= 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Assets.Scripts
{
    class Tetrahedron
    {
        private Vector3 _pointA;
        private Vector3 _pointB;
        private Vector3 _pointC;
        private Vector3 _pointD;

        public Tetrahedron(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD)
        {
            _pointA = pointA;
            _pointB = pointB;
            _pointC = pointC;
            _pointD = pointD;
        }

        public Vector3 PointA
        {
            get { return _pointA; }
            set { _pointA = value; }
        }

        public Vector3 PointB
        {
            get { return _pointB; }
            set { _pointB = value; }
        }

        public Vector3 PointC
        {
            get { return _pointC; }
            set { _pointC = value; }
        }

        public Vector3 PointD
        {
            get { return _pointD; }
            set { _pointD = value; }
        }

        public float CalculateVolume()
        {
            return Math.Abs(Vector3.Dot(Vector3.Cross((PointB - PointC), (PointD - PointC)), (PointA - PointC))) / 6;
        }

        public Vector3 CalculateCenterOfMass()
        {
            return (PointA + PointB + PointC + PointD) / 4;
        }
    }
}

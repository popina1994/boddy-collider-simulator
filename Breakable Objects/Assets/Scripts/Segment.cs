using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class Segment
    {
        private Vector3 _startPoint;
        private Vector3 _endPoint;
        private Vector3 _vectorDirection;

        public Vector3 StartPoint
        {
            get { return _startPoint; }
        }

        public Vector3 EndPoint
        {
            get { return _endPoint; }
        }

        public Vector3 VectorDirection
        {
            get { return _vectorDirection; }
        }

        public Segment(Vector3 startPoint, Vector3 endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _vectorDirection = EndPoint - StartPoint;

        }

        public bool IsOnPlane(Plane plane)
        {
            return Utility.AreEqual(Vector3.Dot(plane.normal, _vectorDirection), 0f);
        }

    }
}

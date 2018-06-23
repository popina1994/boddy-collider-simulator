using UnityEngine;

namespace Assets.Scripts.Utility
{
    class MathUpgrade
    {
        private const float PRECISION = 0.000001f;

        public static bool AreEqual(float first, float second)
        {
            return System.Math.Abs(first - second) < PRECISION;
        }

        public static Vector3 IntersectionSegmentPlane(Plane plane, Vector3 point1, Vector3 point2)
        {
            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 intersection;
            Vector3 lineVec = point2 - point1;
            Vector3 linePoint = point1;
            Vector3 planePoint = plane.ClosestPointOnPlane(new Vector3(0, 0, 0));

            dotNumerator = Vector3.Dot((planePoint - linePoint), plane.normal);
            dotDenominator = Vector3.Dot(lineVec, plane.normal);

            length = dotNumerator / dotDenominator;
            intersection = linePoint + lineVec * length;

            return intersection;
        }
    }
}
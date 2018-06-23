using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityScript.Steps;


namespace Assets.Scripts
{
    class Utility
    {
        private const String FILE_NAME = "log.txt";
        private const float PRECISION = 0.000001f;
        private static void  HandleLog(string logString, string stackTrace, LogType type)
        {
            using (StreamWriter w = File.AppendText(FILE_NAME))
            {
                w.WriteLine(logString);
            }
        }
        
        public static void RegisterLogFile()
        {
            Debug.unityLogger.logEnabled = false;
            Application.logMessageReceived += HandleLog;
            File.WriteAllText(FILE_NAME, String.Empty);
        }

        public static bool AreEqual(float first, float second)
        {
            return Math.Abs(first - second) < PRECISION;
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

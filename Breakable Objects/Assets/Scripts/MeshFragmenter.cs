using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MeshFragmenter : MonoBehaviour
{
    private const int POSITIVE = 0;
    private const int NEGATIVE = 1;
    private const int NUM_SIDES = 2;
    private const int NUM_POINTS = 3;
    private Mesh mesh;
    private int numberOfFragments = 2;
    private MeshTreeNode meshTreeRoot = null;

    private static MeshTreeNode instantiateTreeFromMesh(Mesh mesh)
    {
        return new MeshTreeNode(mesh);
    }

    private static Vector3 IntersectionSegmentPlane(Plane plane, Vector3 point1, Vector3 point2)
    {
        Ray ray = new Ray(point1, point2);
        float intersectionDistance;
        plane.Raycast(ray, out intersectionDistance);
        return  ray.GetPoint(intersectionDistance);
    }

    private static LinkedList<Mesh> SliceMesh(Plane plane, Mesh mesh)
    {
        Vector3[] meshVertices = mesh.vertices;
        MeshBuilder[] meshBuilders = new MeshBuilder[NUM_SIDES];
        Vector3[] points = new Vector3[NUM_POINTS];
        bool [] isPositive = new bool[NUM_POINTS];

        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            meshBuilders[idx] = new MeshBuilder();
        }
        
        for (int idxTriangle = 0; idxTriangle < mesh.triangles.Length; idxTriangle += NUM_POINTS)
        {
            int idxSide = -1;
            for (int idxPoint = 0; idxPoint < NUM_POINTS; idxPoint++)
            {
                points[idxPoint] = meshVertices[mesh.triangles[idxTriangle + idxPoint]];
                isPositive[idxPoint] = plane.GetSide(points[idxPoint]);
            }
            
            if ((isPositive[0] == isPositive[1]) && (isPositive[1] == isPositive[2]))
            {
                idxSide = isPositive[0] ? POSITIVE : NEGATIVE;
                meshBuilders[idxSide].AddTriangle(points[0], points[1], points[2]);
            }
            else
            {
                int idxPointOnOtherSide = -1;
                int idxPrev = -1;
                int idxNext = -1;
                for (int idxPoint = 0; idxPoint < NUM_POINTS; idxPoint++)
                {
                    idxPrev = (idxPoint + 2) % NUM_POINTS;
                    idxNext = (idxPoint + 1) % NUM_POINTS;
                    if ((points[idxPoint] != points[idxPrev]) && (points[idxPoint] != points[idxNext]))
                    {
                        idxSide = isPositive[idxPoint] ? POSITIVE : NEGATIVE;
                        idxPointOnOtherSide = idxPoint;
                        break;
                    }
                }
                Vector3 intersectionPoint1 = IntersectionSegmentPlane(plane, points[idxNext], points[idxPointOnOtherSide]);
                Vector3 intersectionPoint2 = IntersectionSegmentPlane(plane, points[idxPrev], points[idxPointOnOtherSide]);

                // Triangle from the other side. 
                meshBuilders[idxSide].AddTriangle(points[idxPointOnOtherSide], intersectionPoint1, intersectionPoint2);
                int idxOtherSide = (idxSide + 1) % NUM_SIDES;
                meshBuilders[idxOtherSide].AddTriangle(points[idxNext], intersectionPoint1, intersectionPoint2);
                meshBuilders[idxOtherSide].AddTriangle(points[idxPrev], intersectionPoint1, intersectionPoint2);
            }
        }
        LinkedList<Mesh> meshBuild = new LinkedList<Mesh>();
        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            meshBuild.AddLast(meshBuilders[idx].Build());
        }
        return meshBuild;
    }

    private static void SplitNode(MeshTreeNode meshTreeNode)
    {
        Vector3 center = new Vector3(0, 0, 0);
        Vector3 random1 = new Vector3(1, 0, 0);
        Vector3 random2 = new Vector3(0, 1, 0);
        // TODO: Find a way how to calculate center of a mass of the mesh.
        //
        Plane planeCenter = new Plane(center, random1, random2);

        LinkedList<Mesh> slicedMeshResults = SliceMesh(planeCenter, meshTreeNode.Mesh);
        // TODO: Add multiple fragments generation. 
        //
    }   

    private void BuildMeshTree(Mesh mesh, int numberOfFragments)
    {
        meshTreeRoot = instantiateTreeFromMesh(mesh);
        SplitNode(meshTreeRoot);
    }

    // Functions that Unity uses in its execution engine. 
    void Awake()
    {
        MeshFilter viewedModelFilter = GetComponent<MeshFilter>();
        mesh = viewedModelFilter.mesh;
        BuildMeshTree(mesh, numberOfFragments);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}

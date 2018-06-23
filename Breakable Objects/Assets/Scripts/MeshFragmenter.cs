using System;
using System.Diagnostics;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

// TODO: Try to change name of namespace so it corresponds to its file path. 
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

    // TODO: Update this for mesh that represents concave object.
    private static Vector3 CalculateCenterOfMass(Mesh mesh)
    {
        Vector3 []vertices = mesh.vertices;
        Vector3 initCenter = new Vector3(0, 0, 0);
        Vector3 center = new Vector3(0, 0, 0);
        float mass = 0.0f;
        foreach (var vertexId in mesh.triangles)
        {
            initCenter += vertices[vertexId];
        }
        initCenter /= mesh.triangles.Length;

        for (int idx = 0; idx < mesh.triangles.Length; idx += NUM_POINTS)
        {
            Tetrahedron tetrahedron = new Tetrahedron(vertices[mesh.triangles[idx]], vertices[mesh.triangles[idx+1]],
                                                      vertices[mesh.triangles[idx+2]], initCenter);
            center += tetrahedron.CalculateCenterOfMass() * tetrahedron.CalculateVolume();
            mass += tetrahedron.CalculateVolume();
        }

        center /= mass;

        return center;
    }

    private static Mesh[] SliceMesh(Plane plane, Mesh mesh)
    {
        Vector3[] meshVertices = mesh.vertices;
        MeshBuilder[] meshBuilders = new MeshBuilder[NUM_SIDES];
        Vector3[] points = new Vector3[NUM_POINTS];
        bool [] isPositive = new bool[NUM_POINTS];

        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            meshBuilders[idx] = new MeshBuilder(plane);
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
                Debug.Log(isPositive[0] ? "Positive" : "Negative");
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
                    if ((isPositive[idxPoint] != isPositive[idxPrev]) && (isPositive[idxPoint] != isPositive[idxNext]))
                    {
                        idxSide = isPositive[idxPoint] ? POSITIVE : NEGATIVE;
                        idxPointOnOtherSide = idxPoint;
                        break;
                    }
                }
                Vector3 intsecPointNext = Utility.IntersectionSegmentPlane(plane, points[idxNext], points[idxPointOnOtherSide]);
                Vector3 intsecPointPrev = Utility.IntersectionSegmentPlane(plane, points[idxPrev], points[idxPointOnOtherSide]);
                
                // IMPORTANT: Following order of points is important.
                // Triangle whose only point from the bigger one is added.
                Debug.Log("Cutting of a triangle");
                Debug.Log(isPositive[idxPointOnOtherSide] ? "Positive" : "Negative");
                meshBuilders[idxSide].AddTriangle(intsecPointPrev, points[idxPointOnOtherSide], intsecPointNext);
                int idxOtherSide = (idxSide + 1) % NUM_SIDES;
                Debug.Log(isPositive[idxNext] ? "Positive" : "Negative");
                meshBuilders[idxOtherSide].AddTriangle(points[idxNext], intsecPointPrev, intsecPointNext);
                Debug.Log(isPositive[idxPrev] ? "Positive" : "Negative");
                meshBuilders[idxOtherSide].AddTriangle(points[idxPrev], intsecPointPrev, points[idxNext]);
            }
        }
        Mesh[] meshBuild = new Mesh[NUM_SIDES];
        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            Debug.Log(idx);
            meshBuild[idx] = meshBuilders[idx].Build();
        }
        return meshBuild;
    }

    private static void SplitNode(MeshTreeNode meshTreeNode)
    {
        Vector3 center = CalculateCenterOfMass(meshTreeNode.Mesh);
        Vector3 random1 = new Vector3(0.5f,0.5f, 0f);
        Vector3 random2 = new Vector3(0.5f, -0.5f, 0f);
        Plane planeCenter = new Plane(center, random1, random2);

        Mesh[] slicedMeshResults = SliceMesh(planeCenter, meshTreeNode.Mesh);
        // TODO: Add multiple fragments generation. 
        //
        meshTreeNode.Left = new MeshTreeNode(slicedMeshResults[POSITIVE]);
        meshTreeNode.Right = new MeshTreeNode(slicedMeshResults[NEGATIVE]);
        meshTreeNode.Left.Parent = meshTreeNode;
        meshTreeNode.Right.Parent = meshTreeNode;
    }   

    private void BuildMeshTree(Mesh mesh, int numberOfFragments)
    {
        meshTreeRoot = instantiateTreeFromMesh(mesh);
        SplitNode(meshTreeRoot);
    }

    // Functions that Unity uses in its execution engine. 
    void Awake()
    {
        Utility.RegisterLogFile();
        MeshFilter viewedModelFilter = GetComponent<MeshFilter>();
        MeshRenderer viewModelRenderer = GetComponent<MeshRenderer>();
        mesh = viewedModelFilter.mesh;
        BuildMeshTree(mesh, numberOfFragments);
        GameObject gameObjectLocal = new GameObject("A");
        // TODO: Be careful about constructor vs Instantiate. 
        // TODO: Refactor this code. 
        gameObjectLocal.transform.position = new Vector3(3, 3, 3);
        gameObjectLocal.transform.localScale = GetComponent<Transform>().localScale;
        MeshCollider meshColider = gameObjectLocal.AddComponent<MeshCollider>() as MeshCollider;
        meshColider.sharedMesh = meshTreeRoot.Left.Mesh;
        meshColider.transform.parent = Selection.activeTransform;
        MeshFilter meshFilter = gameObjectLocal.AddComponent<MeshFilter>() as MeshFilter;
        meshFilter.mesh = meshTreeRoot.Left.Mesh;
        MeshRenderer meshRenderer = gameObjectLocal.AddComponent<MeshRenderer>() as MeshRenderer;
        meshRenderer.material = viewModelRenderer.material;

        GameObject gameObjectLocalB = new GameObject("B");
        // TODO: Be careful about constructor vs Instantiate. 
        gameObjectLocalB.transform.position = new Vector3(-5, -5, -5);
        gameObjectLocalB.transform.localScale = GetComponent<Transform>().localScale;
        meshColider = gameObjectLocalB.AddComponent<MeshCollider>() as MeshCollider;
        meshColider.sharedMesh = meshTreeRoot.Right.Mesh;
        meshColider.transform.parent = Selection.activeTransform;
        meshFilter = gameObjectLocalB.AddComponent<MeshFilter>() as MeshFilter;
        meshFilter.mesh = meshTreeRoot.Right.Mesh;
        meshRenderer = gameObjectLocalB.AddComponent<MeshRenderer>() as MeshRenderer;
        meshRenderer.material = viewModelRenderer.material;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

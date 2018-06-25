using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts;
using Assets.Scripts.Utility;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using MathUpgrade = Assets.Scripts.Utility.MathUpgrade;
using Random = UnityEngine.Random;

// TODO: Try to change name of namespace so it corresponds to its file path. 
[RequireComponent(typeof(MeshFragmenter))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
public class MeshFragmenter : MonoBehaviour
{
    private const int POSITIVE = 0;
    private const int NEGATIVE = 1;
    private const int NUM_SIDES = 2;
    private const int NUMBER_OF_FRAGMENTS = 128;
    private MeshTreeNode _meshTreeRoot = null;
    private BCRBGraph _fragmentConnectivityGraph;
    private bool _initialized = false;
    private static int _nameId = 0;

    private MeshTreeNode MeshTreeRoot
    {
        get { return _meshTreeRoot; }
        set { _meshTreeRoot = value; }
    }

    private BCRBGraph FragmentConnectivityGraph
    {
        get { return _fragmentConnectivityGraph; }
        set { _fragmentConnectivityGraph = value; }
    }

    public bool Initialized
    {
        get { return _initialized; }
        set { _initialized = value; }
    }

    public static int NameId
    {
        get { return _nameId++; }
    }

    private static MeshTreeNode InstantiateTreeFromMesh(Mesh mesh)
    {
        return new MeshTreeNode(mesh);
    }

    // TODO: Update this for mesh that represents concave object.

    private static Mesh[] SliceMesh(Plane plane, Mesh mesh)
    {
        Vector3[] meshVertices = mesh.vertices;
        MeshBuilder[] meshBuilders = new MeshBuilder[NUM_SIDES];
        Vector3[] points = new Vector3[MeshUpgrade.NUM_POINTS];
        bool [] isPositive = new bool[MeshUpgrade.NUM_POINTS];

        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            meshBuilders[idx] = new MeshBuilder(plane);
        }
        
        for (int idxTriangle = 0; idxTriangle < mesh.triangles.Length; idxTriangle += MeshUpgrade.NUM_POINTS)
        {
            int idxSide = -1;
            for (int idxPoint = 0; idxPoint < MeshUpgrade.NUM_POINTS; idxPoint++)
            {
                points[idxPoint] = meshVertices[mesh.triangles[idxTriangle + idxPoint]];
                isPositive[idxPoint] = plane.GetSide(points[idxPoint]);
            }
            
            if ((isPositive[0] == isPositive[1]) && (isPositive[1] == isPositive[2]))
            {
                idxSide = isPositive[0] ? POSITIVE : NEGATIVE;
                ////Debug.Log(isPositive[0] ? "Positive" : "Negative");
                meshBuilders[idxSide].AddTriangle(points[0], points[1], points[2]);
            }
            else
            {
                int idxPointOnOtherSide = -1;
                int idxPrev = -1;
                int idxNext = -1;
                for (int idxPoint = 0; idxPoint < MeshUpgrade.NUM_POINTS; idxPoint++)
                {
                    idxPrev = (idxPoint + 2) % MeshUpgrade.NUM_POINTS;
                    idxNext = (idxPoint + 1) % MeshUpgrade.NUM_POINTS;
                    if ((isPositive[idxPoint] != isPositive[idxPrev]) && (isPositive[idxPoint] != isPositive[idxNext]))
                    {
                        idxSide = isPositive[idxPoint] ? POSITIVE : NEGATIVE;
                        idxPointOnOtherSide = idxPoint;
                        break;
                    }
                }
                Vector3 intsecPointNext = MathUpgrade.IntersectionSegmentPlane(plane, points[idxNext], points[idxPointOnOtherSide]);
                Vector3 intsecPointPrev = MathUpgrade.IntersectionSegmentPlane(plane, points[idxPrev], points[idxPointOnOtherSide]);
                
                // IMPORTANT: Following order of points is important.
                // Triangle whose only point from the bigger one is added.
                //Debug.Log("Cutting of a triangle");
                //Debug.Log(isPositive[idxPointOnOtherSide] ? "Positive" : "Negative");
                meshBuilders[idxSide].AddTriangle(intsecPointPrev, points[idxPointOnOtherSide], intsecPointNext);
                int idxOtherSide = (idxSide + 1) % NUM_SIDES;
                //Debug.Log(isPositive[idxNext] ? "Positive" : "Negative");
                meshBuilders[idxOtherSide].AddTriangle(points[idxNext], intsecPointPrev, intsecPointNext);
                //Debug.Log(isPositive[idxPrev] ? "Positive" : "Negative");
                meshBuilders[idxOtherSide].AddTriangle(points[idxPrev], intsecPointPrev, points[idxNext]);
            }
        }
        Mesh[] meshBuild = new Mesh[NUM_SIDES];
        for (int idx = 0; idx < NUM_SIDES; idx++)
        {
            //Debug.Log(idx);
            meshBuild[idx] = meshBuilders[idx].Build();
        }
        return meshBuild;
    }

    private static void SplitNode(MeshTreeNode meshTreeNode, int numberOfFragments)
    {
        // TODO: Refactor this.
        Vector3 centerOfMass = MeshUpgrade.CalculateCenterOfMass(meshTreeNode.Mesh);
        meshTreeNode.CenterOfMass = centerOfMass;
        meshTreeNode.Mass = MeshUpgrade.CalculateMass(meshTreeNode.Mesh);
        if (numberOfFragments == NUMBER_OF_FRAGMENTS)
        {
            Debug.Log(String.Format("CenterOfMass{0}", centerOfMass.ToString()));
            Debug.Log(String.Format("Mass{0}", centerOfMass.ToString()));
        }
        if (numberOfFragments < NUMBER_OF_FRAGMENTS)
        {
            Vector3 random1 = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Vector3 random2 = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Plane planeCenter = new Plane(centerOfMass, random1, random2);

            Mesh[] slicedMeshResults = SliceMesh(planeCenter, meshTreeNode.Mesh);
            meshTreeNode.Left = new MeshTreeNode(slicedMeshResults[POSITIVE]);
            meshTreeNode.Right = new MeshTreeNode(slicedMeshResults[NEGATIVE]);
            numberOfFragments *= 2;

            SplitNode(meshTreeNode.Left, numberOfFragments);
            SplitNode(meshTreeNode.Right, numberOfFragments);
        }
    }   

    private void BuildMeshTree(Mesh mesh, int numberOfFragments)
    {
        MeshTreeRoot = InstantiateTreeFromMesh(mesh);
        SplitNode(MeshTreeRoot, numberOfFragments);
    }

    // Functions that Unity uses in its execution engine. 
    void Awake()
    {
        if (!Initialized)
        {
            LoggingUpgrade.RegisterLogFile();
            BuildMeshTree(GetComponent<MeshFilter>().mesh, 1);
            _fragmentConnectivityGraph = new BCRBGraph(MeshTreeRoot);
        }
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    { 
		
	}

    void FixedUpdate()
    {
    }

    private static void AddThisComponentBeforeAwake(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.AddComponent<MeshFragmenter>();
        gameObject.GetComponent<MeshFragmenter>().Initialized = true;
        gameObject.SetActive(true);
    }

    private static void CreateFragmentGameObject(GameObject gameObject, List<MeshTreeNode> meshTreeNodes)
    {   
        GameObject gameObjectFragmentNew = new GameObject(NameId.ToString(), new Type[]
        {
            typeof(MeshRenderer), typeof(MeshFilter), typeof(Rigidbody), typeof(MeshCollider)
        });

        MeshFilter meshFilter = gameObjectFragmentNew.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObjectFragmentNew.GetComponent<MeshRenderer>();
        MeshCollider meshColider = gameObjectFragmentNew.GetComponent<MeshCollider>();
        Rigidbody rigidbody = gameObjectFragmentNew.GetComponent<Rigidbody>();

        rigidbody.mass = 0;
        CombineInstance[] combine = new CombineInstance[meshTreeNodes.Count];
        int idx = 0;
        meshFilter.mesh = new Mesh();
        foreach (var meshTreeNode in meshTreeNodes)
        {
            combine[idx].mesh = meshTreeNode.Mesh;
            combine[idx].transform = gameObject.GetComponent<MeshFilter>().transform.localToWorldMatrix;
            rigidbody.mass += meshTreeNode.Mass;
            idx++;
        }
        meshFilter.mesh.CombineMeshes(combine);
        rigidbody.useGravity = gameObject.GetComponent<Rigidbody>().useGravity;
        meshColider.convex = true;
        meshColider.sharedMesh = meshFilter.mesh;
        
        AddThisComponentBeforeAwake(gameObjectFragmentNew);

        meshRenderer.material = gameObject.GetComponent<MeshRenderer>().material;

    }

    private static void InitMeshTreeNodes(MeshTreeNode rootTreeNode)
    {
        LinkedList<MeshTreeNode> nodesToVisit = new LinkedList<MeshTreeNode>();
        nodesToVisit.AddLast(rootTreeNode);
        while (nodesToVisit.Count != 0)
        {
            MeshTreeNode visitNode = nodesToVisit.First.Value;
            nodesToVisit.RemoveFirst();
            visitNode.Reset();
            if (visitNode.Left != null)
            {
                nodesToVisit.AddLast(visitNode.Left);
            }
            if (visitNode.Right != null)
            {
                nodesToVisit.AddLast(visitNode.Right);
            }
        }
    }

    // TODO: Upgrade for non 2base version of number of fragments.
    private static void VisitAndGenerateMinimalMeshTrees(MeshTreeNode root)
    {
        if (root.IsLeaf())
        {
            return;
        }

        VisitAndGenerateMinimalMeshTrees(root.Left);
        VisitAndGenerateMinimalMeshTrees(root.Right);
        if (root.Left.IsMinmal && root.Right.IsMinmal)
        {
            root.IsMinmal = true;
        }
    }

    List<MeshTreeNode> GetMinimalMeshTrees(BCRBGraph graph)
    {
        List<MeshTreeNode> minimalMeshTrees = new List<MeshTreeNode>();
        LinkedList<MeshTreeNode> visitNodes = new LinkedList<MeshTreeNode>();
        InitMeshTreeNodes(MeshTreeRoot);
        foreach (MeshTreeNode fragment in graph.Fragments)
        {
            fragment.IsMinmal = true;
        }

        VisitAndGenerateMinimalMeshTrees(MeshTreeRoot);
        visitNodes.AddLast(MeshTreeRoot);
        while (visitNodes.Count != 0)
        {
            MeshTreeNode visitNode = visitNodes.First.Value;
            visitNodes.RemoveFirst();
            if (visitNode.IsMinmal)
            {
                minimalMeshTrees.Add(visitNode);
            }
            else
            {
                if (visitNode.Left != null)
                {
                    visitNodes.AddLast(visitNode.Left);
                }
                if (visitNode.Right != null)
                {
                    visitNodes.AddLast(visitNode.Right);
                }
            }
        }

        return minimalMeshTrees;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Initialized)
        {
            return;
        }
        ContactPoint contactPoint = collision.contacts[0];
        float impulseIntensity = Vector3.Magnitude(
                        this.gameObject.GetComponent<Rigidbody>().velocity * 
                        this.gameObject.GetComponent<Rigidbody>().mass
                        + 
                        collision.gameObject.GetComponent<Rigidbody>().velocity * 
                        collision.gameObject.GetComponent<Rigidbody>().mass);

        if (_fragmentConnectivityGraph.OnCollisionDamaged(contactPoint, this.gameObject.GetComponent<Transform>(),
                                                         impulseIntensity))
        {
            this.gameObject.GetComponent<MeshCollider>().enabled = false;
            List<BCRBGraph> components = _fragmentConnectivityGraph.RecomputeConnectivity();
            Debug.Log(String.Format("Number of components{0}", components.Count));
            foreach (var componentGraph in components)
            {
                List<MeshTreeNode> meshTrees = GetMinimalMeshTrees(componentGraph);
                CreateFragmentGameObject(this.gameObject, meshTrees);
            }
            Destroy(this.gameObject);
        }
    }
}

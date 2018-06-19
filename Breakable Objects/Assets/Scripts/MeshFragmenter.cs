using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MeshFragmenter : MonoBehaviour
{
    private Mesh mesh;
    private int numberOfFragments = 2;
    private MeshTreeNode meshTreeRoot = null;

    private MeshTreeNode instantiateTreeFromMesh(Mesh mesh)
    {
        return new MeshTreeNode(mesh);
    }

    private Mesh[] SliceMesh(Plane plane, Mesh mesh)
    {
        return null;
    }

    private void SplitNode(MeshTreeNode meshTreeNode)
    {
        Vector3 center = new Vector3(0, 0, 0);
        Vector3 random1 = new Vector3(1, 0, 0);
        Vector3 random2 = new Vector3(0, 1, 0);
        // TODO: Find a way how to calculate center of a mass of the mesh.
        //
        Plane planeCenter = new Plane(center, random1, random2);

        Mesh[] slicedMeshResults = SliceMesh(planeCenter, meshTreeNode.Mesh);
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

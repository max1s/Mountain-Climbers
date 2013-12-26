using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleAgent : MonoBehaviour 
{
    public List<NaturalMesh.Point> myGraph;
	// Use this for initialization
	void Start () 
    {
        myGraph = NaturalMesh.myGraph;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    float CalculateStepCost()
    {
        return 0f;
    }
}

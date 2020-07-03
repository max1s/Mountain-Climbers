using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour 
{
	public NaturalMesh graphStructure; //

	private static float[,] myHeights;
	public static float[,] mySteepness;
    public static List<Vector4> myForestIntersections;

	CharacterController myController;

	public Vector3 vertical;
	public Vector3 horizontal;


	public virtual void Start () 
	{
		GetData();

		vertical = transform.TransformDirection(Vector3.forward);
		horizontal = transform.TransformDirection(Vector3.right);
		myController  = GetComponent<CharacterController>();
		myController.radius = 10;
		myController.height = 0.01f;
        myController.slopeLimit = 90f;
		myController.center = new Vector3(0f, 5f, 0f);
		graphStructure = GameObject.Find ("myTerrain").GetComponent<NaturalMesh>();
	}

	public virtual void Update () 
	{
		
		if(!Grounded ())
			myController.Move(new Vector3(0, -1, 0));
        if (Input.GetKey(KeyCode.UpArrow))
            Move(transform.right);
		animation.Play("walk");
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			Quaternion rot = new Quaternion();
			rot.eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 5, 0);
			transform.rotation = rot;
		}
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = transform.rotation.eulerAngles + new Vector3(0, 5, 0);
            transform.rotation = rot;
        }
	}

	//public virtual List<NaturalMesh.Point> Deliberate();

	void GetData()
	{
		mySteepness = NaturalMesh.steepness;
		myHeights = TerrainGeneration.heights;
        myForestIntersections = TerrainGeneration.GetForestPositions();
	}


    void Rotate(Quaternion q)
    {
        transform.rotation = q;
    }

	void Move(Vector3 intention)
	{
        Vector3 myCurrentAdjustedPosition = Helper.WorldToTerrainPosition(Terrain.activeTerrain, 512, transform.position);
        //Vector3 myTargetAdjustedPosition = myCurrentAdjustedPosition + Helper.WorldToTerrainPosition(Terrain.activeTerrain, 512, intention);

        //calculate terrain variant;
        float heightVal = 0.0f;
        float adjustedTerrainValue = myHeights[(int)myCurrentAdjustedPosition.x, (int)myCurrentAdjustedPosition.z];
        
        if (adjustedTerrainValue > 0.48f && adjustedTerrainValue < 0.61f)
        {
            intention *= 0.9f;
            heightVal = 0.9f;
        }
        if (adjustedTerrainValue > 0.61f)
        {
            intention *= 0.85f;
            heightVal = 0.85f;
        }
        //calculate tree collisions;
        bool forest = false;
        for (int j = 0; j < myForestIntersections.Count; j++)
        {
            if (myCurrentAdjustedPosition.x >= myForestIntersections[j].x && myCurrentAdjustedPosition.z >= myForestIntersections[j].y
               && myCurrentAdjustedPosition.x <= myForestIntersections[j].z && myCurrentAdjustedPosition.z <= myForestIntersections[j].w)
            {
                forest = true;
                intention *= 0.6f;
            }
        }

                //calculate Steepness variant;
        float steep = mySteepness[(int)myCurrentAdjustedPosition.x, (int)myCurrentAdjustedPosition.z];
        intention *= (steep == 0) ? 1f : (1 - steep); //Mathf.Pow(steep,2))  ;

        //Debug.Log(steep + " " + heightVal + " " + forest + " " + (intention.magnitude) + transform.position);
        //Quaternion yRotation = new Quaternion();
        myController.Move(intention * 0.5f);
	}

    public void MoveTo(Vector3 initialPosition, Vector3 targetPosition)
    {
        var heading = initialPosition - targetPosition;
        var normalisedDirection = heading / heading.magnitude;
		if(initialPosition != targetPosition)
			transform.right = new Vector3(normalisedDirection.x, 0f, normalisedDirection.z);
		//Debug.Log(normalisedDirection);
        Move(normalisedDirection);
    }


	bool Grounded()
	{
		return (myController.collisionFlags == CollisionFlags.Below);
	}
	
}

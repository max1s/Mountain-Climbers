using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Astar : Agent
{
    Vector3 target;
	List<Point>  myGraph;

	public override void Start()
    {
        base.Start();
		myGraph = graphStructure.myGraph;      
	}
	
	// Update is called once per frame
	public override void Update () 
    {
		MouseHandle ();
	}

	public List<Point> Deliberate(Vector3 start, Vector3 goal)
	{
		graphStructure.AddNode(start);
		myGraph[myGraph.Count - 1].myWeights.Add(0);
		myGraph[myGraph.Count - 1].g = 0;
		myGraph[myGraph.Count - 1].h = Vector3.Distance(start, goal);
		myGraph[myGraph.Count -1].f = Vector3.Distance(start, goal);
		graphStructure.AddNode(goal);

		var closedSet = new List<Point>();
		var openSet = new List<Point>();
		openSet.Add (myGraph[myGraph.Count -2]);

		//Point current = myGraph[myGraph.Count - 2];
		while(openSet.Count != 0)
		{
			Point current = openSet[0];
			foreach(Point p in openSet)
			{
				if(openSet.Count > 1)
				{
				   if(p.f < current.f)
					current = p;
				}
				else
				{
					current = p;
				}
			}
			if(current.Position == goal)
				return ReconstructPath(current);

			openSet.Remove(current);
			closedSet.Add(current);


			for(int i = 0; i < current.myNeighbours.Count; i++ )
			{
				if(closedSet.Contains(current.myNeighbours[i]))
					continue;
				float tentative_g = current.g + current.myWeights[i];

				if(!openSet.Contains(current.myNeighbours[i]) || tentative_g < current.myNeighbours[i].g)
				{
					current.myNeighbours[i].myParent = current;
					current.myNeighbours[i].g = tentative_g;
					current.myNeighbours[i].f = tentative_g + Vector3.Distance(current.myNeighbours[i].Position, goal);
					if(!openSet.Contains(current.myNeighbours[i]))
					{
						openSet.Add (current.myNeighbours[i]);
					}
				}
			}

		}

		return new List<Point>();

	}

	public List<Point> ReconstructPath(Point end)
	{

		List<Point> path = new List<Point>();
		path.Add(end);
		Point start = myGraph[myGraph.Count - 2];
		Point current = end;
		while(current != start)
		{
			Debug.Log (current.Position);
			path.Add(current.myParent);
			current = current.myParent;
		}

		Debug.Log (current.Position);
		return path;

	}

	void MouseHandle()
	{
		if(Input.GetMouseButton(0))
		{
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit r;
			if(Physics.Raycast(ray, out r))
			{
				target = r.point;
				Debug.Log (r.point);	
			}
			if (!(transform.position == target))
				MoveTo(target, transform.position );
			
		}
		base.Update();
		
	}

	
}

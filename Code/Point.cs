using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point
{
	private Vector3 myPosition;
	public List<Point> myNeighbours;
	public List<float> myWeights;

	public float f;
	public float g;
	public float h;


	public Point myParent;
	public int groupID;
	
	public Point(int id, Vector3 position, int noOfNeighbours)
	{
		groupID = id;
		myPosition = position;
		myNeighbours = new List<Point>();
		myWeights = new List<float>();
	}
	
	public Vector3 Position
	{
		get
		{
			return myPosition;
		}
		set
		{
			myPosition = value;
		}
	}
	
	
	public static bool operator ==(Point a, Point b)
	{
		return a.myPosition == b.myPosition;
	}
	
	public static bool operator !=(Point a, Point b)
	{
		return a.myPosition != b.myPosition;
	}
	
	public void Merge(Point that)
	{
		if (this.groupID == that.groupID)
			return; 
		this.groupID = that.groupID;
		foreach (Point p in myNeighbours)
			p.Merge(this);
	}
	
}

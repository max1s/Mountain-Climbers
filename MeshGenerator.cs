using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeshGenerator : MonoBehaviour
{
	readonly Color[] colors = new Color[]{Color.black, Color.blue, Color.yellow, Color.green, Color.red};
	private Shader graphShader;
	private Material graphMat;
	
	public static List<Point> myGraph;
	
	GameObject graphRepresentation;
	
	private float lineSize = 0.03f;
	private float pointSize = 2f;
	
	public static float[,] steepness;
	int terrainX;
	int terrainZ;
	
	int powerOfTerrainModifier = 3;
	
	int numberOfNeighbours = 4;
	int randomnessDisplacement = 0;
	
	TerrainData terrainData;
	
    public class Point
	{
		private Vector3 myPosition;
		public List<Point> myNeighbours;
        public int groupID;
		
		public Point(int id, Vector3 position, int noOfNeighbours)
		{
            groupID = id;
			myPosition = position;
			myNeighbours = new List<Point>();
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

    public bool CanConnectTo(Point @this, Point @that)
    {
        for (float i = 0; i < 1f; i += 0.1f)
        {
            Vector3 linerp = Vector3.Lerp( TerrainToWorldPosition(Terrain.activeTerrain, 512, @this.Position) ,
                                            TerrainToWorldPosition(Terrain.activeTerrain, 512, @that.Position), i );
            if (Terrain.activeTerrain.terrainData.GetSteepness(linerp.x, linerp.z) != null)
                return false;
                          
        }
        return true;
    }

	
	void Start()
	{
        Random.seed = 3;
		myGraph = new List<Point>();
		graphShader = (Shader)Resources.Load("RedShader");
		graphMat = new Material(graphShader);
		graphRepresentation = new GameObject();
		terrainData = Terrain.activeTerrain.terrainData;
		terrainX = 512;
		terrainZ = 512;
		steepness = new float[terrainX,terrainZ];
		CalculateSteepness();
		GeneratePoints(64, 16);
		//GenerateEvenPoints();
		//DrawPoints(myGraph);
		GenerateEdges();
		DrawGraph(myGraph, numberOfNeighbours);
	}
	
	void CalculateSteepness()
	{
		
		for(int i = 1; i < terrainX-2; i++)
		{
			for(int j = 1; j < terrainZ-2; j++)
			{
				
				if((j == 1 || i == 1 || j == terrainZ -2 || i == terrainX -2))
				{
					if(j == 0 && i == 0)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i+1, j) - terrainData.GetHeight(i,j)), Mathf.Abs(terrainData.GetHeight(i,j+1) - terrainData.GetHeight(i,j)));
					if(i == 0 && j > 0)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1)));
					if(j == 0 && i > 0)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1)));
					if(i == terrainX-1 && j == terrainZ-1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j)));
					if(i == terrainX-1 && j < terrainZ-1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i,j+1)));
					if(j == terrainZ-1 && i < terrainX-1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j)));
				}
				else
				{
					float gradient = Mathf.Max(Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1))),
									 		   Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j+1))));
					steepness[i,j] = gradient;
				}
			}
			
		}
		
	}
	
	void GenerateEvenPoints()
	{
		for(int i = 0; i < terrainX; i= i + 20)
		{
			for(int j = 0; j < terrainZ; j = j + 20)
			{
				
				Vector3 temp = TerrainToWorldPosition(Terrain.activeTerrain, 512, new Vector3(i, 0, j));
				temp.y = terrainData.GetHeight((int)Mathf.Round(temp.x),(int)Mathf.Round(temp.z));
				if(!(temp.x < 1 || temp.z < 1 || temp.x > terrainX -1  || temp.z > terrainZ - 1))
					myGraph.Add(new Point(myGraph.Count, temp, numberOfNeighbours));
				
			}
		}
	}
	
	void GeneratePoints(int pointDensity, int areaSize)
	{
		//size of squares
		int numberOfPoints;
		int areaSquare = (int)Mathf.Round(Mathf.Sqrt(areaSize));
		int areaX = terrainX/areaSquare;
		int areaZ = terrainZ/areaSquare;

		for(int i = 0; i < areaSquare; i++)
		{
			for(int j = 0; j < areaSquare; j++)
			{
				//pointer to where square begins
				int squareX = areaX * i;
				int squareZ = areaZ * j;

				float roughness = 0.0f;
		
				for(int x = squareX; x < (squareX + areaX); x++)
				{
					for(int z = squareZ; z < (squareZ + areaZ); z++)
					{
						if( x == 0 || z == 0 || x > terrainX -1 || z > terrainZ - 1)
						{
							roughness += 1f;
						}
						else
						{
							roughness += steepness[x,z];
						}
					}
				}
				roughness = roughness/(areaX * areaZ);
				
				numberOfPoints = (int)Mathf.Ceil((Mathf.Pow(roughness, powerOfTerrainModifier) * pointDensity));
				//numberOfPoints = 9;
				Vector2[] points = new Vector2[numberOfPoints];
				points = NaturalPlacementOfPoints(squareX, squareZ, squareX + areaX, squareZ + areaZ, numberOfPoints, points);
				foreach(Vector2 point in points)
				{
					Vector3 temp = TerrainToWorldPosition(Terrain.activeTerrain, 512, new Vector3(point.x, 0,  point.y));
					temp.y = terrainData.GetHeight((int)Mathf.Round(temp.x),(int)Mathf.Round(temp.z));
                    if (!(temp.x < 1 || temp.z < 1 || temp.x > 511 || temp.z > 511))
                    {
                        myGraph.Add(new Point(myGraph.Count, temp, numberOfPoints));
                    }
				}
	
			}
		}

	}
		
	Vector2[] NaturalPlacementOfPoints(int x0, int y0, int x1, int y1, int pointNumber, Vector2[] points)
	{
		float threshold = (x1-x0)/2f;
		int failCount= 0;
		int placementCount = 0;
		bool failed = false;

		while(placementCount < pointNumber)
		{
			Vector2 pointToAdd = new Vector2(Random.Range(x0, x1), Random.Range(y0, y1) );
			foreach(Vector2 point in points)
			{
				if( Vector2.Distance(point, pointToAdd) > threshold * Mathf.Pow(1.1f, (float)failCount) )
				{
					failed = true;
					failCount++;
					break;
				}
			}
			if(!failed)
			{
				points[placementCount] = new Vector2();
				points[placementCount] = pointToAdd;
				placementCount++;
				failCount = 0;
				
			}
			failed = false;

		}
		return points;
	}
	
	void GenerateEdges()
	{
        foreach (Point point in myGraph)
        {
            point.myNeighbours = myGraph.Where(x => x != point && CanConnectTo(point, x)).OrderBy(x => Vector3.Distance(point.Position, x.Position)).Take(numberOfNeighbours).ToList();
            foreach (Point neighbour in point.myNeighbours)
            {
                if (neighbour.groupID > point.groupID)
                    neighbour.Merge(point);
            }
        }
        foreach (Point point in myGraph)
        {
            point.myNeighbours = point.myNeighbours.Union(myGraph.Where(x => x != point && x.myNeighbours.Contains(point) && CanConnectTo(point, x))).ToList();
            foreach (Point neighbour in point.myNeighbours)
            {
                if (neighbour.groupID > point.groupID)
                    neighbour.Merge(point);
            }
        }
        for (; ; )
        {
            var groups = myGraph.Select(x => x.groupID).Distinct().Select(x => myGraph.Where(y => y.groupID == x));
            if (groups.Count() <= 1)
                break;
            Point bestStart = null;
            Point bestEnd = null;
            float bestPairDistance2 = float.MaxValue;
            foreach (Point point in groups.First())
            {
                Point bestPoint = null;
                float bestDistance2 = float.MaxValue;
                foreach (Point other in groups.Skip(1).SelectMany(x => x))
                {
                    float distance2 = (point.Position - other.Position).sqrMagnitude;
                    if (distance2 < bestDistance2)
                    {
                        bestDistance2 = distance2;
                        bestPoint = other;
                    }
                }
                if (bestDistance2 < bestPairDistance2)
                {
                    bestPairDistance2 = bestDistance2;
                    bestStart = point;
                    bestEnd = bestPoint;
                }
            }
            bestStart.myNeighbours.Add(bestEnd);
            bestEnd.myNeighbours.Add(bestStart);
            if (bestStart.groupID < bestEnd.groupID)
                bestEnd.Merge(bestStart);
            else
                bestStart.Merge(bestEnd);
        }
	}

	
	
	void DrawPoints(List<Point> graph)
	{
		
		for(int i = 0; i < graph.Count; i++)
		{
			
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.renderer.material = graphMat;
			//cube.transform.Rotate(0,0,Mathf.PI);
			cube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
			cube.transform.position = graph[i].Position; // + new Vector3(0, pointSize/2, 0);
		}
		
	}
	
	void DrawGraph(List<Point> graph, int noOfNeighbours)
	{	
		foreach(Point origin in myGraph)
		{
            int lineCount = 0;

            GameObject go = new GameObject();
            LineRenderer lines = go.AddComponent<LineRenderer>();
            lines.renderer.material.color = colors[origin.groupID % colors.Length];
            lines.SetVertexCount(origin.myNeighbours.Count * 2 + 1);
            lines.SetWidth(0.3f, 0.3f);

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//cube.renderer.material = graphMat;
            cube.renderer.material.color = colors[origin.groupID % colors.Length];
			cube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
			cube.transform.position = origin.Position + new Vector3(0, 0.5f, 0);
			//inefficient!
            lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 0.5f, 0));
			foreach(Point point in origin.myNeighbours)
			{
                lines.SetPosition(lineCount++, point.Position + new Vector3(0, 0.5f, 0));
				lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 0.5f, 0));
			}
		}	
	}
	
	void Update()
	{
	}
		

	
	static Vector3 WorldToTerrainPosition(Terrain terrain, int mapRes, Vector3 worldPos) 
	{
		Vector3 terrainPos = terrain.transform.position;
		Vector3 sizeOfTerrain = terrain.terrainData.size;
		Vector3 relativePos = worldPos - terrainPos;
		float terrainX = relativePos.x/sizeOfTerrain.x*mapRes;
		float terrainY = relativePos.y/sizeOfTerrain.y*mapRes;
		float terrainZ = relativePos.z/sizeOfTerrain.z*mapRes;
		return new Vector3(terrainX, terrainY, terrainZ);
	}
	
	
	static Vector3 TerrainToWorldPosition(Terrain terrain, int mapRes, Vector3 terrainPos) 
	{
		Vector3 sizeOfTerrain = terrain.terrainData.size;
		float worldX = terrainPos.x/mapRes*sizeOfTerrain.x;
		float worldY = terrainPos.y/mapRes*sizeOfTerrain.y;
		float worldZ = terrainPos.z/mapRes*sizeOfTerrain.z;
		return new Vector3(worldX, worldY, worldZ);
	}

    static Vector2 Lerp(Vector2 from, Vector2 to, float t)
    {
        t = Mathf.Clamp01(t);
        return new Vector2(from.x + ((to.x - from.x) * t),
            from.y + ((to.y - from.y) * t));
    }
	
	
}

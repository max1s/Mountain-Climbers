using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NaturalMesh : MonoBehaviour
{
	readonly Color[] colors = new Color[]{Color.black, Color.blue, Color.yellow, Color.green, Color.red};
	private Shader graphShader;
	private Material graphMat;

	bool toggle = true;

	public static List<Vector4> treePos;
	
	public  List<Point> myGraph;
	private float pointSize = 2f;
	
	public static float[,] steepness;
	int terrainX;
	int terrainZ;
	int numberOfNeighbours = 4;
	
	TerrainData terrainData;
		
	void Start()
	{
        //Random.seed = 5;
		myGraph = new List<Point>();
		graphShader = (Shader)Resources.Load("RedShader");
		graphMat = new Material(graphShader);
		terrainData = Terrain.activeTerrain.terrainData;
		terrainX = 512;
		terrainZ = 512;
		steepness = new float[terrainX,terrainZ];
		steepness = Helper.CalculateSteepness(terrainData, steepness, terrainX, terrainZ );
		GeneratePoints(1, 16);
		//GenerateEvenPoints();
		//DrawPoints(myGraph);
		GenerateEdges();
		GenerateInitialWeights(treePos);
			//DrawGraph(myGraph, numberOfNeighbours);
	}



	public void GenerateInitialWeights(List<Vector4> forestLocs)
	{
		foreach(Point p in myGraph)
		{
			foreach(Point neighbour in p.myNeighbours)
			{
				float weight = 0.0f;
				float terrainModifier = 0.0f;
				float steepnessModifier = 0.0f;
				float treeModifier = 0.0f;

				for(int i = 1; i < 11; i++)
				{
				//samples
					Vector3 samplePoint = Vector3.Lerp(p.Position, neighbour.Position, i);
					Vector3 translatedSamplePoint = WorldToTerrainPosition(Terrain.activeTerrain, 512, samplePoint);
				
				//terrainModiffier
					float height = terrainData.GetHeight((int)translatedSamplePoint.x, (int)translatedSamplePoint.y);

					if(height > 0.2f && height < 0.5f)
						terrainModifier += 0.1f;
									
					if(height > 0.48f && height < 0.61f)
						terrainModifier += 0.2f;

					if(height > 0.6f)
						terrainModifier += 0.4f;


				//averageSteepness
					float avgSteepness = 0.0f;
					avgSteepness += steepness[(int)translatedSamplePoint.x, (int)translatedSamplePoint.y];
					avgSteepness += steepness[(int)translatedSamplePoint.x + 1, (int)translatedSamplePoint.y];
					avgSteepness += steepness[(int)translatedSamplePoint.x - 1, (int)translatedSamplePoint.y];
					avgSteepness += steepness[(int)translatedSamplePoint.x , (int)translatedSamplePoint.y + 1];
					avgSteepness += steepness[(int)translatedSamplePoint.x , (int)translatedSamplePoint.y - 1];
					avgSteepness /= 5;
					steepnessModifier += avgSteepness;

				//tree locs
					for(int j = 0; j < forestLocs.Count; j++)
					{
						if(translatedSamplePoint.x >= forestLocs[j].x && translatedSamplePoint.z >= forestLocs[j].y
						   && translatedSamplePoint.x <= forestLocs[j].z && translatedSamplePoint.z <= forestLocs[j].w)
						{
							treeModifier += 0.3f;
						}
					}

				}
				weight = (terrainModifier + treeModifier + steepnessModifier) * Vector3.Distance(p.Position, neighbour.Position);
				p.myWeights.Add (weight);

			}
		}
		//foreach(Vector4 loc in forestLocs)
		//{
		//	Debug.Log(loc);
		//}
	}
	

	List<float> DynamicWeights(Point p, Vector3 endPoint)
	{
		List<float> weights = new List<float>();
		for(int i = 0; i < p.myNeighbours.Count; i++)
		{
			float modWeight = p.myWeights[i] + Vector3.Distance(p.myNeighbours[i].Position, endPoint);
				weights.Add(modWeight);
		}
		return weights;
	}

	public void AddNode(Vector3 node)
	{
		Point point = new Point(myGraph.Count, node, 1);
		Point connect = myGraph[getNearestPoint(node)];
		point.myNeighbours.Add(connect);
		connect.myNeighbours.Add (point);
		connect.myWeights.Add (0);
		myGraph.Add (point);

	}

	public void removeNode(Vector3 node)
	{
		myGraph.Remove (myGraph.Find (x => x.Position == node));
	}

	public int getNearestPoint(Vector3 point)
	{
		Point nearest = (Point)myGraph.OrderBy(x => Vector3.Distance(point, x.Position)).Take(1).ToList()[0];
		int index = myGraph.FindIndex( p => p == nearest);
		return index;
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
						if( x == 0 || z == 0 || x > terrainX -2 || z > terrainZ - 2)
						{
                            roughness += 0f;
						}
						else
						{
							roughness += steepness[x,z];
						}
					}
				}
                roughness = roughness / (areaX + areaZ);
               // Debug.Log(roughness);
                
				numberOfPoints = (int)Mathf.Ceil(roughness * pointDensity);
                //Debug.Log(numberOfPoints);
				//numberOfPoints = 9;
				Vector2[] points = new Vector2[numberOfPoints];
				points = NaturalPlacementOfPoints(squareX, squareZ, squareX + areaX, squareZ + areaZ, numberOfPoints, points);
				foreach(Vector2 point in points)
				{
					Vector3 temp = TerrainToWorldPosition(Terrain.activeTerrain, 512, new Vector3(point.x, 0,  point.y));
					temp.y = terrainData.GetHeight((int)Mathf.Round(temp.x),(int)Mathf.Round(temp.z));
                    if (!(temp.x < 4 || temp.z < 4 || temp.x > 508  || temp.z > 508))
                    {
                        myGraph.Add(new Point(myGraph.Count, temp, numberOfPoints));
                    }
				}
	
			}
		}

	}
		
	Vector2[] NaturalPlacementOfPoints(int x0, int y0, int x1, int y1, int pointNumber, Vector2[] points)
	{
		float threshold = (x1-x0)/2;
		int failCount= 0;
		int placementCount = 0;
		bool failed = false;

		while(placementCount < pointNumber)
		{
			Vector2 pointToAdd = new Vector2(Random.Range(x0, x1), Random.Range(y0, y1) );
			foreach(Vector2 point in points)
			{
				if( Vector2.Distance(point, pointToAdd) < threshold * Mathf.Pow(0.9f, (float)failCount) )
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

	public bool CanConnectTo(Point @this, Point @that)
	{
		if(Vector3.Distance (@this.Position, @that.Position) < 20)
			return false;

		return true;
	}
	
	void GenerateEdges()
	{
        foreach (Point point in myGraph)
        {
			point.myNeighbours = myGraph.Where(x => x != point)
										.OrderBy(x => Vector3.Distance(point.Position, x.Position))
										.Take(numberOfNeighbours).ToList();

			point.myNeighbours = point.myNeighbours.Where ( x => CanConnectTo(point, x) ).ToList();
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

            GameObject go = new GameObject("line");
            LineRenderer lines = go.AddComponent<LineRenderer>();
            lines.renderer.material.color = colors[origin.groupID % colors.Length];
            lines.SetVertexCount(origin.myNeighbours.Count * 2 + 1);
            lines.SetWidth(0.3f, 0.3f);
			go.transform.parent = this.transform;

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//cube.renderer.material = graphMat;
            cube.renderer.material.color = colors[origin.groupID % colors.Length];
			cube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
			cube.transform.position = origin.Position + new Vector3(0, 1.0f, 0);
			cube.transform.parent = this.transform;
			//inefficient!
            lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 1.0f, 0));
			foreach(Point point in origin.myNeighbours)
			{
                lines.SetPosition(lineCount++, point.Position + new Vector3(0, 1.0f, 0));
				lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 1.0f, 0));
			}
		}	
	}
	
	void DrawGraphWithPath(List<Point> graph, List<Point> path)
	{

	}
	public void DrawGraphWithPaths(List<Point> graph, List<List<Point>> paths)
	{
		foreach(Point origin in myGraph)
		{
			GameObject go = new GameObject("line");
			LineRenderer lines = go.AddComponent<LineRenderer>();
			bool tog = false; 
			int lineCount = 0;

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			int i;
			for(i = 0; i < paths.Count; i++)
			{
				if(paths[i].Contains (origin))
				{
					cube.renderer.material.color = colors[i % colors.Length];
					tog = true;
					break;
				}
			}
			if(!tog)
				cube.renderer.material.color = Color.gray;

			//cube.renderer.material = graphMat;

			cube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
			cube.transform.position = origin.Position + new Vector3(0, 1.0f, 0);
			cube.transform.parent = this.transform;
			//inefficient!

			lines.SetVertexCount(origin.myNeighbours.Count * 2 + 1);
			lines.SetWidth(0.3f, 0.3f);
			go.transform.parent = this.transform;





			lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 1.0f, 0));
			foreach(Point point in origin.myNeighbours)
			{
				if(tog && paths[i].Contains(point))
				{
					lines.renderer.material.color = colors[i % colors.Length];
					lines.SetPosition(lineCount++, point.Position + new Vector3(0, 1.0f, 0));
					lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 1.0f, 0));
				}
				else
				{
					lines.renderer.material.color = Color.gray;
					lines.SetPosition(lineCount++, point.Position + new Vector3(0, 1.0f, 0));
					lines.SetPosition(lineCount++, origin.Position + new Vector3(0, 1.0f, 0));
				}
			}
		}	
	}

	void ClearGraph()
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.M))
		{
			if(toggle)
			{
				ClearGraph();
				toggle = !toggle;
			}
			else
			{
				DrawGraph(myGraph, numberOfNeighbours);
				toggle = !toggle;
			}
		} 
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

	public static void populateTreePositions(List<Vector4> trees)
	{
		treePos = new List<Vector4>();
		treePos = trees;
	}

}

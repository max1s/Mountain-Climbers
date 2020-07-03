using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGeneration
{	
	//terrainSizes
	static int terrainX = 512;
	static int terrainZ = 512;
	static int terrainY = 200;

	private static List<Vector4> forestPositions;
	
	public static float[,] heights;

	public static TerrainData GenerateTerrain() 
	{
		TerrainData myTerrain = new TerrainData();
		myTerrain.size = new Vector3(terrainX, terrainY, terrainZ);
		myTerrain.heightmapResolution = 512;

		heights = new float[terrainX, terrainZ];
		heights = HeightMap.GenerateMixedTerrain(4, 512, 512, 8);

		for(int i = 0; i < terrainX; i++)
		{
			for(int j = 0; j < terrainZ; j++)
			{
				if((j == 0 || i == 0 || j == terrainZ -1 || i == terrainX -1))
				{
					heights[i,j] = 0;
				}
				
			}
		}
		myTerrain.size = new Vector3(terrainX, terrainY, terrainZ);
		myTerrain.SetHeights(0,0, heights);
		myTerrain.splatPrototypes = BindTexturesToBrush();
		myTerrain.SetAlphamaps(0,0, CalculateAlphas());
		//myTerrain.RefreshPrototypes();
		//myTerrain.treePrototypes = BindTreeToBrush();
		//myTerrain.RefreshPrototypes();
		//myTerrain.treeInstances = TreeMaking (myTerrain);
		//myTerrain.RefreshPrototypes();
		return myTerrain;

	}

	public static TerrainData GenerateTerrain(string uniform) 
	{
		TerrainData myTerrain = new TerrainData();
		myTerrain.size = new Vector3(terrainX, terrainY, terrainZ);
		myTerrain.heightmapResolution = 512;
		
		heights = new float[terrainX, terrainZ];
		heights = HeightMap.GenerateUniformTerrain(uniform, 513, 513, 8);

		for(int i = 0; i < terrainX; i++)
		{
			for(int j = 0; j < terrainZ; j++)
			{
				if((j == 0 || i == 0 || j == terrainZ -1 || i == terrainX -1))
				{
					heights[i,j] = 0;
				}
				
			}
		}
		myTerrain.size = new Vector3(terrainX, terrainY, terrainZ);
		myTerrain.SetHeights(0,0, heights);
		myTerrain.splatPrototypes = BindTexturesToBrush();
		myTerrain.SetAlphamaps(0,0, CalculateAlphas());
		myTerrain.treePrototypes = BindTreeToBrush();
		myTerrain.RefreshPrototypes();	
		myTerrain.treeInstances = TreeMaking (myTerrain);
		return myTerrain;
		
	}
	
	
	public static float[,] ReturnHeights()
	{
		return heights;
	}

		
	private static SplatPrototype[] BindTexturesToBrush()
	{
		SplatPrototype[] textures = new SplatPrototype[5];
		textures[0] = new SplatPrototype();
		textures[0].texture = (Texture2D)Resources.Load ("Grass", typeof(Texture2D));
		textures[0].tileOffset = new Vector2(0, 0);
		textures[0].tileSize = new Vector2(15,15);
		
		textures[1] = new SplatPrototype();
		textures[1].texture = (Texture2D)Resources.Load ("Rock", typeof(Texture2D));
		textures[1].tileOffset = new Vector2(0, 0);
		textures[1].tileSize = new Vector2(15,15);
		
		textures[2] = new SplatPrototype();
		textures[2].texture = (Texture2D)Resources.Load ("Cliff", typeof(Texture2D));
		textures[2].tileOffset = new Vector2(0, 0);
		textures[2].tileSize = new Vector2(15,15);
		
		textures[3] = new SplatPrototype();
		textures[3].texture = (Texture2D)Resources.Load ("Snow", typeof(Texture2D));
		textures[3].tileOffset = new Vector2(0, 0);
		textures[3].tileSize = new Vector2(15,15);
		
		textures[4] = new SplatPrototype();
		textures[4].texture = (Texture2D)Resources.Load ("Sand", typeof(Texture2D));
		textures[4].tileOffset = new Vector2(0, 0);
		textures[4].tileSize = new Vector2(15,15);

		return textures;

	}
	public static TreePrototype[] BindTreeToBrush()
	{
		TreePrototype[] trees = new TreePrototype[1];
		trees[0] = new TreePrototype();
		trees[0].prefab = Resources.Load("Trees Ambient-Occlusion/ScotsPineTypeA") as GameObject;
		return trees;

	}
	public static TreeInstance[] TreeMaking(TerrainData myTerrain)
	{
		List<Vector3> treeLocations = TreePositioning(400, 3);
		List<TreeInstance> trees = new List<TreeInstance>();
		for(int i = 0; i < treeLocations.Count; i++ )
		{
			TreeInstance tree = new TreeInstance();
			tree.color = Color.red;
			tree.prototypeIndex = 1;
			Vector3 actualPosition =  treeLocations[i];//Helper.TerrainToWorldPosition(myTerrain, 512, treeLocations[i]);
			actualPosition.y = 200;//heights[(int)actualPosition.x, (int)actualPosition.z];
			tree.position = actualPosition;
			tree.widthScale = 5;
			tree.heightScale = 5;


			trees.Add(tree);
			if(i < 10)
				Debug.Log (treeLocations[i]);
		}
		return trees.ToArray();

	}

	public static List<Vector3> TreePositioning(int noOfTrees, int noOfForests)
	{
		forestPositions = new List<Vector4>();
		List<Vector3> trees = new List<Vector3>();
		for(int i = 0; i < noOfForests; i++)
		{
			Vector3 rootTree = new Vector3(Random.Range (0, 512), 0, Random.Range (0, 512));
			int x0 = (rootTree.x - (int)(noOfTrees *0.1 ) > 1) ? (int)rootTree.x - (int)(noOfTrees *0.1  ) : 1;
			int y0 = (rootTree.z - (int)(noOfTrees *0.1  ) > 1) ? (int)rootTree.z - (int)(noOfTrees *0.1  ) : 1;
			int x1 = (rootTree.x + (int)(noOfTrees *0.1  ) < 511) ? (int)rootTree.x + (int)(noOfTrees *0.1 ) : 511;
			int y1 = (rootTree.z + (int)(noOfTrees *0.1  ) < 511) ? (int)rootTree.z + (int)(noOfTrees *0.1 ) : 511;
			forestPositions.Add (new Vector4( x0, y0, x1, y1) );
			trees.AddRange(NaturalPlacementOfTrees(x0, y0, x1, y1, noOfTrees, rootTree));
		}

		

		return trees;

	}

	public static  List<Vector4> GetForestPositions()
	{
		return forestPositions;
	}


	private static List<Vector3> NaturalPlacementOfTrees(int x0, int y0, int x1, int y1, int noOfTrees, Vector3 rootTree)
	{
		List<Vector3> trees = new List<Vector3>();
		trees.Add (rootTree);
		int placementCount = 0;
		float threshold = 5f;
		bool failed = false;
		int failCount = 0;

		int otherFailCount = 0;
		while(placementCount < noOfTrees)
		{
			Vector3 treeToAdd = new Vector3(Random.Range(x0, x1), 0, Random.Range (y0, y1));
			failed = false;

			foreach(Vector3 tree in trees)
			{
				if(Vector3.Distance (treeToAdd, tree) > threshold * Mathf.Pow (1.1f, failCount))
				{

					otherFailCount++;
				}

				if(otherFailCount > trees.Count)
				{
					otherFailCount = 0;
					failCount++;
					failed = true;
					break;
				}
			}

			if(!failed)
			{
				trees.Add (treeToAdd);
				placementCount++;
				failCount = 0;
			}
		}

		return trees;
	}


	private static float[,,] CalculateAlphas()
	{
		
		float[,,] alphaMaps = new float[terrainX, terrainZ, 5];
		//alphaMaps = myTerrain.GetAlphamaps(0, 0, terrainX, terrainZ);

		for(int i = 0; i < terrainX; i++)
		{
			for(int j = 0; j < terrainZ; j++)
			{
				float total = 0.01f;
				float[] vals = new float[5];
				if(heights[i,j] > 0.2f && heights[i,j] < 0.5f)
				{
					total += 1f;
					vals[0] = 1f;
				}
				
				if(heights[i,j] > 0.48f && heights[i,j] < 0.61f)
				{
					vals[1] = 1.0f;
					total += 1f;
				}
				if(heights[i,j] > 0.6f)
				{
					total += 1f;
					vals[3] = 1f;
				}
				
				if((j == 0 || i == 0 || j == terrainZ -1 || i == terrainX -1))
				{
					vals[1] = 1.0f;
					total += 1.0f;
				}
				else
				{
					float gradient = Mathf.Max(Mathf.Abs(heights[i,j] - heights[i-1,j-1]), Mathf.Abs(heights[i,j] - heights[i+1,j+1]));
					//if(i == 1)
					//	Debug.Log (gradient);
					if(gradient < 0.001 || heights[i,j] < 0.21f)
					{
						vals[4] = 1.0f;
						total += 1.0f;
					}
					if(gradient > 0.015)
					{
						vals[2] = 1.0f;
						total += 1.0f;
					}
					
					
				}
				
				for(int k = 0; k < 5; k++)
				{
					vals[k] /= total;
					alphaMaps[i,j,k] = vals[k];
				}
					
			}
		
		}

		return alphaMaps;
		
	}

		
}

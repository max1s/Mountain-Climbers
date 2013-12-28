using UnityEngine;
using System.Collections;

public class TerrainGeneration
{	
	//terrainSizes
	static int terrainX = 512;
	static int terrainZ = 512;
	static int terrainY = 200;
	
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
		myTerrain.RefreshPrototypes();

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
		myTerrain.RefreshPrototypes();
		
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

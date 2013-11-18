using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HeightMapGeneration 
{
	public static string[] keyVals = new string[]{"mountain","desert"};
	
	public static Dictionary<string,float[]> terrainTypes = new Dictionary<string, float[]>()
	{
		{"mountain", new float[]{1f,0.02f,0.02f,0.02f,0.8f,0.3f,0.2f, 0.1f}},
		{"desert", new float[]{0.3f,0.3f,0.2f,0.2f,0.01f,0.01f,0.01f,0.01f}}
		
	};
	
	public static string[] GetRandomKeys(int noOfKeys)
	{
		string[] keys = new string[noOfKeys];
		for(int i = 0; i < noOfKeys; i++)
		{
			 keys[i] = keyVals[Random.Range(0, keyVals.Length)];
		}
		return keys;	
	}
	
	public static List<Vector2> GenerateCentralPoints(int numberOfTerrainPoints, int width, int height)
	{
		float widthFactor = width/Mathf.Sqrt(Mathf.Round(numberOfTerrainPoints));
		float heightFactor = height/Mathf.Sqrt(Mathf.Round(numberOfTerrainPoints));
		
		List<Vector2> centralPoints = new List<Vector2>();
		for(int i = 1; i <= Mathf.Sqrt(numberOfTerrainPoints); i++)
		{
			for(int j = 1; j <= Mathf.Sqrt(numberOfTerrainPoints); j++)
			{
				centralPoints.Add (new Vector2(i + widthFactor/2, j + heightFactor/2));
			}
		}
		return centralPoints;
	}
	public static float[,] GenerateMixedTerrain(int numberOfTerrainPoints, int width, int height, int octaves)
	{
		string[] randomKeys = GetRandomKeys(numberOfTerrainPoints);
		Dictionary<string, float[,]> perlinMaps = new Dictionary<string, float[,]>();
		float[,] finalMap = new float[width, height];
		foreach(string s in keyVals)
		{
			perlinMaps.Add(s, PerlinNoise.Blend (width, height, octaves, terrainTypes[s]));
		}
		
		List<Vector2> centralPoints = GenerateCentralPoints(numberOfTerrainPoints,width, height);
		
		
		Dictionary<string,float> terrainWeightings = new Dictionary<string,float>();
		foreach(string s in keyVals)
		{
			terrainWeightings.Add (s,0f);
		}
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				
				foreach(string s in keyVals)
				{
					terrainWeightings[s] = 0;
				}
				float totalHeight = 0.0f;
				for(int x = 0; x < numberOfTerrainPoints; x++)
				{
					float weight = 1f/Vector2.Distance(new Vector2(i,j), centralPoints[x]);
					terrainWeightings[randomKeys[x]] += weight;
					totalHeight += weight;

				}
				
				foreach(string s in keyVals)
				{
					finalMap[i,j] += (terrainWeightings[s]/totalHeight) * perlinMaps[s][i,j];

				}

			}
		}
		
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				if(finalMap[i,j] < 0.09f)
				{
					finalMap[i,j] = finalMap[i + 3, j + 3];
				}
			}
		}
		
		return finalMap;
		
	}
	
		
	public static float[,] GenerateUniformTerrain(string typeOfTerrain, int width, int height, int octaves)
	{
		float[] vals = new float[octaves];
		terrainTypes.TryGetValue(typeOfTerrain,out vals);
		return PerlinNoise.Blend(width, height,octaves, vals);
	}
}

using UnityEngine;
using System.Collections;

public class Helper
{
	
	public static Vector3 TerrainToWorldPosition(Terrain terrain, int mapRes, Vector3 terrainPos) 
	{
		Vector3 sizeOfTerrain = terrain.terrainData.size;
		float worldX = terrainPos.x/mapRes*sizeOfTerrain.x;
		float worldY = terrainPos.y/mapRes*sizeOfTerrain.y;
		float worldZ = terrainPos.z/mapRes*sizeOfTerrain.z;
		return new Vector3(worldX, worldY, worldZ);
	}
	
	public static Vector3 WorldToTerrainPosition(Terrain terrain, int mapRes, Vector3 worldPos) 
	{
		Vector3 terrainPos = terrain.transform.position;
		Vector3 sizeOfTerrain = terrain.terrainData.size;
		Vector3 relativePos = worldPos - terrainPos;
		float terrainX = relativePos.x/sizeOfTerrain.x*mapRes;
		float terrainY = relativePos.y/sizeOfTerrain.y*mapRes;
		float terrainZ = relativePos.z/sizeOfTerrain.z*mapRes;
		return new Vector3(terrainX, terrainY, terrainZ);
	}

    public static float[,] CalculateSteepness(TerrainData terrainData, float[,] steepness, int terrainX, int terrainZ)
    {
		float max = 0.0f;
        float min = 1.0f;
		for(int i = 0; i < terrainX; i++)
		{
			for(int j = 0; j < terrainZ; j++)
			{
                if (j == 0 || i == 0 || j == terrainZ - 1 || i == terrainX - 1)
                {
                    steepness[i, j] = 0f;
                    continue;
                }
				if((j == 1 || i == 1 || j == terrainZ - 2 || i == terrainX -2))
				{
					if(j == 1 && i == 1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i+1, j) - terrainData.GetHeight(i,j)), Mathf.Abs(terrainData.GetHeight(i,j+1) - terrainData.GetHeight(i,j)));
					if(i == 1 && j > 1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1)));
					if(j == 1 && i > 1)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1)));
					if(i == terrainX-2 && j == terrainZ-2)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j)));
					if(i == terrainX-2 && j < terrainZ-2)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i,j+1)));
					if(j == terrainZ-2 && i < terrainX-2)
						steepness[i,j] = Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j)));
				}
				else
				{
					float gradient = Mathf.Max(Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j+1))),
									 		   Mathf.Max(Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i+1,j-1)), Mathf.Abs(terrainData.GetHeight(i,j) - terrainData.GetHeight(i-1,j+1))));
					steepness[i,j] = gradient;
                    if (gradient < min)
                        min = gradient;
                    if (gradient > max)
                    {
                        max = gradient;
                    }
				}
			}
			
		}
        if (max > 2)
            max = 2;
        for (int i = 0; i < terrainX; i++)
        {
            for (int j = 0; j < terrainZ; j++)
            {
                
                steepness[i, j] = (steepness[i,j] < 2) ? (steepness[i, j] - min) / (max - min) : 0.99f;
            }
        }

        return steepness;
		
	
    }
	

}

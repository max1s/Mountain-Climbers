using UnityEngine;
using System.Collections;

public class Helper
{
	
	static Vector3 TerrainToWorldPosition(Terrain terrain, int mapRes, Vector3 terrainPos) 
	{
		Vector3 sizeOfTerrain = terrain.terrainData.size;
		float worldX = terrainPos.x/mapRes*sizeOfTerrain.x;
		float worldY = terrainPos.y/mapRes*sizeOfTerrain.y;
		float worldZ = terrainPos.z/mapRes*sizeOfTerrain.z;
		return new Vector3(worldX, worldY, worldZ);
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
	

}

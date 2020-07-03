using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	bool menuActive = true;

	GameObject myGUI;
	GameObject myCamera;
	GameObject myTerrain;
	GameObject myLight;
	List<GameObject> treefab;
	GameObject rockfab;
	
	void Start () 
	{
		myGUI = new GameObject("GUI");
		myGUI.AddComponent("ControlGUI");
		myGUI.transform.parent = this.transform;

	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(menuActive)
			{
				Destroy (myGUI);
				menuActive = !menuActive;
			}
			else
			{
				myGUI = new GameObject("GUI");
				myGUI.AddComponent("ControlGUI");
				myGUI.transform.parent = this.transform;
				menuActive = !menuActive;

			}
		}
	
	}

	void CreateNewWorld()
	{
		DestroyWorld();
		myCamera = new GameObject("Camera");
		myCamera.AddComponent(typeof(Camera));
		myCamera.AddComponent("RTSCamera");
		myCamera.transform.parent = this.transform;

		myLight = new GameObject("Lighting");
		myLight.AddComponent<Light>();
		myLight.light.type = LightType.Directional;
		myLight.light.transform.position = new Vector3(256, 100, 256);
		myLight.light.transform.rotation = Quaternion.Euler (new Vector3(90, 0, 0));
		myLight.light.intensity = 0.5f;
		myLight.transform.parent = this.transform;

		rockfab = Resources.Load ("Rocks/RockMesh") as GameObject;
		myTerrain = Terrain.CreateTerrainGameObject(TerrainGeneration.GenerateTerrain());
		///////terrain = Terrain.CreateTerrainGameObject(TerrainGeneration.GenerateTerrain("art"));
		List<Vector3> l = TerrainGeneration.TreePositioning( 200, 2);
		PopulateTreeBrush();

		foreach(Vector3 vec in l)
		{
			Vector3 temp = vec;
			Helper.WorldToTerrainPosition(Terrain.activeTerrain, 512, temp);
			temp.y = Terrain.activeTerrain.terrainData.GetHeight((int)temp.x,(int)temp.z);
			int i = Random.Range(1,10);
			Instantiate(treefab[i], temp, Quaternion.identity);
		}
		for(int i = 0; i < 200; i++)
		{
			Vector3 temp = new Vector3(Random.Range(0, 512), 0, Random.Range (0,512));
			Helper.WorldToTerrainPosition(Terrain.activeTerrain, 512, temp);
			temp.y = Terrain.activeTerrain.terrainData.GetHeight((int)temp.x,(int)temp.z);
			Instantiate(rockfab, temp, Quaternion.identity);
		}
		NaturalMesh.populateTreePositions(TerrainGeneration.GetForestPositions());
		myTerrain.AddComponent("NaturalMesh");
		myTerrain.name = "myTerrain";
		myTerrain.transform.parent = this.transform;


		Debug.Log (Terrain.activeTerrain.terrainData.GetSteepness(200,200));
	}

	void DestroyWorld()
	{
		foreach(Transform child in transform)
		{
			if (!(child.name == "GUI"))
			{
				Destroy (child.gameObject);
			}
		}
	}

	void PathFind()
	{
		List<List<Point>> path = new List<List<Point>>();
		path.Add (GameObject.Find ("bot").GetComponent<IDAStar>().
		          Deliberate(new Vector3(100, 100, 100), new Vector3(200, 200, 200)));
		myTerrain.GetComponentInChildren<NaturalMesh>().
			DrawGraphWithPaths( GameObject.Find ("myTerrain").GetComponent<NaturalMesh>().myGraph, 
			                   path   );
	}

	void SpawnCritter()
	{
		GameObject go = (GameObject)Instantiate(Resources.Load("spiderbot"));
			go.name = "bot";
		go.transform.position = new Vector3 (100, 200, 100);
		go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
		go.AddComponent<CharacterController>();
		go.AddComponent<IDAStar>();
	}

	//void SpawnSkeleton()
	//{d
	//	GameObject go = (GameObject)Instantiate(Resources.Load("skeleton"));
	//	go.transform.position = new Vector3 (150, 200, 150);
	//	go.transform.localScale = new Vector3(50, 50, 150);
	//	go.AddComponent<CharacterController>();
	//	go.AddComponent<ZombieScript>();
	//
	//}

	void ResetCamera()
	{
		Destroy (myCamera);
		myCamera = new GameObject("Camera");
		myCamera.AddComponent(typeof(Camera));
		myCamera.AddComponent("RTSCamera");
		myCamera.transform.parent = this.transform;

	}

	void PopulateTreeBrush()
	{
		treefab = new List<GameObject>(10);

		for(int i = 1; i <= 10; i++)
		{
			GameObject tree = new GameObject();
			tree = Resources.Load("Trees/tree" + 1) as GameObject;
			tree.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
			treefab.Add(tree);
		}
	}

}

using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	bool menuActive = true;

	GameObject gui;
	GameObject camera;
	GameObject terrain;
	GameObject light;
	
	void Start () 
	{
		gui = new GameObject("GUI");
		gui.AddComponent("ControlGUI");
		gui.transform.parent = this.transform;

	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(menuActive)
			{
				Destroy (gui);
				menuActive = !menuActive;
			}
			else
			{
				gui = new GameObject("GUI");
				gui.AddComponent("ControlGUI");
				gui.transform.parent = this.transform;
				menuActive = !menuActive;

			}
		}
	
	}

	void CreateNewWorld()
	{
		DestroyWorld();
		camera = new GameObject("Camera");
		camera.AddComponent(typeof(Camera));
		camera.AddComponent("RTSCamera");
		camera.transform.parent = this.transform;

		light = new GameObject("Lighting");
		light.AddComponent<Light>();
		light.light.type = LightType.Directional;
		light.light.transform.position = new Vector3(256, 100, 256);
		light.light.transform.rotation = Quaternion.Euler (new Vector3(90, 0, 0));
		light.light.intensity = 0.5f;
		light.transform.parent = this.transform;


		terrain = Terrain.CreateTerrainGameObject(TerrainGeneration.GenerateTerrain());
		//terrain = Terrain.CreateTerrainGameObject(TerrainGeneration.GenerateTerrain("art"));

		terrain.AddComponent("NaturalMesh");
		terrain.transform.parent = this.transform;


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

	void ResetCamera()
	{
		Destroy (camera);
		camera = new GameObject("Camera");
		camera.AddComponent(typeof(Camera));
		camera.AddComponent("RTSCamera");
		camera.transform.parent = this.transform;

	}

}

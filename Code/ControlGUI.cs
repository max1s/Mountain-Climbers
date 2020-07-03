using UnityEngine;
using System.Collections;


public class ControlGUI : MonoBehaviour 
{

	void OnGUI () 
	{
		// Make a background box
		GUI.Box(new Rect(10,10,120,400), "Control Menu");

		if(GUI.Button(new Rect(20,40,100,20), "Start Testing")) 
		{
			SendMessageUpwards("CreateNewWorld");
		}

		if(GUI.Button(new Rect(20,70,100,20), "Stop Testing")) 
		{
			SendMessageUpwards ("SpawnCritter");
		}

		if(GUI.Button(new Rect(20,100,100,20), "Reset Camera")) 
		{
			SendMessageUpwards("ResetCamera");
		}

		if(GUI.Button(new Rect(20,130,100,20), "PathFind")) 
		{
			SendMessageUpwards("PathFind");
		}

		if(GUI.Button(new Rect(20,160,100,20), "Show Search Grid")) 
		{
			SendMessageUpwards ("Spawn Skeleton");
		}


	}
}

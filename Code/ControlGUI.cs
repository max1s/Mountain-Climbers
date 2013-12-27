using UnityEngine;
using System.Collections;


public class ControlGUI : MonoBehaviour 
{

	void OnGUI () 
	{
		// Make a background box
		GUI.Box(new Rect(10,10,120,400), "Control Menu");

		if(GUI.Button(new Rect(20,40,100,20), "Create New World")) 
		{
			SendMessageUpwards("CreateNewWorld");
		}

		if(GUI.Button(new Rect(20,70,100,20), "Spawn Critter")) 
		{

		}

		if(GUI.Button(new Rect(20,100,100,20), "Reset Camera")) 
		{
			SendMessageUpwards("ResetCamera");
		}
		if(GUI.Button(new Rect(20,130,100,20), "Clear")) 
		{
			SendMessageUpwards("DestroyWorld");
		}

	}
}

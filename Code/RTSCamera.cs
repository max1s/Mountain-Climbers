using UnityEngine;
using System.Collections;



public class RTSCamera : MonoBehaviour 
{

	// THIS CLASS DESCRIBES MOVEMENT  AND INERTIA OF CAMERA
	//Movement
	float movementSpeed = 100.0f; //XZ axis scroll spped
	float inertiaMovementTime = 0.5f;
	int inertiaMovement = 0;
	float inertiaMovementDeltaTime = 0.0f;
	float inertiaMovementVelocity = 5.0f;
	Vector4 movementBoundaries;
	int movedAway = 0;
	//Zoom
	bool zoomOfCamera = true;
	float zoomSpeed = 2.0f; //Just so we know when to stop
	float inertiaZoomTime = 0.8f;
	bool inertiaZoom = false;
	float inertiaZoomVelocity = 0.005f;
	float inertiaZoomDeltaTime = 1.0f;
	RaycastHit zoomPoint;
	
	//Rotation
	bool rotationOfCamera = true;
	float angularSpeed = 50.0f;
	float inertiaRotationTime = 0.2f;
	bool inertiaRotation = false;
	float inertiaRotationVelocity = 0.0f;
	float inertiaRotationDeltaTime = 0.0f;

	//positions
	
	float scrollPosition;
	float mousePositionX;
	
	//terrain variables
	GameObject terrObj;
	Terrain terrain;
	Vector3 terrainSize;
	
	public Transform target;
	
	Vector3 rotationalPoint = new Vector3(0f,0f,0f);
	
	//
	Vector3 snapPoint;
	Quaternion snapRotation;
	Vector3 overheadPoint = new Vector3(200f,600f,200f);
	Quaternion overheadRotation = Quaternion.Euler(90f,0f,0f);
	
	//
	bool moving = false;
	bool snap = true;
	float snapRotationTime = 10.0f;
	float snapRotationDeltaTime = 0.0f;

	
	

	
	void CheckMovement()
	{
		float position = Time.deltaTime * movementSpeed;
		Vector3 camEuler =  transform.eulerAngles;

#if UNITY_EDITOR	  
		
		if((Input.GetKey("w") || Input.mousePosition.y > GetMainGameViewSize().y * 0.97f))
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
			
				transform.Translate(new Vector3(Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f ,Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
				if(Input.mousePosition.y > GetMainGameViewSize().y * 0.97f)
					movedAway = 1;
			}
		}
		if(Input.GetKey("s") || Input.mousePosition.y < GetMainGameViewSize().y * 0.03f )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				transform.Translate(new Vector3(-Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
				if( Input.mousePosition.y < GetMainGameViewSize().y * 0.03f )
					movedAway = 2;
			}
		}
		if(Input.GetKey("a") || Input.mousePosition.x < GetMainGameViewSize().x * 0.03f )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				transform.Translate(new Vector3(-Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
				if(Input.mousePosition.x < GetMainGameViewSize().x * 0.03f)
					movedAway = 3;
			}
		}
		if(Input.GetKey("d") || Input.mousePosition.x > GetMainGameViewSize().x * 0.97f )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				transform.Translate(new Vector3(Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
				if(Input.mousePosition.x > GetMainGameViewSize().x * 0.97f)
					movedAway = 4;
			}
		}
		
		if(Input.GetKeyUp("w") ||( Input.mousePosition.y < GetMainGameViewSize().y * 0.97f && movedAway == 1)) 
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				inertiaMovement = 1;
				movedAway = 0;
			}
			else
			{
				inertiaMovement = 2;
				movedAway = 0;
			}
		}
		if(Input.GetKeyUp("s") || (Input.mousePosition.y > GetMainGameViewSize().y * 0.03f && movedAway ==2) )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				inertiaMovement = 2;
				movedAway = 0;
			}
			else
			{
				inertiaMovement = 1;
				movedAway = 0;
			}
		}
		if(Input.GetKeyUp("a") || (Input.mousePosition.x > GetMainGameViewSize().x * 0.03f && movedAway ==3) )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				inertiaMovement = 3;
				movedAway = 0;
			}
			else
			{
				inertiaMovement = 4;
				movedAway = 0;
			}
		}
		if(Input.GetKeyUp("d") || (Input.mousePosition.x < GetMainGameViewSize().x * 0.97f && movedAway ==4) )
		{
			if(Physics.Raycast(transform.position,transform.forward))
			{
				inertiaMovement = 4;
				movedAway = 0;
			}
			else
			{
				inertiaMovement = 3;
				movedAway = 0;
			}
		}
	
		
#elif UNITY_ANDROID
		//TOUCH
	
#else
		
		if(Input.GetKey("w") || Input.mousePosition.y >Screen.height * 0.97f )
		{
			transform.Translate(new Vector3(Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f ,Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
			if(Input.mousePosition.y > Screen.height * 0.97f)
				movedAway = 1;
		}
		if(Input.GetKey("s") || Input.mousePosition.y < Screen.height * 0.03f )
		{
			transform.Translate(new Vector3(-Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
			if( Input.mousePosition.y <Screen.height * 0.03f )
				movedAway = 2;
		}
		if(Input.GetKey("a") || Input.mousePosition.x < Screen.width * 0.03f )
		{
			transform.Translate(new Vector3(-Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
			if(Input.mousePosition.x <Screen.width * 0.03f)
				movedAway = 3;
		}
		if(Input.GetKey("d") || Input.mousePosition.x > Screen.width * 0.97f )
		{
			transform.Translate(new Vector3(Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*position , Space.World);
			if(Input.mousePosition.x > Screen.width * 0.97f)
				movedAway = 4;
		}
		
		if(Input.GetKeyUp("w") ||( Input.mousePosition.y < Screen.height * 0.97f && movedAway == 1)) 
		{
			inertiaMovement = 1;
			movedAway = 0;
		}
		if(Input.GetKeyUp("s") || (Input.mousePosition.y > Screen.height * 0.03f && movedAway ==2) )
		{
			inertiaMovement = 2;
			movedAway = 0;
		}
		if(Input.GetKeyUp("a") || (Input.mousePosition.x > Screen.height * 0.03f && movedAway ==3) )
		{
			inertiaMovement = 3;
			movedAway = 0;
		}
		if(Input.GetKeyUp("d") || (Input.mousePosition.x < Screen.height * 0.97f && movedAway ==4) )
		{
			inertiaMovement = 4;
			movedAway = 0;
		}
#endif
		
		
	}
	
	void CheckRotation()
	{
				
#if UNITY_ANDROID
		
#else
		if(Input.GetMouseButton(1))
		{
		
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit rotationalAxis;
			if(Physics.Raycast(ray, out rotationalAxis))
			{
				rotationalPoint = rotationalAxis.point;
				if(mousePositionX != Input.mousePosition.x)
				{
				
					inertiaRotationVelocity = (Input.mousePosition.x - mousePositionX) * 0.5f;
					transform.RotateAround(rotationalPoint, new Vector3(0f,0.1f,0f), inertiaRotationVelocity * Time.deltaTime * angularSpeed );
				}
				
				
				
			}
			
		}
		if(Input.GetMouseButtonUp(1))
		{
			inertiaRotation = true;
		}
		
#endif
	}
	
	void CheckZoom()
	{
		
#if UNITY_ANDROID
#else	
		
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			if(zoomOfCamera)
			{
				inertiaZoomVelocity = (Input.GetAxis("Mouse ScrollWheel") - scrollPosition) * zoomSpeed * 0.2f;
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				
				
				if(Physics.Raycast(ray, out zoomPoint))
				{
					transform.Translate((zoomPoint.point - transform.position) * (Input.GetAxis("Mouse ScrollWheel") - scrollPosition) * zoomSpeed, Space.World);
					inertiaZoom = true;	
				}
			}
			else
			{
				MapDeSnap();
				
			}
		}

	}
	
			
	
#endif
	
	void InertiaCheck()
	{
		
		//Rotation
		if(inertiaRotation && inertiaRotationDeltaTime < inertiaRotationTime && inertiaRotationVelocity != 0.0f)
		{
		
			inertiaRotationDeltaTime += Time.smoothDeltaTime;
			inertiaRotationVelocity = Mathf.Lerp(inertiaRotationVelocity, 0.0f,inertiaRotationDeltaTime);
		    transform.RotateAround(rotationalPoint, new Vector3(0f,0.3f,0f), inertiaRotationVelocity );
			
		}
		else
		{
			inertiaRotationDeltaTime = 0.0f;
			inertiaRotation = false;
		}
		//ZOOM
		if(scrollPosition == 0 && inertiaZoom  && inertiaZoomDeltaTime < inertiaZoomTime)
		{
			inertiaZoomDeltaTime += Time.smoothDeltaTime;
			inertiaZoomVelocity = Mathf.Lerp(inertiaZoomVelocity, 0.0f, inertiaZoomDeltaTime);
			transform.Translate((zoomPoint.point - transform.position)*inertiaZoomVelocity , Space.World);
			Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out zoomPoint);
			if(zoomPoint.distance > 600.0f && snap)
			{
				snap = !snap;
				snapPoint = transform.position;
				snapRotation = transform.rotation;
				
			}
			if(zoomPoint.distance > 600.0f && snapRotationDeltaTime < snapRotationTime)
			{
				MapSnap();	
			}
		}
		if(inertiaZoomDeltaTime >= inertiaZoomTime)
		{
			inertiaZoomDeltaTime = 0.0f;
			inertiaZoom = false;
		}

		//MOVEMENT
		Vector3 camEuler =  transform.eulerAngles;
		
		if(inertiaMovement > 0 && inertiaMovementDeltaTime < inertiaMovementTime)
		{
			inertiaMovementDeltaTime += Time.smoothDeltaTime;
			inertiaMovementVelocity = Mathf.Lerp(inertiaMovementVelocity, 0.0f, inertiaMovementDeltaTime);
	  
			if(inertiaMovement == 1)
				transform.Translate(new Vector3(Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f ,Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*inertiaMovementVelocity , Space.World);
			if(inertiaMovement == 2)
				transform.Translate(new Vector3(-Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ))*inertiaMovementVelocity  , Space.World);
			if(inertiaMovement == 3)
				transform.Translate(new Vector3(-Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*inertiaMovementVelocity  , Space.World);
			if(inertiaMovement == 4)
				transform.Translate(new Vector3(Mathf.Cos(camEuler.y * Mathf.PI / 180.0f ),0.0f , -Mathf.Sin(camEuler.y * Mathf.PI / 180.0f ))*inertiaMovementVelocity  , Space.World);
			if(!Physics.Raycast(transform.position,transform.forward))
			{
				if(inertiaMovement == 1 || inertiaMovement == 3)
				{
					inertiaMovement += 1;
					inertiaMovementDeltaTime = 0f;
				}
				else
				{
					inertiaMovement -= 1;
					inertiaMovementDeltaTime = 0f;
				}
			}
			
		}
		else
		{
			inertiaMovementDeltaTime = 0.0f;
			inertiaMovement = 0;
			inertiaMovementVelocity = 5.0f;
		}
	}
	
	void MapSnap()
	{
		if(!moving)
		{
			StartCoroutine(MoveObject(transform, snapPoint, overheadPoint, snapRotation, overheadRotation, 1.0f));
			rotationOfCamera = false;
			zoomOfCamera=false;
		}
	}
	
	void MapDeSnap()
	{
		if(!moving)
		{
			Vector3 tempPosition = transform.position;
			StartCoroutine(MoveObject(transform, tempPosition ,snapPoint,overheadRotation ,snapRotation, 1.0f));
			rotationOfCamera = true;
			zoomOfCamera=true;
			snap = true;
		}
	}

	void Start() 
	{
		transform.position = new Vector3(10f, 150f, 10f);
		transform.rotation = Quaternion.Euler( new Vector3(30f, 30f, 0f) );
		camera.fieldOfView = 60f;
		camera.farClipPlane = 1000f;
		camera.tag = "MainCamera";
	}
	
	
	void Update() 
	{
		
		CheckMovement();
		if(rotationOfCamera)
		{
			CheckRotation();
		}
		CheckZoom();
		InertiaCheck();
		
		mousePositionX = Input.mousePosition.x;
		scrollPosition = Input.GetAxis("Mouse ScrollWheel");
	}
	
	public static Vector2 GetMainGameViewSize()
	{
	    System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
	    System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
	    System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
	    return (Vector2)Res;
	}
	
	IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float time){
        moving = true; // MoveObject started
        float i = 0;
        float rate = 1/time;
        while (i < 1)         {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            thisTransform.rotation = Quaternion.Slerp (startRot, endRot, i);
            yield return 0;
        }
        moving = false; // MoveObject ended
    }
	
	
}

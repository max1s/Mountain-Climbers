using UnityEngine;
using System.Collections;

public class ZombieScript : MonoBehaviour {
	
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public float slopeLimit = 45.0f;

	CharacterController controller; 
	
	public Vector3 vertical;
		public Vector3 horizontal;
	
	public void Start()
	{
		vertical = transform.TransformDirection(Vector3.forward);
		horizontal = transform.TransformDirection(Vector3.right);
		controller  = GetComponent<CharacterController>();
	}

	public void Update()
	{
		animation.Play ("stopping");
	}


	public void movement() {

		if(Input.GetKey(KeyCode.UpArrow)){
			Debug.Log ( (vertical * 3f * Time.deltaTime) );
			controller.Move ((vertical * 3f * Time.deltaTime));
		}
		
	}
	
}
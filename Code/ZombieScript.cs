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
		controller.radius = 10;
		controller.height = 0.01f;
		controller.center = new Vector3(0f, 5f, 0f);
	}

	public void Update()
	{
		if(!Grounded ())
		controller.Move(new Vector3(0, -1, 0));
		if(Input.GetKey(KeyCode.UpArrow))
			controller.Move (horizontal * 0.5f);
		animation.Play("walk");
	}

	public bool Grounded()
	{
		return (controller.collisionFlags == CollisionFlags.Below);
	}
	

}
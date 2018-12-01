using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(LineController))]
public class OutScreen : MonoBehaviour
{
	Vector3 initialPosition;
	BoxCollider boxCollider;
	CubeController cube;
	LineController line;
	Rigidbody rb;

	void Start()
	{
		initialPosition = transform.position;
		boxCollider = GetComponent<BoxCollider>();
		cube = GetComponent<CubeController>();
		line = GetComponent<LineController>();
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		// Get the bounds of the cube
		var bounds = boxCollider.bounds.extents;
		var topFrontLeftPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x - bounds.x, transform.position.y - bounds.y, transform.position.z + bounds.z));
		var topRearLeftPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x - bounds.x, transform.position.y - bounds.y, transform.position.z - bounds.z));
		var topFrontRightPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x + bounds.x, transform.position.y - bounds.y, transform.position.z + bounds.z));
		var topRearRightPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x + bounds.x, transform.position.y - bounds.y, transform.position.z - bounds.z));
		var bottomFrontLeftPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x - bounds.x, transform.position.y + bounds.y, transform.position.z + bounds.z));
		var bottomRearLeftPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x - bounds.x, transform.position.y + bounds.y, transform.position.z - bounds.z));
		var bottomFrontRightPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x + bounds.x, transform.position.y + bounds.y, transform.position.z + bounds.z));
		var bottomRearRightPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x + bounds.x, transform.position.y + bounds.y, transform.position.z - bounds.z));

		// Check if any of the points are inside the camera view
		if (Camera.main.rect.Contains(topFrontLeftPoint) == false
			&& Camera.main.rect.Contains(topRearLeftPoint) == false
			&& Camera.main.rect.Contains(bottomFrontLeftPoint) == false
			&& Camera.main.rect.Contains(bottomRearLeftPoint) == false
			&& Camera.main.rect.Contains(topFrontRightPoint) == false
			&& Camera.main.rect.Contains(topRearRightPoint) == false
			&& Camera.main.rect.Contains(bottomFrontRightPoint) == false
			&& Camera.main.rect.Contains(bottomRearRightPoint) == false)
		{
			// Reset to the initial state
			line.ResetLine();
			line.drawLine = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.position = initialPosition;
			cube.HandleInputEnd(Input.mousePosition);
		}
	}
}

/* *
 *  Author: Michael Stevenson (https://gist.github.com/mstevenson/4552515)
 *  Modified and updated to Unity 5.3+ by Otavio Pliger
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
	public float force = 600;
	public float damping = 6;

	Transform jointTrans;
	float dragDepth;
	bool handle;

	void OnMouseDown()
	{
		if (handle) {
			HandleInputBegin(Input.mousePosition);
		}
	}

	void OnMouseUp()
	{
		HandleInputEnd(Input.mousePosition);
	}

	void OnMouseDrag()
	{
		HandleInput(Input.mousePosition);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Plane") {
			handle = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.name == "Plane") {
			handle = false;
		}
	}

	public void HandleInputBegin(Vector3 screenPosition)
	{
		var ray = Camera.main.ScreenPointToRay(screenPosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive")) {
				dragDepth = Camera.main.transform.InverseTransformPoint(hit.point).z;
				jointTrans = AttachJoint(hit.rigidbody, hit.point);
			}
		}
	}

	public void HandleInput(Vector3 screenPosition)
	{
		if (jointTrans == null) return;
		jointTrans.position = ScreenToWorldPlanePoint(Camera.main, dragDepth, screenPosition);
	}

	public void HandleInputEnd(Vector3 screenPosition)
	{
		if (jointTrans == null) return;
		Destroy(jointTrans.gameObject);
	}

	float AngleProportion(float angle, float proportion)
	{
		float oppisite = Mathf.Tan(angle * Mathf.Deg2Rad);
		float oppisiteProportion = oppisite * proportion;
		return Mathf.Atan(oppisiteProportion) * Mathf.Rad2Deg;
	}

	Vector2 ViewportPointToAngle(Camera camera, Vector2 viewportCoord)
	{
		float adjustedAngle = AngleProportion(camera.fieldOfView / 2, camera.aspect) * 2;
		float xProportion = ((viewportCoord.x - 0.5f) / 0.5f);
		float yProportion = ((viewportCoord.y - 0.5f) / 0.5f);
		float xAngle = AngleProportion(adjustedAngle / 2, xProportion) * Mathf.Deg2Rad;
		float yAngle = AngleProportion(camera.fieldOfView / 2, yProportion) * Mathf.Deg2Rad;
		return new Vector2(xAngle, yAngle);
	}

	Vector3 ViewportToWorldPlanePoint(Camera camera, float zDepth, Vector2 viewportCoord)
	{
		Vector2 angles = ViewportPointToAngle(camera, viewportCoord);
		float xOffset = Mathf.Tan(angles.x) * zDepth;
		float yOffset = Mathf.Tan(angles.y) * zDepth;
		Vector3 cameraPlanePosition = new Vector3(xOffset, yOffset, zDepth);
		cameraPlanePosition = camera.transform.TransformPoint(cameraPlanePosition);
		return cameraPlanePosition;
	}

	Vector3 ScreenToWorldPlanePoint(Camera camera, float zDepth, Vector3 screenCoord)
	{
		var point = Camera.main.ScreenToViewportPoint(screenCoord);
		return ViewportToWorldPlanePoint(camera, zDepth, point);
	}

	Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
	{
		GameObject go = new GameObject("Attachment Point");
		go.hideFlags = HideFlags.HideInHierarchy;
		go.transform.position = attachmentPosition;

		var newRb = go.AddComponent<Rigidbody>();
		newRb.isKinematic = true;

		var joint = go.AddComponent<ConfigurableJoint>();
		joint.connectedBody = rb;
		joint.configuredInWorldSpace = true;
		joint.xDrive = NewJointDrive(force, damping);
		joint.yDrive = NewJointDrive(force, damping);
		joint.zDrive = NewJointDrive(force, damping);
		joint.slerpDrive = NewJointDrive(force, damping);
		joint.rotationDriveMode = RotationDriveMode.Slerp;
		return go.transform;
	}

	JointDrive NewJointDrive(float force, float damping)
	{
		JointDrive drive = new JointDrive();
		drive.positionSpring = force;
		drive.positionDamper = damping;
		drive.maximumForce = Mathf.Infinity;
		return drive;
	}
}

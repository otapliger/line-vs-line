/* *
 *  Author: Michael Guerrero
 *  
 *  Description:
 *
 *  (http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html)
 *
 *  The shake animation all takes place within a coroutine which allows us to keep our timing variables
 *  local to this function as opposed to declaring them for the entire class.  The shake can be configured
 *  to last any length of time using the duration variable and this number will be used to determine what
 *  percentage of that duration has elapsed (0% to 100% or in this case 0.0 to 1.0).
 *
 *  Usage:
 *
 *     InitializeScreenShake()
 *
 *  Options:
 *
 *     duration: float // How long it lasts
 *     speed: float //  How fast it is
 *     magnitude: float // How powerful it isS
 *
 */

using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
	AnimationCurve damper = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0.2f), new Keyframe(1f, 0f));
	Vector3 originalPos;

	public string [ ] collisionTags;
	public float duration = 0.4f;
	public float speed = 100f;
	public float magnitude = 0.1f;
	public bool testNormal = false;
	public bool testProjection = false;

	void OnEnable()
	{
		originalPos = transform.localPosition;
	}

	void OnCollisionEnter(Collision collision)
	{
		foreach (var tag in collisionTags) {
			if (collision.gameObject.tag == tag) {
				InitializeScreenShake();
			}
		}
	}

	void Update()
	{
		if (testNormal) {
			testNormal = false;
			StopAllCoroutines();
			StartCoroutine(Shake(transform, originalPos, duration, speed, magnitude, damper));

		} else if (testProjection) {
			testProjection = false;
			StopAllCoroutines();
			StartCoroutine(ShakeCamera(Camera.main, duration, speed, magnitude / 10, damper));
		}
	}

	public void InitializeScreenShake(float duration = 0, float speed = 0, float magnitude = 0)
	{
		if (duration == 0) {
			duration = this.duration;
		}

		if (speed == 0) {
			speed = this.speed;
		}

		if (magnitude == 0) {
			magnitude = this.magnitude;
		}

		StopAllCoroutines();
		StartCoroutine(ShakeCamera(Camera.main, duration, speed, magnitude / 10, damper));
	}

	IEnumerator Shake(Transform transform, Vector3 originalPosition, float duration, float speed, float magnitude, AnimationCurve damper = null)
	{
		float elapsed = 0f;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
			float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMag) - (damperedMag / 2f);
			float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMag) - (damperedMag / 2f);
			transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
			yield return null;
		}

		transform.localPosition = originalPosition;
	}

	IEnumerator ShakeCamera(Camera camera, float duration, float speed, float magnitude, AnimationCurve damper = null)
	{
		float elapsed = 0f;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
			float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMag) - (damperedMag / 2f);
			float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMag) - (damperedMag / 2f);

			// Offset camera obliqueness - http://answers.unity3d.com/questions/774164/is-it-possible-to-shake-the-screen-rather-than-sha.html

			float frustrumHeight = 2 * camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			float frustrumWidth = frustrumHeight * camera.aspect;
			Matrix4x4 matrix = camera.projectionMatrix;
			matrix [0, 2] = 2 * x / frustrumWidth;
			matrix [1, 2] = 2 * y / frustrumHeight;
			camera.projectionMatrix = matrix;
			yield return null;
		}

		camera.ResetProjectionMatrix();
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScene : MonoBehaviour
{
	enum fadeTo { transparent, color };
	public float fadeinSeconds;
	public float fadeoutSeconds;
	public float displaySeconds;

	Renderer r;
	bool fadeIn;
	bool fadeOut;
	bool loadGame;

	void Start()
	{
        GetComponent<MeshRenderer>().enabled = true;
		r = GetComponent<Renderer>();
		fadeIn = true;
		fadeOut = false;
		loadGame = false;
	}

	void Update()
	{
		if (fadeIn) {
			if (r.material.color.a < 0f) {
				if (displaySeconds > 0f) {
					StartCoroutine(DisplayFor(displaySeconds));

				} else {
					fadeIn = false;
					fadeOut = true;
					loadGame = false;
				}

			} else {
				Fade(fadeTo.transparent);
			}
		}

		if (fadeOut) {
			if (r.material.color.a > 1f) {
				fadeIn = false;
				fadeOut = false;
				loadGame = true;

			} else {
				Fade(fadeTo.color);
			}
		}

		if (loadGame) {
			SceneManager.LoadScene(1);
		}
	}

	void Fade(fadeTo f)
	{
		var color = r.material.color;

		switch (f) {
			case fadeTo.transparent:
				r.material.color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime / fadeinSeconds));
				break;

			case fadeTo.color:
				r.material.color = new Color(color.r, color.g, color.b, color.a + (Time.deltaTime / fadeoutSeconds));
				break;
		}
	}

	IEnumerator DisplayFor(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		displaySeconds = 0;
	}
}
